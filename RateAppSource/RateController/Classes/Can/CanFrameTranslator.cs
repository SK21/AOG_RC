using System;
using System.Collections.Generic;

namespace RateController.Classes.Can
{
    /// <summary>
    /// Translates between proprietary CAN frames (0xFF00-0xFF0B) and RC UDP PGN byte arrays.
    /// Stateful: caches partial frames per module to assemble multi-source PGNs.
    ///
    /// Frame protocol (Teensy → RC):
    ///   0xFF00: Sensor rate/qty  (200ms cyclic)
    ///   0xFF01: Sensor PWM/Hz    (200ms cyclic, paired with 0xFF00)
    ///   0xFF02: Module status    (200ms cyclic)
    ///   0xFF08: Module ident     (500ms cyclic)
    ///   0xFF09: Wheel counts     (500ms, calibration mode)
    ///
    /// Frame protocol (RC → Teensy):
    ///   0xFF03: Rate command     (from PGN 32500)
    ///   0xFF04: Relay command    (from PGN 32501)
    ///   0xFF05: PID settings 1   (from PGN 32502, bytes 3-9)
    ///   0xFF06: PID settings 2   (from PGN 32502, bytes 10-18)
    ///   0xFF07: Wheel config     (from PGN 32504)
    ///   0xFF0A: Flow calibration (from PGN 32500, bytes 6-8)
    ///   0xFF0B: PID settings 3   (from PGN 32502, bytes 13, 19-22)
    ///   0xFF0C: Module config 1  (from PGN 32700, bytes 2-9)
    ///   0xFF0D: Module config 2  (from PGN 32700, bytes 10-17)
    ///   0xFF0E: Module config 3  (from PGN 32700, bytes 18-25)
    ///   0xFF0F: Module config 4  (from PGN 32700, bytes 26-31, commit)
    ///   0xFF11: Board label set 1 (from PGN 32506, chars 0-6)
    ///   0xFF12: Board label set 2 (from PGN 32506, chars 7-13)
    ///   0xFF13: Board label set 3 (from PGN 32506, chars 14-15, commit)
    ///   0xFF17: Sensor pins      (from PGN 32507, one frame per sensor)
    ///
    /// Frame protocol (Teensy → RC):
    ///   0xFF14: Board label rpt 1 (→ PGN 32403, chars 0-6)
    ///   0xFF15: Board label rpt 2 (→ PGN 32403, chars 7-13)
    ///   0xFF16: Board label rpt 3 (→ PGN 32403, chars 14-15, assemble)
    /// Board-label frames carry moduleId in data[0] low nibble + 7 chars in data[1-7].
    /// </summary>
    public class CanFrameTranslator
    {
        // Source address used in CAN IDs for frames sent by RC
        private const byte RC_SOURCE_ADDRESS = 0xF9;

        // Per-module cache for assembling multi-frame inbound PGNs
        private class ModuleState
        {
            public byte[] Last0xFF00;  // SensorRateQty
            public byte[] Last0xFF01;  // SensorPwmHz
            public byte[] Last0xFF02;  // ModuleStatus
            public byte[] Last0xFF08;  // ModuleIdentification
            public byte[] Last0xFF09;  // WheelCounts (optional, calibration mode)
            public byte[] Last0xFF14;  // BoardLabel report part 1
            public byte[] Last0xFF15;  // BoardLabel report part 2
            public byte[] Last0xFF16;  // BoardLabel report part 3
            public DateTime Last0xFF08Time = DateTime.MinValue;
            public bool WasConnected = false;
        }

        private readonly Dictionary<int, ModuleState> _modules = new Dictionary<int, ModuleState>();

        /// <summary>Raised when PGN 32400 (sensor data) is assembled from 0xFF00+0xFF01.</summary>
        public event EventHandler<PgnAssembledEventArgs> PGN32400Ready;

        /// <summary>Raised when PGN 32401 (module status) is assembled from 0xFF02+0xFF08.</summary>
        public event EventHandler<PgnAssembledEventArgs> PGN32401Ready;

        /// <summary>Raised when PGN 32403 (board label) is assembled from 0xFF14+0xFF15+0xFF16.</summary>
        public event EventHandler<PgnAssembledEventArgs> PGN32403Ready;

        /// <summary>Raised when 0xFF08 is first seen from a module (module came online).</summary>
        public event EventHandler<int> ModuleConnected;

