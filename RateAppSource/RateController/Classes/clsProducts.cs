using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RateController
{
    public class clsProducts
    {
        public IList<clsProduct> Items; // access records by index
        private List<clsProduct> cProducts = new List<clsProduct>();
        private DateTime LastSave;
        private FormStart mf;

        public clsProducts(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cProducts.AsReadOnly();
        }

        public bool AlarmOn()
        {
            double AlarmSetPoint;
            bool cAlarmOn = false;

            for (int i = 0; i < mf.MaxProducts; i++)
            {
                if ((cProducts[i].WorkRate() > 0) && (cProducts[i].UseOffRateAlarm))
                {
                    // too low?
                    AlarmSetPoint = (100 - cProducts[i].OffRateSetting) / 100.0;
                    if (cProducts[i].SmoothRate() < (cProducts[i].TargetRate() * AlarmSetPoint))
                    {
                        cAlarmOn = true;
                        break;
                    }

                    // too high?
                    AlarmSetPoint = (100 + cProducts[i].OffRateSetting) / 100.0;
                    if (cProducts[i].SmoothRate() > (cProducts[i].TargetRate() * AlarmSetPoint))
                    {
                        cAlarmOn = true;
                        break;
                    }
                }
            }
            return cAlarmOn;
        }

        public bool Connected()
        {
            bool Result = false;
            try
            {
                if (cProducts.Count > 0)
                {
                    // returns true if at least one module is connected
                    for (int i = 0; i < mf.MaxProducts; i++)
                    {
                        if (cProducts[i].ArduinoModule.Connected())
                        {
                            Result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clsProducts/Connected: " + ex.Message);
            }
            return Result;
        }

        public int Count()
        {
            return cProducts.Count;
        }

        public clsProduct Item(int ProdID)  // access records by Product ID
        {
            int IDX = ListID(ProdID);
            if (IDX == -1) throw new ArgumentOutOfRangeException();
            return cProducts[IDX];
        }

        public void Load()
        {
            cProducts.Clear();
            for (int i = 0; i < mf.MaxProducts; i++)
            {
                clsProduct Prod = new clsProduct(mf, i);
                cProducts.Add(Prod);
                Prod.Load();
            }

            for (int i = 0; i < mf.MaxProducts; i++)
            {
                clsProduct Prod = cProducts[i];
                if (Prod.IsNew())
                {
                    // set initial module ID
                    for (int j = 0; j < 255; j++)
                    {
                        if (UniqueModSen(j, Prod.SensorID, Prod.ID))
                        {
                            Prod.ModuleID = j;
                            break;
                        }
                    }
                    Prod.PIDkp = 1;
                    Prod.PIDki = 0;
                    Prod.PIDkd = 0;
                    Prod.PIDmax = 100;
                    Prod.PIDmin = 5;
                    Prod.ProductName = "P" + (i + 1).ToString();
                    Prod.Save();
                }
            }
        }


        public string ProductComm(int Port)
        {
            string Result = "";
            if (Port >= 0 && Port < 3)
            {
                for (int i = 0; i < cProducts.Count; i++)
                {
                    if (cProducts[i].SerialPort == Port)
                    {
                        Result = cProducts[i].ProductName;
                        break;
                    }
                }
            }
            return Result;
        }

        public int ProductID(int ModuleID)
        {
            int Result = -1;
            for (int i = 0; i < cProducts.Count; i++)
            {
                if (cProducts[i].ModuleID == ModuleID) Result = i;
            }
            return Result;
        }

        public void Save(int ProdID = 0)
        {
            if (ProdID == 0)
            {
                // save all
                for (int i = 0; i < cProducts.Count; i++)
                {
                    cProducts[i].Save();
                }
            }
            else
            {
                // save selected
                cProducts[ListID(ProdID)].Save();
            }
        }

        public void SaveComm(string ProdName, int Port)
        {
            if (Port >= 0 && Port < 3)
            {
                if (ProdName == "-")
                {
                    // remove 'Port' from all product serial ports
                    for (int i = 0; i < cProducts.Count; i++)
                    {
                        if (cProducts[i].SerialPort == Port)
                        {
                            cProducts[i].SerialPort = -1;
                            cProducts[i].Save();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < cProducts.Count; i++)
                    {
                        if (cProducts[i].ProductName == ProdName)
                        {
                            cProducts[i].SerialPort = Port;
                            cProducts[i].Save();
                            break;
                        }
                    }
                }
            }
        }

        public bool UniqueModSen(int ModID, int SenID, int ProdID)
        {
            // checks if product module ID/sensor ID pair are unique
            bool Result = true;
            for (int i = 0; i < Count(); i++)
            {
                if ((cProducts[i].ID != ProdID) && (cProducts[i].ModuleID == ModID && cProducts[i].SensorID == SenID))
                {
                    Result = false;
                    break;
                }
            }
            return Result;
        }

        public void Update()
        {
            for (int i = 0; i < mf.MaxProducts; i++)
            {
                cProducts[i].Update();
            }

            if ((DateTime.Now - LastSave).TotalSeconds > 60)
            {
                for (int i = 0; i < mf.MaxProducts; i++)
                {
                    cProducts[i].Save();
                }
                LastSave = DateTime.Now;
            }
        }

        public void UpdatePID()
        {
            for (int i = 0; i < cProducts.Count; i++)
            {
                if (cProducts[i].ArduinoModule.Connected()) cProducts[i].SendPID();
            }
        }

        public void UpdateVirtualNano()
        {
            for (int i = 0; i < mf.MaxProducts; i++)
            {
                if (mf.SimMode == SimType.VirtualNano) cProducts[i].VirtualNano.MainLoop();
            }
        }

        private int ListID(int ProdID)
        {
            int Result = -1;
            for (int i = 0; i < cProducts.Count; i++)
            {
                if (cProducts[i].ID == ProdID) Result = i;
            }
            return Result;
        }
    }
}