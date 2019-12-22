/*
 - This sketch is set up as data-ready interrupt driven; the data ready interrupt goes
   high when new gyro data is ready and resets when the Sentral event status register is read

 - All configuration definitions are in "config.h"

 - After doing any calibrations to the USFS, ALWAYS POWER-CYCLE THE USFS for the calibrations to take effect

 - Algorithm Status Byte:
 0 - Indeterminate; insufficient data to judge. This is usually the default at startup
 1 - Standhy; algorithm halted
 2 - Algorithm "Slowdown"
 4 - Algorithm "Still"
 8 - Algorithm calibration stable; this is the desired run state
 16 - Magnetic transient
 32 - Unreliable sensor data
 
 */
