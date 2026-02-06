using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public class PGN32502
    {
        // PGN32502, Control settings from RC to module
        // 0    246
        // 1    126
        // 2    Mod/Sen ID     0-15/0-15
        // 3    MaxPWM
        // 4    MinPWM
        // 5    Kp
        // 6    Ki
        // 7    Deadband        %       actual X 10
        // 8    Brakepoint      %
        // 9    PIDslowAdjust   %
        // 10   Slew Rate
        // 11   Max Integral      actual X 10
        // 12   -
        // 13   TimedMinStart
        // 14   TimedAdjust Lo
        // 15   TimedAdjust Hi
        // 16   TimedPause Lo
        // 17   TimedPause Hi
        // 18   PIDtime
        // 19   PulseMinHz              actual X 10
        // 20   PulseMaxHz Lo
        // 21   PulseMaxHz Hi
        // 22   PulseSampleSize
        // 23   CRC

        private const byte cByteCount = 24;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 246;
        private readonly clsProduct Prod;

        public PGN32502(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
        }

        public void Send()
        {
            if (Prod.ModuleID >= 0 && Prod.SensorID >= 0)
            {
                clsSensorSettings Sen = Prod.RateSensor;
                byte[] Data = new byte[cByteCount];
                Data[0] = HeaderLo;
                Data[1] = HeaderHi;
                Data[2] = Core.Tls.BuildModSenID((byte)Prod.ModuleID, Prod.SensorID);

                Data[3] = Sen.MaxPWM;
                Data[4] = Sen.MinPWM;
                Data[5] = Sen.KP;
                Data[6] = Sen.KI;
                Data[7] = Sen.DeadBand;
                Data[8] = Sen.BrakePoint;
                Data[9] = Sen.PIDslowAdjust;
                Data[10] = Sen.SlewRate;
                Data[11] = Sen.MaxIntegral;
                Data[12] = 0;
                Data[13] = Sen.TimedMinStart;
                Data[14] = (byte)Sen.TimedAdjust;
                Data[15] = (byte)(Sen.TimedAdjust >> 8);
                Data[16] = (byte)Sen.TimedPause;
                Data[17] = (byte)(Sen.TimedPause >> 8);
                Data[18] = Sen.PIDtime;
                Data[19] = Sen.PulseMinHz;
                Data[20] = (byte)(Sen.PulseMaxHz);
                Data[21] = (byte)(Sen.PulseMaxHz >> 8);
                Data[22] = Sen.PulseSampleSize;

                Data[23] = Core.Tls.CRC(Data, cByteCount - 1);

                // send - route through gateway if ISOBUS enabled
                if (Props.IsobusEnabled && Core.IsobusComm != null)
                {
                    Core.IsobusComm.SendModuleCommand(Data);
                }
                else
                {
                    Core.UDPmodules.Send(Data);
                }
            }
        }
    }
}
