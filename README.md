# AOG
RateController is a stand-alone app that works with AgOpenGPS (https://github.com/farmerbriantee/AgOpenGPS/) for rate control. To use it AgOpenGPS is modified to send one PGN with worked area, working width, speed and section control bytes to RateController. This modified version is in the folder AgOpenGPS_Apps. 

The two apps communicate over UDP so UDP has to been turned on in AgOpenGPS. A control box with relays and/or switches connects to RateController with a serial comm port on USB or UDP over a network.

There are example PCBs in the folders RC31 and RC4. There is example code in the folder RCarduino. This code will work with a Nano or a Nano33. Some of the code is based on AgRate by BrianTee and physical switch code by MTZ8302.

RateController uses a VCN (valve cal number) for controlling the rate. Some info:

https://www.k-state.edu/precisionag/variable-rate/automatic-section-control/index.html

https://talk.newagtalk.com/forums/thread-view.asp?tid=82927&mid=594748#M594748

http://ravenprecision.force.com/knowledgebase/articles/Tech_Tip/Valve-Cal-Number-Explained/?q=valve+cal&l=en_US&fs=Search&pn=1



To do a simulation:

For RateController200 the AOG version 'AgOpenGPS_Apps' in the folder RateController200/AgOpenGPS_Apps/ needs to be used. It sends an extra PGN that is needed by RC. Start AOG and turn on UDP. Create a new field and turn on the sections.

 Install RateController from the zip file. The main window of RC can be dragged to the best location and it will restart at that position. It will stay on top of other windows such as AOG. Go to Settings. There are 5 different products that can be controlled. Enter the rate info for the first product. Rate Cal # is the number of counts of the sensor for each unit of product being applied. For simulation use a number around 50. Choose either VCN or PID. Enter values on set-up tabs for these. On the Port tab to simulate flow select Virtual Nano. RC should now show the rate being applied.

