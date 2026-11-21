using RateController.Classes;
using System;

namespace RateController.PGNs
{
    public class PGN32509
    {
        // PGN32509, a product's units for a task controller, RC to module (per product, like 32500).
        // An ISOBUS board driven by a TC gets the rate in the ISO unit of the DDI (litres, kg or
        // seeds per ha); its flow cal is per RC unit. This tells it the type and how to convert.
        // Over CAN: 0xFF1D (CanFrameTranslator). The board stores it; only the ISOBUS build uses it.
        //0     HeaderLo    253
        //1     HeaderHi    126
        //2     ModSenID    high nibble module, low nibble sensor
        //3     DDI lo      ISO rate type: 1 volume, 6 mass, 11 count; 0 = the unit has no type
        //4     DDI hi
        //5-8   float32     metered units per litre, kg or seed (0 = no type)
        //9     CRC
        //
        // Sent when a product's type or factor changes and when its module connects, not
        // periodically: the board writes EEPROM when the value changes.

        private const byte cByteCount = 10;
        private readonly bool[] WasConnected = new bool[Props.MaxModules];
        private readonly string[] LastSent = new string[Props.MaxProducts];

        /// <summary>Call every second: sends each rate product whose units changed, or whose module just connected.</summary>
        public void Check()
        {
            try
            {
                bool[] JustConnected = new bool[Props.MaxModules];
                for (int m = 0; m < Props.MaxModules; m++)
                {
                    bool On = Core.ModulesStatus.Connected(m);
                    JustConnected[m] = On && !WasConnected[m];
                    WasConnected[m] = On;
                }

                for (int i = 0; i < Props.MaxProducts - 2; i++)     // the last two are fans
                {
                    clsProduct Prd = Core.Products.Item(i);
                    int Mod = Prd.ModuleID;
                    int Sen = Prd.SensorID;
                    if (Prd.Enabled && Mod >= 0 && Mod < Props.MaxModules && Sen < Props.MaxSensorsPerModule && Core.ModulesStatus.Connected(Mod))
                    {
                        Units(Prd, out ushort DDI, out float Factor);
                        string Key = Mod + "/" + Sen + "/" + DDI + "/" + Factor.ToString("R");
                        if (JustConnected[Mod] || Key != LastSent[i])
                        {
                            Send((byte)Mod, (byte)Sen, DDI, Factor);
                            LastSent[i] = Key;
                        }
                    }
                    else
                    {
                        LastSent[i] = null;     // sent again once it is back
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PGN32509/Check: " + ex.Message);
            }
        }

        // metered units per ISO unit: the board meters in the quantity unit, or in quantity / density
        // when density is on (clsProduct.TargetUPM), so one ISO unit is 1 / (factor x density) of them
        // (clsProduct.TCunitsPerISO, shared with the implement's actuals)
        private static void Units(clsProduct Prd, out ushort DDI, out float Factor)
        {
            DDI = 0;
            Factor = 0;
            double PerISO = Prd.TCunitsPerISO();
            if (PerISO > 0)
            {
                DDI = Prd.QuantityDDI;
                Factor = (float)PerISO;
            }
        }

        private void Send(byte Mod, byte Sen, ushort DDI, float Factor)
        {
            byte[] Data = new byte[cByteCount];
            Data[0] = 253;
            Data[1] = 126;
            Data[2] = Core.Tls.BuildModSenID(Mod, Sen);
            Data[3] = (byte)DDI;
            Data[4] = (byte)(DDI >> 8);
            byte[] F = BitConverter.GetBytes(Factor);   // little-endian on Windows, as the Teensy
            Array.Copy(F, 0, Data, 5, 4);
            Data[cByteCount - 1] = Core.Tls.CRC(Data, cByteCount - 1);

            // route through the CAN bridge or Ethernet, as PGN32505
            if (Props.CanEnabled && Core.CanBridgeComm != null)
            {
                Core.CanBridgeComm.SendModuleCommand(Data);
            }
            else
            {
                Core.UDPmodules.Send(Data);
            }
        }
    }
}
