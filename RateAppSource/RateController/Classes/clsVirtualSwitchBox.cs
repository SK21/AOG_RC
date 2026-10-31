using RateController.PGNs;
using System;

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
        private bool WasRealConnected = false;

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
            get { return Core.SwitchBox.RealConnected() ? Core.SwitchBox.SwitchIsOn(SwIDs.AutoRate) : cAutoRate; }
        }

        public bool AutoSectionOn
        {
            get { return Core.SwitchBox.RealConnected() ? Core.SwitchBox.SwitchIsOn(SwIDs.AutoSection) : cAutoSection; }
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
            if (Core.SwitchBox.RealConnected())
            {
                // a real switchbox is connected, take over like the phone page until a cab switch moves
                Core.SwitchBox.RemoteMasterPressed(RemoteSources.Screen);
            }
            else
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
                    ReleaseTimer.Enabled = true;    // pulse, like a momentary switch
                    break;

                case SwIDs.MasterOff:
                    PGNdata[2] = Core.Tls.BitClear(PGNdata[2], 1);
                    PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 2);
                    ReleaseTimer.Enabled = true;    // pulse, like a momentary switch
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

        public void PrimePressed()
        {
            if (Core.SwitchBox.RealConnected())
            {
                Core.SwitchBox.RemotePrimePressed(RemoteSources.Screen);
            }
            else
            {
                PressSwitch(SwIDs.MasterOn);
                ReleaseTimer.Enabled = true;
                Core.SectionControl.StartPrime();
            }
        }

        public void RateDownPressed()
        {
            if (Core.SwitchBox.RealConnected())
            {
                Core.SwitchBox.RemoteRateHold(-1, RemoteSources.Screen);
            }
            else
            {
                RateDownIsPressed = true;
                PressSwitch(SwIDs.RateDown);
                SendTimer.Stop();   // reset to prevent double send
                SendTimer.Start();
                SendPGN();
                ReleaseTimer.Enabled = true;
            }
        }

        public void RateDownReleased()
        {
            RateDownIsPressed = false;
            PGNdata[2] = Core.Tls.BitClear(PGNdata[2], 4);
            PGNdata[5] = Core.Tls.CRC(PGNdata, 5);
            SendPGN();

            // never restarts a session the cab ended during the hold
            if (Core.SwitchBox.RemoteActive) Core.SwitchBox.RemoteRateHold(0, RemoteSources.Screen);
        }

        public void RateUpPressed()
        {
            if (Core.SwitchBox.RealConnected())
            {
                Core.SwitchBox.RemoteRateHold(1, RemoteSources.Screen);
            }
            else
            {
                RateUpIsPressed = true;
                PressSwitch(SwIDs.RateUp);
                SendTimer.Stop();   // reset to prevent double send
                SendTimer.Start();
                SendPGN();
                ReleaseTimer.Enabled = true;
            }
        }

        public void RateUpReleased()
        {
            RateUpIsPressed = false;
            PGNdata[2] = Core.Tls.BitClear(PGNdata[2], 3);
            PGNdata[5] = Core.Tls.CRC(PGNdata, 5);
            SendPGN();

            if (Core.SwitchBox.RemoteActive) Core.SwitchBox.RemoteRateHold(0, RemoteSources.Screen);
        }

        public void ScreenPress(SwIDs ID, bool FromMain = false)
        {
            // on-screen auto and section buttons. With a real switchbox connected the virtual
            // packet isn't sent, so these take over like the phone page until a cab switch moves.
            if (Core.SwitchBox.RealConnected())
            {
                switch (ID)
                {
                    case SwIDs.AutoRate:
                        Core.SwitchBox.RemoteAutoRatePressed(RemoteSources.Screen);
                        break;

                    case SwIDs.AutoSection:
                        Core.SwitchBox.RemoteAutoSectionPressed(RemoteSources.Screen);
                        break;

                    default:
                        if (ID >= SwIDs.sw0 && ID <= SwIDs.sw15) Core.SwitchBox.RemoteSectionPressed(ID - SwIDs.sw0, RemoteSources.Screen);
                        break;
                }
            }
            else
            {
                PressSwitch(ID, FromMain);
            }
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
            if (!Core.SwitchBox.RealConnected()) Core.SwitchBox.ParseByteData(PGNdata, false);
        }

        private void SendTimer_Tick(object sender, EventArgs e)
        {
            bool RealConnected = Core.SwitchBox.RealConnected();
            if (WasRealConnected && !RealConnected) SyncFromSwitchBox();
            WasRealConnected = RealConnected;
            SendPGN();
        }

        private void SyncFromSwitchBox()
        {
            // the real switchbox dropped out, carry on from the switch states in use instead of
            // the states this box last had before the switchbox connected
            PGNdata[3] = 0;
            PGNdata[4] = 0;
            for (int i = 0; i < Props.MaxSwitches; i++)
            {
                cSwitch[i] = Core.SwitchBox.SwitchIsOn(SwIDs.sw0 + i);
                if (cSwitch[i])
                {
                    if (i < 8)
                    {
                        PGNdata[3] = Core.Tls.BitSet(PGNdata[3], i);
                    }
                    else
                    {
                        PGNdata[4] = Core.Tls.BitSet(PGNdata[4], i - 8);
                    }
                }
            }

            cAutoSection = Core.SwitchBox.SwitchIsOn(SwIDs.AutoSection);
            cAutoRate = Core.SwitchBox.SwitchIsOn(SwIDs.AutoRate);

            // rate up and rate down are momentary, leave them released. A momentary master is
            // left released too, a maintained master is read as a level so it is kept.
            PGNdata[2] = 0;
            if (Props.MasterMaintained && Core.SwitchBox.MasterOn) PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 1);
            if (cAutoSection) PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 5);
            if (cAutoRate) PGNdata[2] = Core.Tls.BitSet(PGNdata[2], 6);
            PGNdata[5] = Core.Tls.CRC(PGNdata, 5);
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