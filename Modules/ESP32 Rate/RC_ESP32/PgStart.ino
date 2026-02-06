String GetPage0()
{
	// version
	uint16_t yr = InoID % 10 + 2020;
	uint16_t rest = InoID / 10;
	uint8_t mn = rest % 100;
	uint16_t dy = rest / 100;

	String fwVer;
	if (mn <= 12 && dy <= 31)
	{
		fwVer = "Firmware Version: v";
		fwVer += String(yr);
		fwVer += ".";
		// zero-pad month/day for nicer display
		if (mn < 10) fwVer += "0";
		fwVer += String(mn);
		fwVer += ".";
		if (dy < 10) fwVer += "0";
		fwVer += String(dy);
	}
	else
	{
		fwVer = "Firmware Version: invalid";
	}
	
	String st = "<HTML>";
	st += "";
	st += "  <head>";
	st += "    <META content='text/html; charset=utf-8' http-equiv=Content-Type>";
	st += "    <meta name=vs_targetSchema content='HTML 4.0'>";
	st += "    <meta name='viewport' content='width=device-width, initial-scale=1.0'>";
	st += "    <title>Rate Control</title>";
	st += "    <style>";
	st += "      html {";
	st += "        font-family: Helvetica, Arial, sans-serif;";
	st += "        display: inline-block;";
	st += "        margin: 0px auto;";
	st += "        text-align: center;";
	st += "      }";
	st += "";
	st += "      h1 {";
	st += "        color: #444444;";
	st += "        margin: 50px auto 30px;";
	st += "      }";
	st += "";
	st += "      table.center {";
	st += "        margin-left: auto;";
	st += "        margin-right: auto;";
	st += "      }";
	st += "";
	st += "      .buttonOn {";
	st += "        background-color: #00ff00;";
	st += "        border: none;";
	st += "        color: white;";
	st += "        padding: 15px 32px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        display: inline-block;";
	st += "        margin: 4px 2px;";
	st += "        cursor: pointer;";
	st += "        font-size: 15px;";
	st += "        width: 30%;";
	st += "      }";
	st += "";
	st += "      .buttonOff {";
	st += "        background-color: #ff0000;";
	st += "        border: none;";
	st += "        color: white;";
	st += "        padding: 15px 32px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        display: inline-block;";
	st += "        margin: 4px 2px;";
	st += "        cursor: pointer;";
	st += "        font-size: 15px;";
	st += "        width: 30%;";
	st += "      }";
	st += "";
	st += "      /* MATCH GetPage2 button: fixed control width with responsive max */";
	st += "      .button-72 {";
	st += "        align-items: center;";
	st += "        background-color: initial;";
	st += "        background-image: linear-gradient(rgba(179, 132, 201, .84), rgba(57, 31, 91, .84) 50%);";
	st += "        border-radius: 42px;";
	st += "        border-width: 0;";
	st += "        box-shadow: rgba(57, 31, 91, 0.24) 0 2px 2px, rgba(179, 132, 201, 0.4) 0 8px 12px;";
	st += "        color: #FFFFFF;";
	st += "        cursor: pointer;";
	st += "        display: inline-flex;";
	st += "        justify-content: center;";
	st += "        align-items: center;";
	st += "        font-family: Quicksand, sans-serif;";
	st += "        font-size: 18px;";
	st += "        font-weight: 700;";
	st += "        letter-spacing: .04em;";
	st += "        line-height: 16px;";
	st += "        margin: 12px auto;";
	st += "        padding: 12px 18px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        text-shadow: rgba(255, 255, 255, 0.4) 0 0 4px, rgba(255, 255, 255, 0.2) 0 0 12px, rgba(57, 31, 91, 0.6) 1px 1px 4px, rgba(57, 31, 91, 0.32) 4px 4px 16px;";
	st += "        user-select: none;";
	st += "        -webkit-user-select: none;";
	st += "        touch-action: manipulation;";
	st += "        vertical-align: baseline;";
	st += "        width: 320px;";       // same logical width as GetPage2 controls
	st += "        max-width: 90%;";
	st += "      }";
	st += "";
	st += "      .InputCell {";
	st += "        text-align: center;";
	st += "      }";
	st += "";
	st += "    </style>";
	st += "  </head>";
	st += "";
	st += "  <BODY>";
	st += "    <style>";
	st += "      body {";
	st += "        margin-top: 50px;";
	st += "        background-color: wheat;";
	st += "      }";
	st += "";
	st += "      font-family: Arial,";
	st += "      Helvetica,";
	st += "      Sans-Serif;";
	st += "";
	st += "    </style>";
	st += "";
	st += "    <h1 align=center>RC_ESP32";
	st += "    </h1>";
	st += "    <p style='margin:0 0 12px 0; color:#666; font-size:14px;'>";
	st += fwVer;
	st += "</p>";
	st += "    <form id=FORM1 method=post action='/'>&nbsp;";
	st += "";
	st += "      <p> <a class='button-72' href='/page1' >Switches</a> </p>";
	st += "      <p> <a class='button-72' href='/page2' >Network</a> </p>";
	st += "      <p> <a class='button-72' href='/update' >Update Firmware</a> </p>";
	st += "";
	st += "    </form>";
	st += "";
	st += "</HTML>";

	return st;
}
