#include "p32mx340f256h.h"

// Include all headers for any enabled TCPIP Stack functions
#include "TCPIP Stack/TCPIP.h"

#if defined(STACK_USE_ZEROCONF_LINK_LOCAL)
#include "TCPIP Stack/ZeroconfLinkLocal.h"
#endif
#if defined(STACK_USE_ZEROCONF_MDNS_SD)
#include "TCPIP Stack/ZeroconfMulticastDNS.h"
#endif

// Include functions specific to this stack application
#include "MainDemo.h"
#include "i2c.h"

#define versionhigh 2
#define versionlow 1

#define CMDNULL 0
#define CMDGETSW 1
#define CMDSETLED 2
#define CMDGETCOUNT 3
#define CMDMOTORSTATUS 4
#define CMDSETMOTOR1 5
#define CMDSETMOTOR2 6
#define CMDSETBOTHMOTORS 7
#define CMDGETLINEFOLLOW 8
#define CMDSETLINEFOLLOW 9
#define CMDGETLIGHTTEMP 17
#define CMDPOWERSWITCH 21
#define CMDPOWERSERV01 22
#define CMDPOWERSERV02 23
#define CMDPOWERREADAD0 24
#define CMDPOWERREADAD1 25
#define CMDPOWERREADphotodiode 26
#define CMDPOWERREADthermistor 27
#define CMDREADemfs 30
#define CMDRDlightsensors 31
#define CMDMyMAC 32
#define CMDRDMMA8452 40
#define CMDRDgyroSTATUS 46
#define CMDRDgyro 43
#define CMDMagSTATUS 44
#define CMDMag 45
#define CMDdoSTEP 50
#define CMDStepStatus 51
#define CMDStepRead 52
#define CMDstepInstRead 53
#define CMDreadSounds 54
#define CMDGetMotorSpeedWord 55
#define CMDMotorSpeedClosed 56
//#define CMDGETstarpos 57
#define CMDGETusgps 58

#define CMDGETversion 63

#define speedhyst 100



void initi2Cs(void);

// Private helper functions.
// These may or may not be present in all applications.
void InitAppConfig(void);
void InitializeBoard(void);
void ProcessIO(void);
void POSTTCPchar(char b);
void POSTTCPhead(int len,int id);


//extern volatile BYTE GPSRX[8];
//extern volatile int GPSRXcount;
//extern volatile int GPSRXcount;
volatile int LINEA,LINEB;
volatile int LINEAa,LINEBa;
volatile int emf1v,emf2v;;
volatile int photov,thermv;
volatile int lightav,lightbv;
volatile char ToggleLine=1;

int emfoffset1=16380;
int emfoffset2=16380;
int setemfoff=8000;

int LINETa=512;	// if below this value then black line
int LINETb=512;

char sounds[2048];
volatile int soundhead=0;
int soundtail=0;

//volatile int pwmgate;

char countup1=1;  // new - better distance
char countup2=1;  // new - better distance
int speed1=0;
int pos1=0;
int speed2=0;
int pos2=0;
volatile DWORD lasticA=0;
volatile DWORD lasticB=0;
volatile DWORD HlasticA0,HlasticA1;
volatile DWORD HlasticB0,HlasticB1;
volatile UINT16 CspeedA,CspeedB;
volatile unsigned char IC1F,IC3F;
volatile UINT16 /*dificA0,dificA1,*/lasticAS0,lasticAS1;
volatile UINT16 /*dificB0,dificB1,*/lasticBS0,lasticBS1;
volatile UINT32 dificA1t,dificA0t,dificB0t,dificB1t;
volatile UINT32 PeriodA,PeriodB;
volatile int timeoutA,timeoutB;

char closedloop=0;
volatile char CLflag=0;
INT16 DesiredL=0;
INT16 DesiredR=0;
INT16 erroriL,erroriR;
INT16 errorL,errorR;
INT16 LasterrorL,LasterrorR;
INT16 McmdL=0;
INT16 McmdR=0;
volatile int MotorPWMcount=0;
volatile int MotorleftV=0;	
volatile int MotorrightV=0;	

#define RXBUFFERsize 256
volatile unsigned char RX1[RXBUFFERsize];
volatile int RxHead1,RxTail1;
volatile unsigned char RX2[RXBUFFERsize];
volatile int RxHead2,RxTail2;


void startmyINTs(void)
{
IEC0bits.IC1IE=1;
IEC0bits.IC3IE=1;
IEC1bits.AD1IE=1;
IEC0bits.U1EIE=1;
IEC0bits.U1RXIE=1;
IEC0bits.U1TXIE=1;
IEC1bits.U2EIE=1;
IEC1bits.U2RXIE=1;
IEC1bits.U2TXIE=1;
}

int testU1(void)
{
    int nchars;
    nchars=RxHead1-RxTail1;
    if (nchars<0) nchars+=RXBUFFERsize;
    return (nchars);
}
unsigned char readU1()
{
    unsigned char res;
    res=0;
    if (testU1())
    {
        res=RX1[RxTail1++];RxTail1&=(RXBUFFERsize-1);
    }
    return(res);
}

unsigned char peakU1()
{
    unsigned char res;
    res=0;
    if (testU1())
    {
        res=RX1[RxTail1];
    }
    return(res);
}

void flushU1(int n)
{
    int i;
    for (i=0;i<n;i++) readU1();
}

