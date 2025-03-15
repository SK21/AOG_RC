using System.Text;

namespace RateController.PGNs
{
    public class PGNs_WifiConfig
    {
        // PGN 19   Network name bytes 0-7
        // PGN 20   Network name bytes 8-15
        // PGN 21   Network password bytes 0-7
        // PGN 22   Network password bytes 8-15

        private string cNetworkName;
        private string cNetworkPassword;
        private FormStart mf;

        public PGNs_WifiConfig(FormStart Main)
        {
            mf = Main;
            Load();
        }

        public string NetworkName
        {
            set
            { cNetworkName = value; }
            get { return cNetworkName; }
        }

        public string NetworkPassword
        {
            set
            { cNetworkPassword = value; }
            get { return cNetworkPassword; }
        }

        public void Load()
        {
            cNetworkName = mf.Tls.LoadProperty("NetworkName");
            cNetworkPassword = mf.Tls.LoadProperty("NetworkPassword");
        }

        public void Save()
        {
            mf.Tls.SaveProperty("NetworkName", cNetworkName);
            mf.Tls.SaveProperty("NetworkPassword", cNetworkPassword);
        }

        public void Send()
        {
            SendField(cNetworkName, 19, 20);  // Network name: PGN 19 and 20
            SendField(cNetworkPassword, 21, 22); // Password: PGN 21 and 22
        }

        private void SendField(string value, byte firstPgn, byte secondPgn)
        {
            string paddedValue = value.PadRight(16, '\0');

            // First 8 bytes
            byte[] data = Encoding.ASCII.GetBytes(paddedValue.Substring(0, 8));
            mf.CanBus1.SendCanMessage(firstPgn, 0, 0, data);

            // Next 8 bytes
            data = Encoding.ASCII.GetBytes(paddedValue.Substring(8, 8));
            mf.CanBus1.SendCanMessage(secondPgn, 0, 0, data);
        }
    }
}