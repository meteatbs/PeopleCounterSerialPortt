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
using System.IO;



namespace CP_Serial_Port_Terminal
{
    public partial class Form1 : Form
    {
        string dataOUT;
        string sendWith;
        string dataIN;
        string str, val1,val2,val3,val4,substr1,substr2,info;
        double num1, num2Total, num3PeoEntry, num4PeoExit,rate,upEnter,upExit,upRate,upTotalPeople, uppTotalPeople;
        DataTable dt = new DataTable();
        int selectedRow;

        StreamWriter objStreamWriter;
        string pathFile;
        bool state_AppendText = true;

        #region My Own Method
        private void SaveDataToTxtFile()
        {
            if(saveToTxtFileToolStripMenuItem.Checked)
            {
                try
                {
                    objStreamWriter = new StreamWriter(pathFile, state_AppendText);
                    if (toolStripComboBox_writeLineOrwriteText.Text == "WriteLine")
                    {
                        objStreamWriter.WriteLine(dataIN);
                    }
                    else if (toolStripComboBox_writeLineOrwriteText.Text == "Write")
                    {
                        objStreamWriter.Write(dataIN + " ");
                    }

                    objStreamWriter.Close();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }
        #endregion

        #region GUI Method
        public Form1()
        {
            InitializeComponent();
        }

 
        private void Form1_Load(object sender, EventArgs e)
        {
            btnUpdate.Enabled = false;
            string[] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.AddRange(ports);

            dt.Columns.AddRange(new DataColumn[7] {
                new DataColumn("Durum", typeof(string)),
                new DataColumn("Toplam Kişi",typeof(double)),
                new DataColumn("Kapasite",typeof(double)),
                new DataColumn("Giren Kişi",typeof(double)),
                new DataColumn("Çıkan Kişi",typeof(double)),
                new DataColumn("Saat",typeof(string)),
                new DataColumn("Tarih",typeof(string)),
            });

            chBoxDtrEnable.Checked = false;
            serialPort1.DtrEnable = false;
            chBoxRTSEnable.Checked = false;
            serialPort1.RtsEnable = false;
            btnSendData.Enabled = true;
            sendWith = "Both";
            toolStripComboBox3.Text = "TOP";

            toolStripComboBox1.Text = "Add to Old Data";
            toolStripComboBox2.Text = "Both";

            toolStripComboBox_appendOrOverwriteText.Text = "Append Text";
            toolStripComboBox_writeLineOrwriteText.Text = "WriteLine";

            pathFile = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            pathFile += @"\_My Source File\SerialData.txt";

            saveToTxtFileToolStripMenuItem.Checked = false;
        }

        private void oPENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxCOMPORT.Text;
                serialPort1.BaudRate = Convert.ToInt32(CBoxBaudRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);

                serialPort1.Open();
                progressBar1.Value = 100;
            }

            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cLOSEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                progressBar1.Value = 0;
            }
        }

