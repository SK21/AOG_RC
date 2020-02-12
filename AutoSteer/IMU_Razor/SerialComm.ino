void CommToAutoSteer()
{
	//header bytes for 32500
	Serial1.write(126);
	Serial1.write(244);

	// heading
	int temp = imu.yaw * 16;	// 16 * actual
	Serial1.write((byte)(temp >> 8));
	Serial1.write((byte)temp);

	// roll
	temp = imu.roll * 16;	// 16 * actual
	Serial1.write((byte)(temp >> 8));
	Serial1.write((byte)temp);
}