        /// <summary>Raised when 0xFF08 heartbeat absent > 2 seconds (module went offline).</summary>
        public event EventHandler<int> ModuleDisconnected;

        /// <summary>
        /// Call periodically (e.g., every 500ms) to detect timed-out modules.
        /// </summary>
        public void CheckTimeouts()
        {
            var toDisconnect = new List<int>();
            foreach (var kv in _modules)
            {
                var st = kv.Value;
                if (st.WasConnected && (DateTime.Now - st.Last0xFF08Time).TotalSeconds > 2.0)
                    toDisconnect.Add(kv.Key);
            }
            foreach (int id in toDisconnect)
            {
                _modules[id].WasConnected = false;
                ModuleDisconnected?.Invoke(this, id);
            }
        }

        /// <summary>
        /// Process a received CAN frame. Triggers PGN32400Ready or PGN32401Ready
        /// when enough frames have been collected to assemble a complete PGN.
        /// Only processes extended Proprietary B frames (PF=0xFF).
        /// </summary>
        public void ProcessFrame(CanFrame frame)
        {
            if (!frame.IsExtended || frame.Data == null || frame.Dlc < 1) return;

            // Extract PDU Format (PF) from 29-bit CAN ID
            // ID layout: [28-26]=priority, [25]=R, [24]=DP, [23-16]=PF, [15-8]=PS, [7-0]=SA
            byte pf = (byte)((frame.Id >> 16) & 0xFF);
            byte ps = (byte)((frame.Id >> 8) & 0xFF);

            if (pf != 0xFF) return;  // Only handle Proprietary B

            byte pgnLow = ps;
            int moduleId = frame.Data[0] & 0x0F;
            var state = GetOrCreateState(moduleId);

            switch (pgnLow)
            {
                case 0x00:  // 0xFF00 — Sensor Rate/Quantity
                    if (frame.Dlc >= 8)
                    {
                        state.Last0xFF00 = CloneData(frame.Data, 8);
                        TryAssemble32400(state);
                    }
                    break;

                case 0x01:  // 0xFF01 — Sensor PWM/Hz
                    if (frame.Dlc >= 8)
                    {
                        state.Last0xFF01 = CloneData(frame.Data, 8);
                        TryAssemble32400(state);
                    }
                    break;

                case 0x02:  // 0xFF02 — Module Status
                    if (frame.Dlc >= 6)
                    {
                        state.Last0xFF02 = CloneData(frame.Data, 8);
                        TryAssemble32401(state);
                    }
                    break;

                case 0x08:  // 0xFF08 — Module Identification
                    if (frame.Dlc >= 4)
                    {
                        bool firstTime = !state.WasConnected;
                        state.Last0xFF08 = CloneData(frame.Data, 8);
                        state.Last0xFF08Time = DateTime.Now;
                        if (firstTime)
                        {
                            state.WasConnected = true;
                            ModuleConnected?.Invoke(this, moduleId);
                        }
                        // Do NOT call TryAssemble32401 here — 0xFF08 is a 500ms heartbeat
                        // and would cause spurious short elapsed times when it interleaves
                        // with the 200ms 0xFF02 cycle. Assembly is driven by 0xFF02 only.
                    }
                    break;

                case 0x09:  // 0xFF09 — Wheel Counts (24-bit, calibration mode)
                    if (frame.Dlc >= 4)
                        state.Last0xFF09 = CloneData(frame.Data, 8);
                    break;

                case 0x14:  // 0xFF14 — Board label report part 1
                    if (frame.Dlc >= 8)
                    {
                        state.Last0xFF14 = CloneData(frame.Data, 8);
                        TryAssemble32403(state);
                    }
                    break;

                case 0x15:  // 0xFF15 — Board label report part 2
                    if (frame.Dlc >= 8)
                    {
                        state.Last0xFF15 = CloneData(frame.Data, 8);
                        TryAssemble32403(state);
                    }
                    break;

                case 0x16:  // 0xFF16 — Board label report part 3 (assemble)
                    if (frame.Dlc >= 3)
                    {
                        state.Last0xFF16 = CloneData(frame.Data, 8);
                        TryAssemble32403(state);
                    }
                    break;
            }
        }

        // --- Inbound assembly ---