int testU2(void)
{
     int nchars;
    nchars=RxHead2=RxTail2;
    if (nchars<0) nchars+=RXBUFFERsize;
    return (nchars);
}
unsigned char readU2()
{
    unsigned char res;
    res=0;
    if (testU2())
    {
        res=RX2[RxTail2++];RxTail2&=(RXBUFFERsize-1);
    }
    return(res);
}
unsigned char peakU2()
{
    unsigned char res;
    res=0;
    if (testU2())
    {
        res=RX2[RxTail2];
    }
    return(res);
}


void doclosedloop(void)
{
 INT16 I16;
 INT16 derrorL,derrorR;
 if (DesiredL==0) 
	{
		speed2=0;
		erroriL=0;
	}
	else
	{
     
      if (countup2) I16=-CspeedB; else	I16=CspeedB;
    
//	  I16=(emf2v-emfoffset2);
      if (I16>8191) I16=8191;
      if (I16<-8191) I16=-8191;
      errorL=(DesiredL+I16);
      if (errorL>1000) errorL=1000; else if (errorL<-1000) errorL=-1000;
      derrorL=LasterrorL-errorL;
      LasterrorL=errorL;
	  erroriL+=errorL/50;
      if (erroriL>1000) erroriL=1000; else if (erroriL<-1000) erroriL=-1000;
      McmdL=(errorL/4)+erroriL-derrorL;
      if (McmdL>2047) speed2=2047; 
		else 
		{
	      if (McmdL<-2047) speed2=-2047; 
			else 
			{speed2=McmdL;
			}
		}
                               
	}
 if (DesiredR==0) 
	{
		speed1=0;
		erroriR=0;
	}
	else
	{
     if (countup1) I16=-CspeedA; else	I16=CspeedA;
//      I16=(emf1v-emfoffset1);
      if (I16>8191) I16=8191;
      if (I16<-8191) I16=-8191;
      errorR=(DesiredR+I16);
      if (errorR>1000) errorR=1000; else if (errorR<-1000) errorR=-1000;
      derrorR=LasterrorR-errorR;
      LasterrorR=errorR;
	  erroriR+=errorR/50;
      if (erroriR>1000) erroriR=1000; else if (erroriR<-1000) erroriR=-1000;
      McmdR=(errorR/4)+erroriR-derrorR;
      if (McmdR>2047) speed1=2047; 
		else 
		{
	      if (McmdR<-2047) speed1=-2047; 
			else 
			{speed1=McmdR;
			}
		}
	}
CLflag=0;
}



volatile unsigned char setLineLed=0;

void __attribute((interrupt(ipl3), vector(_ADC_VECTOR), nomips16)) _ADCInterrupt(void)
{
  int v;	
MotorPWMcount++;
if (MotorPWMcount>(511+(8*16))) 
	{
	if (speed1)
		{
		  if (speed1<0)
			{_LATD3=1;MotorleftV=(-speed1)>>2;
			}
			else
			{_LATD4=1;MotorleftV=(speed1)>>2;
			}
		}
	if (speed2)
		{
		  if (speed2<0)
			{_LATD1=1;MotorrightV=(-speed2)>>2;
			}
			else
			{_LATD2=1;MotorrightV=(speed2)>>2;
			}
		}
	MotorPWMcount=0;
	if (closedloop ) CLflag=1;
	}
	else
	{
	if (MotorleftV<=MotorPWMcount)
		{LATD&=~0x0018;
		}
	if (MotorrightV<=MotorPWMcount)
		{LATD&=~0x0006;
		}
	}
  
  if (ToggleLine)
	{
		if (MotorPWMcount>256) {if (setLineLed==1) setLineLed=2;} else {if (setLineLed==0) setLineLed=3;}
	}
  if (AD1CON2bits.BUFS)
		{
		   if (ToggleLine)
			{
			  if (setLineLed & 2)
			  {
				setLineLed&=1;
				if (setLineLed) 
					{LINEA=LINEAa-ADC1BUF0;LINEB=LINEBa-ADC1BUF1;LED2_IO=1;LED3_IO=1;
					}
					else
					{LINEAa=ADC1BUF0;LINEBa=ADC1BUF1;LED2_IO=0;LED3_IO=0;
					}
			  }
			}
			else
			{
			   LINEA=ADC1BUF0;
			   LINEB=ADC1BUF1;
			}
		   if (MotorPWMcount>(511+32)) {   
								emf2v+=(int)ADC1BUF2-(emf2v/16);   
								emf1v+=(int)ADC1BUF3-(emf1v/16); 
								if (setemfoff) 
									{
										setemfoff--;
										if (setemfoff==0) {emfoffset1=emf1v;emfoffset2=emf2v;}
									}
												
								v=emf1v-emfoffset1;
							    if (v>speedhyst) {//if (countup1) {lasticA0=lasticA1=0;dificA=0xffff;}
												  countup1=0;}
							    else if (v<-speedhyst) {//if (!countup1) {lasticA0=lasticA1=0;dificA=0xffff;}
												  countup1=1;}
                                if (timeoutA<0xff)
                                {
                                    timeoutA++;
                                }
                                else PeriodA=0x400000;
                           //     if ((TickGet()-HlasticA)>0x11000) PeriodA=0xfffff;
								v=emf2v-emfoffset2;
							    if (v>speedhyst) {//if (countup2) {lasticB0=lasticB1=0;dificB=0xffff;}
												  countup2=0;}
                                else
							    if (v<-speedhyst) {//if (!countup2) {lasticB0=lasticB1=0;dificB=0xffff;}
												  countup2=1;}
                                if (timeoutB<0xff)
                                {
                                    timeoutB++;
                                }
                                else PeriodB=0x400000;
                           //     if ((TickGet()-HlasticB)>0x11000) PeriodB=0xfffff;
						 }
		   photov=ADC1BUF4;
		   sounds[soundhead++]=photov>>2;soundhead&=2047;
		   thermv=ADC1BUF5;
		   lightav=ADC1BUF6;
		   lightbv=ADC1BUF7;
		}
		else
		{
		   if (ToggleLine)
			{
			  if (setLineLed & 2)
			  {
				setLineLed&=1;
				if (setLineLed) 
					{LINEA=LINEAa-ADC1BUF8;LINEB=LINEBa-ADC1BUF9;LED2_IO=1;LED3_IO=1;
					}
					else
					{LINEAa=ADC1BUF8;LINEBa=ADC1BUF9;LED2_IO=0;LED3_IO=0;
					}
			  }
			}
			else
			{
			   LINEA=ADC1BUF8;
			   LINEB=ADC1BUF9;
			}
		   if (MotorPWMcount>(511+32)) {   
							emf2v+=(int)ADC1BUFA-(emf2v/16);   	   
							emf1v+=(int)ADC1BUFB-(emf1v/16); 
								if (setemfoff) 
									{
										setemfoff--;
										if (setemfoff==0) {emfoffset1=emf1v;emfoffset2=emf2v;}
									}
								v=emf1v-emfoffset1;
							    if (v>speedhyst) {//if (countup1) {lasticA0=lasticA1=0;dificA=0xffff;}
												  countup1=0;}
							    if (v<-speedhyst) {//if (!countup1) {lasticA0=lasticA1=0;dificA=0xffff;}
												  countup1=1;}
								v=emf2v-emfoffset2;
							    if (v>speedhyst) {//if (countup2) {lasticB0=lasticB1=0;dificB=0xffff;}
												  countup2=0;}
							    if (v<-speedhyst) {//if (!countup2) {lasticB0=lasticB1=0;dificB=0xffff;}
												  countup2=1;}
						 }
		   photov=ADC1BUFC;
		   sounds[soundhead++]=photov>>2;soundhead&=2047;
		   thermv=ADC1BUFD;
		   lightav=ADC1BUFE;
		   lightbv=ADC1BUFF;
		}

IFS1bits.AD1IF=0;
}

