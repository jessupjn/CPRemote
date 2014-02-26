#include <SoftwareSerial.h>  
#include <IRLib.h>
#include <IRLibMatch.h>

//Format of strings -<protocol>.<code>.<nbits>/

int bluetoothTx =5 ;  // TX-O pin of bluetooth mate 
int bluetoothRx = 4;  // RX-I pin of bluetooth mate

SoftwareSerial bluetooth(bluetoothTx, bluetoothRx);


class IRdecodeGIcable: public virtual IRdecodeBase
{
public:
  bool decode(void); 
};
class IRsendGIcable: public virtual IRsendBase
{
public:
  void send(unsigned long data, bool Repeat);
};

#define GICABLE (LAST_PROTOCOL+1)

/* The IRP notation for this protocol according to
 * http://www.hifi-remote.com/johnsfine/DecodeIR.html#G.I. Cable
 * is "{38.7k,490}<1,-4.5|1,-9>(18,-9,F:8,D:4,C:4,1,-84,(18,-4.5,1,-178)*) {C = -(D + F:4 + F:4:4)}"
 */
bool IRdecodeGIcable::decode(void) {
  ATTEMPT_MESSAGE(F("GIcable"));
  // Check for repeat
  if (rawlen == 4 && MATCH(rawbuf[1], 490*18) && MATCH(rawbuf[2],2205)) {
    bits = 0;
    value = REPEAT;
    decode_type= static_cast<IRTYPES>GICABLE;
    return true;
  }
  if(!decodeGeneric(36, 18*490, 9*490, 0, 490, 9*490, 2205/*(4.5*490)*/)) return false;
  decode_type= static_cast<IRTYPES>GICABLE;
  return true;
};

void IRsendGIcable::send(unsigned long data, bool Repeat) {
  ATTEMPT_MESSAGE(F("sending GIcable"));
  if(Repeat) {
    enableIROut(39);
    mark (490*18); space (2205); mark (490); space(220);delay (87);//actually "space(87200);"
  } else {
    sendGeneric(data,16, 490*18, 490*9, 490, 490, 490*9, 2205, 39, true);
  }
};

//Create a custom class that combines this new protocol with all the others
class MyCustomSend: 
public virtual IRsend,
public virtual IRsendGIcable
{
public:
  void send(IRTYPES Type, unsigned long data, int nbits);
};
void MyCustomSend::send(IRTYPES Type, unsigned long data, int nbits) {
  if (Type==GICABLE){
    IRsendGIcable::send(data,false);
    //un-comment the line below to text repeats
    //delay(3000); IRsendGIcable::send(data,true);
  }
  else
    IRsend::send(Type, data, nbits);
}
class MyCustomDecode: 
public virtual IRdecode,
public virtual IRdecodeGIcable
{
public:
  virtual bool decode(void);    // Calls each decode routine individually
  void DumpResults(void);
};
bool MyCustomDecode::decode(void) {
  if (IRdecodeGIcable::decode()) return true;
  return IRdecode::decode ();
}
void MyCustomDecode::DumpResults(void){
  if(decode_type==GICABLE) {
    Serial.print(F("Decoded GIcable: Value:")); Serial.print(value, HEX);
  };
  IRdecode::DumpResults();
};

MyCustomDecode My_Decoder;

MyCustomSend My_Sender;

int RECV_PIN =11;

IRrecv My_Receiver(RECV_PIN);


void setup()
{
  Serial.begin(57600);  // Begin the serial monitor at 9600bps
  bluetooth.begin(57600);  // Start bluetooth serial at 9600
}


void loop()
{
  // If the bluetooth sent any characters
  if(bluetooth.available())  
  {
    char recvddata[80];
    memset(recvddata, 0, 80); 
    String b_protocol=""; 
    String b_code=""; 
    String b_nbits=""; 
    char endl='/'; 
    int len= bluetooth.readBytesUntil(endl, recvddata, 80);
    recvddata[len]='\0'; 
    parseString(recvddata, b_protocol, b_code, b_nbits); 
    Serial.println("Protocol: "+b_protocol); 
    Serial.println("code: "+b_code); 
    IRTYPES b_protocol_enum; 
    unsigned long b_code_long=0; 
    int b_nbits_long; 
  
    changeTypesofData (b_protocol, b_code, b_nbits, b_protocol_enum, b_code_long, b_nbits_long); 
    
    Serial.println("Now Sending "+ b_protocol); 
    Serial.print("with b_code "); 
    Serial.println( b_code_long, DEC); 
    Serial.print("and b_nbits "); 
    Serial.println( b_nbits_long, DEC);

    My_Sender.send(b_protocol_enum,b_code_long,b_nbits_long);
 
    

   
  }
  
  //check if you need to send anything via bluetooth 
  sendBluetoothMessage(); 
 
}

void changeTypesofData (String b_protocol, String b_code, String b_nbits,IRTYPES &b_protocol_enum, unsigned long &b_code_long, int &b_nbits_long){ 
  char * pEnd; 
  b_code_long = strtol(b_code.c_str(), &pEnd, 10); 
  b_nbits_long = atoi(b_nbits.c_str()); 

  if (b_protocol == "NEC") {
     b_protocol_enum = NEC; 
  }
  
  else if (b_protocol == "SONY") {
    b_protocol_enum = SONY; 
  }
  
  else if (b_protocol == "RC5") {
      b_protocol_enum =RC5; 
  }
  
  else if (b_protocol == "RC6") {
     b_protocol_enum = RC6; 
  }
  
  else if (b_protocol == "PANASONIC_OLD") {
       b_protocol_enum = PANASONIC_OLD; 
  }
  
  
  else if (b_protocol == "JVC") {
     b_protocol_enum = JVC; 
  }
  
  else if (b_protocol == "NECX") {
       b_protocol_enum = NECX; 
  }
  
  else if (b_protocol == "GICABLE") {
       b_protocol_enum = static_cast<IRTYPES>GICABLE; 
  }
  
}


void parseString (String message, String &b_protocol, String &b_code, String &b_nbits)
{
  //Serial.println(message); 
  int dashPos = message.indexOf('-');
  int periodPos =  message.indexOf('.');
  int periodPosLast =  message.indexOf('.', periodPos+1);

  
  if(dashPos != -1)
  {
     b_protocol = message.substring(dashPos+1,periodPos-dashPos);    
     b_code = message.substring(periodPos+1,periodPosLast); 
     b_nbits = message.substring(periodPosLast+1);
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
