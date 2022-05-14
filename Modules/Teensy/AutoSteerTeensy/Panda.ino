IPAddress PandaSendIP(192, 168, PCB.IPpart3, 255);

//Conversion to Hexidecimal
const char* asciiHex = "0123456789ABCDEF";

//the new PANDA sentence buffer
char nme[100];

//GGA
char fixTime[12];
char latitude[15];
char latNS[3];
char longitude[15];
char lonEW[3];
char fixQuality[2];
char numSats[4];
char HDOP[5];
char altitude[12];
char ageDGPS[10];

//VTG
char vtgHeading[12];
char speedKnots[10];

//imu
char imuHeading[6];
char imuRoll[6];
char imuPitch[6];
char imuYawRate[6];

bool isTriggered = false;
uint32_t PandalastTime = PCB.IMUdelay;
int16_t Ptemp;

void DoPanda()
{
    // NMEA, from receiver to parser
    if (SerialNMEA->available()) parser << SerialNMEA->read();

    // RTCM, from AGIO to receiver
    int packetSize = UDPgps.parsePacket();
    if (packetSize)
    {
        UDPgps.read(GPSbuffer, packetSize);
        SerialRTCM->write(GPSbuffer, packetSize);
    }

    // IMU
    if (isTriggered && millis() - PandalastTime > PCB.IMUdelay)
    {
        imuHandler();
        isTriggered = false;
    }
}

void imuHandler()
{
    ReadIMU();

    Ptemp = (int16_t)IMU_Heading;
    itoa(Ptemp, imuHeading, 10);

    Ptemp = (int16_t)IMU_Roll;
    itoa(Ptemp, imuRoll, 10);

    Ptemp = (int16_t)IMU_Pitch;
    itoa(Ptemp, imuPitch, 10);

    if (PCB.GyroOn)
    {
        Ptemp = (int16_t)IMU_YawRate;
        itoa(Ptemp, imuYawRate, 10);
    }
}

// if odd characters showed up.
void errorHandler()
{
    //nothing at the moment
}

void VTG_Handler()
{
    //vtg heading
    if (parser.getArg(0, vtgHeading));

    //vtg Speed knots
    if (parser.getArg(4, speedKnots));
    if (!PCB.GGAlast) BuildPanda();
}

void GGA_Handler() //Rec'd GGA
{
    // fix time
    if (parser.getArg(0, fixTime));

    //latitude
    if (parser.getArg(1, latitude));
    if (parser.getArg(2, latNS));

    //longitude
    if (parser.getArg(3, longitude));
    if (parser.getArg(4, lonEW));

    //fix quality
    if (parser.getArg(5, fixQuality));

    //satellite #
    if (parser.getArg(6, numSats));

    //HDOP
    if (parser.getArg(7, HDOP));

    //altitude
    if (parser.getArg(8, altitude));

    //time of last DGPS update
    if (parser.getArg(12, ageDGPS));

    if (PCB.GGAlast) BuildPanda();
}

void BuildPanda()
{
    // build Panda
    strcpy(nme, "");

    strcat(nme, "$PANDA,");

    strcat(nme, fixTime);
    strcat(nme, ",");

    strcat(nme, latitude);
    strcat(nme, ",");

    strcat(nme, latNS);
    strcat(nme, ",");

    strcat(nme, longitude);
    strcat(nme, ",");

    strcat(nme, lonEW);
    strcat(nme, ",");

    //6
    strcat(nme, fixQuality);
    strcat(nme, ",");

    strcat(nme, numSats);
    strcat(nme, ",");

    strcat(nme, HDOP);
    strcat(nme, ",");

    strcat(nme, altitude);
    strcat(nme, ",");

    //10
    strcat(nme, ageDGPS);
    strcat(nme, ",");

    //11
    if (atof(speedKnots) > 0.3) strcat(nme, speedKnots);
    strcat(nme, ",");

    //12
    strcat(nme, imuHeading);
    strcat(nme, ",");

    //13
    strcat(nme, imuRoll);
    strcat(nme, ",");

    //14
    strcat(nme, imuPitch);
    strcat(nme, ",");

    //15
    strcat(nme, imuYawRate);

    strcat(nme, "*");

    CalculateChecksum();

    strcat(nme, "\r\n");

    PandalastTime = millis();
    isTriggered = true;

    uint16_t len = strlen(nme);
    UDPgps.beginPacket(PandaSendIP, AGIOport);
    UDPgps.write(nme, len);
    UDPgps.endPacket();
}

void CalculateChecksum(void)
{

    int16_t sum = 0, inx;
    char tmp;

    // The checksum calc starts after '$' and ends before '*'
    for (inx = 1; inx < 200; inx++)
    {
        tmp = nme[inx];
        // * Indicates end of data and start of checksum
        if (tmp == '*')
            break;
        sum ^= tmp;    // Build checksum
    }

    byte chk = (sum >> 4);
    char hex[2] = { asciiHex[chk],0 };
    strcat(nme, hex);

    chk = (sum % 16);
    char hex2[2] = { asciiHex[chk],0 };
    strcat(nme, hex2);
}


/*
$GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M ,  ,*47
   0     1      2      3    4      5 6  7  8   9    10 11  12 13  14
        Time      Lat       Lon     FixSatsOP Alt
Where:
     GGA          Global Positioning System Fix Data
     123519       Fix taken at 12:35:19 UTC
     4807.038,N   Latitude 48 deg 07.038' N
     01131.000,E  Longitude 11 deg 31.000' E
     1            Fix quality: 0 = invalid
                               1 = GPS fix (SPS)
                               2 = DGPS fix
                               3 = PPS fix
                               4 = Real Time Kinematic
                               5 = Float RTK
                               6 = estimated (dead reckoning) (2.3 feature)
                               7 = Manual input mode
                               8 = Simulation mode
     08           Number of satellites being tracked
     0.9          Horizontal dilution of position
     545.4,M      Altitude, Meters, above mean sea level
     46.9,M       Height of geoid (mean sea level) above WGS84
                      ellipsoid
     (empty field) time in seconds since last DGPS update
     (empty field) DGPS station ID number
     *47          the checksum data, always begins with *
 *
 *
$GPRMC,123519,A,4807.038,N,01131.000,E,022.4,084.4,230394,003.1,W*6A
 0      1    2   3      4    5      6   7     8     9     10   11
        Time      Lat        Lon       knots  Ang   Date  MagV

Where:
     RMC          Recommended Minimum sentence C
     123519       Fix taken at 12:35:19 UTC
     A            Status A=active or V=Void.
     4807.038,N   Latitude 48 deg 07.038' N
     01131.000,E  Longitude 11 deg 31.000' E
     022.4        Speed over the ground in knots
     084.4        Track angle in degrees True
     230394       Date - 23rd of March 1994
     003.1,W      Magnetic Variation
     *6A          The checksum data, always begins with *
 *
$GPVTG,054.7,T,034.4,M,005.5,N,010.2,K*48
 *
    VTG          Track made good and ground speed
    054.7,T      True track made good (degrees)
    034.4,M      Magnetic track made good
    005.5,N      Ground speed, knots
    010.2,K      Ground speed, Kilometers per hour
    *48          Checksum
    */