int delta1=0;
int delta2=0;


void __attribute((interrupt(ipl4), vector(_INPUT_CAPTURE_1_VECTOR), nomips16)) _IC1Interrupt(void)
{
UINT16 vv;
UINT32 TD;
//if (countup1) pos1++; else pos1--;  // new - better distance
//if (speed1>=0) pos1++; else pos1--;
if (speed1>0) delta1=1;
if (speed1<0) delta1=-1;
pos1+=delta1;
lasticA=TickGet();
timeoutA=0;
		vv=IC1BUF & 0xffff;
		if (IC1F)
			{
				dificA1t=(UINT16)(vv-lasticAS1);
                TD=lasticA-HlasticA1;
                if (TD>0xf000) PeriodA+=(TD-(PeriodA/8));
                else 
                    PeriodA+=(dificA1t-(PeriodA/8));
                if (PeriodA>0x100000) PeriodA=0x100000;
                    lasticAS1=vv;
                    HlasticA1=lasticA;
				IC1F=0;
			}
			else
			{
				dificA0t=(UINT16)(vv-lasticAS0);
                TD=lasticA-HlasticA0;
                if (TD>0xf000) PeriodA+=(TD-(PeriodA/8));
                else 
                    PeriodA+=(dificA0t-(PeriodA/8));
                if (PeriodA>0x100000) PeriodA=0x100000;
                        lasticAS0=vv;
                        HlasticA0=lasticA;
				IC1F=1;
			}

        

if (!IC1CONbits.ICBNE) IFS0bits.IC1IF=0;
}

void __attribute((interrupt(ipl4), vector(_INPUT_CAPTURE_2_VECTOR), nomips16)) _IC2Interrupt(void)
{
IFS0bits.IC2IF=0;
}



void __attribute((interrupt(ipl4), vector(_INPUT_CAPTURE_3_VECTOR), nomips16)) _IC3Interrupt(void)
{
UINT16 vv;
UINT32 TD;
//if (countup2>0) pos2++; else pos2--;  // new - better distance
lasticB=TickGet();
//if (speed2>=0) pos2++; else pos2--;
if (speed2>0) delta2=1;
if (speed2<0) delta2=-1;
pos2+=delta2;
timeoutB=0;

		vv=IC3BUF & 0xffff;

		if (IC3F)
			{
				dificB1t=(UINT16)(vv-lasticBS1);
                TD=lasticB-HlasticB1;
                if (TD>0xf000) PeriodB+=(TD-(PeriodB/8));
                else 
                    PeriodB+=(dificB1t-(PeriodB/8));
                if (PeriodB>0x100000) PeriodB=0x100000;
                    lasticBS1=vv;
                    HlasticB1=lasticB;
				IC3F=0;
			}
			else
			{
				dificB0t=(UINT16)(vv-lasticBS0);
                TD=lasticB-HlasticB0;
                if (TD>0xf000) PeriodB+=(TD-(PeriodB/8));
                else 
                    PeriodB+=(dificB0t-(PeriodB/8));
                if (PeriodB>0x100000) PeriodB=0x100000;
                        lasticBS0=vv;
                        HlasticB0=lasticB;
				IC3F=1;
			}




if (!(IC3CONbits.ICBNE)) IFS0bits.IC3IF=0;
}

void __attribute((interrupt(ipl4), vector(_INPUT_CAPTURE_4_VECTOR), nomips16)) _IC4Interrupt(void)
{
IFS0bits.IC4IF=0;
}

