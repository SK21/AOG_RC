
// Firmware update from RateController, over UDP (PGN 32810-32812) or CAN (0xFF1A-0xFF1C).
// Design: Design Docs\Teensy_CAN_Firmware_Update.md
//
// The app sends the hex file as numbered blocks of 11 records. Each block carries a CRC-16
// and is acknowledged before the next one goes. A good block is rebuilt into the packet the
// Ethernet update uses (EthernetUpdate.ino, PCBsetup) and handed to process_hex_record(), so
// FlasherX does the flash work as before: the running firmware is replaced only when the app
// commits after the record count matched. Anything else reboots into the old firmware.
//
// While updating the module does no control: Master off and every sensor treated as not
// connected, so relays and outputs sit in their comm-loss state (as when the app stops).

const uint16_t UpdSilentMs = 10000;		// no update traffic this long: cancel, reboot into the old firmware
const uint16_t UpdFrameGapMs = 100;		// CAN: a block still missing frames this long after its last frame is asked for again
const uint16_t UpdNoBlock = 0xFFFF;

uint16_t UpdLastBlock = UpdNoBlock;		// last block written
uint32_t UpdLastRx = 0;					// millis() of the last update traffic
uint32_t UpdLoopLast = 0;

// CAN block reassembly: 0xFF1A command 2 (header), then 0xFF1B frames of 7 bytes
uint8_t UpdCanBuf[UpdMaxBlock];
uint16_t UpdCanBlock = 0;
uint16_t UpdCanLen = 0;
uint16_t UpdCanCRC = 0;
uint64_t UpdCanHave = 0;				// one bit per frame received
uint64_t UpdCanNeed = 0;				// one bit per frame in the block
bool UpdCanPending = false;
uint32_t UpdCanLastFrame = 0;

// PGN 32810 (UDP) or 0xFF1A commands 1, 3, 4 (CAN): only the board with this MAC acts
void UpdControl(uint8_t Command, const uint8_t* MAC, uint8_t Type, uint8_t Link)
{
	uint8_t Own[6];
	TeensyMAC(Own);
	if (memcmp(MAC, Own, 6) == 0)
	{
		switch (Command)
		{
		case 1:
			UpdStart(Type, Link);
			break;

		case 3:
			// commit: FlasherX copies the new firmware over the running one and reboots
			if (UpdActive) UpdCommit();
			break;

		case 4:
			// abort: the buffer is freed and the board reboots into the old firmware
			if (UpdActive) UpdCancel();
			break;
		}
	}
}

void UpdStart(uint8_t Type, uint8_t Link)
{
	if (UpdActive)
	{
		// the app asks again when Ready was lost
		if (Link == UpdLink) UpdSendOn(Link, UpdReady, 0, 0);
	}
	else if (Type != 0xFF && Type != InoType)
	{
		UpdSendOn(Link, UpdWrongType, 0, 0);
	}
	else if (MasterOn || FirmwareUpdateMode)
	{
		UpdSendOn(Link, UpdRefused, 0, 0);
	}
	else if (Link == UpdLinkCAN && !ISOBUSid.addressClaimed)
	{
		// nothing can be sent back on CAN without a claimed address (CommMode 0)
	}
	else
	{
		UpdSafeOutputs();
		Serial.println("");
		Serial.println("Firmware update: erasing the flash buffer.");
		if (firmware_buffer_init(&buffer_addr, &buffer_size))
		{
			Serial.printf("buffer = %1luK %s (%08lX - %08lX)\n", buffer_size / 1024, IN_FLASH(buffer_addr) ? "FLASH" : "RAM", buffer_addr, buffer_addr + buffer_size);
			UpdActive = true;
			UpdLink = Link;
			UpdLastBlock = UpdNoBlock;
			UpdCanPending = false;
			UpdLastRx = millis();

			if (Link == UpdLinkCAN)
			{
				// From here the CAN library queues received frames under interrupt (256 deep)
				// and UpdLoop takes them with events(). A block is 34 frames, more than the 16
				// mailboxes polling can hold while flash is written. events() is called once
				// first so the interrupt queues frames instead of handling them inside the ISR.
				ISOBUS.events();
				ISOBUS.onReceive(CANBus_Handle);
				ISOBUS.enableMBInterrupts();
			}
			UpdSendStatus(UpdReady, 0, 0);
		}
		else
		{
			Serial.println("Firmware update: unable to create the update buffer.");
			UpdSendOn(Link, UpdError, 0, 0);
		}
	}
}

