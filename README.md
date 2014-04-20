CPRemote
========

Group project for EECS481 - Software Engineering. 

The project is a Windows 8.1 remote control application for a patient with Cerebal Palsy. The application communicates to the arduino via bluetooth that eventually sends the IR signals. 

Build Instructions 
==================
The project consists of the Windows App code and the Arduino Code 

Windows App
-----------
Use Visual Studio 2013 to compile and deploy the windows app code in Windows_Application/CPRemote
<br>
<b>Build Source</b><br>
1. Clone the <i>master branch</i> to PC<br>
2. Navigate to Windows_Application/CPRemoteApp<br>
3. Open CPRemoteApp.sln<br>
4. Press the run button to build and deploy the code.<br>

The code has been optimized for Windows Surface Tablet running Windows 8.1

Arduino 
-------
The Arduino code can be found in HardwareStuff/Arduino/bluetooth_ir/

The code also uses an CY's IRLib (github.com/cyborg5/IRLib/). The library can also be found in HardwareStuff/Arduino/IRLib/

Use the Arduino IDE to first install IRLib and then upload the arduino code to the hardware. 

For everything to work, you must have the following setup as shown in the image below: 

1. BlueSmirf Bluetooth Modem (TX =5, RX =4) 
2. IR emittor connected to a 220 ohm resistor (Pin 3) 
3. IR receiver (pin 11) 

![Wiring diagram](https://s3.amazonaws.com/martinzhu/image/Arduino+Schematics.png)

<br> 

Operating Instructions
----------------------
Once the code has been deployed to the Windows Surface tablet and the Arduino. First navigate to the <i>Settings Menu </i> and select the bluetooth connection to connect to the arduino. Then learn the infrared codes for Channel and Volume devices by adding a new device. Once volume and channel devices have been trained, add channels to the channel list. A

The application is now all set to be used. Navigate to the Remote Menu and start using the application.  

<br>

<b>Team:</b>Adnan Tahir, Luke Graham, Jackson Jessup, Catie Edwards, Wei Zhu

To report any issues please contact the team at cpremote481@umich.edu




