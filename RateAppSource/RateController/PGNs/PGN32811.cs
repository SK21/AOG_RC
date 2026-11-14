using RateController.Classes;

namespace RateController.PGNs
{
    public class PGN32811
    {
        //PGN32811, firmware update block, RC to module
        //0     HeaderLo    43
        //1     HeaderHi    128
        //2-3   block number
        //4-5   data length n (up to 11 hex records)
        //6..   hex records: len, addrHi, addrLo, type, data, checksum
        //6+n   CRC-16 Lo
        //7+n   CRC-16 Hi
        //8+n   CRC
        // Over CAN it goes as a 0xFF1A block header ([0] 2, [1-2] block, [3-4] n, [5-6] CRC-16)
        // then 0xFF1B frames ([0] frame index, [1-7] 7 data bytes).
        // Design: Design Docs\Teensy_CAN_Firmware_Update.md

        public const int MaxBlock = 11 * 21;    // 11 records of up to 16 data bytes; the module's limit

        public void Send(int Block, byte[] Records, bool OverCan)
        {
            int n = Records.Length;
            byte[] Data = new byte[n + 9];
            Data[0] = 43;
            Data[1] = 128;
            Data[2] = (byte)Block;
            Data[3] = (byte)(Block >> 8);
            Data[4] = (byte)n;
            Data[5] = (byte)(n >> 8);
            System.Array.Copy(Records, 0, Data, 6, n);
            ushort crc = CRC16(Records);
            Data[6 + n] = (byte)crc;
            Data[7 + n] = (byte)(crc >> 8);
            Data[8 + n] = Core.Tls.CRC(Data, n + 8);

            if (OverCan)
            {
                Core.CanBridgeComm?.SendModuleCommand(Data);
            }
            else
            {
                Core.UDPmodules.Send(Data);
            }
        }

        // CRC-16/CCITT-FALSE (poly 0x1021, init 0xFFFF), the same in the firmware
        public static ushort CRC16(byte[] Data)
        {
            ushort Result = 0xFFFF;
            for (int i = 0; i < Data.Length; i++)
            {
                Result ^= (ushort)(Data[i] << 8);
                for (int b = 0; b < 8; b++)
                {
                    Result = (Result & 0x8000) != 0 ? (ushort)((Result << 1) ^ 0x1021) : (ushort)(Result << 1);
                }
            }
            return Result;
        }
    }
}
