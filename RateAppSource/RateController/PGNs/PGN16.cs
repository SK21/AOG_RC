using System;

namespace RateController.PGNs
{
    public class PGN16
    {
        // module config 2
        // 0	sensor 0, flow pin
        // 1	sensor 0, dir pin
        // 2	sensor 0, pwm pin
        // 3	sensor 1, flow pin
        // 4	sensor 1, dir pin
        // 5	sensor 1, pwm pin

        private byte[] cData;
        private int cModuleID;
        private FormStart mf;

        public PGN16(FormStart Main, int ModuleID)
        {
            mf = Main;
            cData = new byte[6];
            Load();
            cModuleID = ModuleID;
        }

        public byte Sensor0Dir
        {
            get { return cData[1]; }
            set { cData[1] = value; }
        }

        public byte Sensor0Flow
        {
            get { return cData[0]; }
            set { cData[0] = value; }
        }

        public byte Sensor0PWM
        {
            get { return cData[2]; }
            set { cData[2] = value; }
        }

        public byte Sensor1Dir
        {
            get { return cData[4]; }
            set { cData[4] = value; }
        }

        public byte Sensor1Flow
        {
            get { return cData[3]; }
            set { cData[3] = value; }
        }

        public byte Sensor1PWM
        {
            get { return cData[5]; }
            set { cData[5] = value; }
        }
        public byte[] GetData()
        {
            return cData;
        }

        public void Save()
        {
            String Name;
            for (int i = 2; i < cData.Length; i++)
            {
                Name = "Module" + cModuleID.ToString() + "_Config2_Data" + i.ToString();
                mf.Tls.SaveProperty(Name, cData[i].ToString());
            }
        }

        public void Send()
        {
            mf.CanBus1.SendCanMessage(16, (byte)cModuleID, 0, cData);
        }

        private void Load()
        {
            String Name;
            Array.Clear(cData, 0, cData.Length);

            for (int i = 2; i < cData.Length; i++)
            {
                Name = "Module" + cModuleID.ToString() + "_Config2_Data" + i.ToString();
                if (byte.TryParse(mf.Tls.LoadProperty(Name), out byte Val))
                {
                    cData[i] = Val;
                }
            }
        }
    }
}