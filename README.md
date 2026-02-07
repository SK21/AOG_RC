RateController (RC) is an add-on app that works with AgOpenGPS (https://github.com/agopengps-official/AgOpenGPS/) for product application rate control.

**RC Installation**
1.	Go to the latest release.
2.	Download RateControllerApp.zip
3.	Extract the file to a folder of your choice.
4.	Find and run RateController.exe

RC starts with the main screen. If AOG is connected the AOG icon will be green. There are icons for 5 products and fans. The icons will be green if connected. A module will show blue if it is sending data but not receiving. Red if there is a rate error and clear if not connected. Press the icons to show status information about the product. A graph shows the quantity remaining in the tank. Press 'Target Rate' to switch rate settings. Press Rx to use variable rate. Press 'Quantity' to switch settings. Press 'Area' to switch settings. Press quantity amount to reset tank. Press area amount to reset area.

  If there is a rate alarm due to over or under rate application a warning button will flash. The icon of the product in error will also flash. Press the warning button to silence the alarm.

  Most pages in the app menu have help available by pressing the '?'. Troubleshooting information can be found on the menu help page by pressing the '?'.


## Rate control repositories

- AOG_RC (main application)
  https://github.com/SK21/AOG_RC

- Rate_Control (module pcbs)
  https://github.com/AgOpenGPS-Official/Rate_Control

- PCBsetup (module firmware installation)
  https://github.com/SK21/PCBsetup




**Disclaimer**

This software is for discussion and learning. It is not in any way to be used on any physical equipment and is meant for use by a simulator only.

