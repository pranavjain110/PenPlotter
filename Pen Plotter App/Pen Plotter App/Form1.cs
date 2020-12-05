using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pen_Plotter_App
{
    public partial class Form1 : Form
    {



        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();
        ConcurrentQueue<string> timeQueue = new ConcurrentQueue<string>();
        Dictionary<string, double[,]> letter = new Dictionary<string, double[,]>();
        int startPosX = 0, startPosY = 0;
        int dataState = 0, bufferSizeMCU = 0, dataByte1_H, dataByte1_L, dataByte2_H, dataByte2_L;
        int stepperSteps = 0, motorDCTicks = 0, setDataFlag = 0;
        ConcurrentQueue<int> commandToSend = new ConcurrentQueue<int>();



        public Form1()
        {
            InitializeComponent();
            loadLetterDictionary();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateSerial();  //Written in initialize.cs
        }

        private void textBoxMachineStatus_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnDisconnectSerial_Click(object sender, EventArgs e)
        {
            toggleConnectSerialButton();  //Written in initialize.cs
        }

        private void comboBoxCOMPorts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int newByte = 0;
            int bytesToRead;


            bytesToRead = serialPort1.BytesToRead;
            while (bytesToRead != 0)
            {
                newByte = serialPort1.ReadByte();
                dataQueue.Enqueue(newByte);
                timeQueue.Enqueue(DateTime.Now.ToString("h:mm:ss.fff"));
                bytesToRead = serialPort1.BytesToRead;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
                txtBytesToRead.Text = serialPort1.BytesToRead.ToString();
            txtItemsInQueue.Text = dataQueue.Count.ToString();

            

            while (dataQueue.Count > 0)
            {
                if (dataQueue.TryDequeue(out int dequeuedElement))
                {
                    timeQueue.TryDequeue(out string dequeuedTime);
                    txtSerialData.AppendText(" , " + dequeuedElement.ToString());

                    
                    if (dequeuedElement == 255)
                        dataState = 1;
                    else
                        switch (dataState)
                        {
                            case 1:
                                dataState = 2;
                                bufferSizeMCU = dequeuedElement;
                                break;
                            case 2:
                                dataState = 3;
                                dataByte1_H = dequeuedElement;
                                break;
                            case 3:
                                dataState = 4;
                                dataByte1_L = dequeuedElement;
                                break;
                            case 4:
                                dataState = 5;
                                dataByte2_H = dequeuedElement;
                                break;
                            case 5:
                                dataState = 6;
                                dataByte2_L = dequeuedElement;
                                break;
                            case 6:
                                dataState = 7;
                                var escByte = dequeuedElement;
                                if (escByte >= 8)
                                {
                                    dataByte1_H = 255;
                                    escByte -= 8;
                                }
                                if (escByte >= 4)
                                {
                                    dataByte1_L = 255;
                                    escByte -= 4;
                                }
                                if (escByte >= 2)
                                {
                                    dataByte2_H = 255;
                                    escByte -= 2;
                                }
                                if (escByte >= 1)
                                {
                                    dataByte2_L = 255;
                                    escByte -= 1;
                                }
                                if (escByte != 0)
                                {
                                    //error here
                                    Console.WriteLine("check error here");
                                }
                                setDataFlag = 1;
                                break;
                            default:
                                break;
                        }
                }

                if (setDataFlag == 1)
                {
                    stepperSteps = dataByte1_H * 256 + dataByte1_L;
                    textBoxPosStepper.Text = (stepperSteps/105).ToString();

                    motorDCTicks = dataByte2_H * 256 + dataByte2_L;
                    textBoxPosDC.Text = (motorDCTicks/3).ToString();
                    setDataFlag = 0;
                }


                if (bufferSizeMCU == 0)
                {
                    if (commandToSend.TryDequeue(out int command))
                    {
                        serialPort1.Encoding = Encoding.Default;
                        serialPort1.Write(((char)command).ToString());
                        Console.WriteLine(command);
                    }
                }

                if(bufferSizeMCU >0 || commandToSend.Count>0)
                {
                    textBoxMachineStatus.Text = "Machine Buzy";
                    textBoxMachineStatus.BackColor = System.Drawing.Color.Red;


                }
                else
                {
                    textBoxMachineStatus.Text = "Machine Idle";
                    textBoxMachineStatus.BackColor = System.Drawing.Color.LightGreen;
                }



            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int milliseconds = 400;

            int offsetX = startPosX;
            int offsetY = startPosY;
            foreach (var inputChar in textBoxStringInput.Text)
            {

                var b = 0;
                var tempDictionary = new Dictionary<string, double[,]>(letter);
                var dataPacket = tempDictionary[inputChar.ToString()];


                var scalingFactor = Convert.ToInt32(textBoxScalingFactor.Text) *2;

                
                for (int i = 0; i < dataPacket.GetLength(0); i++)
                {
                    if (i == dataPacket.GetLength(0) - 1)
                    {
                        offsetX += Convert.ToUInt16(dataPacket[i, 2] * scalingFactor) + 2;
                    }
                    else
                    {

                        commandToSend.Enqueue(Convert.ToUInt16(dataPacket[i, 0] * 1));
                        commandToSend.Enqueue(Convert.ToUInt16(dataPacket[i, 1] * 1));
                        commandToSend.Enqueue(Convert.ToUInt16(dataPacket[i, 2] * scalingFactor + offsetX));
                        commandToSend.Enqueue(Convert.ToUInt16(dataPacket[i, 3] * scalingFactor + offsetY));
                        commandToSend.Enqueue(Convert.ToUInt16(dataPacket[i, 4] * scalingFactor + offsetX));
                        commandToSend.Enqueue(Convert.ToUInt16(dataPacket[i, 5] * scalingFactor + offsetY));
                        commandToSend.Enqueue(Convert.ToUInt16(dataPacket[i, 6] * 1));

                        serialPort1.Encoding = Encoding.Default;
/*                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 0] * 1)).ToString());
                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 1] * 1)).ToString());

                        serialPort1.Write(((char)(Convert.ToUInt16(dataPacket[i, 2] * scalingFactor + offsetX))).ToString());
                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 3] * scalingFactor + offsetY)).ToString());

                        serialPort1.Write(((char)(Convert.ToUInt16(dataPacket[i, 4] * scalingFactor + offsetX))).ToString());
                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 5] * scalingFactor + offsetY)).ToString());

                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 6] * 1)).ToString());*/
                    }
                }

                 tempDictionary = new Dictionary<string, double[,]>();

                serialPort1.Encoding = Encoding.Default;



                Console.WriteLine(b);
            }




        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Encoding = Encoding.Default;

            startPosX = Convert.ToUInt16(textBoxPosX.Text);
            startPosY = Convert.ToUInt16(textBoxPosY.Text);

/*            serialPort1.Write(((char)255).ToString());
            serialPort1.Write(((char)Command.StraightLine).ToString());
            serialPort1.Write(((char)startPosX).ToString());
            serialPort1.Write(((char)startPosY).ToString());
            serialPort1.Write(((char)0).ToString());
            serialPort1.Write(((char)0).ToString());
            serialPort1.Write(((char)0).ToString());*/



            commandToSend.Enqueue(255);
            commandToSend.Enqueue(4);
            commandToSend.Enqueue(startPosX);
            commandToSend.Enqueue(startPosY);
            commandToSend.Enqueue(0);
            commandToSend.Enqueue(0);
            commandToSend.Enqueue(0);
        }
    }
}
