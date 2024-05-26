
//******************************************************************************
// process_hex_record()    process record and return okay (0) or error (1)
//******************************************************************************
int process_hex_record(char* packetBuffer, int packetSize)
{
	if (packetSize < 5)
	{
		return 1;
	}
	else if (packetBuffer[0] != 0x3a)
	{
		Serial.printf("abort - invalid hex code %d\n", hex.code);
		return 1;
	}
	else
	{
		for (byte idx = 1; idx + 4 < packetSize;)
		{
			byte len = packetBuffer[idx];
			unsigned int addr = packetBuffer[idx + 1] << 8 | packetBuffer[idx + 2];

			byte type = packetBuffer[idx + 3];
			if (idx + 4 + len < packetSize)
			{
				unsigned sum = (len & 255) + ((addr >> 8) & 255) + (addr & 255) + (type & 255);

				for (byte j = 0; j < len; j++)
				{
					sum += packetBuffer[idx + 4 + j] & 255;
					hex.data[j] = packetBuffer[idx + 4 + j];
				}
				hex.num = len;
				hex.code = type;
				hex.addr = addr;

				byte Checksum = packetBuffer[idx + 4 + len];
				if (((sum & 255) + (Checksum & 255)) & 255)
				{
					Serial.println("abort - bad hex line");
					return 1;
				}
				else
				{
					if (hex.code == 0)// if data record
					{
						// update min/max address so far
						if (hex.base + hex.addr + hex.num > hex.max)
							hex.max = hex.base + hex.addr + hex.num;
						if (hex.base + hex.addr < hex.min)
							hex.min = hex.base + hex.addr;

						uint32_t addr = buffer_addr + hex.base + hex.addr - FLASH_BASE_ADDR;
						if (hex.max > (FLASH_BASE_ADDR + buffer_size))
						{
							Serial.printf("abort - max address %08lX too large\n", hex.max);
							return 1;
						}
						else if (!IN_FLASH(buffer_addr))
						{
							memcpy((void*)addr, (void*)hex.data, hex.num);
						}
						else if (IN_FLASH(buffer_addr))
						{
							int error = flash_write_block(addr, hex.data, hex.num);
							if (error)
							{
								Serial.println();
								Serial.printf("abort - error %02X in flash_write_block()\n", error);
								return 1;
							}
						}
					}
					else if (hex.code == 1)
					{
						// EOF (:flash command not received yet)
						Serial.println("EOF");
						hex.eof = 1;
					}
					else if (hex.code == 2)
					{
						// extended segment address (top 16 of 24-bit addr)
						hex.base = ((hex.data[0] << 8) | hex.data[1]) << 4;
					}
					else if (hex.code == 4)
					{ // extended linear address (top 16 of 32-bit addr)
						hex.base = ((hex.data[0] << 8) | hex.data[1]) << 16;
					}
					else if (hex.code == 5)
					{ // start linear address (32-bit big endian addr)
						hex.base = (hex.data[0] << 24) | (hex.data[1] << 16)
							| (hex.data[2] << 8) | (hex.data[3] << 0);
					}
					else if (hex.code == 6)
					{
						Serial.printf("UPDATE!!!\n");
						// move new program from buffer to flash, free buffer, and reboot
						flash_move(FLASH_BASE_ADDR, buffer_addr, hex.max - hex.min);

						// should not return from flash_move(), but put REBOOT here as reminder
						REBOOT;
						return 0;
					}
					else if (hex.code == 7)
					{
						Serial.printf("LINES DONT Match\n");
						return 1;
					}
					else
					{
						// error on bad hex code
						Serial.println();
						Serial.printf("abort - invalid hex code %d\n", hex.code);
						return 1;
					}

					hex.lines++;
					Serial.println(hex.lines);

					if (hex.eof)
					{
						Serial.println();
						Serial.printf("\nhex file: %1d lines %1lu bytes (%08lX - %08lX)\n", hex.lines, hex.max - hex.min, hex.min, hex.max);
						SENDCheckUdp();

						// check for non Teensy4.1
//                        // check FSEC value in new code -- abort if incorrect
//
//#if defined(KINETISK) || defined(KINETISL)
//                        uint32_t value = *(uint32_t*)(0x40C + buffer_addr);
//                        if (value == 0xfffff9de)
//                        {
//                            Serial.println();
//                            Serial.printf("new code contains correct FSEC value %08lX\n", value);
//                        }
//                        else
//                        {
//                            Serial.println();
//                            Serial.printf("abort - FSEC value %08lX should be FFFFF9DE\n", value);
//                            return 1;
//                        }
//#endif
//
//                        // check FLASH_ID in new code - abort if not found
//                        if (check_flash_id(buffer_addr, hex.max - hex.min))
//                        {
//                            Serial.println();
//                            Serial.printf("new code contains correct target ID %s\n", FLASH_ID);
//                            SENDCheckUdp();
//                        }
//                        else
//                        {
//                            Serial.println();
//                            Serial.printf("abort - new code missing string %s\n", FLASH_ID);
//                            return 1;
//                        }
					}
					idx += 6 + len;//need for extra ::
				}
			}
			else
			{
				Serial.printf("abort - invalid hex code %d\n", hex.code);
				return 1;
			}
		}
	}
	return 0;
}

void SENDCheckUdp()
{
	byte checkOTA[9] = { 0x4f, 0x54, 0x41, 0x55, 0x70, hex.lines & 255, hex.lines >> 8 & 255, hex.lines >> 16 & 255, hex.lines >> 24 & 255 };

	if (Ethernet.linkStatus() == LinkON)
	{
		// send ethernet
		UDPcomm.beginPacket(DestinationIP, DestinationPort);
		UDPcomm.write(checkOTA, sizeof(checkOTA));
		UDPcomm.endPacket();
	}
}



