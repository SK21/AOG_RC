
String GetPage1()
{
	// switches, laid out like the app's phone page: Master across the top, sections in a
	// 4-wide grid, outlined buttons that turn light green when on. Each press still posts
	// the form to /ButtonPressed (no script needed), which returns this page redrawn.
	String st;
	st.reserve(4000);
	st = "<HTML>";
	st += "  <head>";
	st += "    <META content='text/html; charset=utf-8' http-equiv=Content-Type>";
	st += "    <meta name='viewport' content='width=device-width, initial-scale=1.0'>";
	st += "    <title>Rate Control</title>";
	st += "    <style>";
	st += "      body{font-family:Helvetica,Arial,sans-serif;margin:0;padding:12px;background-color:wheat;color:#222;text-align:center;-webkit-user-select:none;user-select:none}";
	st += "      h1{font-size:22px;color:#444;margin:8px 0 12px}";
	st += "      #pad{display:grid;grid-template-columns:repeat(4,1fr);gap:10px;max-width:480px;margin:0 auto}";
	st += "      #pad button{font-size:22px;padding:18px 0;border-radius:8px;border:1px solid #444;background:transparent;color:#222;-webkit-appearance:none;touch-action:manipulation}";
	st += "      #pad button.on{background:#90ee90}";
	st += "      #pad .wide{grid-column:span 4}";
	st += "      #back{display:block;width:60%;max-width:300px;margin:16px auto 0;padding:16px 0;font-size:22px;border:1px solid #444;border-radius:8px;color:#222;text-decoration:none}";
	st += "    </style>";
	st += "  </head>";
	st += "  <BODY>";
	st += "    <h1>Switches</h1>";
	st += "    <form id=pad method=post action='/ButtonPressed'>";

	st += "      <button class='wide";
	if (WifiMasterOn) st += " on";
	st += "' name='Btn' type=submit value='Master'>Master</button>";

	for (int i = 0; i < 16; i++)
	{
		st += "      <button";
		if (Button[i]) st += " class='on'";
		st += " name='Btn' type=submit value='" + String(i + 1) + "'>" + String(i + 1) + "</button>";
	}

	st += "    </form>";
	st += "    <a id=back href='/page0'>&#8249; Back</a>";
	st += "  </BODY>";
	st += "</HTML>";

	return st;
}
