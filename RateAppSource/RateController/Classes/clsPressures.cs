using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RateController.Classes
{
    public class clsPressures
    {
        private readonly List<clsPressure> cPressures = new List<clsPressure>();
        private readonly List<clsPressureRawData> cRawData = new List<clsPressureRawData>();
        private readonly FormStart mf;
        private int LastID;

        public clsPressures(FormStart CallingForm)
        {
            mf = CallingForm;
            LoadCalData();
        }

        public IReadOnlyList<clsPressureRawData> CalibrationItems => cRawData.AsReadOnly();

        public IReadOnlyList<clsPressure> PressureItems => cPressures.AsReadOnly();

        public bool AddCalData(clsPressureRawData NewData)
        {
            bool Result = false;
            if (NewData.IsValid())
            {
                var index = cRawData.FindIndex(x => x.ID == NewData.ID);
                if (index == -1)
                {
                    NewData.ID = ++LastID;
                }
                cRawData.Add(NewData);
                Result = true;
            }
            return Result;
        }

        public bool DeleteCalData(int ID)
        {
            bool Result = false;
            var index = cRawData.FindIndex(x => x.ID == ID);
            if (index != -1)
            {
                cRawData.RemoveAt(index);
                Result = true;
            }
            return Result;
        }

        public bool DeleteCalDataByModuleID(int moduleID)
        {
            int initialCount = cRawData.Count;
            cRawData.RemoveAll(x => x.ModuleID == moduleID);
            return initialCount != cRawData.Count;
        }

        public bool EditCalData(int ID, clsPressureRawData NewData)
        {
            bool Result = false;
            var index = cRawData.FindIndex(x => x.ID == ID);
            if (index != -1)
            {
                if (NewData.IsValid())
                {
                    cRawData[index] = NewData;
                    Result = true;
                }
            }
            return Result;
        }

        public List<clsPressureRawData> GetCalDataByModuleID(int moduleID)
        {
            return cRawData.Where(x => x.ModuleID == moduleID).ToList();
        }

        public (double Slope, double Intercept) GetSlopeAndInterceptByModuleID(int moduleID)
        {
            var filteredData = GetCalDataByModuleID(moduleID);
            double slope = 0;
            double intercept = 0;
            if (filteredData.Count > 0)
            {
                int n = filteredData.Count;
                double sumX = filteredData.Sum(d => d.RawData);
                double sumY = filteredData.Sum(d => d.Pressure);
                double sumX2 = filteredData.Sum(d => d.RawData * d.RawData);
                double sumXY = filteredData.Sum(d => d.RawData * d.Pressure);

                slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
                intercept = (sumY - slope * sumX) / n;
            }

            return (slope, intercept);
        }

        public clsPressure Item(int ModuleID)
        {
            int idx = ListID(ModuleID);
            if (idx == -1) throw new ArgumentOutOfRangeException();
            return cPressures[idx];
        }

        public void Load()
        {
            cPressures.Clear();
            for (int i = 0; i < mf.MaxModules; i++)
            {
                clsPressure pr = new clsPressure(mf, i);
                cPressures.Add(pr);
                pr.Load();
            }
        }

        public bool PressureItemFound(int ModuleID)
        {
            return (ListID(ModuleID) > -1);
        }

        public void Save(int ModuleID = -1)
        {
            if (ModuleID == -1)
            {
                // save all
                for (int i = 0; i < mf.MaxModules; i++)
                {
                    cPressures[i].Save();
                }
            }
            else
            {
                // save selected
                cPressures[ListID(ModuleID)].Save();
            }
        }

        public void SaveCalData()
        {
            string filePath = Path.Combine(mf.Tls.FilesDir(), "PressureRawData.csv");
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("ID,ModuleID,RawData,Pressure");
                foreach (var reading in cRawData)
                {
                    writer.WriteLine($"{reading.ID},{reading.ModuleID},{reading.RawData},{reading.Pressure}");
                }
            }
        }

        private int ListID(int ModuleID)
        {
            int Result = -1;
            for (int i = 0; i < cPressures.Count; i++)
            {
                if (cPressures[i].ModuleID == ModuleID)
                {
                    Result = i;
                    break;
                }
            }
            return Result;
        }

        private void LoadCalData()
        {
            try
            {
                cRawData.Clear();
                string filePath = Path.Combine(mf.Tls.FilesDir(), "PressureRawData.csv");

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "ID,ModuleID,RawData,Pressure" + Environment.NewLine);
                }

                using (var reader = new StreamReader(filePath))
                {
                    reader.ReadLine();  // skip header

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        if (values.Length == 4 && int.TryParse(values[0], out int id) &&
                            int.TryParse(values[1], out int moduleID) &&
                            int.TryParse(values[2], out int rawData) &&
                            double.TryParse(values[3], out double pressure))
                        {
                            cRawData.Add(new clsPressureRawData { ID = id, ModuleID = moduleID, RawData = rawData, Pressure = pressure });
                            if (id > LastID) LastID = id;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clsPressure/LoadCalData: " + ex.Message);
            }
        }
    }
}