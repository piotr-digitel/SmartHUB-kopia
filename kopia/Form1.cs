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
using System.Globalization;

namespace kopia
{
    public partial class Form1 : Form
    {
        static SerialPort mySerialPort;
        bool isPortOpen = false;  //czy port jest otwarty
        bool backLite = true;
        string OdebranaKomenda;   //komendy od smarthub do ustawienia początkowego programu
        int minutyDwie = 0;


        public Form1()
        {
            InitializeComponent();

            //string[] dniTygodnia = { "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };


            //Pobieranie dostępnych comów
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                cmbPort.Items.Add(port);
            }

            if (cmbPort.Items.Count > 0)
            {
                cmbPort.SelectedIndex = 0;
               // mySerialPort = new SerialPort(cmbPort.Text, 9600);
                mySerialPort = new SerialPort();
                
            }
            //pokazywanie aktualnego zegara
            timer1.Start();

        }


        private void mySerialPort_DataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            //whatever logic and read procedure we want
            //  zamiast tego textBox1.Text = mySerialPort.ReadExisting(); trzeba użyć tego:
            textBox1.Invoke(new Action(delegate () { textBox1.AppendText(mySerialPort.ReadExisting()); }));

            OdebranaKomenda = textBox1.Text;
            if (OdebranaKomenda.Substring(OdebranaKomenda.Length - 1, 1) == "\n")  //odczyt kiedy cała komenda doszła już do bufora - sprawdzenie czy znak końca linii jest na końcu
            {
                //MessageBox.Show(OdebranaKomenda);
                string litera = OdebranaKomenda.Substring(0, 1);
                if (litera == "U") odczytanoUSB1();
                if (litera == "Y") odczytanoUSB2();
                if (litera == "A") odczytanoStanPortow();
            }
        }

        private void odczytanoUSB1()
        {
            int dlugosccalego = OdebranaKomenda.Length;
            int pozycja1dwukropka = OdebranaKomenda.IndexOf(":");
            int dlugoscliczby = pozycja1dwukropka - 1;
            //godzina
            numericUpDown1.Invoke(new Action(() => {
                decimal tempOdbior = Convert.ToDecimal(OdebranaKomenda.Substring(1, dlugoscliczby));
                if (tempOdbior > 23) tempOdbior = 23;
                numericUpDown1.Value = tempOdbior;
            }));
            //minuta
            int pozycja2dwukropka = 1 + pozycja1dwukropka + OdebranaKomenda.Substring(pozycja1dwukropka + 1, dlugosccalego - pozycja1dwukropka - 1).IndexOf(":");
            dlugoscliczby = pozycja2dwukropka - pozycja1dwukropka - 1;
            numericUpDown3.Invoke(new Action(() => {
                decimal tempOdbior = Convert.ToDecimal(OdebranaKomenda.Substring(pozycja1dwukropka + 1, dlugoscliczby));
                if (tempOdbior > 59) tempOdbior = 59;
                numericUpDown3.Value = tempOdbior;
            }));
            //czas trwania
            int pozycja3dwukropka = 1 + pozycja2dwukropka + OdebranaKomenda.Substring(pozycja2dwukropka + 1, dlugosccalego - pozycja2dwukropka - 1).IndexOf(":");
            dlugoscliczby = pozycja3dwukropka - pozycja2dwukropka - 1;
            decimal opu1 = 1;
            opu1 = Convert.ToDecimal(OdebranaKomenda.Substring(pozycja2dwukropka + 1, dlugoscliczby));
            if (opu1 < 1 || opu1 > 999) opu1 = 1;
            numericUpDown2.Invoke(new Action(() => { numericUpDown2.Value = opu1; }));
            //dni tygodnia
            dlugoscliczby = dlugosccalego - pozycja3dwukropka -3;
            decimal tempOdbior1 = Convert.ToDecimal(OdebranaKomenda.Substring(pozycja3dwukropka + 1, dlugoscliczby));
            byte dniTygodnia = 0;
            if (tempOdbior1<256 && tempOdbior1>=0) dniTygodnia = Decimal.ToByte(tempOdbior1);

            if (OperacjeBitowe.GetBit(dniTygodnia, 1))
            {
               checkBox1.Invoke(new Action(() => {checkBox1.Checked = true;}));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 2))
            {
                checkBox2.Invoke(new Action(() => { checkBox2.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 3))
            {
                checkBox3.Invoke(new Action(() => { checkBox3.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 4))
            {
                checkBox4.Invoke(new Action(() => { checkBox4.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 5))
            {
                checkBox5.Invoke(new Action(() => { checkBox5.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 6))
            {
                checkBox6.Invoke(new Action(() => { checkBox6.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 7))
            {
                checkBox7.Invoke(new Action(() => { checkBox7.Checked = true; }));
            }

           // MessageBox.Show("Odczytano ustawienia USB1", "Odczyt");
           //wyczyszczenie textboxa z tego wątku
            textBox1.Invoke(new Action(() => { textBox1.Text = ""; }));

            mySerialPort.Write("Q2");
        }

        private void odczytanoUSB2()
        {
            int dlugosccalego = OdebranaKomenda.Length;
            int pozycja1dwukropka = OdebranaKomenda.IndexOf(":");
            int dlugoscliczby = pozycja1dwukropka - 1;
            //godzina
            numericUpDown6.Invoke(new Action(() => {
                decimal tempOdbior = Convert.ToDecimal(OdebranaKomenda.Substring(1, dlugoscliczby));
                if (tempOdbior > 23) tempOdbior = 23;
                numericUpDown6.Value = tempOdbior;
            }));
            //minuta
            int pozycja2dwukropka = 1 + pozycja1dwukropka + OdebranaKomenda.Substring(pozycja1dwukropka + 1, dlugosccalego - pozycja1dwukropka - 1).IndexOf(":");
            dlugoscliczby = pozycja2dwukropka - pozycja1dwukropka - 1;
            numericUpDown4.Invoke(new Action(() => {
                decimal tempOdbior = Convert.ToDecimal(OdebranaKomenda.Substring(pozycja1dwukropka + 1, dlugoscliczby));
                if(tempOdbior > 59) tempOdbior = 59;
                numericUpDown4.Value = tempOdbior;
            }));
            //czas trwania
            int pozycja3dwukropka = 1 + pozycja2dwukropka + OdebranaKomenda.Substring(pozycja2dwukropka + 1, dlugosccalego - pozycja2dwukropka - 1).IndexOf(":");
            dlugoscliczby = pozycja3dwukropka - pozycja2dwukropka - 1;
            decimal opu1 = 1;
            opu1 = Convert.ToDecimal(OdebranaKomenda.Substring(pozycja2dwukropka + 1, dlugoscliczby));
            if (opu1 < 1 || opu1 > 999) opu1 = 1;
            numericUpDown5.Invoke(new Action(() => { numericUpDown5.Value = opu1; }));
            //dni tygodnia
            dlugoscliczby = dlugosccalego - pozycja3dwukropka - 3;
            decimal tempOdbior1 = Convert.ToDecimal(OdebranaKomenda.Substring(pozycja3dwukropka + 1, dlugoscliczby));
            byte dniTygodnia = 0;
            if (tempOdbior1 < 256 && tempOdbior1 >= 0) dniTygodnia = Decimal.ToByte(tempOdbior1);

            if (OperacjeBitowe.GetBit(dniTygodnia, 1))
            {
                checkBox14.Invoke(new Action(() => { checkBox14.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 2))
            {
                checkBox13.Invoke(new Action(() => { checkBox13.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 3))
            {
                checkBox12.Invoke(new Action(() => { checkBox12.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 4))
            {
                checkBox11.Invoke(new Action(() => { checkBox11.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 5))
            {
                checkBox10.Invoke(new Action(() => { checkBox10.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 6))
            {
                checkBox9.Invoke(new Action(() => { checkBox9.Checked = true; }));
            }
            if (OperacjeBitowe.GetBit(dniTygodnia, 7))
            {
                checkBox8.Invoke(new Action(() => { checkBox8.Checked = true; }));
            }
            textBox1.Invoke(new Action(() => { textBox1.Text = ""; }));

            mySerialPort.Write("Q0");
        }

        private void odczytanoStanPortow()
        {
            int dlugosccalego = OdebranaKomenda.Length;
            int pozycja1dwukropka = OdebranaKomenda.IndexOf(":");
            int dlugoscliczby = pozycja1dwukropka - 1;
            bool przelacz = false;
            if (OdebranaKomenda.Substring(1, 1)=="1")  //port 1
            {
                przelacz = false;
            }
            else
            {
                przelacz = true;
            }
            pictureBox6.Invoke(new Action(() => { pictureBox6.Visible = przelacz; }));
            pictureBox11.Invoke(new Action(() => { pictureBox11.Visible = !przelacz; }));
            przelacz = false;
            if (OdebranaKomenda.Substring(4, 1) == "1") //port 2
            {
                przelacz = false;
            }
            else
            {
                przelacz = true;
            }
            pictureBox7.Invoke(new Action(() => { pictureBox7.Visible = przelacz; }));
            pictureBox12.Invoke(new Action(() => { pictureBox12.Visible = !przelacz; }));
            textBox1.Invoke(new Action(() => { textBox1.Text = ""; }));
        }


            private void button2_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                mySerialPort.Write("Q3");
                textBox1.Text = "";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cmbPort.Items.Count > 0)
            {
                mySerialPort.Close();  //zamyka port przy wyjściu z formy
                isPortOpen = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbPort.Items.Count > 0)
                {
                    if (mySerialPort.IsOpen == false)
                    { //if not open, open the port

                        mySerialPort = new SerialPort();
                        mySerialPort.PortName = cmbPort.Text;
                        mySerialPort.BaudRate = 9600;
                        mySerialPort.Parity = Parity.None;
                        mySerialPort.StopBits = StopBits.One;
                        mySerialPort.DataBits = 8;
                        mySerialPort.Handshake = Handshake.None;
                        mySerialPort.DataReceived += new SerialDataReceivedEventHandler(mySerialPort_DataRecieved);
                        mySerialPort.Open();
                        button1.Text = "Zamknij port";
                        notifyIcon1.BalloonTipText  = "Podłączony " + cmbPort.Text;
                        pictureBox3.Visible = true;
                        pictureBox2.Visible = false;
                        isPortOpen = true;
                        textBox1.Text = "";
                        mySerialPort.Write("Q1");
                    }
                    else
                    {
                        mySerialPort.Close();
                        button1.Text = "Otwórz port";
                        notifyIcon1.BalloonTipText = "niepodłączony";
                        pictureBox3.Visible = false;
                        pictureBox2.Visible = true;
                        isPortOpen = false;
                    }
                }

                //mySerialPort.Write("q");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Błąd otwarcia: Port jest zajęty.", cmbPort.Text);
                mySerialPort.Close();
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("Błąd otwarcia: Port jest zajęty przez inny proces.", cmbPort.Text);
                mySerialPort.Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Błąd: Port jest już otwarty.", cmbPort.Text);
                mySerialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił nieznany błąd otwarcia portu com...", cmbPort.Text);
                mySerialPort.Close();
            }
            //jak się port udało otworzyć to:
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTeraz.Text = DateTime.Now.ToString("HH : mm : ss  ddd", new CultureInfo("pl-PL"));
            if (isPortOpen)
            {
                minutyDwie++;
                if (minutyDwie > 30)
                {
                    minutyDwie = 0;
                    mySerialPort.Write("Q0");
                    textBox1.Text = "";
                }
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(100);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Close the form, which closes the application.
            this.Close();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                if (backLite == true)
                {
                    mySerialPort.Write("W5");
                    backLite = false;
                }
                else
                {
                    mySerialPort.Write("W6");
                    backLite = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                DialogResult result = MessageBox.Show("Włączyć port 1", "Włączanie...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    mySerialPort.Write("W1");
                    textBox1.Text = "";
                }

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                DialogResult result = MessageBox.Show("Włączyć port 2", "Włączanie...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    mySerialPort.Write("W2");
                    textBox1.Text = "";
                }
            }
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
               cb.ImageIndex = 1;
            else
               cb.ImageIndex = 0;


            /*
            string str = Controls.OfType<CheckBox>()
                         .Where(ch => ch.Checked)
                         .Aggregate(new StringBuilder(),
                                    (sb, ch) => sb.Append(ch.Name),
                                    sb => sb.ToString());
            MessageBox.Show(str);
            */
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                DialogResult result = MessageBox.Show("Ustawić czas?", "Synchronizacja czasu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // ThhmmssWDDMMYYYY
                    String czas;
                    czas = "T" + DateTime.Now.ToString("HHmmss"); 
                    czas+= (int)DateTime.Now.DayOfWeek + DateTime.Now.ToString("ddMMyyyy");
                    textBox1.Text = "";
                    mySerialPort.Write(czas);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                DialogResult result = MessageBox.Show("Przesłać ustawienia 1 usb?", "Synchronizacja ustawień", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // UggmmWWWttt
                    String czas = "U";
                    decimal wartosc = numericUpDown1.Value;
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;
                    wartosc = numericUpDown3.Value;
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;

                    int dni = 0;
                    if (checkBox1.Checked) dni = 2;
                    if (checkBox2.Checked) dni += 4;
                    if (checkBox3.Checked) dni += 8;
                    if (checkBox4.Checked) dni += 16;
                    if (checkBox5.Checked) dni += 32;
                    if (checkBox6.Checked) dni += 64;
                    if (checkBox7.Checked) dni += 128;

                    if (dni < 100) czas += "0";
                    if (dni < 10) czas += "0";
                    czas += dni;

                    wartosc = numericUpDown2.Value;
                    if (wartosc < 100) czas += "0";
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;
                    textBox1.Text = "";
                    mySerialPort.Write(czas);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                DialogResult result = MessageBox.Show("Przesłać ustawienia 2 usb?", "Synchronizacja ustawień", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // YggmmWWWttt
                    String czas = "Y";
                    decimal wartosc = numericUpDown6.Value;
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;
                    wartosc = numericUpDown4.Value;
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;


                    int dni = 0;
                    if (checkBox14.Checked) dni = 2;
                    if (checkBox13.Checked) dni += 4;
                    if (checkBox12.Checked) dni += 8;
                    if (checkBox11.Checked) dni += 16;
                    if (checkBox10.Checked) dni += 32;
                    if (checkBox9.Checked) dni += 64;
                    if (checkBox8.Checked) dni += 128;

                    if (dni < 100) czas += "0";
                    if (dni < 10) czas += "0";
                    czas += dni;

                    wartosc = numericUpDown5.Value;
                    if (wartosc < 100) czas += "0";
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;
                    textBox1.Text = "";
                    mySerialPort.Write(czas);
                }
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                if (backLite == true)
                {
                    mySerialPort.Write("W5");
                    backLite = false;
                }
                else
                {
                    mySerialPort.Write("W6");
                    backLite = true;
                }
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                bumPbox.Location = pictureBox5.Location;
                timer2.Start();
                if (backLite == true)
                {
                    mySerialPort.Write("W5");
                    backLite = false;
                }
                else
                {
                    mySerialPort.Write("W6");
                    backLite = true;
                }
            }
        }

        private void pictureBox4_Click_1(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                bumPbox.Location = pictureBox4.Location;
                timer2.Start();
                DialogResult result = MessageBox.Show("Ustawić czas?", "Synchronizacja czasu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // ThhmmssWDDMMYYYY
                    String czas;
                    czas = "T" + DateTime.Now.ToString("HHmmss");
                    czas += (int)DateTime.Now.DayOfWeek + DateTime.Now.ToString("ddMMyyyy");
                    textBox1.Text = "";
                    mySerialPort.Write(czas);
                }
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                bumPbox.Location = pictureBox6.Location;
                timer2.Start();
                DialogResult result = MessageBox.Show("Włączyć port 1", "Włączanie...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    mySerialPort.Write("W1");
                    textBox1.Text = "";
                    pictureBox6.Visible = false;
                    pictureBox11.Visible = true;
                }

            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                bumPbox.Location = pictureBox7.Location;
                timer2.Start();
                DialogResult result = MessageBox.Show("Włączyć port 2", "Włączanie...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    mySerialPort.Write("W2");
                    textBox1.Text = "";
                    pictureBox7.Visible = false;
                    pictureBox12.Visible = true;
                }
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                bumPbox.Location = pictureBox8.Location;
                timer2.Start();
                DialogResult result = MessageBox.Show("Przesłać ustawienia 1 usb?", "Synchronizacja ustawień", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // UggmmWWWttt
                    String czas = "U";
                    decimal wartosc = numericUpDown1.Value;
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;
                    wartosc = numericUpDown3.Value;
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;

                    int dni = 0;
                    if (checkBox1.Checked) dni = 2;
                    if (checkBox2.Checked) dni += 4;
                    if (checkBox3.Checked) dni += 8;
                    if (checkBox4.Checked) dni += 16;
                    if (checkBox5.Checked) dni += 32;
                    if (checkBox6.Checked) dni += 64;
                    if (checkBox7.Checked) dni += 128;

                    if (dni < 100) czas += "0";
                    if (dni < 10) czas += "0";
                    czas += dni;

                    wartosc = numericUpDown2.Value;
                    if (wartosc < 100) czas += "0";
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;
                    textBox1.Text = "";
                    mySerialPort.Write(czas);
                }
            }
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                bumPbox.Location = pictureBox9.Location;
                timer2.Start();
                DialogResult result = MessageBox.Show("Przesłać ustawienia 2 usb?", "Synchronizacja ustawień", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // YggmmWWWttt
                    String czas = "Y";
                    decimal wartosc = numericUpDown6.Value;
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;
                    wartosc = numericUpDown4.Value;
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;


                    int dni = 0;
                    if (checkBox14.Checked) dni = 2;
                    if (checkBox13.Checked) dni += 4;
                    if (checkBox12.Checked) dni += 8;
                    if (checkBox11.Checked) dni += 16;
                    if (checkBox10.Checked) dni += 32;
                    if (checkBox9.Checked) dni += 64;
                    if (checkBox8.Checked) dni += 128;

                    if (dni < 100) czas += "0";
                    if (dni < 10) czas += "0";
                    czas += dni;

                    wartosc = numericUpDown5.Value;
                    if (wartosc < 100) czas += "0";
                    if (wartosc < 10) czas += "0";
                    czas += wartosc;
                    textBox1.Text = "";
                    mySerialPort.Write(czas);
                }
            }
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            if (isPortOpen)
            {
                bumPbox.Location = pictureBox10.Location;
                timer2.Start();
                mySerialPort.Write("Q3");
                textBox1.Text = "";
            }
        }

        private void cmbPort_Click(object sender, EventArgs e)  //odczyt wszystkich portów do comboboxa
        {
            string[] ports = SerialPort.GetPortNames();
            cmbPort.Items.Clear();
            cmbPort.Text = "";
            foreach (string port in ports)
            {
                cmbPort.Items.Add(port);
            }
        } 

        private void timer2_Tick(object sender, EventArgs e)
        {
            bumPbox.Location = new Point(0, 590);
            timer2.Stop();
        }

    }






    public static class OperacjeBitowe
    {
        public static bool GetBit(this byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }
    }

}
