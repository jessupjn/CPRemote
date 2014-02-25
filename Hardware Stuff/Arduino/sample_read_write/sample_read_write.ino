#include <SoftwareSerial.h>  

int bluetoothTx =5 ;  // TX-O pin of bluetooth mate, Arduino D2
int bluetoothRx = 4;  // RX-I pin of bluetooth mate, Arduino D3


SoftwareSerial bluetooth(bluetoothTx, bluetoothRx);

void setup()
{
  Serial.begin(57600);  // Begin the serial monitor at 9600bps
  bluetooth.begin(57600);  // Start bluetooth serial at 9600
}


void loop()
{
  if(bluetooth.available())  // If the bluetooth sent any characters
  {
    // Send any characters the bluetooth prints to the serial monitor
    char recvddata[80];
    memset(recvddata, 0, 80); 
    String b_protocol=""; 
    String b_code=""; 
    char endl='/'; 
    int len= bluetooth.readBytesUntil(endl, recvddata, 80);
    recvddata[len]='\0'; 
    parseString(recvddata, b_protocol, b_code); 
    Serial.println("Protocol: "+b_protocol); 
    Serial.println("code: "+b_code); 
    
  }
  
  sendBluetoothMessage(); 
 
}


void parseString (String message, String &b_protocol, String &b_code)
{
  int commaPosition; 
  commaPosition = message.indexOf('.');
  if(commaPosition != -1)
  {
    b_protocol = message.substring(0,commaPosition); 
    b_protocol[commaPosition+1]='\0'; 
    b_code = message.substring(commaPosition+1); 
  }
     
 
}


void sendBluetoothMessage () {
  
  if(Serial.available())  // If stuff was typed in the serial monitor
  {
   char serialdata[80];
   memset(serialdata, 0, 80); 
   int lf=10; 
   
    // Send any characters the Serial monitor prints to the bluetooth
   int len= Serial.readBytesUntil(lf, serialdata, 80);
    sendMessage(serialdata, len ); 
  }  
}



//outbound
void sendMessage(char* message, int len)
{
	Serial.print("> ");
	Serial.println(message);
	int messageLen =len;
	if (messageLen < 256)
	{
		bluetooth.write(messageLen);
		bluetooth.print(message);
	}
}

