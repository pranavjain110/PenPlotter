#include <msp430.h> 
#include<math.h>
#include "initialize.h"
#define sizeOfBuffer 280
#define sizeOfBufferMotor 40

enum CommandType {StraightLine = 1, ClockWise = 2, AntiClockWise = 3, Rapid = 4};
enum MachineStatus {Idle = 1, Buzy = 2};
char byteRemoved[]=" \n\r";
volatile unsigned char RxByte;
volatile unsigned int  bufferLength = 0,readIndex = 0, writeIndex = 0,flagByteRx = 0,escByte, discretizeFlag=0,
        stepperState, steps=0, stepsDesired=0, flagACW = 0, flagCW = 1, moveStepperFlag = 0,state = 6,
        buffer[sizeOfBuffer],bufferDC[sizeOfBufferMotor],bufferStepper[sizeOfBufferMotor],machineFlag = Idle, bufferLengthMotor = 0, readIndexMotor=0, writeIndexMotor = 0,
        discretizedPointX,discretizedPointY;

unsigned int i,j,k, command = 0;
int positionCurrent , posDesiredDC=0  , error = 0, controllerOutput = 0,
        dataByte1, dataByte2, dataByte3, dataByte4, endPosX, endPosY,centerX, centerY, startPosX, startPosY,
        prevDataByte1 = 0, prevDataByte2 = 0 ,diffDataByte1, diffDataByte2, n;


unsigned const int stepperStateTable[] =  { 1, 0, 0, 0,  //State 1
                                            1, 0, 1, 0,  //State 2
                                            0, 0, 1, 0,  //State 3
                                            0, 1, 1, 0,  //State 4
                                            0, 1, 0, 0,  //State 5
                                            0, 1, 0, 1,  //State 6
                                            0, 0, 0, 1,  //State 7
                                            1, 0, 0, 1 }; //State 8

double ans,thetaStart,thetaEnd;

//variables for debugging and testing code
double testVar ;


void  motorControlLaw_DC(void)
{
    positionCurrent = TA1R-TB2R;
    if(positionCurrent < posDesiredDC)
    {
        error = posDesiredDC-positionCurrent;
        if(error>1)
        {
            P3OUT |= BIT6;
            P3OUT &= ~BIT7;
            TB1CCR2 = 0xFFFF;
        }
    }
    else
    {
        error=positionCurrent-posDesiredDC;
        if(error>1)
        {
            P3OUT &= ~BIT6;
            P3OUT |= BIT7;
            TB1CCR2 = 0xFFFF;
        }
    }

    MPYS = 2500;
    OP2 = error;
    controllerOutput = RES0;

    if(error>25)
        TB1CCR2 = 0;
    else
        TB1CCR2 = 0xFFFF - controllerOutput;
}

double findAngle(double diffY, double diffX)
{
    double angle;
    angle = atan2(diffY,diffX);
    if(angle<0)
        angle+=710.0/113.0;
    return angle;
}

int totalPoints(void)
{
    if(dataByte1 > prevDataByte1)
        diffDataByte1 = dataByte1 - prevDataByte1;
    else
        diffDataByte1 = prevDataByte1 - dataByte1;

    if(dataByte2 > prevDataByte2)
        diffDataByte2 = dataByte2 - prevDataByte2;
    else
        diffDataByte2 = prevDataByte2 - dataByte2;

    prevDataByte1 = dataByte1;
    prevDataByte2 = dataByte2;

    if(diffDataByte1>diffDataByte2)
    {
        return diffDataByte1;
    }
    else
        return diffDataByte2;

}

/**
 * main.c
 */
