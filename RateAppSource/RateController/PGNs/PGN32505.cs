using RateController.Classes;
using System;

namespace RateController.PGNs
{
    public class PGN32505
    {
        // PGN32505, Max pressure gate threshold from RC to module (raw ADC counts)
        //0     HeaderLo    249
        //1     HeaderHi    126
        //2     ModuleID    0-7
        //3     MaxLo       raw ADC threshold lo (0xFFFF = gate disabled)
        //4     MaxHi
        //5     CRC

        public const ushort Disabled = 0xFFFF;

        private const byte cByteCount = 6;
        private byte[] cData = new byte[cByteCount];

        // Converts the per-module MaxPressure (engineering units) to a raw ADC threshold,
        // the inverse of the forward calibration used in Props.PressureReading().
        // Returns Disabled (0xFFFF) when there is no usable threshold.
        public ushort RawThreshold(int moduleID)
        {
            ushort Result = Disabled;
            try
            {
                double maxPressure = Props.GetMaxPressure(moduleID);
                if (maxPressure > 0)
                {
                    double MinVol = Props.GetPressureCal(moduleID * 5);       // x1
                    double MinPres = Props.GetPressureCal(moduleID * 5 + 1);  // y1
                    double MaxVol = Props.GetPressureCal(moduleID * 5 + 2);   // x2
                    double MaxPres = Props.GetPressureCal(moduleID * 5 + 3);  // y2

                    if ((MaxVol - MinVol) != 0)
                    {
                        double M = (MaxPres - MinPres) / (MaxVol - MinVol);
                        if (M > 0)
                        {
                            double B = MaxPres - M * MaxVol;
                            double reading = (maxPressure - B) / M;

                            // reading <= 0   : threshold below the sensor's zero, no sensible gate -> leave disabled
                            // reading large  : above any real ADC value, gate would never trip -> clamp just under
                            //                  the sentinel so it stays an (extreme) threshold, never 0xFFFF
                            if (reading > 0)
                            {
                                if (reading >= Disabled) reading = Disabled - 1;
                                Result = (ushort)(reading + 0.5);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PGN32505/RawThreshold: " + ex.Message);
            }
            return Result;
        }

        public void Send(int moduleID)
        {
            if (moduleID >= 0 && moduleID < Props.MaxModules)
            {
                ushort raw = RawThreshold(moduleID);

                Array.Clear(cData, 0, cByteCount);
                cData[0] = 249;
                cData[1] = 126;
                cData[2] = (byte)moduleID;
                cData[3] = (byte)(raw & 0xFF);
                cData[4] = (byte)(raw >> 8);
                cData[cByteCount - 1] = Core.Tls.CRC(cData, cByteCount - 1);

                // send - route through CAN bridge or Ethernet (same pattern as PGN32504)
                if (Props.CanEnabled && Core.CanBridgeComm != null)
                {
                    Core.CanBridgeComm.SendModuleCommand(cData);
                }
                else
                {
                    Core.UDPmodules.Send(cData);
                }
            }
        }
    }
}
