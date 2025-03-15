void SendMessages()
{
	if (millis() - SendLast > SendTime)
	{
		SendLast = millis();

        // PGN 9, module info
        // 0    pressure X 10, byte lo
        // 1    pressure hi
        // 2    InoID Lo
        // 3    InoID Hi
        // 4    Status

        byte Data9[5];

        Data9[0] = (byte)PressureReading;
        Data9[1] = (byte)(PressureReading >> 8);
        Data9[2] = (byte)InoID;
        Data9[3] = InoID >> 8;

        // status
        Data9[4] = 0;
        if (WorkPinOn()) Data9[4] |= 0b00000001;

        if (millis() - ESPtime < 5000)
        {
            if (WifiStrength < -80)
            {
                Data9[4] |= 0b00000010;
            }
            else if (WifiStrength < -70)
            {
                Data9[4] |= 0b00000100;
            }
            else
            {
                Data9[4] |= 0b00001000;
            }
        }

        if (Ethernet.linkStatus() == LinkON) Data9[4] |= 0b00010000;
        if (GoodPins) Data9[4] |= 0b00100000;

        SendCanMessage(9, 0, Data9, 5);

		// PGN 10, sensor info 1
		// 0	rate applied lo		1000 X actual
		// 1	rate mid
		// 2	rate hi
		// 3	pwm lo
		// 4	pwm hi
		// 5	accumulated quantiy X 10 byte lo
        // 6    acc. Mid
        // 7    acc. Hi

		for (int i = 0; i < MDL.SensorCount; i++)
		{
            byte Data[8];

            // rate applied, 1000 X actual
            uint32_t Applied = Sensor[i].UPM * 1000;
            Data[0] = Applied;
            Data[1] = Applied >> 8;
            Data[2] = Applied >> 16;

            //PWM
            Data[3] = (int)Sensor[i].PWM;
            Data[4] = (int)Sensor[i].PWM >> 8;

            // accumulated quantity, 10 X actual
            if (Sensor[i].MeterCal > 0)
            {
                uint32_t Units = Sensor[i].TotalPulses * 10.0 / Sensor[i].MeterCal;
                Data[5] = Units;
                Data[6] = Units >> 8;
                Data[7] = Units >> 16;
            }
            else
            {
                Data[5] = 0;
                Data[6] = 0;
                Data[7] = 0;
            }

            SendCanMessage(10, i, Data, 8);
		}

        // PGN 11, sensor info 2
        // 0    scale reading X 10, byte lo
        // 1    scale Mid
        // 2    scale Hi
        // 3    Status

        for (int i = 0; i < MDL.SensorCount; i++)
        {
            byte Data[4];

            // scale
            Data[0] = 0;
            Data[1] = 0;
            Data[2] = 0;

            // status
            Data[3] = 0;
            if (millis() - Sensor[i].CommTime < 4000) Data[3] |= 0b00000001;

            SendCanMessage(11, i, Data, 8);
        }
	}
}
