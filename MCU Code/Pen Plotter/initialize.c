/*
 * initialize.c
 *
 *  Created on: 26-Nov-2020
 *      Author: Pranav Jain
 */

#include <msp430.h>

void initializeClock(void)
{
    //_______________Setup UART for user input______________________________
    // Configure clocks
    CSCTL0 = 0xA500;                        // Write password to modify CS registers
    CSCTL1 = DCORSEL;           // DCO = 16 MHz, DCOFSEL0 =0, DCOFSEL1 =0
    CSCTL2 = SELM0 + SELM1 + SELA0 + SELA1 + SELS0 + SELS1; // MCLK = DCO, ACLK = DCO, SMCLK = DCO
    CSCTL3 =  DIVA__2 ; //ACLK = 8MHz
    return;
}

void initializeUART(void)
{
    // Configure ports for UCA0
    P2SEL0 &= ~(BIT0 + BIT1);
    P2SEL1 |= BIT0 + BIT1;

    //Configure UCA0
    UCA0CTLW0 = UCSSEL__ACLK; //ACLK
    UCA0BRW = 52;
    UCA0MCTLW = 0x4900 + UCOS16 + UCBRF0;
    UCA0IE |= UCRXIE; //eUSCI_A interrupt enable for when a byte is received

    //setting TImerB0 (TB0.2) for regularly sending reading over UART
    //TB0CCTL2 = CCIE; // TBCCR1 interrupt enabled
//    TB0CCR0 = 0xFFFF;;
//    TB0CTL = TBSSEL_1 + MC_1 + ID_3; // AMCLK, UP mode(counts up to TBCCR0, /8)

    return;
}

unsigned int initializeStepper(void)
{
    //_______________________Setting TImerA0 (TA0.1) for stepper_____________________________

    TA0CCTL1 |= CCIE; // TBCCR0 interrupt enabled
    TA0CCR0 = 0x4500; //0x3C00 is the least possible
    TA0CTL = TASSEL__SMCLK + MC_1 + ID_0; // AMCLK, UP mode
    TA0CCTL1 |=  OUTMOD_4;// Toggle Mode

    //Setting port P1.3, P1.4, P1.5 and P1.6 as output
    P1OUT  &= ~(BIT3 + BIT4 + BIT5 + BIT6);
    P1DIR |= BIT3 + BIT4 + BIT5 + BIT6;
    return 1; //Stepper State returned
}

void initializeEncoder(void)
{
    //______Setting P1.1 and P3.4 to read encoder via D flip flop______
    // Make Port 1.1 as TA1CLK
    P1DIR  &= ~BIT1; //0
    P1SEL1 |=  BIT1; //1
    P1SEL0 &= ~BIT1; //0

    // Make Port 3.4 as TB2CLK
    P3DIR  &= ~BIT4; //0
    P3SEL1 |=  BIT4; //1
    P3SEL0 |=  BIT4; //1

    //Setting Timers TA1 and TB2 to read encoder pulses
    TA1CTL |=  TASSEL_0 + MC_2 ; //TA1CLK , Continuous Mode
    TB2CTL |= TASSEL_0 + MC_2; //TB2CLK , Continuous Mode
    //Clear both counters
    TA1CTL |=  TACLR;
    TB2CTL |= TBCLR;
    return;

}

void initializeDCMotor(void)
{
    //Setting TImerB1 (TB1.2) for generating PWM
    // P3.5 = TB1.2
    P3DIR  |= BIT5;
    P3SEL0 |= BIT5;

    TB1CCR0 = 0xFFFF;
    TB1CCR2 = 0xEFFF;//Motor Stopped by default
    TB1CTL = TBSSEL_1 + MC_1 + ID_0;    // AMCLK, UP mode, /1
    TB1CCTL2 |=  OUTMOD_3;// Toggle Mode

    //Setting Port 3.6 as output for CW rotation
    P3DIR |= BIT6;
    P3OUT &= ~BIT6;

    //Setting Port 3.7 as output for ACW rotation
    P3DIR |=  BIT7;
    P3OUT &= ~BIT7;
    return;
}

void initializeSolenoid(void)
{
    //Setting port P4.0 as output for solenoid mosfet
    P4DIR |= BIT0;
    P4OUT &= ~BIT0; //Turn solenoid off
//    P4OUT |= BIT0;   //Turn solenoid on
}
