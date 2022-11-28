using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PCBsetup
{
    public class clsTextBoxes
    {
        public IList<clsTextBox> Items;
        private int cCounter = 0;
        private List<clsTextBox> cTextBoxes = new List<clsTextBox>();
        private Forms.frmMain mf;

        public clsTextBoxes(Forms.frmMain CallingForm)
        {
            mf = CallingForm;
            Items = cTextBoxes.AsReadOnly();
        }

        public int Add(string FormName, TextBox tb, double MaxValue = 255, double MinValue = 0, bool IsNumber = true)
        {
            clsTextBox NewBox = new clsTextBox(mf, cCounter);
            cTextBoxes.Add(NewBox);

            NewBox.TB = tb;
            NewBox.MaxValue = MaxValue;
            NewBox.MinValue = MinValue;
            NewBox.IsNumber = IsNumber;
            NewBox.FormName= FormName;
            NewBox.Load();
            cCounter++;
            return cCounter - 1;
        }

        public int Count()
        {
            return cTextBoxes.Count;
        }

        public clsTextBox Item(int TextBoxID)
        {
            int IDX = ListID(TextBoxID);
            if (IDX == -1) throw new ArgumentException();
            return cTextBoxes[IDX];
        }

        public void ReLoad(int ID = 0)
        {
            if (ID == 0)
            {
                // load all
                for (int i = 0; i < cCounter; i++)
                {
                    Item(i).Load();
                }
            }
            else
            {
                // load selected
                cTextBoxes[ListID(ID)].Load();
            }
        }

        public void Reset()
        {
            cTextBoxes.Clear();
            cCounter = 0;
        }

        public void Save(int TextBoxID = 0)
        {
            if (TextBoxID == 0)
            {
                // save all
                for (int i = 0; i < cTextBoxes.Count; i++)
                {
                    cTextBoxes[i].Save();
                }
            }
            else
            {
                // save selected
                cTextBoxes[ListID(TextBoxID)].Save();
            }
        }

        public double Value(string TextBoxName)
        {
            double Result = 0;
            for (int i = 0; i < cCounter; i++)
            {
                if(Item(i).NameMatch(TextBoxName))
                {
                    Result = Item(i).Value();
                    break;
                }
            }
            return Result;
        }

        private int ListID(int TextBoxID)
        {
            for (int i = 0; i < cTextBoxes.Count; i++)
            {
                if (cTextBoxes[i].ID == TextBoxID) return i;
            }
            return -1;
        }
    }
}