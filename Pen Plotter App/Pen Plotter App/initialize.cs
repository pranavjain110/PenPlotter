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
            Rapid = 4
        };
        int[,] charA = {
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 3, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 6, 0, 0, 0, 0 },
            { 255, (int)Command.Rapid, 1, 2, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 5, 2, 0, 0, 0 }
        };

        int[,] charB = {
            { 255, (int)Command.Rapid, 0, 0, 0, 0, 0},
            { 255, (int)Command.StraightLine, 0, 6, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 1, 6, 0, 0, 0 },
            { 255, (int)Command.ClockWise, 1, 4, 1, 5, 0 },
            { 255, (int)Command.StraightLine, 0, 4, 0, 0, 0 },
            { 255, (int)Command.StraightLine, 1, 4, 0, 0, 0 },
            { 255, (int)Command.ClockWise, 1, 0, 1, 2, 0 },
            { 255, (int)Command.StraightLine, 0, 0, 0, 0, 0 },
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
            letter.Add("B", charA);
        }


    }
}
