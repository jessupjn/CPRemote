#include <SoftwareSerial.h>  
#include <IRLib.h>
#include <IRLibMatch.h>

//Format of strings -<protocol>.<code>.<nbits>/

int bluetoothTx = 5 ;  // TX-O pin of bluetooth mate 
int bluetoothRx = 4;  // RX-I pin of bluetooth mate


#define PING 0  
#define LEARN  1
#define SEND  2 



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

int recvd_code_type;

IRrecv My_Receiver(RECV_PIN);
IRTYPES codeType; // The type of code
unsigned long codeValue; // The data bits
int codeBits; // The length of the code in bits
bool GotOne; 


IRTYPES b_protocol_enum; 
unsigned long b_code_long=0; 
int b_nbits_long; 
int b_repeat; 
char recvddata[80];
String b_protocol; 



void setup()
{
  Serial.begin(57600);  // Begin the serial monitor at 9600bps
  bluetooth.begin(57600);  // Start bluetooth serial at 9600
  
  My_Receiver.enableIRIn();
}


void loop()
{
  // If the bluetooth sent any characters
  if(bluetooth.available())  
  {
    memset(recvddata, 0, 80); 
    
    char endl='/'; 
    int len= bluetooth.readBytesUntil(endl, recvddata, 80);
    recvddata[len]='\0'; 
    
    
    recvd_code_type = parseString();
    Serial.println(recvd_code_type); 
    
    if (recvd_code_type == PING) {
      char pingBuff[]= "-A"; 
      sendMessage(pingBuff,2);  
    }
   
   // changeTypesofData (b_protocol, b_code, b_nbits, b_protocol_enum, b_code_long, b_nbits_long); 
    if (recvd_code_type == SEND) {
    Serial.println("Now Sending "+ b_protocol); 
    Serial.print("with b_code "); 
    Serial.println( b_code_long, DEC); 
    Serial.print("and b_nbits "); 
    Serial.println( b_nbits_long, DEC);
    Serial.print("and repeat "); 
    Serial.println( b_repeat, DEC);
    
    for (int i=0; i<b_repeat;i++) {
      My_Sender.send(b_protocol_enum,b_code_long,b_nbits_long);
      delay(250);
    }
 
  }
  }
  
   
  if (My_Receiver.GetResults(&My_Decoder) ) {
    Serial.println("In learn"); 
    My_Decoder.decode();
    if(My_Decoder.decode_type == UNKNOWN) {
      Serial.println(F("Unknown type received. Ignoring."));
    } else {
      codeType = My_Decoder.decode_type;
      codeValue = My_Decoder.value;
      codeBits = My_Decoder.bits;
      GotOne=true;
    }
    My_Decoder.DumpResults();
    Serial.println(My_Decoder.value, DEC);
    Serial.println(Pnames(My_Decoder.decode_type));
    delay(1000);
    
    if (GotOne) {
      //construct the string 
      memset(recvddata, 0, 80); 
      int senddatasize = 0; 
      char L = 'L';
      char period = '.';

      char endl = '/';
      
      //S flag
      memcpy (recvddata, &L, 1);
      senddatasize++; 
      memcpy (recvddata+senddatasize, &period, 1);
      senddatasize++; 
      
     char conversion_int[34]; 
     memset(conversion_int, '/0', 34); 
     memcpy(conversion_int, Pnames(My_Decoder.decode_type), 4); 
          conversion_int[4]=  '/0'; 

     

     int s_size = strlen((char*)convertToString(My_Decoder.decode_type)); 
     
     //protocol 
     memcpy (recvddata+senddatasize, convertToString(My_Decoder.decode_type),s_size );
     senddatasize+=s_size; 
     memcpy (recvddata+senddatasize, &period, 1);
     senddatasize++; 

     
  
    //IRcode 
    memset(conversion_int, '/0', 34); 

    ultoa(codeValue,conversion_int,10); 
    s_size= strlen(conversion_int); 
    memcpy (recvddata+senddatasize, conversion_int, s_size);
    senddatasize+=s_size; 
    memcpy (recvddata+senddatasize, &period, 1);
    senddatasize++; 
    
    //Bits    
    memset(conversion_int, '/0', 34); 
    ultoa(codeBits,conversion_int,10); 
    s_size= strlen(conversion_int); 
    
    memcpy (recvddata+senddatasize, &conversion_int, s_size);
    senddatasize+=s_size; 
    memcpy (recvddata+senddatasize, &endl, 1);
    senddatasize++;
   
   
   sendMessage(recvddata, senddatasize); 
    
      

      
      
    }
    recvd_code_type =-1;
    GotOne= false;  
    My_Receiver.resume();
    
  }
  
  //check if you need to send anything via bluetooth 
  sendBluetoothMessage(); 
 
}

void changeTypesofData (String buff){ 
   

  if (buff == "NEC") {
     Serial.println("NEC FOUND"); 
     b_protocol_enum = NEC; 
  }
  
  if (buff == "NECx") {
     Serial.println("NECx FOUND"); 
     b_protocol_enum = NECX; 
  }
  
  else if (buff == "SONY") {
    b_protocol_enum = SONY; 
  }
  
  else if (buff == "RC5") {
      b_protocol_enum =RC5; 
  }
  
  else if (buff == "RC6") {
     b_protocol_enum = RC6; 
  }
  
  else if (buff == "PANASONIC_OLD") {
       b_protocol_enum = PANASONIC_OLD; 
  }
  
  
  else if (buff == "JVC") {
     b_protocol_enum = JVC; 
  }
  
  else if (buff == "NECX") {
       b_protocol_enum = NECX; 
  }
  
  else if (buff == "GICABLE") {
       b_protocol_enum = static_cast<IRTYPES>GICABLE; 
  }
  
   b_protocol= buff; 
  
}

int findIndex (char d, int index) {
  for (int i=index; i<80; i++) {
    if (recvddata[i] == d) {
      return i; 
    }
  } 
}

int parseString ()
{
  //Serial.println(message); 
  int dashPos = findIndex('-',0);
  int periodPos1 =  findIndex('.',0);
  int periodPos2 =  findIndex('.', periodPos1+1);
  int periodPos3 =  findIndex('.', periodPos2+1);
  int periodPos4 =  findIndex('.', periodPos3+1);
  
  char buff[20]; 
  
  memset(buff, 0, 20); 
  memcpy(buff, recvddata+dashPos+1,periodPos1-dashPos-1); 
  
  if (buff[0] == 'L') {
    return LEARN;
  }
  else if (buff[0] == 'P') {
    return PING;
  }
  
  memset(buff, 0, 20); 
  memcpy(buff, recvddata+periodPos1+1,periodPos2-periodPos1-1); 
  changeTypesofData(buff);
   
  
  memset(buff, 0, 20); 
  memcpy(buff, recvddata+periodPos2+1,periodPos3-periodPos2-1); 
  char * pEnd; 
  b_code_long = strtol(buff, &pEnd, 10); 
  
  memset(buff, 0, 20); 
  memcpy(buff, recvddata+periodPos3+1,periodPos4-periodPos3-1);
  b_nbits_long = atoi(buff); 
  
  memset(buff, 0, 20); 
  memcpy(buff, recvddata+periodPos4+1,4);
  b_repeat = atoi(buff);
  
  return SEND;
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
