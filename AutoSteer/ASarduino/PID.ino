// modified from: https://www.phidgets.com/?view=articles&article=DcMotorsPidControl

// The purpose of the integral term in a control loop is to look back at all of the past error 
// and accumulate it into an offset that the control loop can use.

// Derivative control looks at past errors in the system and calculates the slope of those
// errors to predict future error values. 

float clCurrentError = 0.0;		// error is the difference between the target and the actual position
float clErrorLast = 0.0;	// errorlast is the error in the previous iteration of the control loop
float output = 0.0;			// output is the result from the control loop calculation
float clIntegral = 0.0;
float clDerivative = 0.0;

unsigned long clInterval;
unsigned long clLast;

int DoPID(float clError, float clSetPoint, byte MinPWM, byte MaxPWM,
	float clKP, float clKi, float clKd, float deadBand)
{
	clInterval = millis() - clLast;
	clLast = millis();

	// Calculate how far we are from the target
	clErrorLast = clCurrentError;
	clCurrentError = clError;

	// If the error is within the specified deadband, and the motor is moving slowly enough
	// Or if the motor's target is a physical limit and that limit is hit
	// (within deadband margins)
	if (abs(clCurrentError) <= ((deadBand / 100) * clSetPoint) && abs(output) <= MinPWM)
	{
		// Stop the motor
		output = 0;
		clCurrentError = 0;
	}
	else
	{
		// Else, update motor duty cycle with the newest output value
		// This equation is a simple PID control loop
		output = ((clKP * clCurrentError) + (clKi * clIntegral)) + (clKd * clDerivative);
	}

	// Prevent output value from exceeding maximum output specified by user,
	// And prevent the duty cycle from falling below the minimum velocity (excluding zero)
	// The minimum velocity exists because some DC motors with gearboxes will not be able
	// to overcome the detent torque of the gearbox at low velocities.
	if (output >= MaxPWM)
		output = MaxPWM;
	else if (output <= -MaxPWM)
		output = -MaxPWM;
	else if (output < MinPWM && output > 0)
		output = MinPWM;
	else if (output > MinPWM * (-1) && output < 0)
		output = MinPWM * (-1);
	else
		clIntegral += (clCurrentError * (float)clInterval); // The integral is only accumulated when the duty cycle isn't saturated at 100% or -100%.

	// Calculate the derivative for the next iteration
	if (clInterval > 0)	clDerivative = (clCurrentError - clErrorLast) / (float)clInterval;
	else clDerivative = 0.0;

	return output;
}




