using System.Windows.Forms;

namespace PCBsetup
{
    public class clsTextBox
    {
        private string cFormName;
        private int cID;
        private bool cIsNumber;
        private double cMaxValue;
        private double cMinValue;
        private TextBox cTB;
        private Forms.frmMain mf;

        public clsTextBox(Forms.frmMain CallingForm, int ID, bool IsNumber = true)
        {
            mf = CallingForm;
            cIsNumber = IsNumber;
            cID = ID;
            TB = new TextBox();
            cMaxValue = 255;
            cMinValue = 0;
        }

        public string FormName
        { get { return cFormName; } set { cFormName = value; } }

        public int ID
        { get { return cID; } }

        public bool IsNumber
        { get { return cIsNumber; } set { cIsNumber = value; } }

        public double MaxValue
        { get { return cMaxValue; } set { cMaxValue = value; } }

        public double MinValue
        { get { return cMinValue; } set { cMinValue = value; } }

        public TextBox TB
        { get { return cTB; } set { cTB = value; } }

        public string FullName()
        {
            return cFormName + "/" + TB.Name;
        }

        public void Load()
        {
            if (cIsNumber)
            {
                double val = 0;
                double.TryParse(mf.Tls.LoadProperty(FullName()), out val);
                cTB.Text = val.ToString();
            }
            else
            {
                cTB.Text = mf.Tls.LoadProperty(FullName());
            }
        }

        public bool NameMatch(string Name)
        {
            return (cFormName + "/" + Name == FullName());
        }

        public void Save()
        {
            mf.Tls.SaveProperty(FullName(), cTB.Text);
        }

        public double Value()
        {
            double tmp = 0;
            double.TryParse(TB.Text, out tmp);
            return tmp;
        }
    }
}