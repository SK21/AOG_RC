using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RateController
{
    public class clsSections
    {
        // to use ForEach
        public IList<clsSection> Items;

        private bool AOGmasterOff;
        private bool AOGmasterOn;
        private bool AutoChanged;
        private bool AutoLast;
        private DateTime AutoTime;
        private float[] cProdWorkedArea_ha = new float[16];
        private List<clsSection> cSections = new List<clsSection>();
        private float cWorkingWidth_cm;

        private int EraseDelay = 50;
        private bool MasterChanged;
        private bool MasterLast;
        private bool MasterOn;
        private DateTime MasterTime;
        private FormStart mf;

        // sent back to AOG, see PGN234, byte 5
        private byte OutCommand;

        private byte OutLast;
        private bool PinState;

        // rate change amount for each step.  ex: 0.10 means 10% for each step
        private float RateCalcFactor = 0.05F;

        private DateTime RateLastTime;

        // used by RateController
        private byte[] SectionControlByte = new byte[2];

        private byte[] SectionControlLast = new byte[2];
        private byte[] SectionOffToAOG = new byte[2];
        private byte[] SectionOffToAOGlast = new byte[2];

        private byte[] SectionOnFromAOG = new byte[2];
        private byte[] SectionOnToAOG = new byte[2];
        private byte[] SectionOnToAOGlast = new byte[2];

        // ms between adjustments
        private int StepDelay = 500;

        private DateTime SWreadTime;
        private int Tmp;
        private PGN234 ToAOG;

        public clsSections(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cSections.AsReadOnly();

            mf.AutoSteerPGN.RelaysChanged += AOGnew_RelaysChanged;

            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;

            ToAOG = new PGN234(mf);
            mf.SectionsPGN.SectionsChanged += AOG_SectionsChanged;
        }

        public int Count
        {
            get
            {
                int tmp = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (cSections[i].Enabled) tmp++;
                }
                return tmp;
            }
            set
            {
                if (value < 0 | value > 16)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    for (int i = 0; i < 16; i++)
                    {
                        Tmp = ListID(i);
                        if (i < value)
                        {
                            cSections[Tmp].Enabled = true;
                        }
                        else
                        {
                            cSections[Tmp].Enabled = false;
                            cSections[Tmp].Width_inches = 0;
                        }
                        cSections[Tmp].Save();
                    }
                }
            }
        }

        public void CheckSwitchDefinitions()
        {
            bool Changed = false;
            for (int i = 0; i < 16; i++)
            {
                if (cSections[i].SwitchChanged) Changed = true;
                cSections[i].SwitchChanged = false;
            }
            if (Changed)
            {
                SendStatusUpdate();
                mf.SwitchIDs.Send();
            }
        }

        public bool IsMasterOn()
        {
            return MasterOn;
        }

        public clsSection Item(int SectionID)
        {
            int IDX = ListID(SectionID);
            if (IDX == -1) throw new IndexOutOfRangeException();
            return cSections[IDX];
        }

        public void Load()
        {
            cSections.Clear();
            for (int i = 0; i < 16; i++)
            {
                clsSection Sec = new clsSection(mf, i);
                Sec.Load();
                cSections.Add(Sec);
            }
            UpdateSectionsOn();
        }

        public void Save()
        {
            for (int i = 0; i < cSections.Count; i++)
            {
                cSections[i].Save();
            }
        }

        public byte SectionHi()
        {
            return SectionControlByte[1];
        }

        public byte SectionLo()
        {
            return SectionControlByte[0];
        }

        public void SendStatusUpdate(bool SourceAOG = false)
        {
            bool SectionSwitchesChanged = false;
            if (mf.SwitchBox.Connected())
            {
                ReadRateSwitches();
                SectionSwitchesChanged = ReadSectionSwitches();
            }
            else
            {
                SectionControlByte[0] = SectionOnFromAOG[0];
                SectionControlByte[1] = SectionOnFromAOG[1];

                AOGmasterOn = false;
                AOGmasterOff = false;
            }

            bool RelaysChanged = UpdateSectionsOn();

            if (!SourceAOG & (RelaysChanged | SectionSwitchesChanged))
            {
                // send to AOG
                ToAOG.OnHi = SectionOnToAOG[1];
                ToAOG.OnLo = SectionOnToAOG[0];
                ToAOG.OffHi = SectionOffToAOG[1];
                ToAOG.OffLo = SectionOffToAOG[0];
                ToAOG.Command = OutCommand;
                ToAOG.Send();
            }
        }

        public float TotalWidth(bool UseInches = true)
        {
            float Result = 0;
            for (int i = 0; i < 16; i++)
            {
                if (cSections[i].Enabled)
                {
                    if (UseInches)
                    {
                        Result += cSections[i].Width_inches;
                    }
                    else
                    {
                        Result += cSections[i].Width_cm;
                    }
                }
            }
            return Result;
        }

        public bool UpdateSectionsOn()
        {
            // returns true if section on status has changed for any section
            bool Result = false;
            bool tmp = false;
            try
            {
                cWorkingWidth_cm = 0;
                for (int i = 0; i < 16; i++)
                {
                    tmp = cSections[i].IsON;

                    if (i < 8)
                    {
                        cSections[i].IsON = mf.Tls.BitRead(SectionControlByte[0], i);
                    }
                    else
                    {
                        cSections[i].IsON = mf.Tls.BitRead(SectionControlByte[1], i - 8);
                    }

                    if (cSections[i].IsON != tmp) Result = true;

                    if (cSections[i].IsON) cWorkingWidth_cm += cSections[i].Width_cm;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clsSections/UpdateSectionsOn: " + ex.Message);
                mf.Tls.ShowHelp(ex.Message, "Sections", 3000, true);
            }
            return Result;
        }

        private void AOG_SectionsChanged(object sender, EventArgs e)
        {
            double Wdth;
            string Units;
            if (mf.UseInches)
            {
                Units = "Inches";
            }
            else
            {
                Units = "cm";
            }

            string Message = "Section width changes from AOG, " + mf.SectionsPGN.SectionCount().ToString() + " section(s).";
            Message += "\nUnits are in " + Units + ".";

            for (int i = 0; i < mf.SectionsPGN.SectionCount(); i++)
            {
                if (mf.UseInches)
                {
                    Wdth = Math.Round((double)(mf.SectionsPGN.Width_cm(i)) * 0.393701);
                }
                else
                {
                    Wdth = mf.SectionsPGN.Width_cm(i);
                }
                Message += "\n" + "Section " + (i + 1).ToString() + "    " + Wdth.ToString();
            }
            Message += "\n\n Accept the changes?";

            var Hlp = new frmMsgBox(mf, Message, "Section Changes");
            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                // change sections
                Count = mf.SectionsPGN.SectionCount();
                for (int i = 0; i < mf.SectionsPGN.SectionCount(); i++)
                {
                    Items[i].Width_cm = (float)mf.SectionsPGN.Width_cm(i);
                }
                mf.Tls.ShowHelp("Sections changed. Check switch definitions.", "Sections", 5000);
            }
        }

        private void AOGnew_RelaysChanged(object sender, PGN254.RelaysChangedArgs e)
        {
            // AOG section bytes have changed
            SectionOnFromAOG[0] = e.RelayLo;
            SectionOnFromAOG[1] = e.RelayHi;

            if (SectionOnFromAOG[0] == 0 && SectionOnFromAOG[1] == 0)
            {
                AOGmasterOff = true;    // simulates a momentary switch off
            }
            else
            {
                AOGmasterOn = true;     // simulates a momentary switch on
            }
            SendStatusUpdate(true);
        }

        private int ListID(int SectionID)
        {
            for (int i = 0; i < cSections.Count; i++)
            {
                if (cSections[i].ID == SectionID) return i;
            }
            return -1;
        }

        private void ReadRateSwitches()
        {
            if (mf.SwitchBox.SwitchOn(SwIDs.RateUp) || mf.SwitchBox.SwitchOn(SwIDs.RateDown))
            {
                SWreadTime = DateTime.Now;

                if ((SWreadTime - RateLastTime).TotalMilliseconds > StepDelay)
                {
                    RateLastTime = DateTime.Now;
                    if (mf.SwitchBox.SwitchOn(SwIDs.RateUp))
                    {
                        if (mf.Tls.BitRead(OutCommand, 2))
                        {
                            if (!mf.Tls.BitRead(OutCommand, 3))
                            {
                                OutCommand = mf.Tls.BitSet(OutCommand, 3);
                                OutCommand = mf.Tls.BitClear(OutCommand, 2);
                            }
                        }
                        else
                        {
                            OutCommand = mf.Tls.BitSet(OutCommand, 2);
                        }
                        OutCommand = mf.Tls.BitClear(OutCommand, 4); // left
                        OutCommand = mf.Tls.BitSet(OutCommand, 5);   // rate up
                    }

                    if (mf.SwitchBox.SwitchOn(SwIDs.RateDown))
                    {
                        if (mf.Tls.BitRead(OutCommand, 2))
                        {
                            if (!mf.Tls.BitRead(OutCommand, 3))
                            {
                                OutCommand = mf.Tls.BitSet(OutCommand, 3);
                                OutCommand = mf.Tls.BitClear(OutCommand, 2);
                            }
                        }
                        else
                        {
                            OutCommand = mf.Tls.BitSet(OutCommand, 2);
                        }
                        OutCommand = mf.Tls.BitClear(OutCommand, 4); // left
                        OutCommand = mf.Tls.BitClear(OutCommand, 5); // rate down
                    }
                }

                // update rate for currently displayed product
                // rate change amount
                int RateSteps = 0;
                if ((OutCommand & 4) == 4) RateSteps = 1;
                if ((OutCommand & 8) == 8) RateSteps += 2;

                if (RateSteps > 0)
                {
                    // rate direction
                    bool RateUp = false;
                    RateUp = (OutCommand & 32) == 32;

                    // change rate
                    float ChangeAmount = 1;
                    for (byte a = 1; a <= RateSteps; a++)
                    {
                        if (RateUp)
                        {
                            ChangeAmount *= (1 + RateCalcFactor);
                        }
                        else
                        {
                            ChangeAmount *= (1 - RateCalcFactor);
                        }
                    }
                    clsProduct Prd = mf.Products.Item(mf.CurrentProduct() - 1);

                    // set manual adjustment rate
                    double Dir = -1.0;
                    if (RateUp) Dir = 1.0;
                    switch (RateSteps)
                    {
                        case 2:
                            Prd.ManualAdjust = (double)Prd.PIDminPWM / 100.0 * 255.0 * 2.0;
                            if (Prd.ManualAdjust > 255) Prd.ManualAdjust = 255;
                            Prd.ManualAdjust *= Dir;
                            break;

                        case 3:
                            Prd.ManualAdjust = (double)Prd.PIDminPWM / 100.0 * 255.0 * 3.0;
                            if (Prd.ManualAdjust > 255) Prd.ManualAdjust = 255;
                            Prd.ManualAdjust *= Dir;
                            break;

                        default:
                            Prd.ManualAdjust = (double)Prd.PIDminPWM / 100.0 * 255.0 * Dir;
                            break;
                    }

                    if (mf.SwitchBox.SwitchOn(SwIDs.Auto))
                    {
                        double CurrentRate = Prd.RateSet;
                        if (RateUp & CurrentRate == 0) CurrentRate = 1; // provide a starting point
                        CurrentRate = Math.Round(CurrentRate * ChangeAmount, 1);
                        Prd.RateSet = CurrentRate;
                    }
                }
            }
            else
            {
                // rate switch not pressed

                if ((SWreadTime - RateLastTime).TotalMilliseconds > EraseDelay)
                {
                    // clear rate values after delay
                    OutCommand = mf.Tls.BitClear(OutCommand, 2);
                    OutCommand = mf.Tls.BitClear(OutCommand, 3);
                    OutCommand = mf.Tls.BitClear(OutCommand, 4);
                    OutCommand = mf.Tls.BitClear(OutCommand, 5);
                    mf.Products.Item(mf.CurrentProduct() - 1).ManualAdjust = 0;
                }
            }
        }

        private bool ReadSectionSwitches()
        {
            int ID = 0;

            for (int i = 0; i < 2; i++)
            {
                SectionOnToAOG[i] = 0;
                SectionOffToAOG[i] = 0;
            }

            SWreadTime = DateTime.Now;

            // Master switch
            if (mf.SwitchBox.SwitchOn(SwIDs.MasterOff)) MasterOn = false;
            if (mf.SwitchBox.SwitchOn(SwIDs.MasterOn)) MasterOn = true;

            if ((MasterOn != MasterLast) & !MasterChanged)
            {
                // create AOG master notification
                MasterTime = SWreadTime;
                MasterChanged = true;
            }

            if (SWreadTime > MasterTime.AddMilliseconds(EraseDelay))
            {
                // delay over, cancel AOG master notification
                MasterChanged = false;
                MasterLast = MasterOn;
            }

            // set state of AOG master switch (btnSectionOffAutoOn)
            OutCommand = mf.Tls.BitClear(OutCommand, 0);
            OutCommand = mf.Tls.BitClear(OutCommand, 1);
            if (MasterChanged)
            {
                if (MasterOn) OutCommand = mf.Tls.BitSet(OutCommand, 0);    // request AOG master switch on, section buttons to auto
                if (!MasterOn) OutCommand = mf.Tls.BitSet(OutCommand, 1);   // request AOG master switch off, section buttons to off
            }

            // check if AOG sections changed
            if (AOGmasterOff)
            {
                if (!AutoChanged && !AutoLast)
                {
                    MasterOn = false;
                }
                AOGmasterOff = false;
            }

            if (AOGmasterOn)
            {
                MasterOn = true;
                AOGmasterOn = false;
            }

            // auto state
            if ((mf.SwitchBox.SwitchOn(SwIDs.Auto) != AutoLast) & !AutoChanged)
            {
                // create AOG auto notification
                AutoTime = SWreadTime;
                AutoChanged = true;
            }

            if (SWreadTime > AutoTime.AddMilliseconds(EraseDelay))
            {
                // cancel AOG auto notification
                AutoChanged = false;
                AutoLast = mf.SwitchBox.SwitchOn(SwIDs.Auto);
            }

            // Relays
            if (MasterOn)
            {
                for (int SwByte = 0; SwByte < 2; SwByte++)
                {
                    for (int SwBit = 0; SwBit < 8; SwBit++)
                    {
                        ID = mf.Sections.Item(SwBit + SwByte * 8).SwitchID;
                        PinState = mf.SwitchBox.SectionSwitchOn(ID);

                        if (PinState)
                        {
                            // master on, section switch on
                            if (mf.SwitchBox.SwitchOn(SwIDs.Auto))
                            {
                                // master on, section switch on, auto on
                                // AOG in control
                                if (mf.Tls.BitRead(SectionOnFromAOG[SwByte], SwBit))
                                {
                                    SectionControlByte[SwByte] = mf.Tls.BitSet(SectionControlByte[SwByte], SwBit); // turn section on
                                }
                                else
                                {
                                    SectionControlByte[SwByte] = mf.Tls.BitClear(SectionControlByte[SwByte], SwBit); // turn section off
                                }
                            }
                            else
                            {
                                // master on, section switch on, auto off
                                // manual on
                                SectionControlByte[SwByte] = mf.Tls.BitSet(SectionControlByte[SwByte], SwBit);  // turn section on

                                // update On byte to AOG
                                SectionOnToAOG[SwByte] = SectionControlByte[SwByte];
                            }
                        }
                        else
                        {
                            // section switch off
                            SectionControlByte[SwByte] = mf.Tls.BitClear(SectionControlByte[SwByte], SwBit);    // turn off section
                            SectionOffToAOG[SwByte] = mf.Tls.BitSet(SectionOffToAOG[SwByte], SwBit);    // set AOG section button to off
                        }
                    }
                }
            }
            else
            {
                // master off
                // turn relays off
                for (int i = 0; i < 2; i++)
                {
                    SectionControlByte[i] = 0;

                    SectionOnToAOG[i] = 0;
                    SectionOffToAOG[i] = 255;
                }
            }

            if (AutoChanged)
            {
                if (mf.SwitchBox.SwitchOn(SwIDs.Auto))
                {
                    // auto on
                    // change section buttons to auto state by resending master on
                    OutCommand = mf.Tls.BitSet(OutCommand, 0);
                }
            }

            // record relay states
            if (!AutoChanged)
            {
                for (int i = 0; i < 2; i++)
                {
                    SectionControlLast[i] = SectionControlByte[i];
                }
            }

            bool Result = MasterChanged | (OutCommand != OutLast);

            OutLast = OutCommand;

            for (int i = 0; i < 2; i++)
            {
                //check if control bytes changed
                if (SectionOnToAOG[i] != SectionOnToAOGlast[i]) Result = true;
                if (SectionOffToAOG[i] != SectionOffToAOGlast[i]) Result = true;

                SectionOnToAOGlast[i] = SectionOnToAOG[i];
                SectionOffToAOGlast[i] = SectionOffToAOG[i];
            }

            return Result;
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            // switch box switches have changed
            SendStatusUpdate();
        }

        public class MasterChangedArgs : EventArgs
        {
            public bool MasterOn { get; set; }
        }
    }
}