using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RateController
{
    public class clsJobs
    {
        private FormRateSettings RS;
        private string JobsFile;
        private string[] cNames = new string[500];
        private string JobNamesFile;
        private string[] Parts;

        public byte cQuantityUnits;
        public byte cCoverageUnits;
        public double cRateSet;
        public double cFlowCal;
        public double cTankSize;
        public byte cValveType;
        public double cTankRemain;
        public byte cKP;
        public byte cKI;
        public byte cKD;
        public byte cDeadband;
        public byte cMinPWM;
        public byte cMaxPWM;
        public SimType cSim;
        public byte cAdjFactor;

        public clsJobs(FormRateSettings CallingForm, string SettingsDir)
        {
            RS = CallingForm;
            JobsFile = SettingsDir + "\\Jobs.txt";
            JobNamesFile = SettingsDir + "\\JobNames.txt";
        }

        private void LoadNames()
        {
            try
            {
                FileStream fs = new FileStream(JobNamesFile, FileMode.Open, FileAccess.Read);
                StreamReader SR = new StreamReader(fs);
                // Read to the file using StreamReader class
                SR.BaseStream.Seek(0, SeekOrigin.Begin);
                string Line = SR.ReadLine();
                int Count = 0;
                while(Line!=null & Count<500)
                {
                    cNames[Count] = Line;
                    Count++;
                    Line = SR.ReadLine();
                }
                SR.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public string[] Names()
        {
            return cNames;
        }

        public bool Load(string Name)
        {
            bool result = false;
            try
            {
                FileStream fs = new FileStream(JobsFile, FileMode.Open, FileAccess.Read);
                StreamReader SR = new StreamReader(fs);
                // Read to the file using StreamReader class
                SR.BaseStream.Seek(0, SeekOrigin.Begin);
                string Line = SR.ReadLine();
                int Count = 0;
                while (Line != null & Count < 500)
                {
                    Parts = Line.Split(',');
                    if (Parts[0] == Name & Parts.Length == 16)
                    {
                        byte.TryParse(Parts[1], out cQuantityUnits);
                        byte.TryParse(Parts[2], out cCoverageUnits);
                        double.TryParse(Parts[3], out cRateSet);
                        double.TryParse(Parts[4], out cFlowCal);
                        double.TryParse(Parts[5], out cTankSize);
                        byte.TryParse(Parts[6], out cValveType);
                        double.TryParse(Parts[7], out cTankRemain);
                        byte.TryParse(Parts[8], out cKP);
                        byte.TryParse(Parts[9], out cKI);
                        byte.TryParse(Parts[10], out cKD);
                        byte.TryParse(Parts[11], out cDeadband);
                        byte.TryParse(Parts[12], out cMinPWM);
                        byte.TryParse(Parts[13], out cMaxPWM);
                        Enum.TryParse(Parts[14], out cSim);
                        byte.TryParse(Parts[15], out cAdjFactor);
                        result = true;
                        break;
                    }
                    Count++;
                    Line = SR.ReadLine();
                }
                SR.Close();
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public void Save(string Name)
        {

        }
    }
}
