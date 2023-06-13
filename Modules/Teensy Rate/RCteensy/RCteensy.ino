
#include <Wire.h>
#include <EEPROM.h> 
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Watchdog_t4.h>	// https://github.com/tonton81/WDT_T4
#include <HX711.h>			// https://github.com/bogde/HX711

// rate control with Teensy 4.1

# define InoDescription "RCteensy :  13-Jun-2023"
const uint16_t InoID = 13063;	// change to send defaults to eeprom, ddmmy, no leading 0

#define MaxReadBuffer 100	// bytes
#define MaxProductCount 2

struct ModuleConfig	
{
	uint8_t ID = 0;
	uint8_t SensorCount = 2;       // up to 2 sensors
	uint8_t IPpart2 = 168;			// ethernet IP address
	uint8_t	IPpart3 = 1;
	uint8_t IPpart4 = 60;			// 60 + ID
	uint8_t RelayOnSignal = 1;	    // value that turns on relays
	uint8_t FlowOnDirection = 0;	// sets on value for flow valve or sets motor direction
	uint8_t RelayControl = 5;		// 0 - no relays, 1 - RS485, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - Teensy GPIO
	uint8_t ESP8266SerialPort = 1;	// serial port to connect to ESP8266
	uint8_t RelayPins[16] = { 8,9,10,11,12,25,26,27,0,0,0,0,0,0,0,0 };		// pin numbers when GPIOs are used for relay control (5), default RC11
	uint8_t LOADCELL_DOUT_PIN[2] = { 16,0 };
	uint8_t LOADCELL_SCK_PIN[2] = { 17,0 };
};

ModuleConfig MDL;

struct SensorConfig
{
	uint8_t FlowPin;
	uint8_t DirPin;
	uint8_t PWMPin;
	bool FlowEnabled;
	float RateError;		// rate error X 1000
	float UPM;				// upm X 1000
	int pwmSetting;
	uint32_t CommTime;
	byte InCommand;			// command byte from RateController
	byte ControlType;		// 0 standard, 1 combo close, 2 motor, 3 motor/weight, 4 fan
	uint32_t TotalPulses;
	float RateSetting;
	float MeterCal;
	float ManualAdjust;
	float KP;
	float KI;
	float KD;
	byte MinPWM;
	byte MaxPWM;
	byte Deadband;
	byte BrakePoint;
	bool UseMultiPulses;	// 0 - time for one pulse, 1 - average time for multiple pulses
	uint8_t Debounce;
};

SensorConfig Sensor[2];

struct AnalogConfig
{
	int16_t AIN0;	// Pressure 0
	int16_t AIN1;	// Pressure 1
	int16_t AIN2;	
	int16_t AIN3;
};

AnalogConfig AINs;

// ethernet
EthernetUDP UDPcomm;
uint16_t ListeningPort = 28888;
uint16_t DestinationPort = 29999;
IPAddress DestinationIP(192, MDL.IPpart2, MDL.IPpart3, 255);

// AGIO
EthernetUDP AGIOcomm;
uint16_t ListeningPortAGIO = 8888;		// to listen on
uint16_t DestinationPortAGIO = 9999;	// to send to

// Relays
byte RelayLo = 0;	// sections 0-7
byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;

// WifiSwitches connection to ESP8266
unsigned long WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[6];

const uint16_t LoopTime = 50;      //in msec = 20hz
uint32_t LoopLast = LoopTime;
const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

WDT_T4<WDT1> wdt;
bool MasterOn = false;
bool AutoOn = true;

uint8_t ErrorCount;
bool ADSfound = false;
const int16_t AdsI2Caddress = 0x48;
uint32_t Analogtime;
uint32_t SaveTime;

extern float tempmonGetTemp(void);

int8_t Wifi_dBm;
uint32_t WifiTime;
uint32_t WifiLastTime;

HardwareSerial* SerialESP8266;
byte ESPdebug1;
bool ESPconnected;

HX711 scale[2];
bool ScaleFound[2] = { false,false };

const byte ResetPin = 33;
bool ResetESP8266 = false;

volatile unsigned long debug1;
volatile int debug2;
volatile unsigned long debug3;
volatile unsigned long debug4;
int debug5;

bool SendStatusPGN;

double DurOff = 1000000;

