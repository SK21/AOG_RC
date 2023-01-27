using System;
using System.Collections.Generic;

namespace RateController
{
    public class clsProducts
    {
        public IList<clsProduct> Items; // access records by index
        private List<clsProduct> cProducts = new List<clsProduct>();
        private DateTime LastSave;
        private int MaxRecords = 5;
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

            for (int i = 0; i < MaxRecords; i++)
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
            for (int i = 0; i < MaxRecords; i++)
            {
                clsProduct Prod = new clsProduct(mf, i);
                cProducts.Add(Prod);
                Prod.Load();
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
            for (int i = 0; i < MaxRecords; i++)
            {
                cProducts[i].Update();
            }

            if ((DateTime.Now - LastSave).TotalSeconds > 60)
            {
                for (int i = 0; i < MaxRecords; i++)
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
                cProducts[i].SendPID();
            }
        }

        public void UpdateVirtualNano()
        {
            for (int i = 0; i < MaxRecords; i++)
            {
                if (mf.SimMode == SimType.VirtualNano) cProducts[i].VirtualNano.MainLoop();
            }
        }

        private int ListID(int ProdID)
        {
            for (int i = 0; i < cProducts.Count; i++)
            {
                if (cProducts[i].ID == ProdID) return i;
            }
            return -1;
        }
    }
}