#if(CommType == 0)
// PGN 32741 Comm data out
// to Rate Controller from Arduino
//0	HeaderHi		    127
//1	HeaderLo		    229
//2	rate applied Hi		100 X actual
//3	rate applied Lo
//4	acc.Quantity 3		100 X actual
//5	acc.Quantity 2
//6 acc.Quantity 1
//7 PWM Hi
//8 PWM Lo
// 9 -
// total 10 bytes
//
// PGN 32761
//	0. HeaderHi			127		
//	1. HeaderLo			249		
//	2. -
//  3. -
//  4. -
//  5. Relay Hi			8-15
//	6. Relay Lo			0-7
//	7. SecSwOff[1] Hi	sections 8 to 15
//	8. SecSwOff[0] Lo	sections 0 to 7
//	9. Command		    command byte out to AOG
//			- bit 0		- AOG section buttons auto (xxxxxxx1)
//			- bit 1		- AOG section buttons auto off (xxxxxx1x)
//			- bits 2,3	- change rate steps 0 (xxxx00xx) no change, 1 (xxxx01xx), 2 (xxxx10xx), 3 (xxxx11xx)
//			- bit 4		- 0 change rate left (xxx0xxxx), 1 change rate right (xxx1xxxx)
//			- bit 5		- 0 rate down (xx0xxxxx), 1 rate up (xx1xxxxx)
// total 10 bytes

void SendSerial()
{
	// PGN 32741
	Serial.print(127);	// headerHi
	Serial.print(",");
	Serial.print(229);	// headerLo
	Serial.print(",");

	// rate applied, 100 X actual
	Temp = ((int)FlowRateFiltered * 100) >> 8;
	Serial.print(Temp);
	Serial.print(",");
	Temp = (FlowRateFiltered * 100);
	Serial.print(Temp);
	Serial.print(",");

	// accumulated quantity, 3 bytes
	long Units = accumulatedCounts * 100.0 / MeterCal;
	Temp = Units >> 16;
	Serial.print(Temp);
	Serial.print(",");
	Temp = Units >> 8;
	Serial.print(Temp);
	Serial.print(",");
	Temp = Units;
	Serial.print(Temp);
	Serial.print(",");

	// pwmSetting
	Temp = (int)((300 - pwmSetting) * 10) >> 8;	// account for negative values
	Serial.print(Temp);
	Serial.print(",");
	Temp = (300 - pwmSetting) * 10;
	Serial.print(Temp);
	Serial.print(",");

	Serial.print(0);

	Serial.println();
	Serial.flush();   // flush out buffer     

	// PGN 32761
	Serial.print(127);	// headerHi
	Serial.print(",");
	Serial.print(249);	// headerLo
	Serial.print(",");

	Serial.print(0);
	Serial.print(",");
	Serial.print(0);
	Serial.print(",");
	Serial.print(0);
	Serial.print(",");

	Serial.print(0);	// Relay Hi
	Serial.print(",");
	Serial.print(RelayToAOG);	// Relay Lo
	Serial.print(",");

	Serial.print(SecSwOff[1]);
	Serial.print(",");
	Serial.print(SecSwOff[0]);
	Serial.print(",");

	Serial.print(OutCommand);

	Serial.println();
	Serial.flush();   // flush out buffer     
}

//  Comm data in
//  PGN 32742
//	0. HeaderHi		127		
//	1. HeaderLo		230		
//	2. Relay Hi		8-15
//  3. Relay Lo		0-7
//  4. Rate Set Hi	100 X actual
//  5. Rate Set Lo  100 X actual
//	6. Flow Cal Hi	100 X actual
//  7. Flow Cal Lo	100 X actual
//	8. Command 		
//			- bit 0 		reset accumulated quantity
//			- bits 1 + 2 	0 (xxxxx00x) standard valve, 1 (xxxxx01x) fast close valve, 2 (xxxxx10x) valve type 2, 3 (xxxxx11x) valve type 3
//			- bit 3			simulate flow
//  total 9 bytes
//
//  PGN 32743
//	0. HeaderHi		127		
//	1. HeaderLo		231		
//  2. KP			P control gain, 10 times actual
//  3. KI			I control gain, 10000 times actual
//  4. KD			D control gain, 10 times actual
//  5. Deadband		% error that can still be present when the motor stops, 10 times actual
//  6. MinPWM			
//  7. MaxPWM		
//  total 8 bytes

void ReceiveSerial()
{
	if (Serial.available() > 0 && !PGN32742Found && !PGN32743Found) //find the header, 
	{
		UnsignedTemp = Serial.read();
		header = tempHeader << 8 | UnsignedTemp;               //high,low bytes to make int
		tempHeader = UnsignedTemp;                             //save for next time
		PGN32742Found = (header == 32742);
		PGN32743Found = (header == 32743);
	}

	if (Serial.available() > 6 && PGN32742Found)
	{
		PGN32742Found = false;

		RelayHi = Serial.read();
		RelayFromAOG = Serial.read();

		// rate setting, 100 times actual
		UnsignedTemp = Serial.read() << 8 | Serial.read();
		rateSetPoint = (float)(UnsignedTemp * 0.01);

		//Meter Cal, 100 times actual
		UnsignedTemp = Serial.read() << 8 | Serial.read();
		MeterCal = (float)(UnsignedTemp * 0.01);

		// command byte
		InCommand = Serial.read();
		if ((InCommand & 1) == 1) accumulatedCounts = 0;	// reset accumulated count

		ValveType = 0;
		if ((InCommand & 2) == 2) ValveType += 1;
		if ((InCommand & 4) == 4) ValveType += 2;

		SimulateFlow = ((InCommand & 8) == 8);

		//reset watchdog as we just heard from AgOpenGPS
		watchdogTimer = 0;
		AOGconnected = true;
	}

	if (Serial.available() > 6 && PGN32743Found)
	{
		PGN32743Found = false;

		// PID
		KP = (float)Serial.read() * 0.1;
		KI = (float)Serial.read() * 0.0001;
		KD = (float)Serial.read() * 0.1;
		DeadBand = (float)Serial.read();
		MinPWMvalue = Serial.read();
		MaxPWMvalue = Serial.read();
		AdjustmentFactor = Serial.read()/100.0;

		//reset watchdog as we just heard from AgOpenGPS
		watchdogTimer = 0;
		AOGconnected = true;
	}
}
#endif



