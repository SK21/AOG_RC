void CommToAutoSteer()
{
	//header bytes for 32500
	Serial.write(126);
	Serial.write(244);

	// heading
	int temp = heading[0] * 16;	// 16 * actual
	Serial.write((byte)(temp >> 8));
	Serial.write((byte)temp);

	// roll
	temp = angle[0][0] * 16;	// 16 * actual
	Serial.write((byte)(temp >> 8));
	Serial.write((byte)temp);
}
