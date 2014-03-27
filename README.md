CPRemote
========

Group project for EECS481 - Software Engineering. 

The project is a Windows 8 remote control application for a patient with Cerebal Palsy. The application communicates to the arduino via bluetooth that eventually sends the IR signals. 

Build Instructions 
==================
The project consists of the Windows App code and the Arduino Code 

Windows App
-----------
Use Visual Studio 2013 to compile and deploy the windows app code in Windows_Application/CPRemote

Arduino 
-------
The Arduino code can be found in HardwareStuff/Arduino/CPRemote/bluetooth_ir/

The code also uses an CY's IRLib (github.com/cyborg5/IRLib/)

Use the Arduino IDE to first install IRLib and then upload the arduino code to the hardware. 

For everything to work, you must have the following setup

1. BlueSmirf Bluetooth Modem (TX =5, RX =4) 
2. IR emittor connected to a 100 ohm resistor (Pin 3) 
3. IR receiver (pin 11) 



