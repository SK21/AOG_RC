void SetRelays(void)
{
  byte RelayTemp = 0;
  if (ApplicationOn[0] || ApplicationOn[1]) RelayTemp = RelayLo;

  if (bitRead(RelayTemp, 0)) mcp.digitalWrite(Relay1, HIGH); else mcp.digitalWrite(Relay1, LOW);
  if (bitRead(RelayTemp, 1)) mcp.digitalWrite(Relay2, HIGH); else mcp.digitalWrite(Relay2, LOW);
  if (bitRead(RelayTemp, 2)) mcp.digitalWrite(Relay3, HIGH); else mcp.digitalWrite(Relay3, LOW);
  if (bitRead(RelayTemp, 3)) mcp.digitalWrite(Relay4, HIGH); else mcp.digitalWrite(Relay4, LOW);
  if (bitRead(RelayTemp, 4)) mcp.digitalWrite(Relay5, HIGH); else mcp.digitalWrite(Relay5, LOW);
  if (bitRead(RelayTemp, 5)) mcp.digitalWrite(Relay6, HIGH); else mcp.digitalWrite(Relay6, LOW);
  if (bitRead(RelayTemp, 6)) mcp.digitalWrite(Relay7, HIGH); else mcp.digitalWrite(Relay7, LOW);

#if(UseSwitchedPowerPin == 0)
  if (bitRead(RelayTemp, 7)) mcp.digitalWrite(Relay8, HIGH); else mcp.digitalWrite(Relay8, LOW);
#endif
  RelayTemp = 0;
  if (ApplicationOn[0] || ApplicationOn[1]) RelayTemp = RelayHi;

  if (bitRead(RelayTemp, 0)) mcp.digitalWrite(Relay9, HIGH); else mcp.digitalWrite(Relay9, LOW);
  if (bitRead(RelayTemp, 1)) mcp.digitalWrite(Relay10, HIGH); else mcp.digitalWrite(Relay10, LOW);
  if (bitRead(RelayTemp, 2)) mcp.digitalWrite(Relay11, HIGH); else mcp.digitalWrite(Relay11, LOW);
  if (bitRead(RelayTemp, 3)) mcp.digitalWrite(Relay12, HIGH); else mcp.digitalWrite(Relay12, LOW);
  if (bitRead(RelayTemp, 4)) mcp.digitalWrite(Relay13, HIGH); else mcp.digitalWrite(Relay13, LOW);
  if (bitRead(RelayTemp, 5)) mcp.digitalWrite(Relay14, HIGH); else mcp.digitalWrite(Relay14, LOW);
  if (bitRead(RelayTemp, 6)) mcp.digitalWrite(Relay15, HIGH); else mcp.digitalWrite(Relay15, LOW);
  if (bitRead(RelayTemp, 7)) mcp.digitalWrite(Relay16, HIGH); else mcp.digitalWrite(Relay16, LOW);
}
