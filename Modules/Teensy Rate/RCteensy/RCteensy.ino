
#include <Wire.h>
#include <EEPROM.h> 
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Watchdog_t4.h>	// https://github.com/tonton81/WDT_T4
#include <HX711.h>			// https://github.com/bogde/HX711

// rate control with Teensy 4.1
# define InoDescription "RCteensy   13-Jan-2023"

#define MaxReadBuffer 100	// bytes
#define MaxProductCount 2

//EEPROM
int16_t EEread = 0;
#define MDL_Ident 5300

struct ModuleConfig	
{
	uint8_t ID = 0;
	uint8_t ProductCount = 1;       // up to 2 sensors
	uint8_t IPpart2 = 168;			// ethernet IP address
	uint8_t	IPpart3 = 1;
	uint8_t IPpart4 = 60;			// 60 + ID
	uint8_t RelayOnSignal = 0;	    // value that turns on relays
	uint8_t FlowOnDirection = 0;	// sets on value for flow valve or sets motor direction
	uint8_t RelayControl = 0;		// 0 - no relays, 1 - RS485, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - Teensy GPIO
	uint8_t WemosSerialPort = 1;	// serial port to connect to Wemos D1 Mini
	uint8_t RelayPins[16];			// pin numbers when GPIOs are used for relay control (5)
	uint8_t LOADCELL_DOUT_PIN[MaxProductCount];	
	uint8_t LOADCELL_SCK_PIN[MaxProductCount];	
	uint8_t Debounce = 3;			// minimum ms pin change
};

ModuleConfig MDL;

struct SensorConfig	
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
	byte ControlType = 0;		// 0 standard, 1 combo close, 2 motor, 3 motor/weight, 4 fan
	uint32_t TotalPulses = 0;
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
	bool CalOn = false;
	byte CalPWM = 0;
};

SensorConfig Sensor[MaxProductCount];

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

// Relays
byte RelayLo = 0;	// sections 0-7
byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;

// WifiSwitches connection to Wemos D1 Mini
unsigned long WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[6];

const uint16_t LoopTime = 50;      //in msec = 20hz
uint32_t LoopLast = LoopTime;
const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

WDT_T4<WDT1> wdt;
bool AutoOn = true;

uint8_t ErrorCount;
bool ADSfound = false;
const int16_t AdsI2Caddress = 0x48;
uint32_t Analogtime;
uint32_t SaveTime;

HX711 scale[2];
bool ScaleFound[2] = { false,false };

extern float tempmonGetTemp(void);

int8_t WifiRSSI;
uint32_t WifiTime;
uint32_t WifiLastTime;

HardwareSerial* SerialWemos;
byte ESPdebug1;
bool ESPconnected;

void setup()
{
	 //watchdog timer
	WDT_timings_t config;
	config.timeout = 60;	// seconds
	wdt.begin(config);

	// initial scale pins
	MDL.LOADCELL_DOUT_PIN[0] = 16;
	MDL.LOADCELL_SCK_PIN[0] = 17;

	// eeprom
	EEPROM.get(100, EEread);
	if (EEread != MDL_Ident)
	{
		EEPROM.put(100, MDL_Ident);
		EEPROM.put(110, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.put(200 + i * 60, Sensor[i]);
		}
	}
	else
	{
		EEPROM.get(110, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.get(200 + i * 60, Sensor[i]);
		}
	}

	if (MDL.ProductCount < 1) MDL.ProductCount = 1;
	if (MDL.ProductCount > MaxProductCount) MDL.ProductCount = MaxProductCount;

	MDL.IPpart4 = MDL.ID + 60;
	if (MDL.IPpart4 > 255) MDL.IPpart4 = 255 - MDL.ID;

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
	IPAddress LocalIP(192, MDL.IPpart2, MDL.IPpart3, MDL.IPpart4);
	static uint8_t LocalMac[] = { 0x00,0x00,0x42,0x00,0x00,MDL.IPpart4 };

	Ethernet.begin(LocalMac, 0);	//https://forum.pjrc.com/threads/65653-non-blocking-Ethernet-begin()-with-cable-disconnected-amp-static-IP
	Ethernet.setLocalIP(LocalIP);
	DestinationIP = IPAddress(192, MDL.IPpart2, MDL.IPpart3, 255);	// update from saved data

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
	for (int i = 0; i < MDL.ProductCount; i++)
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

	 //Wemos D1 Mini serial port
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
				delay(500);
				if (ErrorCount++ > 5) break;
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

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}

void loop()
{
	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.ProductCount; i++)
		{
			Sensor[i].FlowEnabled = (millis() - Sensor[i].CommTime < 4000)
				&& ((Sensor[i].RateSetting > 0 && Sensor[i].MasterOn) || (Sensor[i].ControlType == 4));
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

	if (millis() - Analogtime > 2)
	{
		Analogtime = millis();
		ReadAnalog();
	}

	if (millis() - SaveTime > 3600000)	// 1 hour
	{
		// save sensor data
		SaveTime = millis();
		EEPROM.put(100, MDL_Ident);
		//EEPROM.put(110, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.put(200 + i * 60, Sensor[i]);
		}
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
	for (int i = 0; i < MDL.ProductCount; i++)
	{
		Sensor[i].RateError = Sensor[i].RateSetting - Sensor[i].UPM;

		if (Sensor[i].CalOn)
		{
			// calibration mode 
			Sensor[i].pwmSetting = Sensor[i].CalPWM;
		}
		else
		{
			// normal mode
			switch (Sensor[i].ControlType)
			{
			case 2:
			case 3:
			case 4:
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
}

void ManualControl()
{
	for (int i = 0; i < MDL.ProductCount; i++)
	{
		Sensor[i].RateError = Sensor[i].RateSetting - Sensor[i].UPM;
		if (Sensor[i].CalOn)
		{
			// calibration mode 
			Sensor[i].pwmSetting = Sensor[i].CalPWM;
		}
		else
		{
			// normal mode
			if (millis() - Sensor[i].ManualLast > 1000)
			{
				Sensor[i].ManualLast = millis();

				// adjust rate
				if (Sensor[i].RateSetting == 0) Sensor[i].RateSetting = 1; // to make FlowEnabled

				switch (Sensor[i].ControlType)
				{
				case 2:
				case 3:
				case 4:
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
}

bool State = false;
elapsedMillis BlinkTmr;
elapsedMicros LoopTmr;
byte ReadReset;
uint32_t MaxLoopTime;

void Blink()
{
	if (BlinkTmr > 1000)
	{
		BlinkTmr = 0;
		State = !State;
		digitalWrite(LED_BUILTIN, State);
		Serial.println(".");	// needed to allow PCBsetup to connect

		Serial.print(" Loop micros: ");
		Serial.print(MaxLoopTime);

		Serial.print(", Chip Temp: ");
		Serial.print(tempmonGetTemp());

		Serial.print(", RSSI: ");
		Serial.print(WifiRSSI);

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



