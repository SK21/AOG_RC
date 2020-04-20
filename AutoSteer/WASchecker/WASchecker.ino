#include <Adafruit_ADS1015.h>
#include <Wire.h>



// *********** setup ***********************

float SteerCPD = 237;			// AOG value sent * 2
int SteeringZeroOffset = 16500;
#define AdsWAS 0				// ADS1115 wheel angle sensor pin
#define WORKSW_PIN 4
#define STEERSW_PIN A0
#define UseSteerSwitch 0		// 1 - steer switch, 0 - steer momentary button

// *********** end setup *******************



// WAS RTY090LVNAA voltage output is 0.5 (left) to 4.5 (right). +-45 degrees
// ADS reading of the WAS ranges from 2700 to 24000 (21300)
// counts per degree for this sensor is 237 (21300/90)

int AOGzeroAdjustment = 0;	// AOG value sent * 20 to give range of +-10 degrees
int SteeringPositionZero = SteeringZeroOffset + AOGzeroAdjustment;

Adafruit_ADS1115 ads(0x48);

//steering variables
float steerAngleActual = 0;
int steeringPosition = 0;

unsigned long DelayInterval = 1000;
unsigned long DelayLast;

// steer switch
byte SteerSwitch = HIGH;	// Low on, High off
byte SWreading = HIGH;
byte SWPrevious = LOW;
unsigned int SWtime = 0;
unsigned int SWdebounce = 50;

byte switchByte = 0;
byte workSwitch = 0;

void setup()
{
	ads.begin();

	Wire.begin();
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("WASchecker  :  20/Apr/2020");
	Serial.println();

	pinMode(WORKSW_PIN, INPUT_PULLUP);
	pinMode(STEERSW_PIN, INPUT_PULLUP);
}

void loop()
{
	ReadSwitches();

	if (millis() - DelayLast > DelayInterval)
	{
		DelayLast = millis();

		Serial.println();

		//************** Steering Angle ******************
		steeringPosition = ads.readADC_SingleEnded(AdsWAS);	//read the steering position sensor
		steeringPosition = (steeringPosition - SteeringPositionZero);

		//convert position to steer angle. 6 counts per degree of steer pot position in my case
		//  ***** make sure that negative steer angle makes a left turn and positive value is a right turn *****
		// remove or add the minus for steerSensorCounts to do that.
		steerAngleActual = (float)(steeringPosition) / -SteerCPD;

		Serial.println("Ads Reading " + String(ads.readADC_SingleEnded(AdsWAS)) + "  Angle " + String(steerAngleActual));

		Serial.println("Ads 0 " + String(ads.readADC_SingleEnded(0)));
		Serial.println("Ads 1 " + String(ads.readADC_SingleEnded(1)));
		Serial.println("Ads 2 " + String(ads.readADC_SingleEnded(2)));
		Serial.println("Ads 3 " + String(ads.readADC_SingleEnded(3)));

		Serial.println();
		Serial.println("switchByte " + String(switchByte));
	}
}