#define comsdatasize 1024
char comsdata[comsdatasize];
unsigned int comsdatahead=0;
unsigned int comsdatatail=0;
void putcomsdata(char v)
{
  unsigned int nhead;
  nhead=comsdatahead+1;
  if (nhead>=comsdatasize) nhead-=comsdatasize;
  if (nhead!=comsdatatail)
	{
       comsdatahead=nhead;
       comsdata[comsdatahead]=v;
	}
}

char yescomsdata(void)
{
  if (comsdatatail!=comsdatahead) return(1); else return(0);
}

char getcomsdata(void)
{
  if (comsdatatail==comsdatahead) return(0);
  comsdatatail++;
  if (comsdatatail>=comsdatasize) comsdatatail-=comsdatasize;
  return(comsdata[comsdatatail]);
}

char peekcomsdata(void)
{
  int i;
  if (comsdatatail==comsdatahead) return(0);
  i=comsdatatail+1;
  if (i>=comsdatasize) i-=comsdatasize;
  return(comsdata[i]);
}

UINT16  globalRSSI=255;
void POSTTCPhead(int len,int id)
{
   POSTTCPchar(0xff);POSTTCPchar(len+2);POSTTCPchar(globalRSSI);POSTTCPchar(id);
}
int commandstate=-1;
int commandlen;
unsigned char nextcommand[256];
UINT16 REQCOUNT=0;
int waveid=0;
  int res;

void initi2Cs(void)  // called once when rover starts
{
	//setup accn MMA8452Q
										I2S();
I2send(0x38);
I2send(0x0d);
I2SR();
I2send(0x39);
			  res=I2GET(0);			  I2P();
										I2S();I2send(0x38);I2send(0x2a);I2send(0x20); I2send(0x2);I2P();//50Hz nonactive 
										I2S();I2send(0x38);I2send(0x2a);I2send(0x21);I2P();//50Hz active  

	//setup gyro L3G4200D  
										I2S();I2send(0xd2);I2send(0x0f);I2SR();I2send(0xd3);			  res=I2GET(0);			  I2P();
										I2S();
I2send(0xd2);
I2send(0x20|0x80);
I2send(0x3f);  // 100 sps 25hz cutoff
I2send(0x06); //HP 0.1Hz
I2send(0x00);
I2send(0x11);  // 500 degrees/S full scale  
I2send(0x10);  // HP enable
I2P();

//setup mag MAG3110FCR1 
										I2S();I2send(0x1c);I2send(0x7);
								I2SR();I2send(0x1d);
			  					res=I2GET(0);
			  					I2P();
		if (res==0xc4) 
   {
										I2S();I2send(0x1c);I2send(0x10);I2send(0x09); I2send(0x30);I2P();  //40Hz
  do
  {
										I2S();I2send(0x1c);I2send(0x11);
								I2SR();I2send(0x1d);
			  					res=I2GET(0);
			  					I2P();
  } while (res&0x10);
  
   }
}


void setspeed(int newspeed1,int newspeed2)  // routine sets speed of motors 
											// only alters motors if speed has changed
{
  if (newspeed1!=speed1)
	{
			speed1=newspeed1;
		//	setpwmA();
	}
  if (newspeed2!=speed2)
	{
			speed2=newspeed2;
		//	 setpwmB();
	}
  erroriL=0;erroriR=0;
}

int bptag=0;



