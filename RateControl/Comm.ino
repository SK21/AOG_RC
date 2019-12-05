#if !UseWifi

//  Comm data from arduino to AgOpenGPS(PGN 31100) :
//	1. HeaderHi			1 byte	121
//	2. HeaderLo			1 byte	124
//	3. rate applied		2 bytes	100 times actual
//	4. acc.quantity		2 bytes
//	5. Relay			1 byte	manual relay settings
//	6. SecSwOff			2 bytes section switches off
//			- byte 0 is sections 0 to 7
//			- byte 1 is sections 8 to 15
//	7. Command		    1 byte	command byte out to AOG
//			- bit 0		- AOG section buttons auto (xxxxxxx1)
//			- bit 1		- AOG section buttons auto off (xxxxxx1x)
//			- bits 2,3	- change rate steps 0 (xxxx00xx) no change, 1 (xxxx01xx), 2 (xxxx10xx), 3 (xxxx11xx)
//			- bit 4		- 0 change rate left (xxx0xxxx), 1 change rate right (xxx1xxxx)
//			- bit 5		- 0 rate down (xx0xxxxx), 1 rate up (xx1xxxxx)
// total 10 bytes

void CommToAOG()
{
	// PGN 31100
	Serial.print(121);	// headerHi
	Serial.print(",");
	Serial.print(124);	// headerLo
	Serial.print(",");

	// rate applied, 100 X actual
	Temp = (rateAppliedUPM) >> 8;
	Serial.print(Temp);
	Serial.print(",");
	Temp = rateAppliedUPM;
	Serial.print(Temp);
	Serial.print(",");

	// accumulated quantiy
	Temp = ((int)((float)accumulatedCounts / (float)MeterCal)) >> 8;
	Serial.print(Temp);
	Serial.print(",");
	Temp = ((int)((float)accumulatedCounts / (float)MeterCal));
	Serial.print(Temp);
	Serial.print(",");

	Serial.print(RelayToAOG);	// relayLo
	Serial.print(",");
	Serial.print(SecSwOff[1]);
	Serial.print(",");
	Serial.print(SecSwOff[0]);
	Serial.print(",");
	Serial.print(OutCommand);
	Serial.println();
	Serial.flush();   // flush out buffer     
}

//  Comm data from AgOpenGPs to arduino(PGN 31000) :
//	1. HeaderHi		1 byte	121
//	2. HeaderLo		1 byte	24
//	3. Relay 		1 bytes	7 section control
//	4. Rate Set		2 bytes	100 times actual rate
//	5. Flow Cal		2 bytes 100 times actual
//	6. Command 		1 byte
//			- bit 0 		reset accumulated quantity
//			- bits 1 + 2 	0 (xxxxx00x) standard valve, 1 (xxxxx01x) fast close valve, 2 (xxxxx10x) valve type 2, 3 (xxxxx11x) valve type 3
//			- bit 3			simulate flow
//  7. KP			1 byte	P control gain, 10 times actual
//  8. KI			1 byte	I control gain, 10000 times actual
//  9. KD			1 byte  D control gain, 10 times actual
// 10. Deadband		1 byte	% error that can still be present when the motor stops, 10 times actual
// 11. MinPWM		1 byte	
// 12. MaxPWM		1 byte
// total 14 bytes

void CommFromAOG()
{
	// PGN - 31000
	// header high/low, relay byte, speed byte, rateSetPoint hi/lo
	if (Serial.available() > 0 && !isDataFound ) //find the header, 
	{
		int temp = Serial.read();
		header = tempHeader << 8 | temp;               //high,low bytes to make int
		tempHeader = temp;                             //save for next time
		if (header == 31000) isDataFound = true;     //Do we have a match? 
	}

	if (Serial.available() > 11 && isDataFound)
	{
		//Serial.println("Data found");
		isDataFound = false;
		relayLo = Serial.read();   // read relay control from AgOpenGPS  1 -> 8

		// rate setting, 100 times actual
		unsignedTemp = Serial.read() << 8 | Serial.read();
		rateSetPoint = (float)unsignedTemp * 0.01;

		//Meter Cal, 100 times actual
		unsignedTemp = Serial.read() << 8 | Serial.read();
		MeterCal = (float)unsignedTemp * 0.01;

		// command byte
		InCommand = Serial.read();
		if ((InCommand & 1) == 1) accumulatedCounts = 0;	// reset accumulated count

		ValveType = 0;
		if ((InCommand & 2) == 2) ValveType += 1;
		if ((InCommand & 4) == 4) ValveType += 2;

		SimulateFlow = ((InCommand & 8) == 8);

		// PID
		KP = (float)Serial.read() * 0.1;
		KI = (float)Serial.read() * 0.0001;
		KD = (float)Serial.read() * 0.1;
		DeadBand = (float)Serial.read();
		MinPWMvalue = Serial.read();
		MaxPWMvalue = Serial.read();

		//reset watchdog as we just heard from AgOpenGPS
		watchdogTimer = 0;
		AOGconnected = true;
	}

	//// testing
	//KP = 15.0;
	//KI = .001;
	//KD = 0.0;
	//DeadBand = 3.0;
	//rateSetPoint = 80;
	//MeterCal = 180;
	//SimulateFlow = true;
	//watchdogTimer = 0;
	//AOGconnected = true;
}
#endif


