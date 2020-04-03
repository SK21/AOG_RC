//Kalman variables
float Pc = 0.0;
float G = 0.0;
float P = 1.0;
float Xp = 0.0;
float Zp = 0.0;

const float varRate = 75; // variance, smaller, more filtering
const float varProcess = 10;

float LastValue = 0;

float Filter(float CurrentValue)
{
	//Kalman filter
	Pc = P + varProcess;
	G = Pc / (Pc + varRate);
	P = (1 - G) * Pc;
	Xp = LastValue;
	Zp = Xp;
	LastValue = G * (CurrentValue - Zp) + Xp;

	return LastValue;
}

