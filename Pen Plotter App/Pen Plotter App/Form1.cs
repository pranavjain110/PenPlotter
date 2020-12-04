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




        public Form1()
        {
            InitializeComponent();
            loadLetterDictionary();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            updateSerial();  //Written in initialize.cs
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


                var scalingFactor = 2.0;

                
                for (int i = 0; i < dataPacket.GetLength(0); i++)
                {
                    if (i == dataPacket.GetLength(0) - 1)
                    {
                        offsetX += Convert.ToUInt16(dataPacket[i, 2] * scalingFactor) + 2;
                    }
                    else
                    {

                        serialPort1.Encoding = Encoding.Default;
                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 0] * 1)).ToString());
                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 1] * 1)).ToString());

                        serialPort1.Write(((char)(Convert.ToUInt16(dataPacket[i, 2] * scalingFactor + offsetX))).ToString());
                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 3] * scalingFactor + offsetY)).ToString());

                        serialPort1.Write(((char)(Convert.ToUInt16(dataPacket[i, 4] * scalingFactor + offsetX))).ToString());
                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 5] * scalingFactor + offsetY)).ToString());

                        serialPort1.Write(((char)Convert.ToUInt16(dataPacket[i, 6] * 1)).ToString());
                    }
                }


                serialPort1.Encoding = Encoding.Default;

                foreach (var dataByte in dataPacket)
                {

                        //serialPort1.Write(((char)dataByte ).ToString());
                        //Thread.Sleep(milliseconds);
                }


                Console.WriteLine(b);
            }
            serialPort1.Encoding = Encoding.Default;



        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Encoding = Encoding.Default;

            startPosX = Convert.ToUInt16(textBoxPosX.Text);
            startPosY = Convert.ToUInt16(textBoxPosY.Text);

            serialPort1.Write(((char)255).ToString());
            serialPort1.Write(((char)Command.StraightLine).ToString());
            serialPort1.Write(((char)startPosX).ToString());
            serialPort1.Write(((char)startPosY).ToString());
            serialPort1.Write(((char)0).ToString());
            serialPort1.Write(((char)0).ToString());
            serialPort1.Write(((char)0).ToString());

            
        }
    }
}
