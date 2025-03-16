
// can identifier 11 bits. PPPPPMMMSSS
uint16_t EncodeCanID(uint8_t PGN, uint8_t ModuleID, uint8_t SensorID)
{
	uint16_t Result = 0;
	if (PGN < 32 && ModuleID < 8 && SensorID < 8)
	{
		Result = PGN << 6 | ModuleID << 3 | SensorID;
	}
	return Result;
}

void DecodeCanID(uint16_t ID, uint8_t& PGN, uint8_t& ModuleID, uint8_t& SensorID)
{
	PGN = (ID >> 6) & 0x1F;
	ModuleID = (ID >> 3) & 0x7;
	SensorID = (ID) & 0x7;
}

void SendCanMessage(uint8_t PGN, uint8_t SensorID, uint8_t* data, uint8_t len)
{
	CAN_message_t msg;
	msg.id = EncodeCanID(PGN, MDL.ID, SensorID);
	msg.len = len;
	memcpy(msg.buf, data, len);
	can1.write(msg);

	SendCanMessageUDP(msg.id, data);
}

void ReceiveCanMessage()
{
	CAN_message_t msg;
	if (can1.read(msg)) ProcessCanMessage(msg.id, msg.buf);
}

void ProcessCanMessage(uint16_t ID, uint8_t* data)
{
	uint8_t PGN, ModuleID, SensorID;
	DecodeCanID(ID, PGN, ModuleID, SensorID);
	if (MDL.ID == ModuleID || PGN == 4 || PGN == 19
		|| PGN == 20 || PGN == 21 || PGN == 22)
	{
		ReadMessages(PGN, SensorID, data);
	}
}

void SendCanMessageUDP(uint16_t ID, uint8_t* data)
{
	// PGN 28705, 0x7021, can message to RC
	// 0    0x21
	// 1    0x70
	// 2    ID lo byte
	// 3    ID Hi byte
	// 4    data 0
	// 5    data 1
	// 6    data 2
	// 7    data 3
	// 8    data 4
	// 9    data 5
	// 10   data 6
	// 11   data 7
	// 12   CRC

	int len = sizeof(data);
	if (len > 8) len = 8;

	byte Data[13] = { 0 };
	Data[0] = 0x21;
	Data[1] = 0x70;
	Data[2] = ID;
	Data[3] = ID >> 8;
	for (int i = 0; i < len; i++)
	{
		Data[4 + i] = data[i];
	}
	Data[12] = CRC(Data, 12, 0);

	if (Ethernet.linkStatus() == LinkON)
	{
		// send ethernet
		UDPcomm.beginPacket(DestinationIP, DestinationPort);
		UDPcomm.write(Data, 13);
		UDPcomm.endPacket();
	}
}


