namespace RateController
{
    public class clsSectionControl
    {
        private FormStart mf;

        public struct CTLbytes
        {
            public bool AutoOn;
            public bool MasterOn;
            public bool RateUp;
            public bool RateDown;
            public byte SwLo;       // switches 0-7
            public byte SwHi;       // switches 8-15
            public byte Rlys0;      // section relays 0-7
            public byte Rlys1;      // section relays 8-15
            public byte SBRlys0;    // relays 0-7 based on switchbox switch positions
            public byte SBRlys1;    // relays 8-15 based on switchbox switch positions
            public byte AOGRlys0;
            public byte AOGRlys1;
        }
        CTLbytes CTL = new CTLbytes();

        private PGN234 ToAOG;
        private int SendInterval = 50;

        public clsSectionControl(FormStart CallingForm)
        {
            mf = CallingForm;

            CTL.AutoOn = true;
            CTL.MasterOn = false;
            CTL.RateUp = false;
            CTL.RateDown = false;
            CTL.SwLo = 0;
            CTL.SwHi = 0;
            CTL.Rlys0 = 0;
            CTL.Rlys1 = 0;

            mf.AutoSteerPGN.RelaysChanged += AutoSteerPGN_RelaysChanged;
            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;

            ToAOG = new PGN234(mf);
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            CTL.AutoOn = mf.SwitchBox.SwitchOn(SwIDs.Auto);

            // set relay bytes based on switchbox
            if (mf.SwitchBox.SwitchOn(SwIDs.MasterOff))
            {
                CTL.SBRlys0 = 0;
                CTL.SBRlys1 = 0;
                CTL.MasterOn = false;
            }
            else if (mf.SwitchBox.SwitchOn(SwIDs.MasterOn) || CTL.MasterOn)
            {
                CTL.SBRlys0 = 0;
                CTL.SBRlys1 = 0;
                CTL.MasterOn = true;

                foreach (clsSection Sec in mf.Sections.Items)
                {
                    if (mf.SwitchBox.SwitchOn((SwIDs)(Sec.SwitchID + 5)))
                    {
                        if (Sec.ID < 8)
                        {
                            mf.Tls.BitSet(CTL.SBRlys0, Sec.ID);
                        }
                        else
                        {
                            mf.Tls.BitSet(CTL.SBRlys1, Sec.ID);
                        }
                    }
                }
            }

            // set section relay bytes







                if (mf.SwitchBox.SwitchOn(SwIDs.MasterOff))
            {
                CTL.SBRlys0 = 0;
                CTL.SBRlys1 = 0;
                CTL.MasterOn = false;

                // update AOG
                ToAOG.OnLo = 0;
                ToAOG.OnHi = 0;
                ToAOG.OffLo = 255;
                ToAOG.OffHi = 255;

                if (CTL.AutoOn)
                {
                    ToAOG.Command = 1;
                }
                else
                {
                    ToAOG.Command = 2;
                }

                ToAOG.Send();
            }
            else if (mf.SwitchBox.SwitchOn(SwIDs.MasterOn) || CTL.MasterOn)
            {
                CTL.SBRlys0 = 0;
                CTL.SBRlys1 = 0;
                CTL.MasterOn = true;

                ToAOG.OnLo = 0;
                ToAOG.OnHi = 0;
                ToAOG.OffLo = 0;
                ToAOG.OffHi = 0;

                foreach (clsSection Sec in mf.Sections.Items)
                {
                    if (mf.SwitchBox.SwitchOn((SwIDs)(Sec.SwitchID + 5)))
                    {
                        if (Sec.ID < 8)
                        {
                            if (CTL.AutoOn)
                            {
                                // master on, section switch on, auto on
                                mf.Tls.BitSet(CTL.AOGRlys0, Sec.ID);
                            }
                            else
                            {
                                // master on, section switch on, auto off
                                mf.Tls.BitSet(CTL.SBRlys0, Sec.ID);
                            }
                        }
                        else
                        {
                            if (CTL.AutoOn)
                            {
                                // master on, section switch on, auto on
                                mf.Tls.BitSet(CTL.AOGRlys1, Sec.ID);
                            }
                            else
                            {
                                // master on, section switch on, auto off
                                mf.Tls.BitSet(CTL.SBRlys1, Sec.ID);
                            }
                        }
                    }
                }

                // update AOG
                ToAOG.OnLo = CTL.Rlys0;
                ToAOG.OnHi = CTL.Rlys1;
                ToAOG.OffLo = 0;
                ToAOG.OffHi = 0;

                if (CTL.AutoOn)
                {
                    ToAOG.Command = 1;
                }
                else
                {
                    ToAOG.Command = 2;
                }

                ToAOG.Send();


            }

            mf.Sections.UpdateSectionsOn(CTL.Rlys0, CTL.Rlys1);
        }

        private void AutoSteerPGN_RelaysChanged(object sender, PGN254.RelaysChangedArgs e)
        {
            // Relays
            CTL.AOGRlys0 = e.RelayLo;
            CTL.AOGRlys1 = e.RelayHi;
            // check if switchbox switch positions agree with aog
            if (mf.SwitchBox.Connected())
            {
                CTL.Rlys0 = (byte)(CTL.AOGRlys0 | CTL.SBRlys0);
                CTL.Rlys1 = (byte)(CTL.AOGRlys1 | CTL.SBRlys1);

            }
            else
            {
                CTL.Rlys0 = CTL.AOGRlys0;
                CTL.Rlys1 = CTL.AOGRlys1;
            }

            if (CTL.Rlys0 != CTL.AOGRlys0 || CTL.Rlys1 != CTL.AOGRlys1) UpdateAOG();
            mf.Sections.UpdateSectionsOn(CTL.Rlys0, CTL.Rlys1);
        }

        private void UpdateAOG()
        {
            ToAOG.OnLo = CTL.Rlys0;
            ToAOG.OnHi = CTL.Rlys1;
            ToAOG.OffLo = 0;
            ToAOG.OffHi = 0;

            if (CTL.AutoOn)
            {
                ToAOG.Command = 1;
            }
            else
            {
                ToAOG.Command = 2;
            }

            ToAOG.Send();
        }

    }
}