        private void TryAssemble32400(ModuleState state)
        {
            if (state.Last0xFF00 == null || state.Last0xFF01 == null) return;

            byte[] f00 = state.Last0xFF00;
            byte[] f01 = state.Last0xFF01;

            // PGN 32400 (15 bytes): header 0x90 0x7E
            //  [2]    ModSenId         ← f00[0] nibble-swapped (CAN: low=ModID,high=SenID → UDP: high=ModID,low=SenID)
            //  [3-5]  rateApplied      ← f00[1-3]
            //  [6-8]  accumulatedQty   ← f00[4-6]
            //  [9-10] pwmSetting       ← f01[1-2]
            //  [11]   status           ← f00[7]
            //  [12-13]pulseHz          ← f01[3-4]
            //  [14]   CRC
            var pgn = new byte[15];
            pgn[0] = 0x90;
            pgn[1] = 0x7E;
            pgn[2] = SwapModSenNibbles(f00[0]); // CAN packs ModID in low nibble; UDP expects ModID in high nibble
            pgn[3] = f00[1];
            pgn[4] = f00[2];
            pgn[5] = f00[3];
            pgn[6] = f00[4];
            pgn[7] = f00[5];
            pgn[8] = f00[6];
            pgn[9] = f01[1];
            pgn[10] = f01[2];
            pgn[11] = f00[7];
            pgn[12] = f01[3];
            pgn[13] = f01[4];
            pgn[14] = CalcCrc(pgn, 14);

            PGN32400Ready?.Invoke(this, new PgnAssembledEventArgs(pgn));
        }

        private void TryAssemble32401(ModuleState state)
        {
            if (state.Last0xFF02 == null || state.Last0xFF08 == null) return;

            byte[] f02 = state.Last0xFF02;
            byte[] f08 = state.Last0xFF08;
            byte[] f09 = state.Last0xFF09;  // may be null

            // PGN 32401 (15 bytes): header 0x91 0x7E
            //  [2]    moduleId         ← f02[0] & 0x0F
            //  [3-4]  pressure         ← f02[2-3]
            //  [5-6]  wheelSpeed       ← f02[4-5]
            //  [7-9]  wheelCounts      ← f09[1-3] if available, else f02[6-7] + 0
            //  [10]   inoType          ← f08[1]
            //  [11-12]inoId            ← f08[2-3]
            //  [13]   status           ← reassembled from f02 bits
            //  [14]   CRC
            //
            // Status byte reassembly from 0xFF02:
            //   f02[0] bit4 = workSwitch    → pgn[13] bit0
            //   f02[1] bits0-2 = wifiRSSI  → pgn[13] bits1-3
            //   f02[0] bit5 = ethernetConn → pgn[13] bit4
            //   f02[0] bit6 = goodPinCfg   → pgn[13] bit5
            //   f02[0] bit7 = configRejected → pgn[13] bit7
            var pgn = new byte[15];
            pgn[0] = 0x91;
            pgn[1] = 0x7E;
            pgn[2] = (byte)(f02[0] & 0x0F);
            pgn[3] = f02.Length > 2 ? f02[2] : (byte)0;
            pgn[4] = f02.Length > 3 ? f02[3] : (byte)0;
            pgn[5] = f02.Length > 4 ? f02[4] : (byte)0;
            pgn[6] = f02.Length > 5 ? f02[5] : (byte)0;

            if (f09 != null && f09.Length >= 4)
            {
                pgn[7] = f09[1];
                pgn[8] = f09[2];
                pgn[9] = f09[3];
            }
            else
            {
                pgn[7] = f02.Length > 6 ? f02[6] : (byte)0;
                pgn[8] = f02.Length > 7 ? f02[7] : (byte)0;
                pgn[9] = 0;
            }

            pgn[10] = f08.Length > 1 ? f08[1] : (byte)0;
            pgn[11] = f08.Length > 2 ? f08[2] : (byte)0;
            pgn[12] = f08.Length > 3 ? f08[3] : (byte)0;

            byte status = 0;
            if ((f02[0] & 0x10) != 0) status |= 0x01;
            status |= (byte)(((f02.Length > 1 ? f02[1] : 0) & 0x07) << 1);
            if ((f02[0] & 0x20) != 0) status |= 0x10;
            if ((f02[0] & 0x40) != 0) status |= 0x20;
            if ((f02[0] & 0x80) != 0) status |= 0x80;
            pgn[13] = status;

            pgn[14] = CalcCrc(pgn, 14);

            PGN32401Ready?.Invoke(this, new PgnAssembledEventArgs(pgn));
        }

