using System;
using System.Collections.Generic;

namespace RateController
{
    public class clsCalibrates
    {
        public IList<clsCalibrate> Items;
        private const int MaxCalibrates = 4;

        private List<clsCalibrate> cCalibrates = new List<clsCalibrate>();

        private FormStart mf;

        public clsCalibrates(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cCalibrates.AsReadOnly();
        }

        public event EventHandler<EventArgs> Edited;

        public void Close()
        {
            for (int i = 0; i < cCalibrates.Count; i++)
            {
                cCalibrates[i].Close();
            }
        }

        public int Count()
        {
            return cCalibrates.Count;
        }

        public clsCalibrate Item(int ID)  // access records by ID
        {
            int IDX = ListID(ID);
            if (IDX == -1) throw new ArgumentOutOfRangeException();
            return cCalibrates[IDX];
        }

        public void Load()
        {
            cCalibrates.Clear();
            for (int i = 0; i < MaxCalibrates; i++)
            {
                clsCalibrate Cal = new clsCalibrate(mf, i);
                cCalibrates.Add(Cal);
                Cal.Load();
                Cal.Edited += Cal_Edited;
            }
        }

        public void Reset()
        {
            for (int i = 0; i < cCalibrates.Count; i++)
            {
                cCalibrates[i].Reset();
            }
        }

        public void Running(bool IsRunning)
        {
            for (int i = 0; i < cCalibrates.Count; i++)
            {
                cCalibrates[i].Running = IsRunning;
            }
        }

        public bool ReadyToCalibrate()
        {
            bool Result = false;
            for(int i=0;i<cCalibrates.Count;i++)
            {
                if (cCalibrates[i].PowerOn)
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }

        public void Save(int ID = 0)
        {
            if (ID == 0)
            {
                // save all
                for (int i = 0; i < cCalibrates.Count; i++)
                {
                    cCalibrates[i].Save();
                }
            }
            else
            {
                // save selected
                cCalibrates[ListID(ID)].Save();
            }
        }

        public void Update()
        {
            for (int i = 0; i < cCalibrates.Count; i++)
            {
                cCalibrates[i].Update();
            }
        }

        private void Cal_Edited(object sender, EventArgs e)
        {
            Edited(this, EventArgs.Empty);
        }

        private int ListID(int ID)
        {
            for (int i = 0; i < cCalibrates.Count; i++)
            {
                if (cCalibrates[i].ID == ID) return i;
            }
            return -1;
        }
    }
}