        private void btnSendData_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                dataOUT = tBoxDataOut.Text;
                if (sendWith == "None")
                {
                    serialPort1.Write(dataOUT);
                }
                else if(sendWith=="Both")
                {
                    serialPort1.Write(dataOUT + "\r\n");
                }
                else if (sendWith == "New Line")
                {
                    serialPort1.Write(dataOUT + "\n");
                }
                else if (sendWith == "Carriage Return")
                {
                    serialPort1.Write(dataOUT + "\r");
                }

            }
        }

        private void toolStripComboBox2_DropDownClosed(object sender, EventArgs e)
        {
            //None
            //Both
            //New Line
            //Carriage Return

            if (toolStripComboBox2.Text == "None")
            {
                sendWith = "None";
            }
            else if (toolStripComboBox2.Text == "Both")
            {
                sendWith = "Both";
            }
            else if (toolStripComboBox2.Text == "New Line")
            {
                sendWith = "New Line";
            }
            else if (toolStripComboBox2.Text == "Carriage Return")
            {
                sendWith = "Carriage Return";
            }
        }

        private void chBoxDtrEnable_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxDtrEnable.Checked)
            {
                serialPort1.DtrEnable = true;
                MessageBox.Show("DTR Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else { serialPort1.DtrEnable = false; }
        }

        private void chBoxRTSEnable_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxRTSEnable.Checked)
            {
                serialPort1.RtsEnable = true;
                MessageBox.Show("RTS Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else { serialPort1.RtsEnable = false; }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tBoxDataOut.Text != "")
            {
                tBoxDataOut.Text = "";
            }
        }

        private void tBoxDataOut_TextChanged(object sender, EventArgs e)
        {
            int dataOUTLength = tBoxDataOut.TextLength;
            lblDataOutLength.Text = string.Format("{0:00}", dataOUTLength);
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnUpdate.Enabled = true;
            selectedRow = e.RowIndex;
            DataGridViewRow row = dgv.Rows[selectedRow];
            txtBoxPeopleEntry.Text = row.Cells[3].Value.ToString();
            txtBoxPeopleExit.Text = row.Cells[4].Value.ToString();
            upTotalPeople = Convert.ToDouble(row.Cells[1].Value);

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            uppTotalPeople = upTotalPeople;


            if (txtBoxPeopleEntry.Text=="" || txtBoxPeopleExit.Text=="")
            {
                MessageBox.Show("Giren Kişi ve Çıkan Kişi alanları boş bırakılamaz");
            }
            else
            {
                DataGridViewRow newDataRow = dgv.Rows[selectedRow];
                newDataRow.Cells[3].Value = txtBoxPeopleEntry.Text;
                newDataRow.Cells[4].Value = txtBoxPeopleExit.Text;



                upEnter = Convert.ToDouble(txtBoxPeopleEntry.Text);
                upExit = Convert.ToDouble(txtBoxPeopleExit.Text);

                newDataRow.Cells[1].Value = (upTotalPeople - upExit + upEnter);
                upRate = ((upTotalPeople - upExit + upEnter) / num1) * 100;

                upTotalPeople = (upTotalPeople - upExit + upEnter);


                // num1 = Convert.ToDouble(newDataRow.Cells[0].Value);
                //label12.Text = $"%{upRate:0.00}";
                //label13.Text = $"{uppTotalPeople}";
                lblTotalPeople.Text = $"{upTotalPeople} Toplam Kişi";
                lblPeopleEntry.Text = $"{upEnter} Giren Kişi";
                lblPeopleExit.Text = $"{upExit} Çıkan Kişi";
                lblCapacity.Text = $"{num1} Kapasite";



                if (upRate >= 100)
                {
                    info = "Alarm";
                    lblInfo.Text = $" %{upRate:0.00} Alarm";
                    lblInfo.BackColor = Color.Red;
                    newDataRow.Cells[0].Value = info;

                    var dateTime = DateTime.UtcNow;
                    var dateCal = DateTime.Now.ToString("MM/dd/yyyy");
                    var dateHour = DateTime.Now.ToString("hh:mm tt");
                    dt.Rows.Add(info, upTotalPeople, num1, upEnter, upExit, dateHour, dateCal);
                    this.dgv.DataSource = dt;
                    label12.Text = $"{dateHour} {info} saat";
                }
                else if (upRate >= 81 && upRate <= 99)
                {
                    info = "Yoğun";
                    lblInfo.Text = $" %{upRate:0.00} Yoğun";
                    lblInfo.BackColor = Color.Yellow;
                    newDataRow.Cells[0].Value = info;

                    var dateTime = DateTime.UtcNow;
                    var dateCal = DateTime.Now.ToString("MM/dd/yyyy");
                    var dateHour = DateTime.Now.ToString("hh:mm tt");
                    dt.Rows.Add(info, upTotalPeople, num1, upEnter, upExit, dateHour, dateCal);
                    this.dgv.DataSource = dt;
                    label12.Text = $"{dateHour} {info} saat";
                }
                else if (upRate >= 0 && upRate <= 80)
                {
                    info = "Normal";
                    lblInfo.Text = $" %{upRate:0.00} Normal";
                    lblInfo.BackColor = Color.Green;
                    newDataRow.Cells[0].Value = info;

                    var dateTime = DateTime.UtcNow;
                    var dateCal = DateTime.Now.ToString("MM/dd/yyyy");
                    var dateHour = DateTime.Now.ToString("hh:mm tt");
                    dt.Rows.Add(info, upTotalPeople, num1, upEnter, upExit, dateHour, dateCal);
                    this.dgv.DataSource = dt;
                    label12.Text = $"{dateHour} {info} saat";
                }
            }
            

            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tBoxDataOut_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.doSomething();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void doSomething()
        {
            if (serialPort1.IsOpen)
            {
                dataOUT = tBoxDataOut.Text;
                if (sendWith == "None")
                {
                    serialPort1.Write(dataOUT);
                }
                else if (sendWith == "Both")
                {
                    serialPort1.Write(dataOUT + "\r\n");
                }
                else if (sendWith == "New Line")
                {
                    serialPort1.Write(dataOUT + "\n");
                }
                else if (sendWith == "Carriage Return")
                {
                    serialPort1.Write(dataOUT + "\r");
                }
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            dataIN = serialPort1.ReadExisting();
            str = dataIN;
            string[] values = str.Split(' ');
            val1 = values[0];
            val2 = values[1];
            val3 = values[2];
            val4 = values[3];
            substr1 = val4.Substring(0, 4);
            substr2 = val1.Substring(1, 4);
            this.Invoke(new EventHandler(ShowData));
        }

        private void ShowData(object sender, EventArgs e)
        {
            int dataINLength = dataIN.Length;
            lblDataInLength.Text = string.Format("{0:00}", dataINLength);
            btnUpdate.Enabled = false;
            var dateTime = DateTime.UtcNow;
            var dateCal = DateTime.Now.ToString("MM/dd/yyyy");
            var dateHour = DateTime.Now.ToString("hh:mm tt");
            //label14.Text = (Convert.ToString(dateTime).ToString("MM-dd-yyyy"));
            //label14.Text = Convert.ToString(dateHour);
            if (toolStripComboBox1.Text == "Always Update")
            {
                tBoxDataIN.Text = dataIN;

            }
            else if(toolStripComboBox1.Text == "Add to Old Data")
            {
                if(toolStripComboBox3.Text == "TOP")
                {
                    num1 = Convert.ToDouble(substr2);
                    num2Total = Convert.ToDouble(val2);
                    num3PeoEntry = Convert.ToDouble(val3);
                    num4PeoExit = Convert.ToDouble(substr1);

                    rate = (num2Total / num1) * 100;
                    //lblRate.Text = $"%{rate:0.00}";
                    
                    if (rate >= 100)
                    {
                        info = "Alarm";
                        lblInfo.Text = $" %{rate:0.00} Alarm";
                        lblInfo.BackColor = Color.Red;
                        label12.Text = $"{dateHour} {info} saat";
                    }
                    else if (rate >= 81 && rate <= 99)
                    {
                        info = "Yoğun";
                        lblInfo.Text = $" %{rate:0.00} Yoğun";
                        lblInfo.BackColor = Color.Yellow;
                        label12.Text = $"{dateHour} {info} saat";
                    }
                    else if (rate >= 0 && rate <= 80)
                    {
                        info = "Normal";
                        lblInfo.Text = $" %{rate:0.00} Normal";
                        lblInfo.BackColor = Color.Green;
                        label12.Text = $"{dateHour} {info} saat";
                    }

                    dt.Rows.Add(info, num2Total, num1, num3PeoEntry, num4PeoExit, dateHour, dateCal);
                    this.dgv.DataSource = dt;

                    dt.Rows.Add(info, num2Total, num1, num3PeoEntry, num4PeoExit,dateHour,dateCal);
                    this.dgv.DataSource = dt;


                    tBoxDataIN.Text = tBoxDataIN.Text.Insert(0, dataIN);
                   // lblAlert.Text = substr1;
                    lblTotalPeople.Text = $" {num2Total} Toplam Kişi";
                    lblPeopleEntry.Text = $"{num3PeoEntry} Toplam Giriş";
                    lblPeopleExit.Text = $"{num4PeoExit} Toplam Çıkış";
                    lblCapacity.Text = $"{num1} Kapasite";
                    


                }
                else if(toolStripComboBox3.Text=="BOTTOM")
                {
                    tBoxDataIN.Text += dataIN;
                }      
            }

            SaveDataToTxtFile();
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (tBoxDataIN.Text != "")
            {
                tBoxDataIN.Text = "";
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Mete ", "Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            groupBox12.Width = panel1.Width - 213;
            groupBox12.Height = panel1.Height - 40;

            tBoxDataIN.Height = panel1.Height - 88;
        }

        private void toolStripComboBox_appendOrOverwriteText_DropDownClosed(object sender, EventArgs e)
        {
            if(toolStripComboBox_appendOrOverwriteText.Text == "Append Text")
            {
                state_AppendText = true;
            }
            else
            {
                state_AppendText = false;
            }
        }

        #endregion

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
