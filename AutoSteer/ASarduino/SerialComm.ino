#if(CommType == 0)
void SendSerial()
{
	// PGN32765
	//Send to agopenGPS **** you must send 10 numbers ****
	Serial.print("127,253,");
	Serial.print((int)(steerAngleActual * 100)); //The actual steering angle in degrees
	Serial.print(",");
	Serial.print((int)(steerAngleSetPoint * 100));   //the setpoint originally sent
	Serial.print(",");
	Serial.print((int)(IMUheading * 16));
	Serial.print(",");
	Serial.print((int)(FilteredRoll * 16));;
	Serial.print(",");
	Serial.print(switchByte); //steering switch status
	Serial.print(",");
	Serial.print(pwmDrive);
	Serial.println(",0,0");

	Serial.flush();   // flush out buffer
}

void ReceiveSerial()
{
	//****************************************************************************************
//This runs continuously, outside of the timed loop, keeps checking UART for new data
// header high/low, relay byte, speed byte, high distance, low distance, Steer high, steer low
	if (Serial.available() > 0 && !PGN32766Found && !PGN32764Found && !PGN32762Found && !PGN32763Found)
	{
		int temp = Serial.read();
		header = tempHeader << 8 | temp;               //high,low bytes to make int
		tempHeader = temp;                             //save for next time
		PGN32762Found = (header == 32762);
		PGN32763Found = (header == 32763);
		PGN32764Found = (header == 32764);
		PGN32766Found = (header == 32766);
	}

	if (Serial.available() > 7 && PGN32766Found)
	{
		// autosteer data
		PGN32766Found = false;
		relay = Serial.read();   // read relay control from AgOpenGPS
		CurrentSpeed = (float)Serial.read() / 4.0;	//actual speed times 4, single byte

		//distance from the guidance line in mm
		distanceFromLine = (float)(Serial.read() << 8 | Serial.read());   //high,low bytes

		//set point steer angle * 10 is sent
		steerAngleSetPoint = ((float)(Serial.read() << 8 | Serial.read())) * 0.01; //high low bytes

		//uturn byte read in
		uTurn = Serial.read();

		watchdogTimer = 0;  // reset watchdog

		serialResetTimer = 0; // reset serial timer

		Serial.read();
	}

	if (Serial.available() > 7 && PGN32764Found)
	{
		// autosteer settings
		PGN32764Found = false;  //reset the flag


		Kp = (float)Serial.read() * 1.0;   // read Kp from AgOpenGPS
		LowMaxPWM = Serial.read();	// low max pwm
		Serial.read();	// Kd
		Serial.read();	// Ko
		AOGzeroAdjustment = (Serial.read() - 127) * 20;	// 20 times the steer zero setting displayed in AOG
		MinPWMvalue = Serial.read(); //read the minimum amount of PWM for instant on
		HighMaxPWM = Serial.read();	// high pwm
		SteerCPD = Serial.read() * 2; // 2 times the steer count setting displayed in AOG
	}

	if (Serial.available() > 7 && PGN32762Found)
	{
		PGN32762Found = false;  //reset the flag
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
	}

	if (Serial.available() > 7 && PGN32763Found)
	{
		// AogSettings
		PGN32763Found = false;  //reset the flag
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
		Serial.read();
	}
}
#endif