int main(void)
{

    WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer

    initializeClock();
    initializeUART(); // Use TImerB0 (TB0.2) for regularly sending data
    stepperState = initializeStepper(); //Use TimerA0 (TA0.1) to move Stepper Motor
    initializeEncoder(); // Initialize TimerA1 and TimerB2 to read encoder pulses (via D-flip flop)
    initializeDCMotor(); //Use TImerB1 (TB1.2) to generate PWM
    initializeSolenoid();



    _EINT(); //Global Interrupt Enable


    while(1)
    {
        motorControlLaw_DC();

        if(flagByteRx == 1 && discretizeFlag == 0)// && machineFlag == Idle)
        {
            byteRemoved[0] = buffer[readIndex];
            readIndex++;
            bufferLength--;
            if(readIndex == sizeOfBuffer)
                readIndex=0;

            if(bufferLength == 0)
                flagByteRx = 0;

            // Move to a state based on the byte read
            if(byteRemoved[0] == 255)
                state = 0;
            else
                switch(state)
                {
                case 0:  //Differentiate between position and speed data
                    switch(byteRemoved[0])
                    {
                        case StraightLine:  //Position Related Data
                            command = StraightLine;
                            state = 1;
                            break;
                        case ClockWise: //Speed Related Data
                            state = 1;
                            command = ClockWise;
                            break;
                        case AntiClockWise:
                            state = 1;
                            command = AntiClockWise;
                            break;
                        case Rapid:
                            state = 1;
                            command = Rapid;
                            break;
                        default:
                            state = 6; //Undefined state
                            break;
                    }
                    break;
                case 1:
                    dataByte1 = byteRemoved[0];
                    state = 2;
                    break;
                case 2:
                    dataByte2 = byteRemoved[0];
                    state = 3;
                    break;

                case 3:
                    dataByte3 = byteRemoved[0];
                    state = 4;
                    break;

                case 4:
                    dataByte4 = byteRemoved[0];
                    state = 5;
                    break;

                case 5:
                    state = 6; //This is an undefined state
                    escByte = byteRemoved[0];


                    if (escByte >=8)
                    {
                        dataByte1 = 255;
                        escByte -= 8;
                    }
                    if (escByte >=4)
                    {
                        dataByte2 = 255;
                        escByte -= 4;
                    }
                    if (escByte >= 2)
                    {
                        dataByte3 = 255;
                        escByte -= 2;
                    }
                    if (escByte >= 1)
                    {
                        dataByte4 = 255;
                        escByte -= 1;
                    }
                    if(escByte!=0)
                    {
                        //Program should not reach here
                        __no_operation();                       // For debug only
                    }

                    machineFlag = Buzy;

                    switch(command)
                    {
                        case StraightLine:  //Position Related Data
                        case Rapid:  //Position Related Data
                            endPosX = dataByte1 * 105;
                            endPosY = dataByte2 * 3;

                            n = totalPoints();
                            break;

                        case ClockWise:  //Position Related Data
                        case AntiClockWise:  //Position Related Data
                            endPosX = dataByte1 * 105;
                            endPosY = dataByte2 * 3;
                            centerX = dataByte3 * 105;
                            centerY = dataByte4 * 3;

                            thetaStart =findAngle(prevDataByte2 - dataByte4,prevDataByte1 - dataByte3);
                            thetaEnd = findAngle(prevDataByte2 - dataByte4,prevDataByte1 - dataByte3);

                            break;
                        default:
                            break;
                    }

                    k=1;
                    prevDataByte1 = dataByte1;
                    prevDataByte2 = dataByte2;
                    discretizeFlag=1;
                    break;
                }

        }

        if(machineFlag == Buzy && discretizeFlag == 1 && bufferLengthMotor < sizeOfBufferMotor)
        {
            k++;
            if(command == StraightLine || command == Rapid)
            {
                discretizedPointX = startPosX + (endPosX - startPosX)*(k*1.0/n) ;
                discretizedPointY = startPosY + (endPosY - startPosY)*(k*1.0/n) ;
            }


            bufferDC[writeIndexMotor] = discretizedPointY;
            bufferStepper[writeIndexMotor] = discretizedPointX;
            writeIndexMotor++;
            bufferLengthMotor++;
            if(writeIndexMotor == sizeOfBufferMotor)
                writeIndexMotor = 0;

            if(k==n)
            {
                discretizeFlag=0;
                startPosX = endPosX;
                startPosY = endPosY;
            }

        }

//        testVar = findAngle(-10,10);

        for (i=0;i<20000;i++);
    }
    return 0;
}


