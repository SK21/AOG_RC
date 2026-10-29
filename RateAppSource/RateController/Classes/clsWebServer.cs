using RateController.PGNs;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        private TcpListener Listener;
        private int cPort;
        private bool cRunning = false;
        private Thread ServerThread;

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

        public List<string> PhoneUrls()
        {
            // addresses a phone could reach, most likely first: WiFi (the hotspot),
            // then adapters with a gateway, then the rest, virtual adapters last.
            // module network and link-local excluded
            List<string>[] Ranked = { new List<string>(), new List<string>(), new List<string>(), new List<string>() };
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
                        bool Virtual = ni.Description.IndexOf("Virtual", StringComparison.OrdinalIgnoreCase) >= 0
                            || ni.Description.IndexOf("Hyper-V", StringComparison.OrdinalIgnoreCase) >= 0;

                        int Rank = 2;
                        if (Virtual) Rank = 3;
                        else if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) Rank = 0;
                        else if (Gateway) Rank = 1;

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

        private void HandleClient(TcpClient Client)
        {
            using (Client)
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

                if (Method == "GET" && Path == "/")
                {
                    Send(Stream, "200 OK", "text/html; charset=utf-8", Page);
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
                else
                {
                    Send(Stream, "404 Not Found", "text/plain", "Not found");
                }
            }
        }

        private void Listen()
        {
            while (cRunning)
            {
                try
                {
                    HandleClient(Listener.AcceptTcpClient());
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

        private const string Page = @"<!doctype html>
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
</style></head><body>
<h1>RateController Switches</h1>
<div id='msg'>Connecting...</div><div id='warn'></div>
<div id='pad'></div>
<script>
var on='#90ee90',built=-1,timer=null;
function post(id){return fetch('/press?id='+id,{method:'POST'}).then(function(r){return r.json()}).then(show).catch(function(){});}
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
