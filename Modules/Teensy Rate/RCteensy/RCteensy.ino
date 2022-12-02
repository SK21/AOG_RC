
#include <Wire.h>
#include <EEPROM.h> 
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Watchdog_t4.h>	// https://github.com/tonton81/WDT_T4

// rate control with Teensy 4.1
# define InoDescription "RCteensy   02-Dec-2022"
#define MaxReadBuffer 100	// bytes
#define MaxFlowSensorCount 2

struct ModuleConfig	// 40 bytes
{
	uint8_t ID = 0;
	uint8_t SensorCount = 1;        // up to 2 sensors
	uint8_t	IPpart3 = 1;			// IP address, 3rd octet
	uint8_t RelayOnSignal = 0;	    // value that turns on relays
	uint8_t FlowOnDirection = 0;	// sets on value for flow valve or sets motor direction
	uint8_t RelayControl = 0;		// 0 - no relays, 1 - RS485, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - Teensy GPIO
	uint8_t WemosSerialPort = 3;	// serial port to connect to Wemos D1 Mini
	uint8_t RelayPins[16];			// pin numbers when GPIOs are used for relay control (5)
};

ModuleConfig MDL;

struct SensorConfig	// 46 bytes
{
	uint8_t FlowPin = 28;
	uint8_t	DirPin = 37;
	uint8_t	PWMPin = 36;
	bool MasterOn = false;
	bool FlowEnabled = false;
	float RateError = 0;		// rate error X 1000
	float UPM = 0;				// upm X 1000
	uint16_t pwmSetting = 0;
	uint32_t CommTime = 0;
	byte InCommand = 0;			// command byte from RateController
	byte ControlType = 0;		// 0 standard, 1 combo close, 2 motor
	uint16_t TotalPulses = 0;
	float RateSetting = 0;
	float MeterCal = 0;
	float ManualAdjust = 0;
	uint16_t ManualLast = 0;
	bool UseMultiPulses = 0;	// 0 - time for one pulse, 1 - average time for multiple pulses
	byte KP = 20;
	byte KI = 0;
	byte MinPWM = 50;
	byte LowMax = 100;
	byte HighMax = 255;
	byte Deadband = 3;
	byte BrakePoint = 20;
	byte AdjustTime = 0;
};

SensorConfig Sensor[MaxFlowSensorCount];

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
IPAddress DestinationIP(192, 168, MDL.IPpart3, 255);

// Relays
byte RelayLo = 0;	// sections 0-7
byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;

//EEPROM
int16_t EEread = 0;
#define MDL_Ident 1688

// WifiSwitches connection to Wemos D1 Mini
unsigned long WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[6];

const uint16_t LoopTime = 50;      //in msec = 20hz
uint32_t LoopLast = LoopTime;
const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

HardwareSerial* SerialWemos;
WDT_T4<WDT1> wdt;
bool AutoOn = true;

uint8_t ErrorCount;
bool ADSfound = false;
const int16_t AdsI2Caddress = 0x48;
uint32_t Analogtime;

