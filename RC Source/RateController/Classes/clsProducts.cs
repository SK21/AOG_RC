using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace RateController
{
    public class clsProducts
    {
        public IList<clsProduct> Items; // access records by index
        private List<clsProduct> cProducts = new List<clsProduct>();
        private DateTime LastSave;
        private int MaxRecords = 5;
        private FormStart mf;

        System.IO.Stream Str;
        System.Media.SoundPlayer RateAlarm;

        private double AlarmCounter;
        private bool cAlarmOn;
        private bool cPauseAlarm;
        private bool cShowAlarm;

        private double AlarmLevel = 0.75;

        public clsProducts(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cProducts.AsReadOnly();

            Str = Properties.Resources.Loud_Alarm_Clock_Buzzer_Muk1984_493547174;
            RateAlarm = new System.Media.SoundPlayer(Str);
        }

        public int Count()
        {
            return cProducts.Count;
        }

        public clsProduct Item(int ProdID)  // access records by Product ID
        {
            int IDX = ListID(ProdID);
            if (IDX == -1) throw new IndexOutOfRangeException();
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

        public void Save(int ProdID = 0)
        {
            if (ProdID == 0)
            {
                // save all
                for (int i = 0; i < MaxRecords; i++)
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

        public bool UniqueModSen(int ModID, int SenID, int ProdID)
        {
            // checks if product module ID/sensor ID pair are unique
            bool Result = true;
            for (int i = 0; i < Count(); i++)
            {
                if (cProducts[i].ID != ProdID)  // exclude current product
                {
                    if (cProducts[i].ModuleID == ModID & cProducts[i].SensorID == SenID)
                    {
                        Result = false;
                        break;
                    }
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

        public void UpdateVirtualNano()
        {
            for (int i = 0; i < MaxRecords; i++)
            {
                if (cProducts[i].SimulationType == SimType.VirtualNano) cProducts[i].VirtualNano.MainLoop();
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

        public bool PauseAlarm { set { cPauseAlarm = value; } }

        public bool CheckAlarm()
        {
            cAlarmOn = false;
            for (int i = 0; i < MaxRecords; i++)
            {
                if (cProducts[i].UseOffRateAlarm)
                {
                    if ((cProducts[i].SmoothRate() < (cProducts[i].RateSet * AlarmLevel)) & (cProducts[i].WorkRate() > 0))
                    {
                        cAlarmOn = true;
                        break;
                    }
                }
            }

            if (cAlarmOn)
            {
                if (cPauseAlarm)
                {
                    RateAlarm.Stop();
                }
                else
                {
                    AlarmCounter++;
                    if (AlarmCounter > 5)
                    {
                        RateAlarm.Play();
                        cShowAlarm = true;
                    }
                }
            }
            else
            {
                AlarmCounter = 0;
                RateAlarm.Stop();
                cPauseAlarm = false;
                cShowAlarm = false;
            }
            return cShowAlarm;
        }
    }
}