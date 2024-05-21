using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class clsSwitches
    {
        private readonly List<clsSwitch> cSwitches = new List<clsSwitch>();
        private readonly FormStart mf;
        private IList<clsSwitch> cItems;

        public clsSwitches(FormStart Main)
        {
            mf = Main;
            Items = cSwitches.AsReadOnly();
        }


        public IList<clsSwitch> Items { get => cItems; set => cItems = value; }

        public clsSwitch Item(int SwitchID)
        {
            int IDX = ListID(SwitchID);
            if (IDX == -1) throw new ArgumentOutOfRangeException();
            return cSwitches[IDX];
        }

        public void Load(bool LoadFromFile = true)
        {
            cSwitches.Clear();
            for (int i = 0; i < 16; i++)
            {
                clsSwitch SW = new clsSwitch(mf, i);
                cSwitches.Add(SW);
                if (LoadFromFile) SW.Load();
            }
        }

        public void Reset()
        {
            Load(false);
        }

        public void Save()
        {
            for (int i = 0; i < 16; i++)
            {
                cSwitches[i].Save();
            }
        }

        private int ListID(int SwitchID)
        {
            for (int i = 0; i < cSwitches.Count; i++)
            {
                if (cSwitches[i].ID == SwitchID) return i;
            }
            return -1;
        }
    }
}