void setup()
{
	 //watchdog timer
	WDT_timings_t config;
	config.timeout = 60;	// seconds
	wdt.begin(config);

	// default flow pins
	Sensor[0].FlowPin = 28;
	Sensor[0].DirPin = 37;
	Sensor[0].PWMPin = 36;

	Sensor[1].FlowPin = 29;
	Sensor[1].DirPin = 14;
	Sensor[1].PWMPin = 15;

	// default pid
	Sensor[0].KP = 5;
	Sensor[0].KI = 0;
	Sensor[0].KD = 0;
	Sensor[0].MinPWM = 5;
	Sensor[0].MaxPWM = 50;
	Sensor[0].Deadband = 4;
	Sensor[0].BrakePoint = 20;
	Sensor[0].Debounce = 3;

	Sensor[1].KP = 5;
	Sensor[1].KI = 0;
	Sensor[1].KD = 0;
	Sensor[1].MinPWM = 5;
	Sensor[1].MaxPWM = 50;
	Sensor[1].Deadband = 4;
	Sensor[1].BrakePoint = 20;
	Sensor[1].Debounce = 3;

	Serial.begin(38400);
	delay(5000);
	Serial.println("");
	Serial.println(InoDescription);
	Serial.println("");

	// eeprom
	int16_t StoredID;
	EEPROM.get(100, StoredID);
	if (StoredID == InoID)
	{
		// load stored data
		Serial.println("Loading stored settings.");
		EEPROM.get(110, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.get(200 + i * 80, Sensor[i]);
		}
	}
	else
	{
		// update stored data
		Serial.println("Updating stored data.");
		EEPROM.put(100, InoID);
		EEPROM.put(110, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.put(200 + i * 80, Sensor[i]);
		}
	}

	if (MDL.SensorCount < 1) MDL.SensorCount = 1;
	if (MDL.SensorCount > MaxProductCount) MDL.SensorCount = MaxProductCount;

	MDL.IPpart4 = MDL.ID + 60;
	if (MDL.IPpart4 > 255) MDL.IPpart4 = 255 - MDL.ID;

	Serial.println("");
	Serial.print("Module ID: ");
	Serial.println(MDL.ID);
	Serial.println();

	// I2C
	Wire.begin();			// I2C on pins SCL 19, SDA 18
	Wire.setClock(400000);	//Increase I2C data rate to 400kHz

	// ADS1115
	Serial.println("Starting ADS ...");
	ErrorCount = 0;
	while (!ADSfound)
	{
		Wire.beginTransmission(AdsI2Caddress);
		Wire.write(0b00000000);	//Point to Conversion register
		Wire.endTransmission();
		Wire.requestFrom(AdsI2Caddress, 2);
		ADSfound = Wire.available();
		Serial.print(".");
		if (ErrorCount++ > 10) break;
		delay(500);
	}
	Serial.println("");
	if (ADSfound)
	{
		Serial.println("ADS connected.");
		Serial.println("");
	}
	else
	{
		Serial.println("ADS not found.");
		Serial.println("");
	}

	// ethernet start
	Serial.println("Starting Ethernet ...");
	IPAddress LocalIP(192, MDL.IPpart2, MDL.IPpart3, MDL.IPpart4);
	static uint8_t LocalMac[] = { 0x0A,0x0B,0x42,0x0C,0x0D,MDL.IPpart4 };

	Ethernet.begin(LocalMac, 0);	//https://forum.pjrc.com/threads/65653-non-blocking-Ethernet-begin()-with-cable-disconnected-amp-static-IP
	Ethernet.setLocalIP(LocalIP);
	DestinationIP = IPAddress(192, MDL.IPpart2, MDL.IPpart3, 255);	// update from saved data

	Serial.print("IP Address: ");
	Serial.println(Ethernet.localIP());
	delay(1500);
	if (Ethernet.linkStatus() == LinkON)
	{
		Serial.println("Ethernet Connected.");
	}
	else
	{
		Serial.println("Ethernet Not Connected.");
	}
	Serial.println("");

	// UDP
	UDPcomm.begin(ListeningPort);

	// AGIO
	AGIOcomm.begin(ListeningPortAGIO);

	// sensors
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		pinMode(Sensor[i].FlowPin, INPUT_PULLUP);
		pinMode(Sensor[i].DirPin, OUTPUT);
		pinMode(Sensor[i].PWMPin, OUTPUT);

		switch (i)
		{
		case 0:
			attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR0, FALLING);
			break;
		case 1:
			attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR1, FALLING);
			break;
		}
	}

	// Relay Pins
	if (MDL.RelayControl == 5)
	{
		for (int i = 0; i < 16; i++)
		{
			if (MDL.RelayPins[i] > 0)
			{
				pinMode(MDL.RelayPins[i], OUTPUT);
			}
		}
	}

	 //ESP8266 serial port
	switch (MDL.ESP8266SerialPort)
	{
	case 1:
		SerialESP8266 = &Serial1;
		break;
	case 2:
		SerialESP8266 = &Serial2;
		break;
	case 3:
		SerialESP8266 = &Serial3;
		break;
	case 4:
		SerialESP8266 = &Serial4;
		break;
	case 5:
		SerialESP8266 = &Serial5;
		break;
	case 6:
		SerialESP8266 = &Serial6;
		break;
	case 7:
		SerialESP8266 = &Serial7;
		break;
	default:
		SerialESP8266 = &Serial8;
		break;
	}
	SerialESP8266->begin(115200);

	// load cell
	for (int i = 0; i < MaxProductCount; i++)
	{
		Serial.print("Initializing scale ");
		Serial.println(i);
		ErrorCount = 0;
		ScaleFound[i] = false;

		if (MDL.LOADCELL_DOUT_PIN[i] > 1 && MDL.LOADCELL_SCK_PIN[i] > 1)
		{
			scale[i].begin(MDL.LOADCELL_DOUT_PIN[i], MDL.LOADCELL_SCK_PIN[i]);
			pinMode(MDL.LOADCELL_DOUT_PIN[i], INPUT_PULLUP);
			while (!ScaleFound[i])
			{
				ScaleFound[i] = scale[i].wait_ready_timeout(1000);
				Serial.print(".");
				if (ErrorCount++ > 5) break;
				delay(500);
			}
		}

		Serial.println("");
		if (ScaleFound[i])
		{
			Serial.println("HX711 found.");
		}
		else
		{
			Serial.println("HX711 not found.");
		}
		Serial.println("");
	}

	pinMode(LED_BUILTIN, OUTPUT);

	pinMode(ResetPin, INPUT_PULLUP);

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}

