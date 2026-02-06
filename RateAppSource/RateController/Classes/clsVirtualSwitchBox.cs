using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsVirtualSwitchBox
    {
        // PGN32618 to Rate Controller from virtual switch box
        // 0   106
        // 1   127
        // 2    - bit 0 -
        //      - bit 1 MasterOn
        //      - bit 2 MasterOff
        //      - bit 3 RateUp
        //      - bit 4 RateDown
        //      - bit 5 AutoSection
        //      - bit 6 AutoRate
        //		- bit 7 Work switch
        // 3    sw0 to sw7
        // 4    sw8 to sw15
        // 5    crc

        private bool cAutoRate = false;
        private bool cAutoSection = false;
        private bool[] cSwitch;
        private bool cUsefrmSwitches;
        private bool MasterIsPressed = false;
        private byte[] PGNdata;
        private bool RateDownIsPressed = false;
        private bool RateUpIsPressed = false;
        private System.Windows.Forms.Timer ReleaseTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer SendTimer = new System.Windows.Forms.Timer();

        public clsVirtualSwitchBox()
        {
            cSwitch = new bool[Props.MaxSwitches];
            PGNdata = new byte[6];
            PGNdata[0] = 106;
            PGNdata[1] = 127;

            SendTimer.Tick += SendTimer_Tick;
            SendTimer.Interval = 250;
            SendTimer.Enabled = true;

            ReleaseTimer.Tick += ReleaseTimer_Tick;
            ReleaseTimer.Interval = 500;

            Core.AppExit += Core_AppExit;
        }

        public bool AutoRateOn
        {
            get { return cAutoRate; }
        }

        public bool AutoSectionOn
        {
            get { return cAutoSection; }
        }

        public bool UsefrmSwitches
        {
            get { return cUsefrmSwitches; }
            set
            {
                cUsefrmSwitches = value;
            }
        }

        public void MasterPressed(bool FromMain = false)
        {
            if (Core.SwitchBox.MasterOn)
            {
                PressSwitch(SwIDs.MasterOff, FromMain);
            }
            else
            {
                PressSwitch(SwIDs.MasterOn, FromMain);
            }
            MasterIsPressed = true;
            ReleaseTimer.Enabled = true;
        }

        public void MasterReleased()
        {
            MasterIsPressed = false;
        }

        public void PressSwitch(SwIDs ID, bool FromMain = false)
        {
            // build PGN32618
            switch (ID)
            {
                case SwIDs.AutoRate:
                    if (cAutoRate)
                    {
                        // turn off
                        PGNdata[2] = Core.Tls.BitClear(PGNdata[2], 6);
                        cAutoRate = false;
                    }
                    else
                    {
                        // turn on
                        PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 6);
                        cAutoRate = true;
                    }
                    break;

                case SwIDs.AutoSection:
                    if (cAutoSection)
                    {
                        // turn off
                        PGNdata[2] = Core.Tls.BitClear(PGNdata[2], 5);
                        cAutoSection = false;
                    }
                    else
                    {
                        // turn on
                        PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 5);
                        cAutoSection = true;
                    }
                    break;

                case SwIDs.MasterOn:
                    PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 1);
                    PGNdata[2] = Core.Tls.BitClear(PGNdata[2], 2);
                    if (FromMain) SetSectionsSwitchesMain();
                    break;

                case SwIDs.MasterOff:
                    PGNdata[2] = Core.Tls.BitClear(PGNdata[2], 1);
                    PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 2);
                    break;

                case SwIDs.RateUp:
                    PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 3);
                    PGNdata[2] = Core.Tls.BitClear(PGNdata[2], 4);
                    break;

                case SwIDs.RateDown:
                    PGNdata[2] = Core.Tls.BitClear(PGNdata[2], 3);
                    PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 4);
                    break;

                default:
                    // switches
                    int Num = (int)ID - 5;

                    if (Num < 8)
                    {
                        if (cSwitch[Num])
                        {
                            // turn off, lo
                            PGNdata[3] = Core.Tls.BitClear(PGNdata[3], Num);
                            cSwitch[Num] = false;
                        }
                        else
                        {
                            // turn on, lo
                            PGNdata[3] = Core.Tls.BitSet(PGNdata[3], Num);
                            cSwitch[Num] = true;
                        }
                    }
                    else
                    {
                        if (cSwitch[Num])
                        {
                            // turn off, hi
                            PGNdata[4] = Core.Tls.BitClear(PGNdata[4], Num - 8);
                            cSwitch[Num] = false;
                        }
                        else
                        {
                            // turn on, hi
                            PGNdata[4] = Core.Tls.BitSet(PGNdata[4], Num - 8);
                            cSwitch[Num] = true;
                        }
                    }
                    break;
            }
            PGNdata[5] = Core.Tls.CRC(PGNdata, 5);
        }

        public void RateDownPressed()
        {
            RateDownIsPressed = true;
            SendTimer.Stop();   // reset to prevent double send
            SendTimer.Start();
            SendPGN();
            ReleaseTimer.Enabled = true;
        }

        public void RateDownReleased()
        {
            RateDownIsPressed = false;
        }

        public void RateUpPressed()
        {
            RateUpIsPressed = true;
            SendTimer.Stop();   // reset to prevent double send
            SendTimer.Start();
            SendPGN();
            ReleaseTimer.Enabled = true;
        }

        public void RateUpReleased()
        {
            RateUpIsPressed = false;
        }

        private void Core_AppExit(object sender, EventArgs e)
        {
            SendTimer.Stop();
            ReleaseTimer.Stop();
            Core.AppExit -= Core_AppExit;
        }

        private void ReleaseTimer_Tick(object sender, EventArgs e)
        {
            if (!MasterIsPressed && !RateDownIsPressed && !RateUpIsPressed)
            {
                // set Master bit, Rate Up bit and Rate Down bit to off
                PGNdata[2] = (byte)(PGNdata[2] & 0b11100001);
                PGNdata[5] = Core.Tls.CRC(PGNdata, 5);
                ReleaseTimer.Enabled = false;
            }
        }

        private void SendPGN()
        {
            if (!Core.SwitchBox.RealConnected())
            {
                Core.SwitchBox.ParseByteData(PGNdata, false);
                if (RateDownIsPressed) PressSwitch(SwIDs.RateDown);
                if (RateUpIsPressed) PressSwitch(SwIDs.RateUp);
            }
        }

        private void SendTimer_Tick(object sender, EventArgs e)
        {
            SendPGN();
        }

        private void SetSectionsSwitchesMain()
        {
            // Master switch form main form also turns on all sections
            foreach (clsSection Sec in Core.Sections.Items)
            {
                if (Sec.Enabled)
                {
                    int ID = Sec.SwitchID;
                    cSwitch[ID] = true;
                    if (ID < 8)
                    {
                        PGNdata[3] = (byte)(PGNdata[3] | (byte)(Math.Pow(2, ID)));
                    }
                    else
                    {
                        PGNdata[4] = (byte)(PGNdata[4] | (byte)(Math.Pow(2, ID - 8)));
                    }
                }
            }
        }
    }
}
