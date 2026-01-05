using RateController.Classes;
using System;
using System.Collections.Generic;

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

            for (int i = 0; i < Props.MaxProducts; i++)
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

        public double[] BaseRates()
        {
            int ProductCount = Props.MaxProducts - 2;   // last 2 are fans
            double[] Result = new double[ProductCount];

            if (cProducts.Count >= ProductCount)
            {
                for (int i = 0; i < ProductCount; i++)
                {
                    if (cProducts[i].Enabled && cProducts[i].BumpButtons == false)
                    {
                        Result[i] = cProducts[i].RateSet;
                    }
                    else
                    {
                        Result[i] = 0;
                    }
                }

            }
            return Result;
        }

        public bool Connected()
        {
            bool Result = false;
            try
            {
                if (cProducts.Count > 0)
                {
                    // returns true if at least one module is connected
                    for (int i = 0; i < Props.MaxProducts; i++)
                    {
                        if (cProducts[i].RateSensorData.Connected())
                        {
                            Result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsProducts/Connected: " + ex.Message);
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

            for (int i = 0; i < Props.MaxProducts; i++)
            {
                clsProduct Prd = new clsProduct(mf, i);
                cProducts.Add(Prd);
                Prd.Load();

                if (Prd.IsNew())
                {
                    AssignNextUnusedModSen(Prd);

                    Prd.ProductName = "Prod  " + (char)(65 + i);
                    Prd.ControlType = ControlTypeEnum.Valve;
                    Prd.QuantityDescription = "Gallons";
                    Prd.CoverageUnits = 0;
                    Prd.MeterCal = 1;
                    Prd.EnableProdDensity = false;
                    Prd.ProdDensity = 0;
                    Prd.RateSet = 1;
                    Prd.RateAlt = 100;
                    Prd.TankSize = 1000;
                    Prd.TankStart = 1000;
                    Prd.LoadSensorSettings();
                    Prd.AppMode = ApplicationMode.ControlledUPM;
                    Prd.OffRateSetting = 0;
                    Prd.MinUPM = 0;
                    Prd.BumpButtons = false;
                    Prd.CountsRev = 1;
                    Prd.Enabled = false;
                    Prd.Save();
                }
            }
            SetEnabledDefault();
        }

        public int NextEnabledProduct(int CurrentProduct)
        {
            int Result = CurrentProduct;
            int EnabledID = -1;
            for (int i = CurrentProduct + 1; i < Props.MaxProducts - 2; i++)
            {
                if (cProducts[i].Enabled)
                {
                    EnabledID = i;
                    break;
                }
            }
            if (EnabledID != -1) Result = EnabledID;
            return Result;
        }

        public int PreviousEnabledProduct(int CurrentProduct)
        {
            int Result = CurrentProduct;
            int EnabledID = -1;
            for (int i = CurrentProduct - 1; i >= 0; i--)
            {
                if (cProducts[i].Enabled)
                {
                    EnabledID = i;
                    break;
                }
            }
            if (EnabledID != -1) Result = EnabledID;
            return Result;
        }

        public double[] ProductAppliedRates()
        {
            double[] Result = new double[Props.MaxProducts - 2];
            for (int i = 0; i < Props.MaxProducts - 2; i++)
            {
                if (cProducts[i].RateSensorData.Connected())
                {
                    Result[i] = cProducts[i].CurrentRate();
                }
            }
            return Result;
        }

        public bool ProductsAreOn()
        {
            bool Result = false;
            for (int i = 0; i < Props.MaxProducts; i++)
            {
                if (cProducts[i].ProductOn())
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }


        public void Save()
        {
            for (int i = 0; i < Props.MaxProducts; i++)
            {
                cProducts[i].Update();
            }

            if ((DateTime.Now - LastSave).TotalSeconds > 60)
            {
                for (int i = 0; i < Props.MaxProducts; i++)
                {
                    cProducts[i].Save();
                }
                LastSave = DateTime.Now;
            }
        }

        public void SetEnabledDefault()
        {
            // check for at least one product enabled and for default product
            bool DefaultFound = false;
            int EnabledID = -1;

            for (int i = 0; i < Props.MaxProducts - 2; i++)
            {
                clsProduct Prod = cProducts[i];
                if (Prod.Enabled && !Prod.BumpButtons)
                {
                    EnabledID = i;
                    if (Props.DefaultProduct == i) DefaultFound = true;
                }
            }
            if (EnabledID == -1)
            {
                // no enabled products found, enable product 0
                cProducts[0].Enabled = true;
                cProducts[0].Save();
                EnabledID = 0;
            }
            if (!DefaultFound)
            {
                Props.DefaultProduct = EnabledID;
                cProducts[EnabledID].BumpButtons = false;
            }
        }

        public bool UniqueModSen(int ModID, int SenID, int ProdID)
        {
            // checks if product module ID/sensor ID pair are unique
            bool Result = true;
            for (int i = 0; i < Props.MaxProducts; i++)
            {
                if (cProducts[i].Enabled && cProducts[i].ID != ProdID && cProducts[i].ModuleID == ModID && cProducts[i].SensorID == SenID)
                {
                    Result = false;
                    break;
                }
            }
            return Result;
        }

        public void UpdateSensorSettings()
        {
            for (int i = 0; i < cProducts.Count; i++)
            {
                if (cProducts[i].RateSensorData.Connected()) cProducts[i].SendSensorSettings();
            }
        }

        private void AssignNextUnusedModSen(clsProduct product)
        {
            try
            {
                // Map used pairs
                bool[,] used = new bool[Props.MaxModules, Props.MaxSensorsPerModule];
                for (int i = 0; i < cProducts.Count; i++)
                {
                    int mod = cProducts[i].ModuleID;
                    int sen = cProducts[i].SensorID;
                    if (mod >= 0 && mod < Props.MaxModules && sen >= 0 && sen < Props.MaxSensorsPerModule)
                    {
                        used[mod, sen] = true;
                    }
                }

                // Find the first available pair in Module-major, Sensor-minor order
                for (int mod = 0; mod < Props.MaxModules; mod++)
                {
                    for (int sen = 0; sen < Props.MaxSensorsPerModule; sen++)
                    {
                        if (!used[mod, sen])
                        {
                            product.ModuleID = mod;
                            product.SensorID = (byte)sen;
                            return;
                        }
                    }
                }

                // No free pair found; leave as-is
                Props.WriteErrorLog("AssignNextUnusedModSen: No available ModuleID/SensorID pair found.");
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("AssignNextUnusedModSen: " + ex.Message);
            }
        }

        private int ListID(int ProdID)
        {
            int Result = -1;
            for (int i = 0; i < cProducts.Count; i++)
            {
                if (cProducts[i].ID == ProdID)
                {
                    Result = i;
                    break;
                }
            }
            return Result;
        }
    }
}