void loop()
{
	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			Sensor[i].FlowEnabled = (millis() - Sensor[i].CommTime < 4000) &&
				((Sensor[i].RateSetting > 0 && MasterOn)
					|| ((Sensor[i].ControlType == 4) && (Sensor[i].RateSetting > 0))
					|| (!AutoOn && MasterOn));
		}

		GetUPM();
		CheckRelays();
		AdjustFlow();

		if (AutoOn)
		{
			AutoControl();
		}
		else
		{
			ManualControl();
		}
	}

	if (millis() - SendLast > SendTime)
	{
		SendLast = millis();
		SendData();
	}

	if (ADSfound)
	{
		if (millis() - Analogtime > 2)
		{
			Analogtime = millis();
			ReadAnalog();
		}
	}

	//if (millis() - SaveTime > 3600000)	// 1 hour
	//{
	//	// save sensor data
	//	SaveTime = millis();
	//	EEPROM.put(100, InoID);
	//	//EEPROM.put(110, MDL);

	//	for (int i = 0; i < MaxProductCount; i++)
	//	{
	//		EEPROM.put(200 + i * 60, Sensor[i]);
	//	}
	//}

	ReceiveData();
	ReceiveAGIO();
	Blink();
	CheckResetButton();
	wdt.feed();
}

byte ParseModID(byte ID)
{
	// top 4 bits
	return ID >> 4;
}

byte ParseSenID(byte ID)
{
	// bottom 4 bits
	return (ID & 0b00001111);
}

byte BuildModSenID(byte Mod_ID, byte Sen_ID)
{
	return ((Mod_ID << 4) | (Sen_ID & 0b00001111));
}

bool GoodCRC(byte Data[], byte Length)
{
	byte ck = CRC(Data, Length - 1, 0);
	bool Result = (ck == Data[Length - 1]);
	return Result;
}

byte CRC(byte Chk[], byte Length, byte Start)
{
	byte Result = 0;
	int CK = 0;
	for (int i = Start; i < Length; i++)
	{
		CK += Chk[i];
	}
	Result = (byte)CK;
	return Result;
}

void AutoControl()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		Sensor[i].RateError = Sensor[i].RateSetting - Sensor[i].UPM;

		switch (Sensor[i].ControlType)
		{
		case 2:
		case 3:
		case 4:
			// motor control
			Sensor[i].pwmSetting = PIDmotor(i);
			break;

		default:
			// valve control
			Sensor[i].pwmSetting = PIDvalve(i);
			break;
		}
	}
}

void ManualControl()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		Sensor[i].pwmSetting = Sensor[i].ManualAdjust;
	}
}

bool State = false;
elapsedMillis BlinkTmr;
elapsedMicros LoopTmr;
byte ReadReset;
uint32_t MaxLoopTime;
bool ResetTimerOn;

void Blink()
{
	if (BlinkTmr > 1000)
	{
		BlinkTmr = 0;
		State = !State;
		digitalWrite(LED_BUILTIN, State);
		Serial.println(".");	// needed to allow PCBsetup to connect

		Serial.print(" Micros: ");
		Serial.print(MaxLoopTime);

		Serial.print(", Temp: ");
		Serial.print(tempmonGetTemp());

		Serial.print(", dBm: ");
		Serial.print(Wifi_dBm);

		//Serial.print(", ");
		//Serial.print(debug2);
		//
		//Serial.print(", ");
		//Serial.print(debug1);

		//Serial.print(", ");
		//Serial.print(MasterOn);

		//Serial.print(", ");
		//Serial.print(ResetTimerOn);

		Serial.println("");

		if (ReadReset++ > 10)
		{
			ReadReset = 0;
			MaxLoopTime = 0;
		}
	}
	if (LoopTmr > MaxLoopTime) MaxLoopTime = LoopTmr;
	LoopTmr = 0;
}

uint32_t ResetTime;
void CheckResetButton()
{
	if (digitalRead(ResetPin))
	{
		ResetTimerOn = false;
	}
	else
	{
		if (ResetTimerOn)
		{
			if (millis() - ResetTime > 5000)
			{
				// notify esp8266
				ResetESP8266 = true;
			}

			if (millis() - ResetTime > 6000)
			{
				// change ID to cause defaults to load on startup
				EEPROM.put(100, InoID + 1);
				delay(100);

				// restart the Teensy
				SCB_AIRCR = 0x05FA0004;
			}
		}
		else
		{
			ResetTime = millis();
			ResetTimerOn = true;
		}
	}
}