// ISR
#pragma vector = USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
{
    RxByte = UCA0RXBUF; //store byte in RX buffer in a variable RxByte
    if (bufferLength <= sizeOfBuffer)
    {
        if(bufferLength == sizeOfBuffer)
        {
            __no_operation; //Code should not be reaching here
        }
        else
        {
            buffer[writeIndex]=RxByte;
            writeIndex++;
            bufferLength++;
            if(writeIndex == sizeOfBuffer)
                writeIndex = 0;
        }
    }
    flagByteRx = 1;
}

#pragma vector = TIMER0_A1_VECTOR
__interrupt void Timer_A0 (void)
{
    if(stepsDesired > steps)
    {
        flagCW = 1;
        moveStepperFlag = 1;
    }
    else if(stepsDesired < steps)
    {
        moveStepperFlag = 1;
        flagACW = 1;
    }
    else if(stepsDesired == steps)
    {
        moveStepperFlag = 0;
        flagACW = 0;
        __no_operation();
        TA0CCTL1 &= ~CCIE; // TBCCR0 interrupt disabled
    }

    if(moveStepperFlag == 1)
    {
      switch( TA0IV )
      {
          case 2:
              if(flagACW == 1){
                  stepperState--;
                  steps--;
              }
              else{
                  stepperState++;
                  steps++;
              }
              if(stepperState == 9)
                  stepperState=1;
              if(stepperState == 0)
                  stepperState=8;

              j= (stepperState - 1) * 4; //Used to set indexes from the lookup table

              if(BIT3 & stepperStateTable[j + 0]<<3)
                  P1OUT |= BIT3;
              else
                  P1OUT &= ~BIT3;

              if(BIT4 & stepperStateTable[j + 2]<<4 )
                  P1OUT |= BIT4;
              else
                  P1OUT &= ~BIT4;

              if( BIT5 & stepperStateTable[j + 1]<<5 )
                  P1OUT |= BIT5;
              else
                  P1OUT &= ~BIT5 ;

              if(BIT6 & stepperStateTable[j + 3]<<6 )
                  P1OUT |= BIT6 ;
              else
                  P1OUT &= ~BIT6 ;
              break;
          default:
              __no_operation();                       // For debug only
                  break;
      }
    }
}

#pragma vector = TIMER0_B1_VECTOR
__interrupt void Timer0_B1 (void)
{
    P3OUT ^= BIT1;
    int escVar =0;
    switch( TB0IV )
    {
      case 4:
          while ((UCA0IFG & UCTXIFG)==0); //Check if no transmission is taking place ie. if transmit flag is clear
                  UCA0TXBUF = 255;
          while ((UCA0IFG & UCTXIFG)==0); //Check if no transmission is taking place ie. if transmit flag is clear
          if(TA1R>>8 == 255)
          {
              UCA0TXBUF = 0;
              escVar +=8;
          }
          else
              UCA0TXBUF = TA1R>>8;

          while ((UCA0IFG & UCTXIFG)==0); //Check if no transmission is taking place ie. if transmit flag is clear
          if((TA1R<<8)>>8 == 255)
            {
                UCA0TXBUF = 0;
                escVar +=4;
            }
          else
              UCA0TXBUF = TA1R;

          while ((UCA0IFG & UCTXIFG)==0); //Check if no transmission is taking place ie. if transmit flag is clear
          if(TB2R>>8 == 255)
          {
              UCA0TXBUF = 0;
              escVar +=2;
          }
          else
              UCA0TXBUF =TB2R>>8;

          while ((UCA0IFG & UCTXIFG)==0); //Check if no transmission is taking place ie. if transmit flag is clear
          if((TB2R<<8)>>8 == 255)
            {
                UCA0TXBUF = 0;
                escVar +=1;
            }
          else
                  UCA0TXBUF =TB2R;
          while ((UCA0IFG & UCTXIFG)==0); //Check if no transmission is taking place ie. if transmit flag is clear
                  UCA0TXBUF = escVar;
          break;
      default:
          __no_operation;
              break;
    }
}
