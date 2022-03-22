byte MSB;
byte LSB;

//PGN32613 to Rate Controller from Arduino
//0	HeaderLo		101
//1	HeaderHi		127
//2 Controller ID
//3	rate applied Lo 	10 X actual
//4 rate applied Mid
//5	rate applied Hi
//6	acc.Quantity Lo		10 X actual
//7	acc.Quantity Mid
//8	acc.Quantity Hi
//9 PWM Lo
//10 PWM Hi

void SendSerial()
{
  for (int i = 0; i < SensorCount; i++)
  {
    // PGN 32613
    Serial.print(101);	// headerHi
    Serial.print(",");
    Serial.print(127);	// headerLo
    Serial.print(",");

    Serial.print(BuildModSenID(ModuleID, i));
    Serial.print(",");

    // rate applied, 10 X actual
    Temp = (UPM[i] * 10);
    Serial.print(Temp);
    Serial.print(",");
    Temp = (int)(UPM[i] * 10) >> 8;
    Serial.print(Temp);
    Serial.print(",");
    Temp = (int)(UPM[i] * 10) >> 16;
    Serial.print(Temp);
    Serial.print(",");

    // accumulated quantity, 10 X actual
    long Units = TotalPulses[i] * 10.0 / MeterCal[i];
    Temp = Units;
    Serial.print(Temp);
    Serial.print(",");
    Temp = Units >> 8;
    Serial.print(Temp);
    Serial.print(",");
    Temp = Units >> 16;
    Serial.print(Temp);
    Serial.print(",");

    // pwmSetting
    Temp = (byte)(pwmSetting[i] * 10);
    Serial.print(Temp);
    Serial.print(",");
    Temp = (byte)((pwmSetting[i] * 10) >> 8);
    Serial.print(Temp);

    Serial.println();
    Serial.flush();   // flush out buffer
  }
}

//PGN32614 to Arduino from Rate Controller
//0	HeaderLo		102
//1	HeaderHi		127
//2 Controller ID
//3	relay Lo		0 - 7
//4	relay Hi		8 - 15
//5	rate set Lo		10 X actual
//6 rate set Mid
//7	rate set Hi		10 X actual
//8	Flow Cal Lo		100 X actual
//9	Flow Cal Hi		
//10	Command
//- bit 0		    reset acc.Quantity
//- bit 1, 2		valve type 0 - 3
//- bit 3		    simulate flow
//- bit 4           0 - average time for multiple pulses, 1 - time for one pulse
//- bit 5           AutoOn

void ReceiveSerial()
{
    if (Serial.available() > 0 && !PGN32614Found && !PGN32616Found
        && !PGN32619Found && !PGN32620Found) //find the header
    {
        MSB = Serial.read();
        PGN = MSB << 8 | LSB;               //high,low bytes to make int
        LSB = MSB;                          //save for next time

        PGN32614Found = (PGN == 32614);
        PGN32616Found = (PGN == 32616);
        PGN32619Found = (PGN == 32619);
        PGN32620Found = (PGN == 32620);
    }

    if (Serial.available() > 8 && PGN32614Found)
    {
        PGN32614Found = false;
        byte tmp = Serial.read();
        if (ParseModID(tmp) == ModuleID)
        {
            byte SensorID = ParseSenID(tmp);
            if (SensorID < SensorCount)
            {
                RelayLo = Serial.read();
                RelayHi = Serial.read();

                // rate setting, 10 times actual
                UnSignedTemp = Serial.read() | Serial.read() << 8 | Serial.read() << 16;
                float TmpSet = (float)(UnSignedTemp * 0.1);

                // Meter Cal, 100 times actual
                UnSignedTemp = Serial.read() | Serial.read() << 8;
                MeterCal[SensorID] = (float)(UnSignedTemp * 0.01);

                // command byte
                InCommand[SensorID] = Serial.read();
                if ((InCommand[SensorID] & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

                ControlType[SensorID] = 0;
                if ((InCommand[SensorID] & 2) == 2) ControlType[SensorID] += 1;
                if ((InCommand[SensorID] & 4) == 4) ControlType[SensorID] += 2;

                UseMultiPulses[SensorID] = ((InCommand[SensorID] & 16) == 16);

                AutoOn = ((InCommand[SensorID] & 32) == 32);
                if (AutoOn)
                {
                    RateSetting[SensorID] = TmpSet;
                }
                else
                {
                    NewRateFactor[SensorID] = TmpSet;
                }

                //reset watchdog as we just heard from AgOpenGPS
                watchdogTimer = 0;
                CommTime[SensorID] = millis();
            }
        }
    }

    if (Serial.available() > 7 && PGN32616Found)
    {
        // PID to Arduino from RateController
        PGN32616Found = false;
        byte tmp = Serial.read();
        if (ParseModID(tmp) == ModuleID)
        {
            byte SensorID = ParseSenID(tmp);
            if (SensorID < SensorCount)
            {
                PIDkp[SensorID] = Serial.read();
                PIDminPWM[SensorID] = Serial.read();
                PIDLowMax[SensorID] = Serial.read();
                PIDHighMax[SensorID] = Serial.read();
                PIDdeadband[SensorID] = Serial.read();
                PIDbrakePoint[SensorID] = Serial.read();
                AdjustTime[SensorID] = Serial.read();

                watchdogTimer = 0;
                CommTime[SensorID] = millis();
            }
        }
    }

    if (Serial.available() > 2 && PGN32619Found)
    {
        // from Wemos D1 mini
        // section buttons
        PGN32619Found = false;
        for (int i = 2; i < 5; i++)
        {
            WifiSwitches[i] = Serial.read();
        }
        WifiSwitchesEnabled = true;
        WifiSwitchesTimer = millis();
        SetRelaysWifi();
    }

    if (Serial.available() > 7 && PGN32620Found)
    {
        // from rate controller
        // section switch IDs to arduino
        // 0    108
        // 1    127
        // 2    sec 0-1
        // 3    sec 2-3
        // 4    sec 4-5
        // 5    sec 6-7
        // 6    sec 8-9
        // 7    sec 10-11
        // 8    sec 12-13
        // 9    sec 14-15

        PGN32620Found = false;
        for (int i = 0; i < 8; i++)
        {
            SwitchBytes[i] = Serial.read();
        }
        TranslateSwitchBytes();
    }
}