void processcommand(void)		// the main routine which processes commands
{
 int i,j;
 unsigned char temparray[10];
 unsigned char Blow,Bhigh;

  int temphead;


  switch (nextcommand[0]) // sort on command id (each case is for a different command)
  {
    case CMDNULL:POSTTCPhead(0,CMDNULL);break;
	case CMDGETSW:POSTTCPhead(1,CMDGETSW);    //Ask for Led and Switch status Packet
			i=((~PORTE<<2) & 0xc)| ((LATE<<2) & 0x30);
			if ((PORTD & 0x40)==0) i|=0x2;
			if ((PORTD & 0x80)==0) i|=0x1;
			POSTTCPchar(i);
			break;
	case CMDSETLED:if (commandlen==1)		//Set LEDs
			{
			  if (nextcommand[1] & 2) LED1_IO=0; else LED1_IO=1;
			  if (nextcommand[1] & 1) LED0_IO=0; else LED0_IO=1;
			  POSTTCPhead(0,CMDSETLED);break;
			}
			break;
	case CMDGETCOUNT:POSTTCPhead(2,CMDGETCOUNT);  // get internal counter
			POSTTCPchar((REQCOUNT>>8) & 0xff);POSTTCPchar(REQCOUNT & 0xff);
			REQCOUNT++;
			break;
 	case CMDGETversion:POSTTCPhead(2,CMDGETversion);  // get version
			POSTTCPchar(versionhigh);POSTTCPchar(versionlow);
			break;
	case CMDMOTORSTATUS:  // request motor status here
			 POSTTCPhead(9,CMDMOTORSTATUS);
			  POSTTCPchar(speed1>>4);
			  POSTTCPchar(pos1>>8);
			  POSTTCPchar(pos1);

			  POSTTCPchar(speed2>>4);
			  POSTTCPchar(pos2>>8);
			  POSTTCPchar(pos2);
			  POSTTCPchar(0);
			  POSTTCPchar(0);
			  POSTTCPchar(0);
			break;
                        


	case CMDSETMOTOR1:if (commandlen==1)	// set motor1
			{
              i=(char)nextcommand[1];closedloop=0;
			  setspeed(i*16,speed2);
			 POSTTCPhead(0,CMDSETMOTOR1);
			}
			break;
	case CMDSETMOTOR2:if (commandlen==1)	// set motor2
			{
			 i=(char)nextcommand[1];closedloop=0;
			  setspeed(speed1,i*16);
			 POSTTCPhead(0,CMDSETMOTOR2);
			}
			break;
	case CMDSETBOTHMOTORS:if (commandlen==2)	// set both motors
			{
				i=(char)nextcommand[1];
				j=(char)nextcommand[2];closedloop=0;
			  setspeed(i*16,j*16);
			 POSTTCPhead(0,CMDSETBOTHMOTORS);
			}
			break;


	case CMDGETLINEFOLLOW:	// report line following
			POSTTCPhead(5,CMDGETLINEFOLLOW);
			i=0;
			if (LINEA<LINETa) i|=1;
			if (LINEB<LINETb) i|=2;
			POSTTCPchar(i);
			i=LINEA;
			POSTTCPchar(i>>8);POSTTCPchar(i);
			i=LINEB;
			POSTTCPchar(i>>8);POSTTCPchar(i);
			break;
	case CMDSETLINEFOLLOW:	if (commandlen==5)	// set line following thrsholds and LEDS
            {
			if (nextcommand[1]& 1) LED2_IO=1; else LED2_IO=0;
			if (nextcommand[1]& 2) LED3_IO=1; else LED3_IO=0;
			i=nextcommand[3]|(nextcommand[2]<<8);
			LINETa=i;
			i=nextcommand[5]|(nextcommand[4]<<8);
			LINETb=i;
			POSTTCPhead(0,CMDSETLINEFOLLOW);
            }
			break;

	case CMDGETLIGHTTEMP:
			 if (commandlen==0)	//send light/aux
			 {
			  POSTTCPhead(4,CMDGETLIGHTTEMP);
				i=photov;
			  POSTTCPchar(i>>8);POSTTCPchar(i);
				i=thermv;
			  POSTTCPchar(i>>8);POSTTCPchar(i);

			 } 
			break;

	case CMDPOWERSWITCH:
			 if (commandlen==1)	//POWER SWITCH
			 {
			  POSTTCPhead(0,CMDPOWERSWITCH);
			  I2S();I2send(0x14);I2send(0); I2send(nextcommand[1]);
			  I2P();
			 } 
			break;
	case CMDPOWERSERV01:
			 if (commandlen==1)	//POWER SERV0 1
			 {
			  POSTTCPhead(0,CMDPOWERSERV01);
			  I2S();I2send(0x14);I2send(1); I2send(nextcommand[1]);
			  I2P();
			 } 
			break;
	case CMDPOWERSERV02:
			 if (commandlen==1)	//POWER SERV0 2
			 {
			  POSTTCPhead(0,CMDPOWERSERV02);
			  I2S();I2send(0x14);I2send(2); I2send(nextcommand[1]);
			  I2P();
			 } 
			break;


	case CMDPOWERREADAD0:
			 if (commandlen==0)	//POWER READ AD0
			 {
			  POSTTCPhead(2,CMDPOWERREADAD0);
			  I2S();I2send(0x14);I2send(8);I2SR();I2send(0x15);
                          Blow=I2GET(1);Bhigh=I2GET(0);
			  I2P();
                          POSTTCPchar(Bhigh);
                          POSTTCPchar(Blow);
			 } 
			break;
	case CMDPOWERREADAD1:
			 if (commandlen==0)	//POWER READ AD1
			 {
			  POSTTCPhead(2,CMDPOWERREADAD1);
			  I2S();I2send(0x14);I2send(10);I2SR();I2send(0x15);
			  Blow=I2GET(1);Bhigh=I2GET(0);
			  I2P();
                          POSTTCPchar(Bhigh);
                          POSTTCPchar(Blow);
			 } 
			break;
	case CMDPOWERREADphotodiode:
			 if (commandlen==0)	//POWER READ AD6  - photo diode
			 {
			  POSTTCPhead(2,CMDPOWERREADphotodiode);
			  I2S();I2send(0x14);I2send(12);I2SR();I2send(0x15);
			  Blow=I2GET(1);Bhigh=I2GET(0);
			  I2P();
                          POSTTCPchar(Bhigh);
                          POSTTCPchar(Blow);
			 } 
			break;
	case CMDPOWERREADthermistor:
			 if (commandlen==0)	//POWER READ AD7  - thermistor
			 {
			  POSTTCPhead(2,CMDPOWERREADthermistor);
			  I2S();I2send(0x14);I2send(14);I2SR();I2send(0x15);
			  Blow=I2GET(1);Bhigh=I2GET(0);
			  I2P();
                          POSTTCPchar(Bhigh);
                          POSTTCPchar(Blow);
			 } 
			break;



	case CMDREADemfs:  // back emfs
			 if (commandlen==0)	
			 {
			  POSTTCPhead(4,CMDREADemfs);
				i=emfoffset1-emf1v;
			  POSTTCPchar(i>>8);POSTTCPchar(i);
				i=emfoffset2-emf2v;
			  POSTTCPchar(i>>8);POSTTCPchar(i);

			 } 
			break;
	case CMDRDlightsensors:  // the two integrated light sensors
			 if (commandlen==0)	
			 {
			  POSTTCPhead(4,CMDRDlightsensors);
				i=lightav;
			  POSTTCPchar(i>>8);POSTTCPchar(i);
				i=lightbv;
			  POSTTCPchar(i>>8);POSTTCPchar(i);

			 } 
			break;
	case CMDMyMAC:      // my mac address
			 if (commandlen==0)	
			 {
			  POSTTCPhead(6,CMDMyMAC);
			  for (i=0;i<6;i++)  POSTTCPchar(AppConfig.MyMACAddr.v[i]);
			 } 
			break;
	case CMDRDMMA8452:
			 if (commandlen==0)	//send accn MMA8452Q
			 {
				I2S();I2send(0x38);I2send(0x00);I2SR();I2send(0x39);
		    	i=I2GET(0);I2P();
			    if (i&8)
					{
					POSTTCPhead(6,CMDRDMMA8452);
					I2S();I2send(0x38);I2send(0x01);I2SR();I2send(0x39);
					for (i=1;i<=6;i++)
						{POSTTCPchar(I2GET(i!=6));
						}
				    I2P();
					}
					else POSTTCPhead(0,CMDRDMMA8452);
			 } 
			break;

	case CMDRDgyroSTATUS:
			 if (commandlen==0)	//send gyro L3G4200D status
			 {
			    POSTTCPhead(2,CMDRDgyroSTATUS);
					I2S();I2send(0xd2);I2send(0x26|0x80);I2SR();I2send(0xd3);
					for (i=1;i<=2;i++)
						{POSTTCPchar(I2GET(i!=2));
						}
				    I2P();
			 } 
			break;
	case CMDRDgyro:
			 if (commandlen==0)	//send gyro L3G4200D
			 {
				I2S();I2send(0xd2);I2send(0x27);I2SR();I2send(0xd3);
				if ((I2GET(0) & 0x8)==0)
					{
                                        I2P();
                                        POSTTCPhead(0,CMDRDgyro);
					}
					else
					{
                                        I2P();
                                         POSTTCPhead(8,CMDRDgyro);
					I2S();I2send(0xd2);I2send(0x26|0x80);I2SR();I2send(0xd3);
                                        POSTTCPchar(I2GET(1));POSTTCPchar(I2GET(1));
                                        for (i=0;i<6;i++) temparray[i]=I2GET(i!=5);
                                        I2P();
                                        POSTTCPchar(temparray[1]);POSTTCPchar(temparray[0]);
                                        POSTTCPchar(temparray[3]);POSTTCPchar(temparray[2]);
                                        POSTTCPchar(temparray[5]);POSTTCPchar(temparray[4]);
					}
			 } 
			break;
	case CMDMagSTATUS:
			 if (commandlen==0)	//send mag MAG3110FCR1  status
			 {
			    POSTTCPhead(2,CMDMagSTATUS);
					I2S();I2send(0x1c);I2send(0);I2SR();I2send(0x1d);
					POSTTCPchar(I2GET(0));
				    I2P();
					I2S();I2send(0x1c);I2send(0xf);I2SR();I2send(0x1d);
					POSTTCPchar(I2GET(0));
				    I2P();
			 } 
			break;
	case CMDMag:
			 if (commandlen==0)	//send mag MAG3110FCR1  x,y,z raw
			 {
					I2S();I2send(0x1c);I2send(0);I2SR();I2send(0x1d);
					if ((I2GET(0) & 0x8)==0)
						{
                                                POSTTCPhead(0,CMDMag);
                                                I2P();
						}
						else
						{
                                                I2P();
                                                POSTTCPhead(6,CMDMag);
                                                I2S();I2send(0x1c);I2send(1);I2SR();I2send(0x1d);
                                                for (i=1;i<=6;i++)
                                                        {POSTTCPchar(I2GET(i!=6));
                                                        }
                                                I2P();
						}
			 } 
			break;
	case CMDdoSTEP:
			 if (commandlen==2)	//do Step
			 {
			  I2S();I2send(0x12);I2send(4);I2send(nextcommand[1]);I2send(nextcommand[2]);I2P();
			  I2S();I2send(0x12);I2send(6);I2send(1);I2P();
			  POSTTCPhead(0,CMDdoSTEP);
			 } 
			break;
	case CMDStepStatus:
			 if (commandlen==0)	//Step status
			 {
			  POSTTCPhead(1,CMDStepStatus);
			  I2S();I2send(0x12);I2send(6);I2SR();I2send(0x13);POSTTCPchar(I2GET(0));I2P();
			 } 
			break;
	case CMDStepRead:
			 if (commandlen==0)	//step data read
			 {
			  POSTTCPhead(128,CMDStepRead);
			  I2S();I2send(0x12);I2send(128);I2SR();I2send(0x13);
					for (i=1;i<=128;i++)
						{POSTTCPchar(I2GET(i!=128));
						}
				    I2P();
			 } 
			break;
	case CMDstepInstRead:
			 if (commandlen==0)	//step inst read
			 {
			  POSTTCPhead(2,CMDstepInstRead);
			  I2S();I2send(0x12);I2send(8);I2SR();I2send(0x13);
					for (i=1;i<=2;i++)
						{POSTTCPchar(I2GET(i!=2));
						}
				    I2P();
			 } 
			break;
	case CMDreadSounds:
			 if (commandlen==0)
			 {
			  temphead=soundhead;
			  temphead=(temphead-soundtail)& 2047;
			  if (temphead>128) temphead=128;
			  POSTTCPhead(temphead,CMDreadSounds);
					for (i=1;i<=temphead;i++)
						{POSTTCPchar(sounds[soundtail++]);soundtail&=2047;
						};
			 } 
			break;
	case CMDGetMotorSpeedWord:  // set speed word ... return pos and speeds
		if (commandlen==4)	// set both motors
					{
						i=(char)(nextcommand[1]);
						j=(char)(nextcommand[3]) ;
						i=(i<<8) | nextcommand[2];
						j=(j<<8) | nextcommand[4];closedloop=0;
						setspeed(j,i);
					}
					 POSTTCPhead(12,CMDGetMotorSpeedWord);
			  POSTTCPchar(pos1>>8);
                          POSTTCPchar(pos1);
                          POSTTCPchar(pos2>>8);
			  POSTTCPchar(pos2);
			  

				if (countup1) i=CspeedA; else	i=-CspeedA;
				POSTTCPchar(i>>8);POSTTCPchar(i);
				if (countup2) i=CspeedB; else	i=-CspeedB;
				POSTTCPchar(i>>8);POSTTCPchar(i);
				i=emfoffset1-emf1v;
                                POSTTCPchar(i>>8);POSTTCPchar(i);
				i=emfoffset2-emf2v;
                                POSTTCPchar(i>>8);POSTTCPchar(i);
			break;
	case CMDMotorSpeedClosed:  // set speed accurate
		if (commandlen==5)	// set both motors
                        {
                                i=(char)(nextcommand[1]);
                                j=(char)(nextcommand[3]) ;
                                i=(i<<8) | nextcommand[2];
                                j=(j<<8) | nextcommand[4];
                                closedloop=nextcommand[5];
                                if (!closedloop)
                                        {
                                        DesiredL=0;
                                        DesiredR=0;
                                        erroriL=0;erroriR=0;
                                        speed1=0;speed2=0;
                                        }
                                        else
                                        {
                                        DesiredL=j;
                                        DesiredR=i;
                                        }
                        }
                         POSTTCPhead(0,CMDMotorSpeedClosed);
			break;
/*
      case CMDGETstarpos:
			 if (commandlen==0)
			 {
			  POSTTCPhead(8,CMDGETstarpos);
			  for (i=0;i<6;i++)  POSTTCPchar(GPSRX[i]);
			  POSTTCPchar(GPSRXcount>>8);POSTTCPchar(GPSRXcount);
			 }
			break;
*/
      case CMDGETusgps:
			 if (commandlen==0)
			 {
              while (testU1()>=24) flushU1(12);
              while ((testU1()>=12) && (peakU1()!=0xa6)) readU1();   
              if ((testU1()>=12) && (peakU1()==0xa6))
              {
			  POSTTCPhead(8,CMDGETusgps);
			  for (i=0;i<4;i++)  
              {
                  readU1();
                  POSTTCPchar(readU1());POSTTCPchar(readU1());
              }
              }
              else POSTTCPhead(0,CMDGETusgps);
                  
			 }
			break;
  }
}

