RateController is an add-on app that works with AgOpenGPS version 5 (https://github.com/farmerbriantee/AgOpenGPS/) for rate control. There is an example rate control pcb and code in the RateController_Files folder of the latest release. It can control up to 16 sections and 5 products either by flow-meter or motor rpm.

There are two methods of rate control.
1. VCN (valve cal number) is 4 numbers combined to control the rate.

https://www.k-state.edu/precisionag/variable-rate/automatic-section-control/index.html

https://talk.newagtalk.com/forums/thread-view.asp?tid=82927&mid=594748#M594748

http://ravenprecision.force.com/knowledgebase/articles/Tech_Tip/Valve-Cal-Number-Explained/?q=valve+cal&l=en_US&fs=Search&pn=1


2. PID can be used to control a flow valve or a motor such as used on a meter-roller or spin spreader.


To do a simulation:

With AOG running create a new field and turn on the sections.

Install RateController from the install folder. The main window of RC can be dragged to the best location and it will restart at that position. It will stay on top of AOG. Go to settings and load the example file. RC should now show the rate being applied.