        private void TryAssemble32403(ModuleState state)
        {
            if (state.Last0xFF14 == null || state.Last0xFF15 == null || state.Last0xFF16 == null) return;

            // PGN 32403 (20 bytes): header 0x93 0x7E
            //  [2]     moduleId   ← f14[0] & 0x0F
            //  [3-9]   chars 0-6  ← f14[1-7]
            //  [10-16] chars 7-13 ← f15[1-7]
            //  [17-18] chars 14-15← f16[1-2]
            //  [19]    CRC
            var pgn = new byte[20];
            pgn[0] = 0x93;
            pgn[1] = 0x7E;
            pgn[2] = (byte)(state.Last0xFF14[0] & 0x0F);
            Array.Copy(state.Last0xFF14, 1, pgn, 3, 7);
            Array.Copy(state.Last0xFF15, 1, pgn, 10, 7);
            pgn[17] = state.Last0xFF16[1];
            pgn[18] = state.Last0xFF16[2];
            pgn[19] = CalcCrc(pgn, 19);

            PGN32403Ready?.Invoke(this, new PgnAssembledEventArgs(pgn));
        }

        // --- Outbound translation ---

        /// <summary>
        /// Translate an outbound PGN byte array into one or more CAN frames.
        /// Called by CanBridgeComm.SendModuleCommand().
        /// </summary>
        public List<CanFrame> TranslateOutbound(byte[] pgnData)
        {
            var frames = new List<CanFrame>();
            if (pgnData == null || pgnData.Length < 2) return frames;

            int pgn = pgnData[0] | (pgnData[1] << 8);

            switch (pgn)
            {
                case 32500:
                    // → 0xFF03 (rate command) + 0xFF0A (flow calibration)
                    // PGN32500 layout: [2]=ModSenId (UDP: high=ModID,low=SenID), [3-5]=rateSet, [6-8]=flowCal, [9]=cmd, [10-11]=manualPwm
                    // 0xFF03 Teensy reads: buf[0]=ModSenId (CAN: low=ModID,high=SenID), buf[1-3]=rate, buf[4-5]=manualPwm, buf[6]=cmd
                    if (pgnData.Length >= 12)
                    {
                        byte canModSenId = SwapModSenNibbles(pgnData[2]); // UDP high=ModID → CAN low=ModID
                        frames.Add(BuildFrame(0x03, new byte[] {
                            canModSenId,  // ModSenId (CAN nibble order)
                            pgnData[3],   // rateSetpoint lo
                            pgnData[4],   // rateSetpoint mid
                            pgnData[5],   // rateSetpoint hi
                            pgnData[10],  // manualPwm lo
                            pgnData[11],  // manualPwm hi
                            pgnData[9],   // command byte
                            0             // reserved
                        }));
                        frames.Add(BuildFrame(0x0A, new byte[] {
                            canModSenId,  // ModSenId (CAN nibble order)
                            pgnData[6],   // flowCal lo
                            pgnData[7],   // flowCal mid
                            pgnData[8],   // flowCal hi
                            0, 0, 0, 0
                        }));
                    }
                    break;

                case 32501:
                    // → 0xFF04 (relay states)
                    // PGN32501 layout: [2]=ModId, [3-4]=relayStates, [5-6]=powerRelays, [7-8]=inverted
                    // 0xFF04 Teensy reads: buf[0]=modId, buf[1-2]=relayLo/Hi, buf[3-4]=powerLo/Hi, buf[5-6]=invertLo/Hi
                    if (pgnData.Length >= 9)
                    {
                        frames.Add(BuildFrame(0x04, new byte[] {
                            pgnData[2],  // moduleId
                            pgnData[3],  // relayStatesLo
                            pgnData[4],  // relayStatesHi
                            pgnData[5],  // powerRelaysLo
                            pgnData[6],  // powerRelaysHi
                            pgnData[7],  // invertedLo
                            pgnData[8],  // invertedHi
                            0
                        }));
                    }
                    break;

                case 32502:
                    // → 0xFF05 (PID part 1) + 0xFF06 (PID part 2) + 0xFF0B (PID part 3)
                    // PGN32502 layout: [2]=ModSenId (UDP: high=ModID,low=SenID), [3]=MaxPWM, [4]=MinPWM, [5]=Kp, [6]=Ki,
                    //   [7]=Deadband, [8]=BrakePoint, [9]=PIDslowAdjust, [10]=SlewRate,
                    //   [11]=MaxIntegral, [12]=0, [13]=TimedMinStart, [14-15]=TimedAdjust,
                    //   [16-17]=TimedPause, [18]=PIDtime, [19]=PulseMinHz, [20-21]=PulseMaxHz,
                    //   [22]=PulseSampleSize
                    if (pgnData.Length >= 23)
                    {
                        byte canModSenId = SwapModSenNibbles(pgnData[2]); // UDP high=ModID → CAN low=ModID
                        frames.Add(BuildFrame(0x05, new byte[] {
                            canModSenId,  // ModSenId (CAN nibble order)
                            pgnData[3],   // MaxPWM (0-100%, Teensy applies 255*val/100)
                            pgnData[4],   // MinPWM
                            pgnData[5],   // Kp (scrollbar, Teensy: pow(1.1, val-120))
                            pgnData[6],   // Ki
                            pgnData[7],   // Deadband (×10, Teensy: val/1000)
                            pgnData[8],   // BrakePoint
                            pgnData[9]    // PIDslowAdjust
                        }));
                        frames.Add(BuildFrame(0x06, new byte[] {
                            canModSenId,  // ModSenId (CAN nibble order)
                            pgnData[10],  // SlewRate
                            pgnData[11],  // MaxIntegral (×10, Teensy: val/10)
                            pgnData[14],  // TimedAdjust lo
                            pgnData[15],  // TimedAdjust hi
                            pgnData[16],  // TimedPause lo
                            pgnData[17],  // TimedPause hi
                            pgnData[18]   // PIDtime
                        }));
                        frames.Add(BuildFrame(0x0B, new byte[] {
                            canModSenId,  // ModSenId (CAN nibble order)
                            pgnData[13],  // TimedMinStart (Teensy: val/100)
                            pgnData[19],  // PulseMinHz (Hz×10, Teensy: 10000000/val µs)
                            pgnData[20],  // PulseMaxHz lo (Hz, Teensy: 1000000/val µs)
                            pgnData[21],  // PulseMaxHz hi
                            pgnData[22],  // PulseSampleSize
                            0, 0
                        }));
                    }
                    break;

                case 32504:
                    // → 0xFF07 (wheel speed config)
                    // PGN32504 layout: [2]=moduleId, [3]=GPIO pin, [4-6]=Cal, [7]=commands
                    if (pgnData.Length >= 8)
                    {
                        frames.Add(BuildFrame(0x07, new byte[] {
                            pgnData[2],  // moduleId
                            pgnData[3],  // GPIO pin (255=disable)
                            pgnData[4],  // Cal lo
                            pgnData[5],  // Cal mid
                            pgnData[6],  // Cal hi
                            pgnData[7],  // commands (bit0=eraseCounts)
                            0, 0
                        }));
                    }
                    break;

                case 32505:
                    // → 0xFF10 (max pressure gate threshold)
                    // PGN32505 layout: [2]=moduleId, [3]=MaxLo, [4]=MaxHi (raw ADC, 0xFFFF=disabled)
                    if (pgnData.Length >= 5)
                    {
                        frames.Add(BuildFrame(0x10, new byte[] {
                            pgnData[2],  // moduleId
                            pgnData[3],  // MaxPressureReading lo
                            pgnData[4],  // MaxPressureReading hi
                            0, 0, 0, 0, 0
                        }));
                    }
                    break;

                case 32506:
                    // → 0xFF11/0xFF12/0xFF13 (board label, 16 chars in 3 frames, commit on 0xFF13)
                    // PGN32506 layout: [2]=moduleId, [3-18]=16 chars (0-padded)
                    // Each frame: [0]=moduleId (low nibble), [1-7]=7 chars
                    if (pgnData.Length >= 20)
                    {
                        byte mod = (byte)(pgnData[2] & 0x0F);
                        frames.Add(BuildFrame(0x11, new byte[] {
                            mod, pgnData[3], pgnData[4], pgnData[5], pgnData[6], pgnData[7], pgnData[8], pgnData[9]
                        }));
                        frames.Add(BuildFrame(0x12, new byte[] {
                            mod, pgnData[10], pgnData[11], pgnData[12], pgnData[13], pgnData[14], pgnData[15], pgnData[16]
                        }));
                        frames.Add(BuildFrame(0x13, new byte[] {
                            mod, pgnData[17], pgnData[18], 0, 0, 0, 0, 0
                        }));
                    }
                    break;

                case 32507:
                    // → 0xFF17 (sensor pins, one frame per sensor packet)
                    // PGN32507 layout: [2]=ModSenId (UDP: high=ModID,low=SenID), [3]=Flow pin,
                    //   [4]=Dir pin, [5]=PWM pin, [6]=Bin pin, [7]=flags (bit0=invert bin), [8-9]=spare
                    if (pgnData.Length >= 10)
                    {
                        frames.Add(BuildFrame(0x17, new byte[] {
                            SwapModSenNibbles(pgnData[2]),  // ModSenId (CAN nibble order)
                            pgnData[3],  // flow pin
                            pgnData[4],  // dir pin
                            pgnData[5],  // pwm pin
                            pgnData[6],  // bin pin
                            pgnData[7],  // flags
                            pgnData[8],  // spare
                            pgnData[9]   // spare
                        }));
                    }
                    break;

                case 32700:
                    // → 0xFF0C (config part 1) + 0xFF0D (part 2) + 0xFF0E (part 3) + 0xFF0F (part 4/commit)
                    // PGN32700 layout: [2]=ModID, [3]=SensorCount, [4]=commands,
                    //   [5]=OnboardRelayType, [6]=RemoteRelayType,
                    //   [7-9]=Sensor0 pins (Flow/Dir/PWM), [10-12]=Sensor1 pins,
                    //   [13-28]=RelayPins[0-15], [29]=WorkPin, [30]=PressurePin, [31]=CommMode
                    // 0xFF0F buf[6] = ModID (commit trigger / identity check on Teensy)
                    if (pgnData.Length >= 32)
                    {
                        frames.Add(BuildFrame(0x0C, new byte[] {
                            pgnData[2], pgnData[3], pgnData[4], pgnData[5],
                            pgnData[6], pgnData[7], pgnData[8], pgnData[9]
                        }));
                        frames.Add(BuildFrame(0x0D, new byte[] {
                            pgnData[10], pgnData[11], pgnData[12], pgnData[13],
                            pgnData[14], pgnData[15], pgnData[16], pgnData[17]
                        }));
                        frames.Add(BuildFrame(0x0E, new byte[] {
                            pgnData[18], pgnData[19], pgnData[20], pgnData[21],
                            pgnData[22], pgnData[23], pgnData[24], pgnData[25]
                        }));
                        frames.Add(BuildFrame(0x0F, new byte[] {
                            pgnData[26], pgnData[27], pgnData[28], pgnData[29],
                            pgnData[30], pgnData[31], pgnData[2], 0  // buf[6]=ModID (commit trigger)
                        }));
                    }
                    break;
            }

            return frames;
        }

