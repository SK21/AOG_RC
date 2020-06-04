# AOG
RateController is a stand-alone app that works with AgOpenGPS (https://github.com/farmerbriantee/AgOpenGPS/) for rate control. To use it AgOpenGPS is modified to send one PGN with worked area, working width, speed and section control bytes to RateController. This modified version is in the folder AgOpenGPS_Apps. 

The two apps communicate over UDP so UDP has to been turned on in AgOpenGPS. A control box with relays and/or switches connects to RateController with a serial comm port on USB or UDP over a network.

There is an example PCB in the folder RC31. There is example code in the folder RateControlNano33. This code will work with a Nano or a Nano33. Some of the code is based on AgRate by BrianTee and physical switch code by MTZ8302.

RateController uses a VCN (valve cal number) for controlling the rate. Some info:

https://www.k-state.edu/precisionag/variable-rate/automatic-section-control/index.html

https://talk.newagtalk.com/forums/thread-view.asp?tid=82927&mid=594748#M594748

http://ravenprecision.force.com/knowledgebase/articles/Tech_Tip/Valve-Cal-Number-Explained/?q=valve+cal&l=en_US&fs=Search&pn=1
