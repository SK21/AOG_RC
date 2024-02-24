using System.Text;

namespace RateController
{
    public class PGN32702
    {
        //PGN32702, Network Config to modules
        //0     HeaderLo    190
        //1     HeaderHi    127
        //2-16  Network Name
        //17-31 Network Password
        //32    CRC

        private const byte cByteCount = 33;
        private const byte HeaderLo = 190;
        private const byte HeaderHi = 127;
        private byte[] cData = new byte[cByteCount];
        private string cNetworkName;
        private string cNetworkPassword;
        private FormStart mf;

        public PGN32702(FormStart Main)
        {
            mf = Main;
            cData[0] = HeaderLo;
            cData[1] = HeaderHi;
            Load();
        }

        public string NetworkName
        {
            set
            {
                for (int i = 0; i < 14; i++)
                {
                    cData[i + 2] = 0;
                }
                int Len = value.Length;
                if (Len > 14) Len = 14;
                byte[] Nm = Encoding.ASCII.GetBytes(value);
                for (int i = 0; i < Len; i++)
                {
                    cData[i + 2] = Nm[i];
                }
                cNetworkName = value;
            }
            get { return cNetworkName; }
        }

        public string NetworkPassword
        {
            set
            {
                for (int i = 0; i < 14; i++)
                {
                    cData[i + 17] = 0;
                }
                int Len = value.Length;
                if (Len > 14) Len = 14;
                byte[] Nm = Encoding.ASCII.GetBytes(value);
                for (int i = 0; i < Len; i++)
                {
                    cData[i + 17] = Nm[i];
                }
                cNetworkPassword = value;
            }
            get { return cNetworkPassword; }
        }

        public void Load()
        {
            NetworkName = mf.Tls.LoadProperty("NetworkName");
            NetworkPassword = mf.Tls.LoadProperty("NetworkPassword");
        }

        public void Save()
        {
            mf.Tls.SaveProperty("NetworkName", cNetworkName);
            mf.Tls.SaveProperty("NetworkPassword", cNetworkPassword);
        }

        public void Send()
        {
            // CRC
            cData[cByteCount - 1] = mf.Tls.CRC(cData, cByteCount - 1);

            // send
            mf.SendSerial(cData);
            mf.UDPmodules.SendUDPMessage(cData);
        }
    }
}