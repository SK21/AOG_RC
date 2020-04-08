#if(!UseWifi)
void CommToAOG()
{
	//Send to agopenGPS **** you must send 10 numbers ****
	Serial.print("127,253,");
	Serial.print((int)(steerAngleActual * 100)); //The actual steering angle in degrees
	Serial.print(",");
	Serial.print((int)(steerAngleSetPoint * 100));   //the setpoint originally sent
	Serial.print(",");
	Serial.print(IMUheading); //heading in degrees * 16
	Serial.print(",");
	Serial.print(CurrentRoll);
	Serial.print(",");
	Serial.print(switchByte); //steering switch status
	Serial.println(",,,");

	Serial.flush();   // flush out buffer
}

void CommFromAOG()
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

	// PGN32766 - autosteer data
	if (Serial.available() > 7 && PGN32766Found)
	{
		PGN32766Found = false;
		relay = Serial.read();   // read relay control from AgOpenGPS
		CurrentSpeed = (float)Serial.read() / 4;	//actual speed times 4, single byte

		//distance from the guidance line in mm
		distanceFromLine = (float)(Serial.read() << 8 | Serial.read());   //high,low bytes

		//set point steer angle * 10 is sent
		steerAngleSetPoint = ((float)(Serial.read() << 8 | Serial.read()))*0.01; //high low bytes

		//uturn byte read in
		uTurn = Serial.read();

		watchdogTimer = 0;  // reset watchdog

		serialResetTimer = 0; // reset serial timer

		Serial.read();
	}

	// PGN32764 - autosteer settings
	if (Serial.available() > 7 && PGN32764Found)
	{
		PGN32764Found = false;  //reset the flag

		//change the factors as required for your own PID values
		Kp = (float)Serial.read() * 1.0;   // read Kp from AgOpenGPS
		Ki = (float)Serial.read() * 0.001;   // read Ki from AgOpenGPS
		Kd = (float)Serial.read() * 1.0;   // read Kd from AgOpenGPS
		Ko = (float)Serial.read() * 0.1;   // read Ko from AgOpenGPS

		AOGzeroAdjustment = (Serial.read() - 127) * 20;	// 20 times the setting displayed in AOG
		SteeringPositionZero = SteeringZeroOffset + AOGzeroAdjustment;
		MinPWMvalue = Serial.read(); //read the minimum amount of PWM for instant on
		maxIntegralValue = Serial.read() * 0.1;
		SteerCPD = Serial.read() * 2; // 2 times the setting displayed in AOG
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
