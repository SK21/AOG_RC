using System;

namespace RateController
{
    public class clsVirtualSwitchBox
    {
        // to Rate Controller from virtual switch box
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

        private bool cAutoRate;
        private bool cAutoSection;
        private bool cEnabled;
        private bool cLargeScreenOn;
        private bool[] cSwitch;
        private bool cSwitchScreenOn;
        private FormStart mf;
        private byte[] PressedData;
        private System.Windows.Forms.Timer Timer1 = new System.Windows.Forms.Timer();

        public clsVirtualSwitchBox(FormStart CalledFrom)
        {
            mf = CalledFrom;
            cSwitch = new bool[mf.MaxSwitches];
            PressedData = new byte[6];
            PressedData[0] = 106;
            PressedData[1] = 127;
            Timer1.Tick += new EventHandler(TimerEventProcessor);
            Timer1.Interval = 250;
        }

        public bool AutoRateOn
        {
            get { return cAutoRate; }
        }

        public bool AutoSectionOn
        {
            get { return cAutoSection; }
        }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                if (value && !phySwitchboxConnected())
                {
                    Timer1.Start();
                    cEnabled = true;
                }
                else
                {
                    if ((!cSwitchScreenOn && !cLargeScreenOn) || phySwitchboxConnected())  // disable if physical switchbox connected
                    {
                        Timer1.Stop();
                        cEnabled = false;
                    }
                }
            }
        }

        public bool LargeScreenOn
        {
            get { return cLargeScreenOn; }
            set
            {
                cLargeScreenOn = value;
                Enabled = value;
            }
        }

        public bool SwitchScreenOn
        {
            get { return cSwitchScreenOn; }
            set
            {
                cSwitchScreenOn = value;
                Enabled = value;
            }
        }

        public void PressSwitch(SwIDs ID, bool FromLargeScreen = false)
        {
            // build PGN32618
            switch (ID)
            {
                case SwIDs.AutoRate:
                    if (cAutoRate)
                    {
                        // turn off
                        PressedData[2] = mf.Tls.BitClear(PressedData[2], 6);
                        cAutoRate = false;
                    }
                    else
                    {
                        // turn on
                        PressedData[2] = mf.Tls.BitSet(PressedData[2], 6);
                        cAutoRate = true;
                    }
                    break;

                case SwIDs.AutoSection:
                    if (cAutoSection)
                    {
                        // turn off
                        PressedData[2] = mf.Tls.BitClear(PressedData[2], 5);
                        cAutoSection = false;
                    }
                    else
                    {
                        // turn on
                        PressedData[2] = mf.Tls.BitSet(PressedData[2], 5);
                        cAutoSection = true;
                    }
                    break;

                case SwIDs.MasterOn:
                    PressedData[2] = mf.Tls.BitSet(PressedData[2], 1);
                    PressedData[2] = mf.Tls.BitClear(PressedData[2], 2);
                    if (FromLargeScreen) SetSwitchesLS();
                    break;

                case SwIDs.MasterOff:
                    PressedData[2] = mf.Tls.BitClear(PressedData[2], 1);
                    PressedData[2] = mf.Tls.BitSet(PressedData[2], 2);
                    break;

                case SwIDs.RateUp:
                    PressedData[2] = mf.Tls.BitSet(PressedData[2], 3);
                    PressedData[2] = mf.Tls.BitClear(PressedData[2], 4);
                    break;

                case SwIDs.RateDown:
                    PressedData[2] = mf.Tls.BitClear(PressedData[2], 3);
                    PressedData[2] = mf.Tls.BitSet(PressedData[2], 4);
                    break;

                default:
                    // switches
                    int Num = (int)ID - 5;

                    if (Num < 8)
                    {
                        if (cSwitch[Num])
                        {
                            // turn off, lo
                            PressedData[3] = mf.Tls.BitClear(PressedData[3], Num);
                            cSwitch[Num] = false;
                        }
                        else
                        {
                            // turn on, lo
                            PressedData[3] = mf.Tls.BitSet(PressedData[3], Num);
                            cSwitch[Num] = true;
                        }
                    }
                    else
                    {
                        if (cSwitch[Num])
                        {
                            // turn off, hi
                            PressedData[4] = mf.Tls.BitClear(PressedData[4], Num - 8);
                            cSwitch[Num] = false;
                        }
                        else
                        {
                            // turn on, hi
                            PressedData[4] = mf.Tls.BitSet(PressedData[4], Num - 8);
                            cSwitch[Num] = true;
                        }
                    }
                    break;
            }
            PressedData[5] = mf.Tls.CRC(PressedData, 5);
        }

        public void ReleaseSwitch()
        {
            PressedData[2] = (byte)(PressedData[2] & 0b11100001);
            PressedData[5] = mf.Tls.CRC(PressedData, 5);
        }

        private bool phySwitchboxConnected()
        {
            bool Result = false;
            if (mf.UDPmodules.SwitchBoxConnected)
            {
                Result = true;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (mf.SER[i].SwitchBoxConnected)
                    {
                        Result = true;
                        break;
                    }
                }
            }
            return Result;
        }

        private void SetSwitchesLS()
        {
            // set only section switches on when master pressed from large screen
            foreach (clsSection Sec in mf.Sections.Items)
            {
                if (Sec.Enabled)
                {
                    int ID = Sec.SwitchID;
                    cSwitch[ID] = true;
                    if (ID < 8)
                    {
                        PressedData[3] = (byte)(PressedData[3] | (byte)(Math.Pow(2, ID)));
                    }
                    else
                    {
                        PressedData[4] = (byte)(PressedData[4] | (byte)(Math.Pow(2, ID - 8)));
                    }
                }
            }
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            mf.SwitchBox.ParseByteData(PressedData);
        }
    }
}