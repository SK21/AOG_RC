
String GetPage2()
{
	String st = "<HTML>";
	st += "";
	st += "  <head>";
	st += "    <META content='text/html; charset=utf-8' http-equiv=Content-Type>";
	st += "    <meta name=vs_targetSchema content='HTML 4.0'>";
	st += "    <meta name='viewport' content='width=device-width, initial-scale=1.0'>";
	st += "    <title>Wifi AOG</title>";
	st += "    <style>";
	st += "      html {";
	st += "        font-family: Helvetica;";
	st += "        display: inline-block;";
	st += "        margin: 0px auto;";
	st += "        text-align: center;";
	st += "";
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
	st += "      .button-72 {";
	st += "        align-items: center;";
	st += "        background-color: initial;";
	st += "        background-image: linear-gradient(rgba(179, 132, 201, .84), rgba(57, 31, 91, .84) 50%);";
	st += "        border-radius: 42px;";
	st += "        border-width: 0;";
	st += "        box-shadow: rgba(57, 31, 91, 0.24) 0 2px 2px, rgba(179, 132, 201, 0.4) 0 8px 12px;";
	st += "        color: #FFFFFF;";
	st += "        cursor: pointer;";
	st += "        display: flex;";
	st += "        font-family: Quicksand, sans-serif;";
	st += "        font-size: 18px;";
	st += "        font-weight: 700;";
	st += "        justify-content: center;";
	st += "        letter-spacing: .04em;";
	st += "        line-height: 16px;";
	st += "        margin: auto;";
	st += "        padding: 18px 18px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        text-shadow: rgba(255, 255, 255, 0.4) 0 0 4px, rgba(255, 255, 255, 0.2) 0 0 12px, rgba(57, 31, 91, 0.6) 1px 1px 4px, rgba(57, 31, 91, 0.32) 4px 4px 16px;";
	st += "        user-select: none;";
	st += "        -webkit-user-select: none;";
	st += "        touch-action: manipulation;";
	st += "        vertical-align: baseline;";
	st += "        width: 40%";
	st += "      }";
	st += "";
	st += "      .InputCell {";
	st += "        text-align: center;";
	st += "        font-size: 18px;";
	st += "        font-weight: 700;";
	st += "      }";
	st += "";
	st += "      a:link {";
	st += "        font-size: 150%;";
	st += "      }";
	st += "";
	st += "    </style>";
	st += "  </head>";
	st += "";
	st += "  <BODY>";
	st += "    <style>";
	st += "      body {";
	st += "        margin-top: 50px;";
	st += "        background-color: wheat";
	st += "      }";
	st += "";
	st += "      font-family: Arial,";
	st += "      Helvetica,";
	st += "      Sans-Serif;";
	st += "";
	st += "    </style>";
	st += "";
	st += "    <h1 align=center>Wifi Network </h1>";
	st += "    <form id=FORM1 method=post action='/'>&nbsp;";
	st += "      <table class='center'>";
	st += "";
	st += "        <tr>";
	st += "          <td align='left'>Network</td>";
	st += "          <td><input class='InputCell' size='20' name='prop1' value='" + String(MDL.SSID) + "' ID=Text1></td>";
	st += "        </tr>";
	st += "";
	st += "        <tr>";
	st += "          <td align='left'>Password</td>";
	st += "          <td><input class='InputCell' size='20' name='prop2' value='" + String(MDL.Password) + "' ID=Text2></td>";
	st += "        </tr>";
	st += "      </table>";
	st += "";
	st += "      <p> <input class='button-72' id=Submit1 type=submit value='Save/Restart'></p>";
	st += "      <p> <a href='/page0'>Back</a> </p>";
	st += "    </form>";
	if (WiFi.isConnected())
	{
		st += "<p>Wifi Connected to " + String(MDL.SSID) + "</p>";
	}
	else
	{
		st += "<p>Wifi Not Connected</p>";
	}
	//st += "<P>Debug</p>";
	//st += "<p>" + String(debug1) + "</p>";
	//st += "<p>" + String(debug2) + "</p>";
	//st += "<p>" + String(debug3) + "</p>";
	st += "</body>";
	st += "";
	st += "</HTML>";

	return st;
}







