using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsSensor
    {
        private readonly int cID;       // 0-15
        private readonly FormStart mf;
        private byte cModuleID;         // 0-7
        private byte cKP;
        private byte cKI;
        private byte cMinAdjust;
        private byte cMaxAdjust;
        private byte cDeadband;         // actual X 10
        private byte cBrakePoint;
        private byte cSlowAdjust;
        private byte cSlewRate;
        private byte cMaxMotorIntegral; // actual X 100
        private byte cMaxValveIntegral;
        private byte cMinStart;
        private UInt16 cAdjustTime;
        private UInt16 cPauseTime;
        private byte cSampleTime;
        private byte cMinPulseHz;       // actual X 10
        private UInt16 cMaxPulseHz;     // actual X 10
        private byte cPulseSampleSize;
    }
}
