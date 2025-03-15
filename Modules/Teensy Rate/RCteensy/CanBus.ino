
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
}

void ReceiveCanMessage()
{
	CAN_message_t msg;
	if (can1.read(msg))
	{
		uint8_t PGN, ModuleID, SensorID;
		DecodeCanID(msg.id, PGN, ModuleID, SensorID);
		if (MDL.ID == ModuleID || PGN == 4 || PGN == 19
			|| PGN == 20 || PGN == 21 || PGN == 22)
		{
			ReadMessages(PGN, SensorID, msg.buf);
		}
	}
}
