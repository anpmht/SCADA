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
using System.Threading;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Configuration;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static string serialdata = "";
        public static bool connected = false;
        public static string data = "";
        public string[] seperted_data ;
        public int dg = 0;
        public int n = 0;
        int ID = 0;
        string TABLE = "";
        public int voltage = 0;
        public int power = 0;
        public int current = 0;
        public int energy = 0;
        public DateTime then = DateTime.Now.Date;
        public DateTime date = DateTime.Now.Date;
        public int generator1 = 0;
        public int generator2 = 0;
        public int storage = 0;
        const string connectionString =
             @"Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=Excel 12.0 XML;Data Source=E:\database1.xlxs;";
        public Form1()
        {
            InitializeComponent();
            
        }

        public void scada_load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'database2DataSet.generator_1' table. You can move, or remove it, as needed.
            //this.generator_1TableAdapter.Fill(this.database2DataSet.generator_1);
            // TODO: This line of code loads data into the 'modelDataSet.Table' table. You can move, or remove it, as needed.
            //this.tableTableAdapter.Fill(this.modelDataSet.Table);
            String[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            //chart1.ChartAreas.AxisX.Minimum = 0;

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            bunifuTextBox9.Text = serialdata + bunifuTextBox9 + "\n";
            try
            {   if (Convert.ToString(seperted_data) != "")
                {
                    //bunifuRadialGauge1.Value = Convert.ToInt32(seperted_data[1]);
                    bunifuRadialGauge2.Value = Convert.ToInt32(seperted_data[3]);
                    bunifuRadialGauge3.Value = Convert.ToInt32(seperted_data[1]);
                    bunifuTextBox1.Text = seperted_data[2] + " W";///Convert.ToString(generator1) + " W";
                   // bunifuTextBox2.Text = Convert.ToString(generator2) + " W";
                    //bunifuTextBox3.Text = seperted_data[3] + " W";
                    bunifuTextBox4.Text = Convert.ToString(current - power) + " W";//seperted_data[3] + " W";
                   
                    //////////////////////////////////second page 
                    bunifuRadialGauge15.Value = Convert.ToInt32(seperted_data[2]);
                    //bunifuRadialGauge14.Value = Convert.ToInt32(seperted_data[1]);
                    bunifuRadialGauge13.Value = bunifuRadialGauge2.Value/10;
                    bunifuRadialGauge12.Value = 150;

                    if (current > 1)
                    { bunifuThinButton26.IdleFillColor = Color.Green; bunifuThinButton26.ButtonText = "Online";
                        bunifuThinButton27.IdleFillColor = Color.Green; bunifuThinButton27.ButtonText = "Online";
                        bunifuButton1.IdleFillColor = Color.Green;
                    }
                    else if (current <= 1)
                    { bunifuThinButton26.IdleFillColor = Color.Red; bunifuThinButton26.ButtonText = "Offline";
                        bunifuButton1.IdleFillColor = Color.Red; 
                    }
                    bunifuButton2.IdleFillColor = Color.Red;
                    bunifuButton3.IdleFillColor = Color.Red;
                }
            }
            catch (System.IndexOutOfRangeException)
            { }
            catch(Exception)
            { }
            date = DateTime.Now;
            //  chart1.Series["Series4"].Points.AddXY(n, bunifuRadialGauge2.Value);
            // if (chart1.Series["Series4"].Points.Count > 24)
            //    chart1.Series["Series4"].Points.RemoveAt(0);

        }

        private void bunifuThinButton211_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "" && comboBox2.Text != "" && connected == false)
            {
                serialPort1.PortName = comboBox1.Text;
            }
            try
            {
                serialPort1.Open();
                connected = true;
                timer1.Enabled = true;
                timer2.Enabled = true;
                var t1 = new Task(() => listenTOserialPort());
                t1.Start();
                timer2.Start();
                progressBar1.Value = 100;
               
            }
            catch (System.IO.IOException)
            {
                bunifuTextBox9.Text = "failed to open the port" + "\n";
            }
            catch (System.ArgumentException)
            { }
            
        }
        public void listenTOserialPort()
        {
            for (int a = 0; a < 50; a--)
            {
                try
                {
                    data = serialPort1.ReadLine();
                    seperted_data = data.Split(',');
                    ID = Convert.ToInt32(seperted_data[0]);
                    voltage = Convert.ToInt32(seperted_data[1]);
                    current = Convert.ToInt32(seperted_data[2]);
                   
                    power = Convert.ToInt32(seperted_data[3]);
                    energy = Convert.ToInt32(seperted_data[4]);
                    date = DateTime.Now.Date;
                    TABLE = "user_1";
                    if (ID == 1) { TABLE = "user_1";  }
                    if (ID == 2) { TABLE = "USER_2"; }
                    if (ID == 3) { TABLE = "user_3"; }
                    if (ID == 4) { TABLE = "user_4"; }
                    if (ID == 5) { TABLE = "user_5"; }
                    if (ID == 6) { TABLE = "generator_1"; generator1 = current; bunifuRadialGauge3.Value = voltage; bunifuRadialGauge2.Value = power; }
                    if (ID == 7) { TABLE = "generator_2"; generator2 = power; }
                    if (ID == 8) { TABLE = "storage"; storage = power; }
                    log_data();
                    serialdata = data + "\n" + serialdata + "\n";
                }
                catch (System.FormatException) { }
                catch (System.InvalidOperationException) { }
                catch (System.IO.IOException) { }
                catch (System.IndexOutOfRangeException) { }
                
            }
        }
        public void log_data()
        {
            
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\ANPMH\SOURCE\REPOS\WINDOWSFORMSAPP1\DATABASE2.MDF;Integrated Security=True");
            string sqlInsert = "INSERT INTO ["+TABLE+"] VALUES("+ID+","+voltage+","+current+","+power+","+energy+",'"+date+"')";
            SqlCommand insertCommand = new SqlCommand(sqlInsert, con);
            con.Open();

            insertCommand.ExecuteNonQuery();
            con.Close();
        }
        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel2_Click(object sender, EventArgs e)
        {

        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            then = DateTime.Now;
            panel8.SendToBack();
            panel9.SendToBack();
            panel3.SendToBack();
            panel15.BringToFront();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuThinButton24_Click(object sender, EventArgs e)
        {
            panel9.SendToBack();
            panel3.SendToBack();
            panel15.SendToBack();
            panel8.BringToFront();
        }

        private void bunifuThinButton22_Click(object sender, EventArgs e)
        {
            panel8.SendToBack();
            panel3.SendToBack();
            panel15.SendToBack();
            panel9.BringToFront();

        }

        private void bunifuImageButton1_Click_1(object sender, EventArgs e)
        {

        }


        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void logData()
        {

        }

        private void bunifuRadialGauge1_ValueChanged(object sender, Bunifu.UI.WinForms.BunifuRadialGauge.ValueChangedEventArgs e)
        {

        }

        private void tableBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.tableBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.modelDataSet);

        }

        private void bunifuTextBox9_TextChanged(object sender, EventArgs e)
        {
          
        }

        public void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {

        }

        private void tableBindingNavigator_RefreshItems(object sender, EventArgs e)
        {
                    }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuThinButton26_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            serialPort1.WriteLine("6 ");
        }

        private void bunifuThinButton217_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("6 11 ");
        }

        private void bunifuThinButton216_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("6 01 ");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel16_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {

        }

        private void panel18_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {

        }

        private void bunifuThinButton220_Click(object sender, EventArgs e)
        {
            panel8.SendToBack();
            panel15.SendToBack();
            panel9.SendToBack();
            panel3.BringToFront();
        }

        private void bunifuThinButton219_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("6 12 ");
        }

        private void bunifuThinButton218_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("6 02 ");
        }

        private void bunifuThinButton213_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("6 13 ");
        }

        private void bunifuThinButton212_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("6 03 ");
        }

        private void bunifuLabel39_Click(object sender, EventArgs e)
        {

        }
    }

}