// PGN 32811 (UDP), or a CAN block once all its frames are in
void UpdBlock(uint16_t Block, const uint8_t* Data, uint16_t Len, uint16_t CRC16)
{
	if (UpdActive)
	{
		UpdLastRx = millis();
		if (Block == UpdLastBlock)
		{
			// written already, its acknowledgement was lost
			UpdSendStatus(UpdBlockOK, Block, 0);
		}
		else if (Block != (uint16_t)(UpdLastBlock + 1) || UpdCRC16(Data, Len) != CRC16)
		{
			// out of order or damaged: ask for the next block wanted
			UpdSendStatus(UpdResend, (uint16_t)(UpdLastBlock + 1), 0);
		}
		else
		{
			// rebuild the Ethernet update packet: 0x3A before each record
			static uint8_t Packet[UpdMaxBlock + 11];
			uint16_t P = 0;
			uint16_t i = 0;
			bool Good = true;
			while (Good && i < Len)
			{
				uint16_t RecLen = Data[i] + 5;		// len, addrHi, addrLo, type, data, checksum
				if (i + RecLen > Len || P + RecLen + 1 > sizeof(Packet))
				{
					Good = false;
				}
				else
				{
					Packet[P++] = 0x3A;
					memcpy(Packet + P, Data + i, RecLen);
					P += RecLen;
					i += RecLen;
				}
			}

			if (!Good)
			{
				UpdSendStatus(UpdResend, Block, 0);
			}
			else if (process_hex_record(Packet, P))
			{
				// FlasherX refused a record: nothing has replaced the running firmware
				UpdCancel();
			}
			else
			{
				UpdLastBlock = Block;
				UpdSendStatus(UpdBlockOK, Block, 0);
			}
		}
	}
}

// commit: the record type 6 packet PCBsetup sends after the line check. Only once the end of
// the file has been written; the app commits after the record count matched.
void UpdCommit()
{
	if (hex.eof)
	{
		uint8_t Packet[6] = { 0x3A, 0x00, 0x00, 0x00, 0x06, 0xFA };
		Serial.println("Firmware update: installing.");
		process_hex_record(Packet, 6);		// does not return: copies the new firmware and reboots
	}
	UpdCancel();
}

// the new image in the update buffer holds FwTag (RCteensy.ino) with this board's InoType.
// The buffer is flash and can be read directly; it starts at the image's FLASH_BASE_ADDR.
// Compared against FwTag itself, so the magic exists once in a program, not also as a literal here.
bool UpdTagGood()
{
	bool Result = false;
	uint32_t Size = (hex.max > FLASH_BASE_ADDR) ? hex.max - FLASH_BASE_ADDR : 0;
	if (Size > buffer_size) Size = buffer_size;
	const uint8_t* Image = (const uint8_t*)buffer_addr;
	for (uint32_t i = 0; !Result && i + sizeof(FwTag) <= Size; i++)
	{
		if (Image[i] == FwTag[0] && memcmp(Image + i, FwTag, 8) == 0) Result = (Image[i + 8] == InoType);
	}
	return Result;
}

void UpdCancel()
{
	Serial.println("Firmware update: cancelled, restarting with the old firmware.");
	UpdSendStatus(UpdError, 0, 0);
	delay(200);
	firmware_buffer_free(buffer_addr, buffer_size);
	REBOOT;
}

// 0xFF1A: [0] command. 1 start, 3 commit, 4 abort: [1-6] MAC, [7] InoType.
// 2 block header: [1-2] block number, [3-4] length, [5-6] CRC-16
void UpdCanControl(const CAN_message_t& msg)
{
	if (msg.len >= 8)
	{
		if (msg.buf[0] == 2)
		{
			if (UpdActive)
			{
				uint16_t Len = msg.buf[3] | msg.buf[4] << 8;
				if (Len <= UpdMaxBlock)
				{
					UpdCanBlock = msg.buf[1] | msg.buf[2] << 8;
					UpdCanLen = Len;
					UpdCanCRC = msg.buf[5] | msg.buf[6] << 8;
					uint8_t Frames = (Len + 6) / 7;
					UpdCanNeed = (Frames >= 64) ? ~0ULL : ((1ULL << Frames) - 1);
					UpdCanHave = 0;
					UpdCanPending = true;
					UpdCanLastFrame = millis();
					UpdLastRx = millis();
				}
			}
		}
		else
		{
			UpdControl(msg.buf[0], msg.buf + 1, msg.buf[7], UpdLinkCAN);
		}
	}
}

