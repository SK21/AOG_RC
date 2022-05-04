using System.Windows.Forms;

namespace PCBsetup
{
    public class clsTextBox
    {
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

        public void Load()
        {
            string Name = TB.Parent.Name + "/" + TB.Name;
            if (cIsNumber)
            {
                double val = 0;
                double.TryParse(mf.Tls.LoadProperty(Name), out val);
                cTB.Text = val.ToString();
            }
            else
            {
                cTB.Text = mf.Tls.LoadProperty(Name);
            }
        }

        public void Save()
        {
            string Name = TB.Parent.Name + "/" + TB.Name;
            mf.Tls.SaveProperty(Name, cTB.Text);
        }

        public double Value()
        {
            double tmp = 0;
            double.TryParse(TB.Text, out tmp);
            return tmp;
        }
    }
}