//Process packets
void ProcessIO(void)
{

			  if (PeriodA>512) CspeedA=0x8000000/PeriodA;
			  if (PeriodB>512) CspeedB=0x8000000/PeriodB;
// if ((TickGet()-lasticA)>(TICK_SECOND/10))
  //   if (PeriodA<0x2000000) PeriodA*=2;
 //if ((TickGet()-lasticB)>(TICK_SECOND/10))
   //  if (PeriodB<0x2000000) PeriodB*=2;
 if (CLflag) doclosedloop();
 if (yescomsdata())
 {
  if (commandstate<0)
   {
     if ((unsigned char)getcomsdata()==0xff)
       {
         commandstate=0;
         if (yescomsdata())
			{
               commandstate=(unsigned char)getcomsdata();
			   commandlen=commandstate-1;
			   if (commandstate==0) commandstate=-1;
			}
	   }
   }
   else
   {
     if (commandstate==0)
		{
               commandstate=(unsigned char)getcomsdata();
			   commandlen=commandstate-1;
			   if (commandstate==0) commandstate=-1;
		}
		else
		{
			while (yescomsdata() && (commandstate))
				{
				  nextcommand[1+(commandlen-commandstate)]=getcomsdata();
				  commandstate--;
				}
			if (commandstate==0)
				{
					
					processcommand();
					commandstate=-1;
				}
		}
   }

 }

}


