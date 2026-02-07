using System;
using System.Collections.Generic;

namespace RateController.Classes
{
    public class clsProducts
    {
        public IList<clsProduct> Items; // access records by index
        private List<clsProduct> cProducts = new List<clsProduct>();
        private ProductState[] cProductsState = new ProductState[Props.MaxProducts];
        private DateTime LastSave;

        public clsProducts()
        {
            Items = cProducts.AsReadOnly();
            Core.UpdateStatus += Core_UpdateStatus;
            Core.AppExit += Core_AppExit;

            for (int i = 0; i < Props.MaxProducts; i++)
            {
                cProductsState[i] = ProductState.Off;
            }
        }

        public double[] BaseRates()
        {
            int ProductCount = Props.MaxProducts - 2;   // last 2 are fans
            double[] Result = new double[ProductCount];

            if (cProducts.Count >= ProductCount)
            {
                for (int i = 0; i < ProductCount; i++)
                {
                    Result[i] = cProducts[i].RateSet;
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
                clsProduct Prd = new clsProduct(i);
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
                    Prd.CountsRev = 1;
                    Prd.Enabled = false;
                    Prd.Save();
                }
            }
            SetEnabledDefault();
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

        public ProductState ProductsState(int ID)
        {
            ProductState Result = ProductState.Off;
            if (ID >= 0 && ID < Props.MaxProducts)
            {
                Result = cProductsState[ID];
            }
            return Result;
        }

        public void SetEnabledDefault()
        {
            // check for at least one product enabled and for default product
            bool DefaultFound = false;
            int EnabledID = -1;

            for (int i = 0; i < Props.MaxProducts - 2; i++)
            {
                clsProduct Prod = cProducts[i];
                if (Prod.Enabled)
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

        private void Core_AppExit(object sender, EventArgs e)
        {
            Core.UpdateStatus -= Core_UpdateStatus;
            Core.AppExit -= Core_AppExit;
            Update(true);
        }

        private void Core_UpdateStatus(object sender, EventArgs e)
        {
            Update();
            UpdateStatus();
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

        private void Update(bool SaveNow = false)
        {
            for (int i = 0; i < Props.MaxProducts; i++)
            {
                cProducts[i].Update();
            }

            if (((DateTime.Now - LastSave).TotalSeconds > 60) || SaveNow)
            {
                for (int i = 0; i < Props.MaxProducts; i++)
                {
                    cProducts[i].Save();
                }
                LastSave = DateTime.Now;
            }
        }

        private void UpdateStatus()
        {
            for (int i = 0; i < Props.MaxProducts; i++)
            {
                clsProduct pd = Item(i);
                ProductState NewState;

                if (!pd.Enabled)
                {
                    NewState = ProductState.Off;
                }
                else if (Core.RCalarm.Alarms[i])
                {
                    NewState = ProductState.Error;
                }
                else if (pd.RateSensorData.ModuleSending())
                {
                    if (pd.RateSensorData.ModuleReceiving())
                    {
                        NewState = ProductState.On;
                    }
                    else
                    {
                        NewState = ProductState.Sending;
                    }
                }
                else
                {
                    NewState = ProductState.Off;
                }
                cProductsState[i] = NewState;
            }
        }
    }

    public class RateSetChangedEventArgs : EventArgs
    {
        public RateSetChangedEventArgs(int index)
        {
            ProductIndex = index;
        }

        public int ProductIndex { get; }
    }
}