using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        public static string port_name = "";       // port name
        public static int port_speed = 0;
        public Form3()
        {
            InitializeComponent();
        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {}

        private void bunifuTextBox2_TextChanged(object sender, EventArgs e)
        { }

        private void Form3_Load(object sender, EventArgs e)
        {
            String[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {   
            serialPort1.PortName = comboBox1.Text;
            port_name = serialPort1.PortName;
            
            serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
            port_speed = serialPort1.BaudRate;
/*            try
            { serialPort1.Open(); }
            catch (System.UnauthorizedAccessException)
            {
                bunifuTextBox1.Text = "Failed to connect to " + serialPort1.PortName + " at " + serialPort1.BaudRate + " baudrate";
            }
            catch (System.IO.IOException)
            {
                bunifuTextBox1.Text = "Failed to connect to " + serialPort1.PortName + " at " + serialPort1.BaudRate + " baudrate";
            }
            timer1.Enabled = true;
            bunifuTextBox1.Text = "port opened";*/
            Form1 frm1 = new Form1();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                bunifuTextBox1.Text = bunifuTextBox1.Text + "\n" + serialPort1.ReadLine() + "\n";
            }
            catch (System.InvalidOperationException)
            {
                bunifuTextBox1.Text = "port not connected";
            }
            Form1 frm1 = new Form1();
        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void bunifuThinButton22_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }
    }
}
