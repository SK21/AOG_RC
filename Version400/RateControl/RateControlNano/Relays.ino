void SetRelays(void)
{
	RelayTemp = 0;
	if (AOGconnected) RelayTemp = RelayControl;
	RelaysOn = (RelayTemp != 0);

	if (bitRead(RelayTemp, 0)) digitalWrite(Relay1, HIGH); else digitalWrite(Relay1, LOW);
	if (bitRead(RelayTemp, 1)) digitalWrite(Relay2, HIGH); else digitalWrite(Relay2, LOW);
	if (bitRead(RelayTemp, 2)) digitalWrite(Relay3, HIGH); else digitalWrite(Relay3, LOW);
	if (bitRead(RelayTemp, 3)) digitalWrite(Relay4, HIGH); else digitalWrite(Relay4, LOW);
	if (bitRead(RelayTemp, 4)) digitalWrite(Relay5, HIGH); else digitalWrite(Relay5, LOW);
	if (bitRead(RelayTemp, 5)) digitalWrite(Relay6, HIGH); else digitalWrite(Relay6, LOW);
	if (bitRead(RelayTemp, 6)) digitalWrite(Relay7, HIGH); else digitalWrite(Relay7, LOW);

#if(UseSwitchedPowerPin == 0)
	if (bitRead(RelayTemp, 7)) digitalWrite(Relay8, HIGH); else digitalWrite(Relay8, LOW);
#endif
}