void uartsetup(void)
{
RxHead1=RxTail1=0;
RxHead2=RxTail2=0;

	U1MODECLR = 0x0307;
	U2MODECLR = 0x0307;

	//set BRGH to high speed mode
	U1MODESET = 0x0008;
	U2MODESET = 0x0008;

	//set baud rate
	U1BRG = (GetPeripheralClock()/(57600*4))-1;  	//57600
	U2BRG = (GetPeripheralClock()/(57600*4))-1;  	//57600
	U1STA =  0x0400;
	U2STA = 0x0400;

	//set the URXEN
	U1STASET = 0x1000;
	U2STASET = 0x1000;


	U1STACLR = 0xC000;
	U1STACLR = 0x00C0;
        IFS0bits.U1EIF=0;
        IFS0bits.U1RXIF=0;
        IFS0bits.U1TXIF=0;
        IFS1bits.U2EIF=0;
        IFS1bits.U2RXIF=0;
        IFS1bits.U2TXIF=0;

IPC6bits.U1IS=0;
IPC6bits.U1IP=1;

IPC8bits.U2IS=0;
IPC8bits.U2IP=1;
	//enable uart ON bit
	U1MODESET = 0x8000;
	U2MODESET = 0x8000;

}


void __attribute((interrupt(ipl2), vector(_UART_1_VECTOR ), nomips16)) _U1Interrupt(void)
{
  if (IFS0bits.U1TXIF)
	{
            IFS0bits.U1TXIF=0;
	}
  if (IFS0bits.U1RXIF)
	{
		while (U1STAbits.URXDA)
		{
			RX1[RxHead1] = U1RXREG;
			RxHead1 = (RxHead1 + 1) & (RXBUFFERsize - 1);
		}
        IFS0bits.U1RXIF=0;
	}
  if (IFS0bits.U1EIF)
	{
		if ( U1STAbits.OERR )
		{
			U1STAbits.OERR = 0; // any overrun errors clear
		}
      IFS0bits.U1EIF=0;
	}
}

