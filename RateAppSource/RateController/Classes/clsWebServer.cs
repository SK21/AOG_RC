using RateController.PGNs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RateController.Classes
{
    public class clsWebServer
    {
        // phone remote switches, http on the tablet's network(s)
        // all app state is read/written on the UI thread via Core.MainForm.Invoke

        private const string HotspotRule = "RateController phone hotspot";

        // Windows Mobile Hotspot through its WinRT api in a hidden powershell, no admin needed.
        // ACTION is status, start or stop. prints state|result|ssid|passphrase
        private const string HotspotScript = @"$ErrorActionPreference='Stop'
$ProgressPreference='SilentlyContinue'
Add-Type -AssemblyName System.Runtime.WindowsRuntime
$null=[Windows.Networking.Connectivity.NetworkInformation,Windows.Networking.Connectivity,ContentType=WindowsRuntime]
$null=[Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager,Windows.Networking.NetworkOperators,ContentType=WindowsRuntime]
$T=[Windows.Networking.NetworkOperators.NetworkOperatorTetheringManager]
$N=[Windows.Networking.Connectivity.NetworkInformation]
$p=$N::GetInternetConnectionProfile()
if(-not $p){$p=$N::GetConnectionProfiles()|Where-Object{$T::GetTetheringCapabilityFromConnectionProfile($_) -eq 'Enabled'}|Select-Object -First 1}
$m=$T::CreateFromConnectionProfile($p)
$r=''
$s=[string]$m.TetheringOperationalState
if(('ACTION' -eq 'start' -and $s -ne 'On') -or ('ACTION' -eq 'stop' -and $s -ne 'Off')){
$aw=([System.WindowsRuntimeSystemExtensions].GetMethods()|Where-Object{$_.Name -eq 'AsTask' -and $_.GetParameters().Count -eq 1 -and $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncOperation`1'})[0].MakeGenericMethod([Windows.Networking.NetworkOperators.NetworkOperatorTetheringOperationResult])
if('ACTION' -eq 'start'){$op=$m.StartTetheringAsync()}else{$op=$m.StopTetheringAsync()}
$t=$aw.Invoke($null,@($op))
$null=$t.Wait(30000)
$r=[string]$t.Result.Status
}
$c=$m.GetCurrentAccessPointConfiguration()
Write-Output ('{0}|{1}|{2}|{3}' -f $m.TetheringOperationalState,$r,$c.Ssid,$c.Passphrase)";

        private const int HotspotMaxRestarts = 3;
        private string cCalMsg = "";
        private int cHotspotRestarts = 0;
        private System.Windows.Forms.Timer HotspotWatch = new System.Windows.Forms.Timer();
        private bool cHotspotBusy = false;
        private string cHotspotPassword = "";
        private string cHotspotResult = "";
        private string cHotspotSsid = "";
        private string cHotspotState = "Unknown";
        private TcpListener Listener;
        private int cPort;
        private bool cRunning = false;
        private Thread ServerThread;

        public clsWebServer()
        {
            Core.CalSession.StepOneComplete += CalSession_StepOneComplete;

            // keeps the tablet hotspot on while Tablet Hotspot is checked
            HotspotWatch.Interval = 30000;
            HotspotWatch.Tick += HotspotWatch_Tick;
            HotspotWatch.Enabled = true;
        }

        public bool HotspotGaveUp
        { get { return cHotspotRestarts >= HotspotMaxRestarts && cHotspotState == "Off"; } }

        public bool HotspotBusy
        { get { return cHotspotBusy; } }

        public string HotspotPassword
        { get { return cHotspotPassword; } }

        public string HotspotResult
        { get { return cHotspotResult; } }     // last start/stop, Success or the windows reason

        public string HotspotSsid
        { get { return cHotspotSsid; } }

        public string HotspotState
        { get { return cHotspotState; } }       // On, Off, InTransition or Unknown

        public bool IsRunning
        { get { return cRunning; } }

        public int Port
        { get { return cPort; } }

        public void Start(int Port)
        {
            Stop();
            try
            {
                cPort = Port;
                Listener = new TcpListener(IPAddress.Any, cPort);
                Listener.Start();
                cRunning = true;
                ServerThread = new Thread(Listen);
                ServerThread.IsBackground = true;
                ServerThread.Start();
            }
            catch (Exception ex)
            {
                cRunning = false;
                Props.WriteErrorLog("clsWebServer/Start: " + ex.Message);
            }
        }

        public void Stop()
        {
            cRunning = false;
            try
            {
                Listener?.Stop();    // unblocks AcceptTcpClient
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsWebServer/Stop: " + ex.Message);
            }
            Listener = null;
        }

        public bool AllowHotspot()
        {
            // firewall rule for phones on the tablet's Windows Mobile Hotspot (always 192.168.137.x).
            // its network has no profile so it counts as Public and can't be set to Private.
            // port based so upgrades and folder moves keep working. Windows asks for admin.
            bool Result = false;
            try
            {
                string Rule = "advfirewall firewall {0} rule name=\"" + HotspotRule + "\"";
                string Args = "/c netsh " + string.Format(Rule, "delete") + " & netsh " + string.Format(Rule, "add")
                    + " dir=in action=allow protocol=TCP localport=" + cPort.ToString() + " profile=public remoteip=192.168.137.0/24";
                ProcessStartInfo Info = new ProcessStartInfo("cmd.exe", Args);
                Info.UseShellExecute = true;
                Info.Verb = "runas";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                using (Process P = Process.Start(Info))
                {
                    P.WaitForExit(15000);
                }
                Result = HotspotAllowed();
            }
            catch (Win32Exception)
            {
                // admin prompt declined
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsWebServer/AllowHotspot: " + ex.Message);
            }
            return Result;
        }

        public void HotspotWatchReset()
        {
            // a deliberate start (Save, app start) gets fresh restart attempts
            cHotspotRestarts = 0;
        }

        public void HotspotAsync(string Action, Action Done)
        {
            // status, start or stop on a pool thread (2-5 s); results and Done on the UI thread
            cHotspotBusy = true;
            ThreadPool.QueueUserWorkItem(o =>
            {
                string[] Parts = RunHotspot(Action).Split(new char[] { '|' }, 4);
                Action Apply = () =>
                {
                    cHotspotState = Parts.Length == 4 && Parts[0] != "" ? Parts[0] : "Unknown";
                    if (Parts.Length == 4)
                    {
                        if (Parts[1] != "") cHotspotResult = Parts[1];
                        cHotspotSsid = Parts[2];
                        cHotspotPassword = Parts[3];
                    }
                    cHotspotBusy = false;
                    Done?.Invoke();
                };
                try
                {
                    Core.MainForm.BeginInvoke(Apply);
                }
                catch (Exception ex)
                {
                    cHotspotBusy = false;
                    Props.WriteErrorLog("clsWebServer/HotspotAsync: " + ex.Message);
                }
            });
        }

        public bool HotspotAllowed()
        {
            // reading firewall rules needs no admin, netsh exits 1 when the rule isn't there
            bool Result = false;
            try
            {
                ProcessStartInfo Info = new ProcessStartInfo("netsh", "advfirewall firewall show rule name=\"" + HotspotRule + "\"");
                Info.UseShellExecute = false;
                Info.CreateNoWindow = true;
                Info.RedirectStandardOutput = true;
                using (Process P = Process.Start(Info))
                {
                    P.StandardOutput.ReadToEnd();
                    P.WaitForExit(5000);
                    Result = P.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsWebServer/HotspotAllowed: " + ex.Message);
            }
            return Result;
        }

        public List<string> PhoneUrls()
        {
            // addresses a phone could reach, most likely first: the tablet's own hotspot,
            // then WiFi (the phone's hotspot), then adapters with a gateway, then the rest,
            // virtual adapters last. module network and link-local excluded
            List<string>[] Ranked = { new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() };
            string ModuleNet = Core.UDPmodules.SubNet + ".";
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        IPInterfaceProperties IpProps = ni.GetIPProperties();
                        bool Gateway = false;
                        foreach (GatewayIPAddressInformation gw in IpProps.GatewayAddresses)
                        {
                            if (gw.Address.AddressFamily == AddressFamily.InterNetwork && !gw.Address.Equals(IPAddress.Any)) Gateway = true;
                        }
                        bool Hotspot = ni.Description.IndexOf("Wi-Fi Direct", StringComparison.OrdinalIgnoreCase) >= 0;     // Windows Mobile Hotspot
                        bool Virtual = !Hotspot && (ni.Description.IndexOf("Virtual", StringComparison.OrdinalIgnoreCase) >= 0
                            || ni.Description.IndexOf("Hyper-V", StringComparison.OrdinalIgnoreCase) >= 0);

                        int Rank = 3;
                        if (Hotspot) Rank = 0;
                        else if (Virtual) Rank = 4;
                        else if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) Rank = 1;
                        else if (Gateway) Rank = 2;

                        foreach (UnicastIPAddressInformation ip in IpProps.UnicastAddresses)
                        {
                            string Ip = ip.Address.ToString();
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !Ip.StartsWith(ModuleNet) && !Ip.StartsWith("169.254."))
                            {
                                Ranked[Rank].Add("http://" + Ip + ":" + cPort.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsWebServer/PhoneUrls: " + ex.Message);
            }

            List<string> Result = new List<string>();
            foreach (List<string> r in Ranked) Result.AddRange(r);
            return Result;
        }

        private static string Esc(string Text)
        {
            // json string content
            StringBuilder sb = new StringBuilder();
            foreach (char c in Text ?? "")
            {
                if (c == '"' || c == '\\') sb.Append('\\').Append(c);
                else if (c < ' ') sb.Append(' ');
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private static string Num(double Value)
        {
            // json number, invariant culture
            string Result = "0";
            if (!double.IsNaN(Value) && !double.IsInfinity(Value)) Result = Math.Round(Value, 2).ToString(CultureInfo.InvariantCulture);
            return Result;
        }

        private static Dictionary<string, string> Query(string Path)
        {
            Dictionary<string, string> Result = new Dictionary<string, string>();
            int Start = Path.IndexOf('?');
            if (Start >= 0)
            {
                foreach (string Pair in Path.Substring(Start + 1).Split('&'))
                {
                    int Eq = Pair.IndexOf('=');
                    if (Eq > 0) Result[Pair.Substring(0, Eq)] = Uri.UnescapeDataString(Pair.Substring(Eq + 1));
                }
            }
            return Result;
        }

        private static bool QueryNumber(Dictionary<string, string> Q, string Key, out double Value)
        {
            Value = 0;
            return Q.TryGetValue(Key, out string s) && double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out Value);
        }

        private int CalProduct(Dictionary<string, string> Q)
        {
            // requested product, else the tablet's current product
            int Count = Core.CalSession.Cals.Items.Count;
            int Result = (Props.CurrentProduct >= 0 && Props.CurrentProduct < Count) ? Props.CurrentProduct : 0;
            if (Q.TryGetValue("p", out string s) && int.TryParse(s, out int v) && v >= 0 && v < Count) Result = v;
            return Result;
        }

        private string CalPress(Dictionary<string, string> Q)
        {
            clsCalibrationSession CS = Core.CalSession;
            CS.WebSeen();
            int P = CalProduct(Q);
            clsCalibrate Cal = CS.Cals.Item(P);
            bool Running = CS.IsRunning();
            switch (Q.TryGetValue("id", out string ID) ? ID : "")
            {
                case "power":
                    if (!Running) Cal.PowerOn = !Cal.PowerOn;
                    break;

                case "lock":
                    if (!Running && Cal.PowerOn) Cal.Locked = !Cal.Locked;
                    break;

                case "start":
                    cCalMsg = CS.Start(P, true);
                    if (!CS.IsRunning() && cCalMsg == "") cCalMsg = "Turn on Power for a product first.";
                    break;

                case "stop":
                    CS.Stop();
                    cCalMsg = Cal.Locked ? "Enter measured amount." : "";
                    break;

                case "save":
                    if (!Running)
                    {
                        // same order as the form's Save
                        if (QueryNumber(Q, "s", out double s) && s >= 0 && s <= 40) Props.SimSpeed_KMH = Props.UseMetric ? s : s * Props.MPHtoKPH;
                        if (QueryNumber(Q, "b", out double b)) Cal.BaseRate = b;
                        if (QueryNumber(Q, "m", out double m)) Cal.MeterCal = m;
                        if (QueryNumber(Q, "me", out double me)) Cal.MeasuredAmount = me;
                        if (QueryNumber(Q, "w", out double w)) Cal.CalPWM = (int)Math.Round(w);
                        CS.Cals.Save();
                        cCalMsg = Props.ReadOnly ? "Read-only profile, not saved." : "Saved.";
                    }
                    break;
            }
            return CalStatusJson(P);
        }

        private void CalSession_StepOneComplete(object sender, int ID)
        {
            cCalMsg = "Step 1 complete for product " + (ID + 1).ToString() + ". Run again and measure the output.";
        }

        private string CalStatus(Dictionary<string, string> Q)
        {
            Core.CalSession.WebSeen();
            return CalStatusJson(CalProduct(Q));
        }

        private string CalStatusJson(int P)
        {
            clsCalibrationSession CS = Core.CalSession;
            clsCalibrate Cal = CS.Cals.Item(P);
            clsProduct Prd = Cal.Product;
            bool Running = CS.IsRunning();
            bool Motor = Prd.ControlType == ControlTypeEnum.Motor || Prd.ControlType == ControlTypeEnum.MotorWeights
                || Prd.ControlType == ControlTypeEnum.Fan;
            double Speed = Props.UseMetric ? Props.SimSpeed_KMH : Props.SimSpeed_KMH / Props.MPHtoKPH;

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"p\":").Append(P);
            sb.Append(",\"name\":\"").Append(Esc((P + 1).ToString() + ". " + Prd.ProductName + " - " + Props.ControlTypeDescription(Prd.ControlType))).Append("\"");
            sb.Append(",\"powers\":[");
            for (int i = 0; i < CS.Cals.Items.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(Js(CS.Cals.Items[i].PowerOn));
            }
            sb.Append("]");
            sb.Append(",\"power\":").Append(Js(Cal.PowerOn));
            sb.Append(",\"locked\":").Append(Js(Cal.Locked));
            sb.Append(",\"running\":").Append(Js(Running));
            sb.Append(",\"webOwned\":").Append(Js(CS.WebOwned));
            sb.Append(",\"phoneLost\":").Append(Js(CS.StopReason == CalStopReasons.PhoneLost));
            sb.Append(",\"motor\":").Append(Js(Motor));
            sb.Append(",\"pulses\":").Append(Cal.PulseCount);
            sb.Append(",\"hz\":").Append(Num(Prd.Hz()));
            sb.Append(",\"pwm\":").Append((Running && !Cal.Locked) ? (int)Prd.PWM() : Cal.CalPWM);
            sb.Append(",\"expected\":").Append(Num(Cal.ExpectedAmount));
            sb.Append(",\"measured\":").Append(Num(Cal.MeasuredAmount));
            sb.Append(",\"meterCal\":").Append(Num(Cal.MeterCal));
            sb.Append(",\"baseRate\":").Append(Num(Prd.RateSet));
            sb.Append(",\"speed\":").Append(Num(Speed));
            sb.Append(",\"units\":\"").Append(Props.UseMetric ? "KMH" : "MPH").Append("\"");
            sb.Append(",\"powerEdit\":").Append(Js(!Running));
            sb.Append(",\"lockEdit\":").Append(Js(!Running && Cal.PowerOn));
            sb.Append(",\"speedEdit\":").Append(Js(!Running));
            sb.Append(",\"baseEdit\":").Append(Js(!Running && Cal.PowerOn && !Cal.Locked));
            sb.Append(",\"measEdit\":").Append(Js(!Running && Cal.PowerOn && Cal.Locked && Cal.TestingRate));
            sb.Append(",\"pwmEdit\":").Append(Js(!Running && Cal.PowerOn && Cal.Locked && Motor));
            sb.Append(",\"msg\":\"").Append(Esc(cCalMsg)).Append("\"");
            sb.Append(",\"back\":\"").Append(Hex(Properties.Settings.Default.DisplayBackColour)).Append("\"");
            sb.Append(",\"fore\":\"").Append(Hex(Properties.Settings.Default.DisplayForeColour)).Append("\"");
            sb.Append(",\"on\":\"").Append(Hex(Core.Tls.SwitchOnColor())).Append("\"}");
            return sb.ToString();
        }

        private void HandleClient(TcpClient Client)
        {
            // runs on a pool thread so an idle connection can't hold up other requests
            using (Client)
            {
                try
                {
                    NetworkStream Stream = Client.GetStream();
                    Stream.ReadTimeout = 2000;

                    // request line + headers only, presses carry no body
                    byte[] Buffer = new byte[4096];
                    int Count = 0;
                    bool Open = true;
                    string Request = "";
                    while (Open && Count < Buffer.Length && !Request.Contains("\r\n\r\n"))
                    {
                        int n = Stream.Read(Buffer, Count, Buffer.Length - Count);
                        if (n == 0)
                        {
                            Open = false;
                        }
                        else
                        {
                            Count += n;
                            Request = Encoding.ASCII.GetString(Buffer, 0, Count);
                        }
                    }

                    string[] Parts = Request.Split(' ');
                    string Method = Parts.Length > 1 ? Parts[0] : "";
                    string Path = Parts.Length > 1 ? Parts[1] : "";

                    if (Request == "")
                    {
                        // connection opened and closed with nothing sent, no reply
                    }
                    else if (Method == "POST" && Request.IndexOf("\r\nX-RC:", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        // presses need the page's custom header, another site can't add it without a CORS preflight
                        Send(Stream, "403 Forbidden", "text/plain", "Forbidden");
                    }
                    else if (Method == "GET" && Path == "/")
                    {
                        Send(Stream, "200 OK", "text/html; charset=utf-8", MenuPage);
                    }
                    else if (Method == "GET" && Path == "/switches")
                    {
                        Send(Stream, "200 OK", "text/html; charset=utf-8", SwitchesPage);
                    }
                    else if (Method == "GET" && Path == "/cal")
                    {
                        Send(Stream, "200 OK", "text/html; charset=utf-8", CalPage);
                    }
                    else if (Method == "GET" && Path.StartsWith("/calstatus"))
                    {
                        Dictionary<string, string> Q = Query(Path);
                        Send(Stream, "200 OK", "application/json", OnUI(() => CalStatus(Q)));
                    }
                    else if (Method == "GET" && Path == "/status")
                    {
                        Send(Stream, "200 OK", "application/json", OnUI(StatusJson));
                    }
                    else if (Method == "POST" && Path.StartsWith("/press?id="))
                    {
                        string ID = Path.Substring("/press?id=".Length);
                        Send(Stream, "200 OK", "application/json", OnUI(() => Press(ID)));
                    }
                    else if (Method == "POST" && Path.StartsWith("/calpress?"))
                    {
                        Dictionary<string, string> Q = Query(Path);
                        Send(Stream, "200 OK", "application/json", OnUI(() => CalPress(Q)));
                    }
                    else
                    {
                        Send(Stream, "404 Not Found", "text/plain", "Not found");
                    }
                }
                catch (IOException)
                {
                    // idle preconnect timed out or the phone dropped the connection, normal
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("clsWebServer/HandleClient: " + ex.Message);
                }
            }
        }

        private void Listen()
        {
            while (cRunning)
            {
                try
                {
                    TcpClient Client = Listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(o => HandleClient(Client));
                }
                catch (Exception ex)
                {
                    // Stop() throws out of AcceptTcpClient, only log while running
                    if (cRunning) Props.WriteErrorLog("clsWebServer/Listen: " + ex.Message);
                }
            }
        }

        private string OnUI(Func<string> Fn)
        {
            string Result = "{}";
            try
            {
                Result = (string)Core.MainForm.Invoke(Fn);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsWebServer/OnUI: " + ex.Message);
            }
            return Result;
        }

        private void HotspotWatch_Tick(object sender, EventArgs e)
        {
            if (Props.TabletHotspot && !cHotspotBusy) HotspotAsync("status", HotspotWatchDone);
        }

        private void HotspotWatchDone()
        {
            // up to HotspotMaxRestarts tries after the hotspot is seen off, reset once it's seen on.
            // Unknown (the check itself failed) isn't counted
            if (cHotspotState == "On")
            {
                cHotspotRestarts = 0;
            }
            else if (cHotspotState == "Off" && Props.TabletHotspot && cHotspotRestarts < HotspotMaxRestarts)
            {
                cHotspotRestarts++;
                Props.WriteErrorLog("clsWebServer/HotspotWatch: hotspot off, restart " + cHotspotRestarts.ToString() + " of " + HotspotMaxRestarts.ToString()
                    + (cHotspotResult != "" && cHotspotResult != "Success" ? ", last: " + cHotspotResult : ""));
                HotspotAsync("start", null);
            }
        }

        private string RunHotspot(string Action)
        {
            string Result = "";
            try
            {
                string Script = HotspotScript.Replace("ACTION", Action);
                string Encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(Script));
                ProcessStartInfo Info = new ProcessStartInfo("powershell.exe", "-NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand " + Encoded);
                Info.UseShellExecute = false;
                Info.CreateNoWindow = true;
                Info.RedirectStandardOutput = true;
                Info.RedirectStandardError = true;
                using (Process P = Process.Start(Info))
                {
                    string Output = P.StandardOutput.ReadToEnd();
                    string Errors = P.StandardError.ReadToEnd();
                    P.WaitForExit(40000);
                    Result = Output.Trim();
                    if (Errors.Trim() != "") Props.WriteErrorLog("clsWebServer/RunHotspot " + Action + ": " + Errors.Trim());
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsWebServer/RunHotspot: " + ex.Message);
            }
            return Result;
        }

        private string Press(string ID)
        {
            PGN32618 SB = Core.SwitchBox;
            switch (ID)
            {
                case "master": SB.RemoteMasterPressed(); break;
                case "prime": SB.RemotePrimePressed(); break;
                case "autorate": SB.RemoteAutoRatePressed(); break;
                case "autosection": SB.RemoteAutoSectionPressed(); break;
                case "up": SB.RemoteRateHold(1); break;
                case "down": SB.RemoteRateHold(-1); break;

                // held repeats and release never restart a session the cab ended
                case "uprep": if (SB.RemoteActive) SB.RemoteRateHold(1); break;
                case "downrep": if (SB.RemoteActive) SB.RemoteRateHold(-1); break;
                case "release": if (SB.RemoteActive) SB.RemoteRateHold(0); break;

                default:
                    if (int.TryParse(ID, out int Sw) && Sw >= 0 && Sw < Props.SwitchCount) SB.RemoteSectionPressed(Sw);
                    break;
            }
            return StatusJson();
        }

        private static string Hex(Color C)
        {
            return "#" + C.R.ToString("X2") + C.G.ToString("X2") + C.B.ToString("X2");
        }

        private static string Js(bool Value)
        {
            return Value ? "true" : "false";
        }

        private void Send(NetworkStream Stream, string Status, string ContentType, string Body)
        {
            byte[] Data = Encoding.UTF8.GetBytes(Body);
            string Header = "HTTP/1.1 " + Status + "\r\n"
                + "Content-Type: " + ContentType + "\r\n"
                + "Content-Length: " + Data.Length.ToString() + "\r\n"
                + "Cache-Control: no-store\r\n"
                + "Connection: close\r\n\r\n";
            byte[] Head = Encoding.ASCII.GetBytes(Header);
            Stream.Write(Head, 0, Head.Length);
            Stream.Write(Data, 0, Data.Length);
        }

        private string StatusJson()
        {
            // same states the on-screen panel shows
            PGN32618 SB = Core.SwitchBox;
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"master\":").Append(Js(SB.MasterOn));
            sb.Append(",\"prime\":").Append(Js(Core.SectionControl.PrimeOn));
            sb.Append(",\"primeEnabled\":").Append(Js(!Props.MasterMaintained && Props.Speed_KMH < 0.1));
            sb.Append(",\"up\":").Append(Js(SB.SwitchIsOn(SwIDs.RateUp)));
            sb.Append(",\"down\":").Append(Js(SB.SwitchIsOn(SwIDs.RateDown)));
            sb.Append(",\"autoRate\":").Append(Js(SB.AutoRateOn));
            sb.Append(",\"autoSection\":").Append(Js(SB.AutoSectionOn));
            sb.Append(",\"aog\":").Append(Js(Core.AutoSteerPGN.Connected()));
            sb.Append(",\"remote\":").Append(Js(SB.RemoteActive));
            sb.Append(",\"secondsLeft\":").Append(SB.RemoteSecondsLeft);
            sb.Append(",\"ended\":\"").Append(SB.RemoteEndReason.ToString()).Append("\"");
            sb.Append(",\"back\":\"").Append(Hex(Properties.Settings.Default.DisplayBackColour)).Append("\"");
            sb.Append(",\"fore\":\"").Append(Hex(Properties.Settings.Default.DisplayForeColour)).Append("\"");
            sb.Append(",\"on\":\"").Append(Hex(Core.Tls.SwitchOnColor())).Append("\"");
            sb.Append(",\"switches\":[");
            for (int i = 0; i < Props.SwitchCount; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(Js(SB.SwitchIsOn((SwIDs)((int)SwIDs.sw0 + i))));
            }
            sb.Append("]}");
            return sb.ToString();
        }

        private const string CalPage = @"<!doctype html>
<html><head><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'>
<title>RateController Calibration</title>
<style>
body{font-family:sans-serif;margin:0;padding:12px;-webkit-user-select:none;user-select:none}
h1{font-size:22px;margin:0 0 8px;text-align:center}
#back{display:block;width:60%;margin:16px auto 0;padding:16px 0;font-size:22px;text-align:center;border:1px solid;border-radius:8px;color:inherit;text-decoration:none}
#prods,.row{display:grid;gap:10px;margin-bottom:10px}
#prods{grid-template-columns:repeat(5,1fr)}
.row{grid-template-columns:1fr 1fr}
button{font-size:20px;padding:14px 0;border-radius:8px;border:1px solid;background:transparent;color:inherit}
button:disabled,input:disabled{opacity:.35}
.sel{border-width:3px}
#name{font-size:18px;text-align:center;margin-bottom:6px}
#msg{min-height:1.3em;margin-bottom:4px}
#warn{color:#e90;min-height:1.2em;margin-bottom:8px}
table{width:100%;font-size:18px;border-collapse:collapse;margin-bottom:10px}
td{padding:4px 0}
td+td{text-align:right}
input{width:7em;font-size:18px;text-align:right;padding:4px;border-radius:4px;border:1px solid;background:transparent;color:inherit}
</style></head><body>
<h1>RateController Calibration</h1>
<div id='prods'></div>
<div id='name'></div>
<div id='msg'>Connecting...</div><div id='warn'></div>
<div class='row'><button id='power' onclick=""act('power')"">Power</button><button id='lock' onclick=""act('lock')"">Lock</button></div>
<div class='row'><button id='start' onclick=""act('start')"">Start</button><button id='stop' onclick=""act('stop')"">Stop</button></div>
<table>
<tr><td>Pulses</td><td id='pulses'></td></tr>
<tr><td>Hz</td><td id='hz'></td></tr>
<tr><td id='su'>Speed</td><td><input id='s' inputmode='decimal'></td></tr>
<tr><td>Base Rate</td><td><input id='b' inputmode='decimal'></td></tr>
<tr><td>Meter Cal</td><td><input id='m' inputmode='decimal'></td></tr>
<tr><td>Expected</td><td id='expected'></td></tr>
<tr><td>Measured</td><td><input id='me' inputmode='decimal'></td></tr>
<tr id='pwmrow'><td>PWM</td><td><input id='w' inputmode='numeric'></td></tr>
</table>
<div class='row'><button id='cancel' onclick='cancel()'>Cancel</button><button id='save' onclick='save()'>Save</button></div>
<a id='back' href='/'>&#8249; Menu</a>
<script>
var P=-1,dirty=false,on='#90ee90',built=0,F=['s','b','m','me','w'];
function q(id){return document.getElementById(id)}
function get(url,opt){return fetch(url,opt).then(function(r){return r.json()}).then(show)}
function act(id,extra){get('/calpress?id='+id+'&p='+P+(extra||''),{method:'POST',headers:{'X-RC':'1'}}).catch(function(){});}
function save(){var x='';F.forEach(function(f){x+='&'+f+'='+encodeURIComponent(q(f).value)});dirty=false;act('save',x);}
function cancel(){dirty=false;poll();}
function pick(i){if(!dirty){P=i;poll();}}
F.forEach(function(f){q(f).oninput=function(){dirty=true;q('save').disabled=false;q('cancel').disabled=false;};});
F.forEach(function(f){var e=q(f);e.onfocus=function(){e.placeholder=e.value;e.value='';};e.onblur=function(){if(e.value==='')e.value=e.placeholder;e.placeholder='';};});
function fill(id,v){if(!dirty&&document.activeElement!==q(id))q(id).value=v;}
function show(s){
 document.body.style.background=s.back;document.body.style.color=s.fore;on=s.on;P=s.p;
 var pr=q('prods'),i,b;
 if(built!=s.powers.length){pr.innerHTML='';for(i=0;i<s.powers.length;i++){b=document.createElement('button');b.textContent=i+1;b.id='p'+i;b.onclick=(function(n){return function(){pick(n)}})(i);pr.appendChild(b);}built=s.powers.length;}
 for(i=0;i<s.powers.length;i++){b=q('p'+i);b.style.background=s.powers[i]?on:'transparent';b.className=(i==s.p)?'sel':'';b.disabled=dirty;}
 q('name').textContent=s.name;
 q('msg').textContent=s.running?(s.locked?'Testing rate, press Stop when done':'Setting calibration speed')+(s.webOwned?' - keep this page open':''):s.msg;
 q('warn').textContent=s.phoneLost?'Last run stopped: phone connection lost':'';
 q('power').style.background=s.power?on:'transparent';q('power').disabled=!s.powerEdit;
 q('lock').textContent=s.locked?'Meter set':'Meter not set';q('lock').style.background=s.locked?on:'transparent';q('lock').disabled=!s.lockEdit;
 q('start').style.background=s.running?on:'transparent';q('start').disabled=s.running;q('stop').disabled=!s.running;
 q('pulses').textContent=s.pulses;q('hz').textContent=s.hz.toFixed(1);q('expected').textContent=s.expected.toFixed(1);
 q('su').textContent='Speed ('+s.units+')';q('pwmrow').style.display=s.motor?'':'none';
 fill('s',s.speed.toFixed(1));fill('b',s.baseRate.toFixed(1));fill('m',s.meterCal.toFixed(1));fill('me',s.measured.toFixed(1));fill('w',s.pwm);
 q('s').disabled=!s.speedEdit;q('b').disabled=!s.baseEdit;q('m').disabled=!s.baseEdit;q('me').disabled=!s.measEdit;q('w').disabled=!s.pwmEdit;
 q('save').disabled=!dirty||s.running;q('cancel').disabled=!dirty;
}
function poll(){if(document.hidden)return;get('/calstatus?p='+P).catch(function(){q('msg').textContent='No connection';});}
setInterval(poll,1000);poll();
</script></body></html>";

        private const string MenuPage = @"<!doctype html>
<html><head><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'>
<title>RateController</title>
<style>
body{font-family:sans-serif;margin:0;padding:12px}
h1{font-size:26px;margin:8px 0 24px;text-align:center}
a{display:block;font-size:24px;padding:24px 0;margin:0 0 16px;border:1px solid;border-radius:8px;text-align:center;text-decoration:none;color:inherit}
</style></head><body>
<h1>RateController</h1>
<a href='/switches'>Switches</a>
<a href='/cal'>Calibration</a>
<script>
fetch('/status').then(function(r){return r.json()}).then(function(s){document.body.style.background=s.back;document.body.style.color=s.fore;}).catch(function(){});
</script></body></html>";

        private const string SwitchesPage = @"<!doctype html>
<html><head><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'>
<title>RateController Switches</title>
<style>
body{font-family:sans-serif;margin:0;padding:12px;-webkit-user-select:none;user-select:none;-webkit-touch-callout:none}
h1{font-size:22px;margin:0 0 8px;text-align:center}
#msg{font-size:18px;min-height:1.4em}
#warn{color:#e90;min-height:1.2em;margin-bottom:8px}
#pad{display:grid;grid-template-columns:repeat(4,1fr);gap:10px}
button{font-size:22px;padding:18px 0;border-radius:8px;border:1px solid;background:transparent;color:inherit;touch-action:none}
button:disabled{opacity:.35}
.wide{grid-column:span 2}
#back{display:block;width:60%;margin:16px auto 0;padding:16px 0;font-size:22px;text-align:center;border:1px solid;border-radius:8px;color:inherit;text-decoration:none}
</style></head><body>
<h1>RateController Switches</h1>
<div id='msg'>Connecting...</div><div id='warn'></div>
<div id='pad'></div>
<a id='back' href='/'>&#8249; Menu</a>
<script>
var on='#90ee90',built=-1,timer=null;
function post(id){return fetch('/press?id='+id,{method:'POST',headers:{'X-RC':'1'}}).then(function(r){return r.json()}).then(show).catch(function(){});}
function btn(label,id,cls){var b=document.createElement('button');b.textContent=label;b.id='b_'+id;if(cls)b.className=cls;b.onclick=function(){post(id)};return b;}
function rate(label,id){var b=document.createElement('button');b.textContent=label;b.id='b_'+id;
 b.onpointerdown=function(e){e.preventDefault();stop();post(id);timer=setInterval(function(){post(id+'rep')},250);};
 b.onpointerup=b.onpointercancel=b.onpointerleave=stop;b.oncontextmenu=function(e){e.preventDefault()};return b;}
function stop(){if(timer){clearInterval(timer);timer=null;post('release');}}
document.addEventListener('visibilitychange',stop);
function build(n){var p=document.getElementById('pad');p.innerHTML='';
 p.appendChild(btn('MST','master'));p.appendChild(btn('PRM','prime'));p.appendChild(rate('\u25B2','up'));p.appendChild(rate('\u25BC','down'));
 for(var i=0;i<n;i++)p.appendChild(btn(String(i+1),String(i)));
 p.appendChild(btn('Auto Rate','autorate','wide'));p.appendChild(btn('Auto Section','autosection','wide'));built=n;}
function lit(id,v){var b=document.getElementById('b_'+id);if(b)b.style.background=v?on:'transparent';}
function show(s){
 document.body.style.background=s.back;document.body.style.color=s.fore;on=s.on;
 if(built!=s.switches.length)build(s.switches.length);
 lit('master',s.master);lit('prime',s.prime);lit('up',s.up);lit('down',s.down);lit('autorate',s.autoRate);lit('autosection',s.autoSection);
 document.getElementById('b_prime').disabled=!s.primeEnabled;
 for(var i=0;i<s.switches.length;i++)lit(String(i),s.switches[i]);
 document.getElementById('msg').textContent=s.remote?'Remote in control, '+s.secondsLeft+' s':(s.ended=='Cab'?'Cab switchbox took control':(s.ended=='Timeout'?'Timed out, master off':'Cab in control'));
 document.getElementById('warn').textContent=(s.autoSection&&s.aog)?'Auto section is on - AOG controls sections':'';
}
function poll(){fetch('/status').then(function(r){return r.json()}).then(show).catch(function(){document.getElementById('msg').textContent='No connection';});}
setInterval(poll,1000);poll();
</script></body></html>";
    }
}