// 0xFF1B: [0] frame index in the block, [1-7] data
void UpdCanData(const CAN_message_t& msg)
{
	if (UpdActive && UpdCanPending && msg.len >= 2)
	{
		uint8_t Index = msg.buf[0];
		uint16_t Start = Index * 7;
		if (Start < UpdCanLen)
		{
			uint16_t Count = UpdCanLen - Start;
			if (Count > 7) Count = 7;
			if (Count > msg.len - 1) Count = msg.len - 1;
			memcpy(UpdCanBuf + Start, msg.buf + 1, Count);
			UpdCanHave |= (1ULL << Index);
			UpdCanLastFrame = millis();
			UpdLastRx = millis();

			if ((UpdCanHave & UpdCanNeed) == UpdCanNeed)
			{
				UpdCanPending = false;
				UpdBlock(UpdCanBlock, UpdCanBuf, UpdCanLen, UpdCanCRC);
			}
		}
	}
}

// main loop while updating: update traffic only, outputs held off
void UpdLoop()
{
	if (UpdLink == UpdLinkCAN)
	{
		// frames queued by the CAN interrupt, one per events() call
		for (int i = 0; i < 40; i++) ISOBUS.events();
	}
	else
	{
		ReceiveUDP();
	}

	if (UpdCanPending && millis() - UpdCanLastFrame > UpdFrameGapMs)
	{
		// frames lost: ask for the block again
		UpdCanPending = false;
		UpdSendStatus(UpdResend, (uint16_t)(UpdLastBlock + 1), 0);
	}

	if (millis() - UpdLoopLast >= LoopTime)
	{
		UpdLoopLast = millis();
		UpdSafeOutputs();
	}

	if (millis() - UpdLastRx > UpdSilentMs) UpdCancel();

	Blink();
}

// Master off and every sensor not connected: relays and outputs go to their comm-loss state
void UpdSafeOutputs()
{
	MasterOn = false;
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		SensorConnected[i] = false;
		PIDenabled[i] = false;
		Applying[i] = false;
	}
	CheckRelays();
	AdjustFlow();
}

void UpdSendStatus(uint8_t Status, uint16_t Block, uint32_t Count)
{
	UpdSendOn(UpdLink, Status, Block, Count);
}

// PGN 32812 over UDP, 0xFF1C over CAN
void UpdSendOn(uint8_t Link, uint8_t Status, uint16_t Block, uint32_t Count)
{
	if (Link == UpdLinkCAN)
	{
		uint8_t B[8];
		B[0] = Status;
		B[1] = Block;
		B[2] = Block >> 8;
		B[3] = Count;
		B[4] = Count >> 8;
		B[5] = Count >> 16;
		B[6] = Count >> 24;
		B[7] = 0;
		CANBus_SendProprietaryB(0x1C, B, 8);
		if (UpdActive) ISOBUS.events();		// interrupt mode: events() moves queued frames out
	}
	else if (Ethernet.linkStatus() == LinkON)
	{
		//0     HeaderLo    44
		//1     HeaderHi    128
		//2-7   MAC
		//8     status
		//9-10  block number
		//11-14 record count (status 105)
		//15    CRC
		uint8_t B[16];
		B[0] = 44;
		B[1] = 128;
		TeensyMAC(B + 2);
		B[8] = Status;
		B[9] = Block;
		B[10] = Block >> 8;
		B[11] = Count;
		B[12] = Count >> 8;
		B[13] = Count >> 16;
		B[14] = Count >> 24;
		B[15] = CRC(B, 15, 0);
		UDPcomm.beginPacket(DestinationIP, DestinationPort);
		UDPcomm.write(B, 16);
		UDPcomm.endPacket();
	}
}

// CRC-16/CCITT-FALSE (poly 0x1021, init 0xFFFF), the same in the app
uint16_t UpdCRC16(const uint8_t* Data, uint16_t Len)
{
	uint16_t Result = 0xFFFF;
	for (uint16_t i = 0; i < Len; i++)
	{
		Result ^= (uint16_t)Data[i] << 8;
		for (uint8_t b = 0; b < 8; b++)
		{
			Result = (Result & 0x8000) ? (uint16_t)((Result << 1) ^ 0x1021) : (uint16_t)(Result << 1);
		}
	}
	return Result;
}