        // --- Helpers ---

        private CanFrame BuildFrame(byte pgnLow, byte[] data)
        {
            // Proprietary B CAN ID: Priority=6, PF=0xFF, PS=pgnLow, SA=RC_SOURCE_ADDRESS
            uint id = (6u << 26) | (0xFFu << 16) | ((uint)pgnLow << 8) | RC_SOURCE_ADDRESS;
            return new CanFrame(id, data);
        }

        private ModuleState GetOrCreateState(int moduleId)
        {
            if (!_modules.TryGetValue(moduleId, out var state))
            {
                state = new ModuleState();
                _modules[moduleId] = state;
            }
            return state;
        }

        private static byte[] CloneData(byte[] src, int minSize)
        {
            int len = Math.Max(src.Length, minSize);
            var dst = new byte[len];
            Array.Copy(src, dst, src.Length);
            return dst;
        }

        private static byte CalcCrc(byte[] data, int length)
        {
            byte crc = 0;
            for (int i = 0; i < length; i++) crc += data[i];
            return crc;
        }

        /// <summary>
        /// Swaps high and low nibbles of a ModSenID byte.
        /// CAN frames use (ModID in low nibble, SenID in high nibble).
        /// UDP PGNs use (ModID in high nibble, SenID in low nibble) via BuildModSenID.
        /// This converts in either direction — the operation is its own inverse.
        /// </summary>
        private static byte SwapModSenNibbles(byte b)
        {
            return (byte)(((b & 0x0F) << 4) | ((b >> 4) & 0x0F));
        }
    }

    public class PgnAssembledEventArgs : EventArgs
    {
        public byte[] Data { get; }
        public PgnAssembledEventArgs(byte[] data) { Data = data; }
    }
}
