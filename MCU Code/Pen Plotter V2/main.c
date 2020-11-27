#include <msp430.h> 
#include<math.h>
#include "initialize.h"
#define sizeOfBuffer 280

volatile unsigned char RxByte;

volatile unsigned int  bufferLength = 0,readIndex = 0, writeIndex = 0,flagByteRx = 0,
        stepperState, steps=0, stepsDesired=0, flagACW = 0, flagCW = 1, moveStepperFlag = 0,
        buffer[sizeOfBuffer];

unsigned int i,j;
int positionCurrent , posDesiredDC=200  , error = 0, controllerOutput = 0;

unsigned const int stepperStateTable[] =  { 1, 0, 0, 0,  //State 1
                                            1, 0, 1, 0,  //State 2
                                            0, 0, 1, 0,  //State 3
                                            0, 1, 1, 0,  //State 4
                                            0, 1, 0, 0,  //State 5
                                            0, 1, 0, 1,  //State 6
                                            0, 0, 0, 1,  //State 7
                                            1, 0, 0, 1 }; //State 8

double ans ;


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