void setup()
{
	// watchdog timer
	WDT_timings_t config;
	config.timeout = 60;	// seconds
	wdt.begin(config);

	// eeprom
	EEPROM.get(100, EEread);
	if (EEread != MDL_Ident)
	{
		EEPROM.put(100, MDL_Ident);
		EEPROM.put(110, MDL);

		for (int i = 0; i < MaxFlowSensorCount; i++)
		{
			EEPROM.put(200 + i * 50, Sensor[i]);
		}
	}
	else
	{
		EEPROM.get(110, MDL);

		for (int i = 0; i < MaxFlowSensorCount; i++)
		{
			EEPROM.get(200 + i * 50, Sensor[i]);
		}
	}

	if (MDL.SensorCount < 1) MDL.SensorCount = 1;
	if (MDL.SensorCount > MaxFlowSensorCount) MDL.SensorCount = MaxFlowSensorCount;

	Serial.begin(38400);
	delay(5000);
	Serial.println();
	Serial.println(InoDescription);
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
		delay(500);
		if (ErrorCount++ > 10) break;
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
	IPAddress LocalIP(192, 168, MDL.IPpart3, 201);
	static uint8_t LocalMac[] = { 0x00,0x00,0x42,0x00,0x00,201 };

	Ethernet.begin(LocalMac, 0);	//https://forum.pjrc.com/threads/65653-non-blocking-Ethernet-begin()-with-cable-disconnected-amp-static-IP
	Ethernet.setLocalIP(LocalIP);
	DestinationIP = IPAddress(192, 168, MDL.IPpart3, 255);	// update from saved data

	Serial.print("IP Address: ");
	Serial.println(Ethernet.localIP());
	delay(1000);
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

	// sensors
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		pinMode(Sensor[i].FlowPin, INPUT_PULLUP);
		pinMode(Sensor[i].DirPin, OUTPUT);
		pinMode(Sensor[i].PWMPin, OUTPUT);

		switch (i)
		{
		case 0:
			attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin),ISR0, FALLING);
			break;
		case 1:
			attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR1, FALLING);
			break;
		}
	}

	// Wemos D1 Mini serial port
	switch (MDL.WemosSerialPort)
	{
	case 1:
		SerialWemos = &Serial1;
		break;
	case 2:
		SerialWemos = &Serial2;
		break;
	case 3:
		SerialWemos = &Serial3;
		break;
	case 4:
		SerialWemos = &Serial4;
		break;
	case 5:
		SerialWemos = &Serial5;
		break;
	case 6:
		SerialWemos = &Serial6;
		break;
	case 7:
		SerialWemos = &Serial7;
		break;
	default:
		SerialWemos = &Serial8;
		break;
	}
	SerialWemos->begin(115200);

	pinMode(LED_BUILTIN, OUTPUT);

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
			Sensor[i].FlowEnabled = (millis() - Sensor[i].CommTime < 4000) && Sensor[i].RateSetting > 0 && Sensor[i].MasterOn;
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

	if (millis() - Analogtime > 5)
	{
		Analogtime = millis();
		ReadAnalog();
	}

	ReceiveData();
	Blink();
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
			// motor control
			Sensor[i].pwmSetting = ControlMotor(i);
			break;

		default:
			// valve control
			Sensor[i].pwmSetting = DoPID(i);
			break;
		}
	}
}

void ManualControl()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		Sensor[i].RateError = Sensor[i].RateSetting - Sensor[i].UPM;

		if (millis() - Sensor[i].ManualLast > 1000)
		{
			Sensor[i].ManualLast = millis();

			// adjust rate
			if (Sensor[i].RateSetting == 0) Sensor[i].RateSetting = 1; // to make FlowEnabled

			switch (Sensor[i].ControlType)
			{
			case 2:
				// motor control
				if (Sensor[i].ManualAdjust > 0)
				{
					Sensor[i].pwmSetting *= 1.10;
					if (Sensor[i].pwmSetting < 1) Sensor[i].pwmSetting = Sensor[i].MinPWM;
					if (Sensor[i].pwmSetting > 255) Sensor[i].pwmSetting = 255;
				}
				else if (Sensor[i].ManualAdjust < 0)
				{
					Sensor[i].pwmSetting *= 0.90;
					if (Sensor[i].pwmSetting < Sensor[i].MinPWM) Sensor[i].pwmSetting = 0;
				}
				break;

			default:
				// valve control
				Sensor[i].pwmSetting = Sensor[i].ManualAdjust;
				break;
			}
		}
	}
}

bool State = false;
elapsedMillis BlinkTmr;
elapsedMicros LoopTmr;

void Blink()
{
	if (BlinkTmr > 1000)
	{
		BlinkTmr = 0;
		State = !State;
		digitalWrite(LED_BUILTIN, State);
		Serial.println(".");	// needed to allow PCBsetup to connect

		Serial.print(" elapsed micros: ");
		Serial.println(LoopTmr);
	}
	LoopTmr = 0;
}



