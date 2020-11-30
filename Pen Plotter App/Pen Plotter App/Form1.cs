using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pen_Plotter_App
{
    public partial class Form1 : Form
    {



        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();
        ConcurrentQueue<string> timeQueue = new ConcurrentQueue<string>();
        Dictionary<string, int[,]> letter = new Dictionary<string, int[,]>();




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
            foreach (var inputChar in textBoxStringInput.Text)
            {

                var b = 0;

                var dataPacket = this.letter[inputChar.ToString()];



                for (int i = 0; i < dataPacket.GetLength(0); i++)
                {
                    dataPacket[i, 2] *= 3;
                    dataPacket[i, 3] *= 3;
                    dataPacket[i, 4] *= 3;
                    dataPacket[i, 5] *= 3;


                }



                serialPort1.Encoding = Encoding.Default;

                foreach (var dataByte in dataPacket)
                {

                        serialPort1.Write(((char)dataByte ).ToString());
                }


                Console.WriteLine(b);
            }

        }
    }
}
