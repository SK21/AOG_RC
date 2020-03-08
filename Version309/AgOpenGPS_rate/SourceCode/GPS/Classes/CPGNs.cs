using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgOpenGPS
{
    class CPGNs
    {
        private readonly FormGPS mf;
        private byte cByteCount = 0;
        private byte[] cData = new byte[10];
        private byte[] cBackup = new byte[10];

        public CPGNs(FormGPS CallingForm, byte ByteCount)
        {
            mf = CallingForm;
            cByteCount = ByteCount;
        }

        public byte ByteCount { get { return cByteCount; } }

        public void SetPGN(int NewPGN)
        {
            cData[0] = (byte)(NewPGN >> 8);
            cData[1] = (byte)NewPGN;
        }

        public int GetPGN()
        {
            int Temp = cData[0] << 8;
            Temp |= cData[1];
            return Temp;
        }

        public bool SetByte(byte ID, byte Value)
        {
            if (ID>1 & ID<11)
            {
                cData[ID] = Value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public byte B(byte ID)
        {
            if (ID<11)
            {
                return cData[ID];
            }
            else
            {
                return 0;
            }
        }

        public byte GetBackup(byte ID)
        {
            if(ID<11)
            {
                return cBackup[ID];
            }
            else
            {
                return 0;
            }
        }

        public bool ByteChanged(byte ID)
        {
            if(ID<11)
            {
                return (cData[ID] != cBackup[ID]);
            }
            else
            {
                return false;
            }
        }

        public void Backup(byte ID)
        {
            if(ID<11)
            {
                cBackup[ID] = cData[ID];
            }
        }

        public bool MatchHeader(byte ByteHi, byte ByteLo)
        {
            return ((ByteHi == cData[0]) & (ByteLo == cData[1]));
        }
    }
}
