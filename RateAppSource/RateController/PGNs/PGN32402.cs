using RateController.Classes;
using System;

namespace RateController.PGNs
{
    public class PGN32402
    {
        //PGN32402, PID diagnostics from module to RC (sent at PID-loop cadence)
        //0     HeaderLo    146
        //1     HeaderHi    126
        //2     Mod/Sen ID          0-15/0-15
        //3     millis Lo           PID loop timestamp, uint32
        //4     millis
        //5     millis
        //6     millis Hi
        //7     TargetUPM Lo        1000 X actual
        //8     TargetUPM Mid
        //9     TargetUPM Hi
        //10    UPM Lo              1000 X actual
        //11    UPM Mid
        //12    UPM Hi
        //13    Error Lo            1000 X actual, signed 24-bit
        //14    Error Mid
        //15    Error Hi
        //16    Integral Lo         10 X actual, signed 16-bit
        //17    Integral Hi
        //18    Change Lo           signed 16-bit
        //19    Change Hi
        //20    PWM Lo              signed 16-bit
        //21    PWM Hi
        //22    Samples             pulse count used in the median
        //23    CRC

        private const byte cByteCount = 24;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 146;

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;

            if (Data[0] == HeaderLo && Data[1] == HeaderHi &&
                Data.Length >= cByteCount && Core.Tls.GoodCRC(Data))
            {
                // signed 24-bit error: sign-extend bit 23
                int RawError = Data[13] | Data[14] << 8 | Data[15] << 16;
                if ((RawError & 0x800000) != 0) RawError |= unchecked((int)0xFF000000);

                PidLogSample s = new PidLogSample
                {
                    PCTime = DateTime.Now,
                    ModuleMillis = (uint)(Data[3] | Data[4] << 8 | Data[5] << 16 | Data[6] << 24),
                    ModuleID = Core.Tls.ParseModID(Data[2]),
                    SensorID = Core.Tls.ParseSenID(Data[2]),
                    Target = (Data[7] | Data[8] << 8 | Data[9] << 16) / 1000.0,
                    Applied = (Data[10] | Data[11] << 8 | Data[12] << 16) / 1000.0,
                    Error = RawError / 1000.0,
                    Integral = (Int16)(Data[16] | Data[17] << 8) / 10.0,
                    Change = (Int16)(Data[18] | Data[19] << 8),
                    PWM = (Int16)(Data[20] | Data[21] << 8),
                    Samples = Data[22]
                };

                Core.PidLogger?.Log(s);
                Result = true;
            }
            return Result;
        }
    }
}
