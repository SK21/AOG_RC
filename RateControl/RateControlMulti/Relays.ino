void SetRelays(void)
{
	RelayTemp = 0;
	if (AOGconnected) RelayTemp = RelayControl;

	if (bitRead(RelayTemp, 0)) digitalWrite(Section1, HIGH); else digitalWrite(Section1, LOW);
	if (bitRead(RelayTemp, 1)) digitalWrite(Section2, HIGH); else digitalWrite(Section2, LOW);
	if (bitRead(RelayTemp, 2)) digitalWrite(Section3, HIGH); else digitalWrite(Section3, LOW);
	if (bitRead(RelayTemp, 3)) digitalWrite(Section4, HIGH); else digitalWrite(Section4, LOW);
	if (bitRead(RelayTemp, 4)) digitalWrite(Section5, HIGH); else digitalWrite(Section5, LOW);
	if (bitRead(RelayTemp, 5)) digitalWrite(Section6, HIGH); else digitalWrite(Section6, LOW);
	if (bitRead(RelayTemp, 6)) digitalWrite(Section7, HIGH); else digitalWrite(Section7, LOW);

	RelaysOn = (RelayTemp != 0);
}