void __attribute((interrupt(ipl2), vector(_UART_2_VECTOR ), nomips16)) _U2Interrupt(void)
{
  if (IFS1bits.U2TXIF)
	{
            IFS1bits.U2TXIF=0;
	}
  if (IFS1bits.U2RXIF)
	{
		while (U2STAbits.URXDA)
		{
			RX2[RxHead2] = U2RXREG;
			RxHead2 = (RxHead2 + 1) & (RXBUFFERsize - 1);
		}
        IFS1bits.U2RXIF=0;
	}
  if (IFS1bits.U2EIF)
	{
		if ( U2STAbits.OERR )
		{
			U2STAbits.OERR = 0; // any overrun errors clear
		}
      IFS1bits.U2EIF=0;
	}
}

/****************************************************************************
  Function:
    static void InitializeBoard(void)

  Description:
    This routine initializes the hardware.  
  Precondition:
    None

  Parameters:
    None - None

  Returns:
    None

  Remarks:
    None
  ***************************************************************************/
void InitializeBoard(void)
{
    UINT32              actualClock;
	
  // init ports for analog  AN2 to AN3 are analog
AD1PCFG=0xffff3c33;
	IR_TRIS=1;
//	lastIR=IR_IO;
	IC1_TRIS=1;
	IC2_TRIS=1;
	IC3_TRIS=1;
	IC4_TRIS=1;
TRISDbits.TRISD6=1;
TRISDbits.TRISD7=1;
	// LEDs
LED0_IO=1;
LED1_IO=1;
	LED0_TRIS = 0;
	LED1_TRIS = 0;

	LED2_TRIS = 0;	// used for line following
	LED3_TRIS = 0;
LED2_IO=1;
LED3_IO=1;
		// Enable multi-vectored interrupts
		INTEnableSystemMultiVectoredInt();
		
		// Enable optimal performance
		SYSTEMConfigPerformance(GetSystemClock());
		mOSCSetPBDIV(OSC_PB_DIV_1);				// Use 1:1 CPU Core:Peripheral clocks
		
		// Disable JTAG port so we get our I/O pins back, but first
		// wait 50ms so if you want to reprogram the part with 
		// JTAG, you'll still have a tiny window before JTAG goes away.
		// The PIC32 Starter Kit debuggers use JTAG and therefore must not 
		// disable JTAG.
		DelayMs(50);
		#if !defined(__MPLAB_DEBUGGER_PIC32MXSK) && !defined(__MPLAB_DEBUGGER_FS2)
			DDPCONbits.JTAGEN = 0;
		#endif
		

		T3CON=0;
		PR3=255;
        T3CON=0x8000;
		    AD1CHS = 2;					

	// ADC
	AD1CON1 = 0x0444;			// Turn on, auto sample start, convert on t3, 
	AD1CON2 = 0x041e;			// AVdd, AVss, int every 8 conversions, MUXA only, scan
	AD1CON3 = 0x1003;			// 16 Tad auto-sample, Tad = 3*Tcy
		AD1CSSL = 0xc3cc;				// Scan a/ds
	AD1CON1 = 0x8444;			// Turn on, auto sample start, convert on t3, 


// Deassert all chip select lines so there isn't any problem with 
// initialization order.  Ex: When ENC28J60 is on SPI2 with Explorer 16, 
// MAX3232 ROUT2 pin will drive RF12/U2CTS ENC28J60 CS line asserted, 
// preventing proper 25LC256 EEPROM operation.
#if defined(WF_CS_TRIS)
	WF_CS_IO = 1;
	WF_CS_TRIS = 0;
#endif

    
	LeftMotorF=0;
	LeftMotorF_TRIS=0;
	LeftMotorB=0;
	LeftMotorB_TRIS=0;
	RightMotorF=0;
	RightMotorF_TRIS=0;
	RightMotorB=0;
	RightMotorB_TRIS=0;
	OC3CON=0;OC4CON=0;OC3R=OC3RS=0;OC4R=OC4RS=0;
	OC2CON=0;OC5CON=0;OC2R=OC2RS=0;OC5R=OC5RS=0;

		T2CON=0;
        T2CON=0x8070;

	IC1CON=0x0081;   
	IC1CON=0x8081;
	IC2CON=0x0081;
	IC2CON=0x8081;
	IC3CON=0x0081;
	IC3CON=0x8081;
	IC4CON=0x0081;
	IC4CON=0x8081;
IFS0bits.IC1IF=0;
IFS0bits.IC2IF=0;
IFS0bits.IC3IF=0;
IFS0bits.IC4IF=0;

IPC1bits.IC1IS=0;
IPC1bits.IC1IP=3;

IPC3bits.IC3IS=0;
IPC3bits.IC3IP=3;

UARTTX1_TRIS=0;
UARTRX1_TRIS=1;
UARTTX2_TRIS=0;
UARTRX2_TRIS=1;



IFS1bits.AD1IF=0;
IPC6bits.AD1IS=0;
IPC6bits.AD1IP=2;




    actualClock = I2CSetFrequency(I2C1, GetPeripheralClock(), 100000);
    // Enable the I2C bus
    I2CEnable(I2C1, TRUE);


}

