using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pen_Plotter_App
{

    public partial class Form1 
    {

        enum Command : int
        {
            StraightLine = 1,
            ClockWise = 2,
            AntiClockWise = 3,
            Rapid = 4,
            Offset = 5
        };
        double[,] charA = {
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 3, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 6, 0, 0, 0, 0 },
            { 255, (int)Command.Rapid, 1, 2, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 5, 2, 0, 0, 0 },
            { 255, (int)Command.Offset, 7 , 0, 0, 0, 0}
        };



        double[,] charB = {
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 2, 6, 0, 0, 0 },
            { 255, (int)Command.ClockWise, 2, 3, 2, 4.5, 0 },
            { 255, (int)Command.StraightLine, 0, 3, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 2, 3, 0, 0, 0 },
            { 255, (int)Command.ClockWise, 2, 0, 2, 1.5, 0 },
            { 255, (int)Command.StraightLine, 0, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 3.5 , 0, 0, 0, 0}
        };

        double[,] charC = {
            { 255, (int)Command.Rapid, 3, 6, 0, 0, 0},
            { 255, (int)Command.AntiClockWise, 3, 0, 3, 3, 0 },
            { 255, (int)Command.Offset, 3 , 0, 0, 0, 0}
        };

        double[,] charD = {
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 1, 6, 0, 0, 0 },
            { 255, (int)Command.ClockWise, 1, 0, 1, 3, 0 },
            { 255, (int)Command.StraightLine, 0, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 4 , 0, 0, 0, 0}
        };

        double[,] charE = {
            { 255, (int)Command.Rapid, 4, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 0, 0, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 6, 0, 0, 0, 0 },
            { 255, (int)Command.Rapid, 0, 3, 0, 0, 0},
            { 255, (int)Command.StraightLine, 3, 3, 0, 0, 0 },
            { 255, (int)Command.Offset, 6 , 0, 0, 0, 0}

        };

        double[,] charF = {
            { 255, (int)Command.Rapid, 4, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 0, 0, 0, 0, 0 },
            { 255, (int)Command.Rapid, 0, 3, 0, 0, 0},
            { 255, (int)Command.StraightLine, 3, 3, 0, 0, 0 },
            { 255, (int)Command.Offset, 4 , 0, 0, 0, 0}
        };


        double[,] charG = {
/*            { 255, (int)Command.Rapid, 5, 5, 0, 0, 0},
            { 255, (int)Command.AntiClockWise, 5, 1, 3, 3, 0 },
            { 255, (int)Command.StraightLine, 5, 3, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 3, 3, 0, 0, 0 }*/

            { 255, (int)Command.Rapid, 3, 6, 0, 0, 0},
            { 255, (int)Command.AntiClockWise, 3, 0, 3, 3, 0 },
            { 255, (int)Command.AntiClockWise, 6, 3, 3, 3, 0 },
            { 255, (int)Command.StraightLine, 4.5, 3, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 4.5, 2, 0, 0, 0 },
            { 255, (int)Command.Offset, 6 , 0, 0, 0, 0}
        };

        double[,] charH = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 0, 0, 0, 0 },
            { 255, (int)Command.Rapid, 0, 3, 0, 0, 0},
            { 255, (int)Command.StraightLine, 5, 3, 0, 0, 0 },
            { 255, (int)Command.Rapid, 5, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 5, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 5 , 0, 0, 0, 0}
        };

        double[,] charI = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 6, 6, 0, 0, 0 },
            { 255, (int)Command.Rapid, 3, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 3, 0, 0, 0, 0 },
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 6, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 6 , 0, 0, 0, 0}
        };

        double[,] charJ = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 6, 6, 0, 0, 0 },
            { 255, (int)Command.Rapid, 4, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 4,2, 0, 0, 0 },
            { 255, (int)Command.ClockWise, 0, 2, 2, 2, 0},
            { 255, (int)Command.Offset, 6 , 0, 0, 0, 0}
        };

        double[,] charK = {
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 6, 0, 0, 0 },
            { 255, (int)Command.Rapid, 0, 2, 0, 0, 0},
            { 255, (int)Command.StraightLine, 4, 6, 0, 0, 0 },
            { 255, (int)Command.Rapid, 1, 3, 0, 0, 0},
            { 255, (int)Command.StraightLine, 4, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 4 , 0, 0, 0, 0}
        };

        double[,] charL = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 0, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 4, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 4 , 0, 0, 0, 0}
        };

        double[,] charM = {
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 2.5, 3, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 5, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 5, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 5 , 0, 0, 0, 0}
        };

        double[,] charN = {
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 4, 0, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 4, 6, 0, 0, 0 },
            { 255, (int)Command.Offset, 4 , 0, 0, 0, 0}
        };

        double[,] charO = {
            { 255, (int)Command.Rapid, 3, 0, 0, 0, 0},
            { 255, (int)Command.ClockWise, 3, 6, 3, 3, 0 },
            { 255, (int)Command.ClockWise, 3, 0, 3, 3, 0 },
            { 255, (int)Command.Offset, 6 , 0, 0, 0, 0}
        };
        double[,] charP = {
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 2, 6, 0, 0, 0 },
            { 255, (int)Command.ClockWise, 2, 3, 2, 4.5, 0 },
            { 255, (int)Command.StraightLine, 0, 3, 0, 0, 0 },
            { 255, (int)Command.Offset, 3.5 , 0, 0, 0, 0}
        };

        double[,] charQ = {
            { 255, (int)Command.Rapid, 3, 0, 0, 0, 0},
            { 255, (int)Command.ClockWise, 3, 6, 3, 3, 0 },
            { 255, (int)Command.ClockWise, 3, 0, 3, 3, 0 },
            { 255, (int)Command.Rapid, 3.5, 2.5, 0, 0, 0},
            { 255, (int)Command.StraightLine, 6, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 6 , 0, 0, 0, 0}
        };

        double[,] charR = { 
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 2, 6, 0, 0, 0 },
            { 255, (int)Command.ClockWise, 2, 3, 2, 4.5, 0 },
            { 255, (int)Command.StraightLine, 0, 3, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 4.5, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 4.5 , 0, 0, 0, 0}
        };

        /*        double[,] charS = {
                    { 255, (int)Command.Rapid, 3, 5.5, 0, 0, 0},
                    { 255, (int)Command.AntiClockWise, 1.5, 6, 1.5, 3.5, 0 },
                    { 255, (int)Command.AntiClockWise, 1.5, 3, 1.5, 4.5, 0 },
                    { 255, (int)Command.ClockWise, 1.5, 0, 1.5, 1.5, 0 },
                    { 255, (int)Command.ClockWise, 0.5, 0, 1.5, 2.5, 0 },
                };*/

        double[,] charS = {
            { 255, (int)Command.Rapid, 3, 4.5, 0, 0, 0},
            { 255, (int)Command.AntiClockWise, 1.5, 3, 1.5, 4.5, 0 },
            //{ 255, (int)Command.Rapid, 1.5, 3, 0, 0, 0},
            { 255, (int)Command.ClockWise, 1.5, 0, 1.5, 1.5, 0 },
            { 255, (int)Command.ClockWise, 0, 1.5, 1.5, 1.5, 0 },
            { 255, (int)Command.Offset, 3 , 0, 0, 0, 0}
        };

        double[,] charT = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 6, 6, 0, 0, 0 },
            { 255, (int)Command.Rapid, 3, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 3,0, 0, 0, 0 },
            { 255, (int)Command.Offset, 6 , 0, 0, 0, 0}
        };

        double[,] charU = {
            { 255, (int)Command.Rapid, 0.5, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0.5, 2, 0, 0, 0 },
            { 255, (int)Command.AntiClockWise, 4.5, 2, 2.5, 2, 0 },
            { 255, (int)Command.StraightLine, 4.5, 6, 0, 0, 0 },
            { 255, (int)Command.Offset, 4.5 , 0, 0, 0, 0}
        };

        double[,] charV = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 2.5, 0, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 5, 6, 0, 0, 0 },
            { 255, (int)Command.Offset, 5 , 0, 0, 0, 0}
        };

        double[,] charW = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 1.5, 0, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 3, 4, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 4.5, 0, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 6, 6, 0, 0, 0 },
            { 255, (int)Command.Offset, 6 , 0, 0, 0, 0}
        };

        double[,] charX = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 4, 0, 0, 0, 0 },
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 4, 6, 0, 0, 0 },
            { 255, (int)Command.Offset, 4, 0, 0, 0, 0}
        };

        double[,] charY = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 2, 3, 0, 0, 0 },
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 4, 6, 0, 0, 0 },
            { 255, (int)Command.Offset, 4 , 0, 0, 0, 0}
        };

        double[,] charZ = {
            { 255, (int)Command.Rapid, 0, 6, 0, 0, 0},
            { 255, (int)Command.StraightLine, 5, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 5, 0, 0, 0, 0 },
            { 255, (int)Command.Offset, 5 , 0, 0, 0, 0}
        };

        private void updateSerial()
        {
            comboBoxCOMPorts.Items.Clear();
            comboBoxCOMPorts.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (comboBoxCOMPorts.Items.Count == 0)
                comboBoxCOMPorts.Text = "No COM ports!";
            else
                comboBoxCOMPorts.SelectedIndex = 0;
        }

        private void toggleConnectSerialButton()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                serialPort1.Dispose();
                btnDisconnectSerial.Text = "Connect Serial";
            }
            else
            {
                serialPort1.Close();
                serialPort1.Dispose();
                serialPort1.Open();
                btnDisconnectSerial.Text = "Disconnect Serial";
            }
        }

        private void loadLetterDictionary()
        {
            letter.Add("A", charA);
            letter.Add("B", charB);
            letter.Add("C", charC);
            letter.Add("D", charD);
            letter.Add("E", charE);
            letter.Add("F", charF);
            letter.Add("G", charG);
            letter.Add("H", charH);
            letter.Add("I", charI);
            letter.Add("J", charJ);
            letter.Add("K", charK);
            letter.Add("L", charL);
            letter.Add("M", charM);
            letter.Add("N", charN);
            letter.Add("O", charO);
            letter.Add("P", charP);
            letter.Add("Q", charQ);
            letter.Add("R", charR);
            letter.Add("S", charS);

            letter.Add("T", charT);
            letter.Add("U", charU);
            letter.Add("V", charV);
            letter.Add("W", charW);
            letter.Add("X", charX);
            letter.Add("Y", charY);
            letter.Add("Z", charZ);


        }


    }
}
