using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPortComm
{
    public partial class Form1 : Form
    {
        SerialPort serialPort = new SerialPort();
        System.Timers.Timer timer = new System.Timers.Timer();

        public Form1()
        {
            InitializeComponent();

            //跨线程问题
            CheckForIllegalCrossThreadCalls = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.button1.Text = "打开串口";
            this.button1.BackColor = Color.Green;

            //委托接收信息
            serialPort.DataReceived += new SerialDataReceivedEventHandler(data_Recive);

            //设置串口
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox1.Items.AddRange(ports);
            comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;

            //默认关闭端口
            serialPort.Close();

            //波特率
            this.comboBox2.SelectedIndex = 0;

            //校验位
            this.comboBox4.SelectedIndex = 0;

            //数据位
            this.comboBox3.SelectedIndex = 0;

            //停止位
            this.comboBox5.SelectedIndex = 1;

            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.button1.Text == "打开串口")
                {
                    this.button1.Text = "关闭串口";
                    this.button1.BackColor = Color.Red;
                    serialPort.Open();
                    this.textBox2.Text = serialPort.ReadExisting();
                }
                else
                {
                    this.button1.Text = "打开串口";
                    this.button1.BackColor = Color.Green;
                    serialPort.Close();
                }
            }
            catch (IOException)
            {
                MessageBox.Show("参数设置有误！");
                this.button1.Text = "打开串口";
                this.button1.BackColor = Color.Green;
                serialPort.Close();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "打开串口")
            {
                MessageBox.Show("未打开串口！");
            }
            else
            {
                if (this.checkBox3.Checked == false)
                {
                    serialPort.WriteLine(this.textBox3.Text);
                }
                else
                {
                    serialPort.WriteLine(ascii2String(this.textBox3.Text));
                }
                
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //获取串口
            serialPort.PortName = comboBox1.SelectedItem.ToString();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //设置波特率
            serialPort.BaudRate = int.Parse(comboBox2.SelectedItem.ToString());
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //设置校验位
            //serialPort.Parity = Parity.None;
            //MessageBox.Show(comboBox3.SelectedItem.ToString());
            switch (comboBox4.SelectedItem.ToString())
            {
                case "None":
                    serialPort.Parity = Parity.None;
                    break;
                case "Odd":
                    serialPort.Parity = Parity.Odd;
                    break;
                case "Even":
                    serialPort.Parity = Parity.Even;
                    break;
                case "Mark":
                    serialPort.Parity = Parity.Mark;
                    break;
                case "Space":
                    serialPort.Parity = Parity.Space;
                    break;

            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //设置数据位
            serialPort.DataBits = int.Parse(this.comboBox3.SelectedItem.ToString());
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            //设置停止位
            serialPort.StopBits = StopBits.One;
            switch (comboBox5.SelectedItem.ToString())
            {
                case "None":
                    MessageBox.Show("不能设置为None！");
                    this.comboBox5.SelectedIndex = 1;
                    break;
                case "One":
                    serialPort.StopBits = StopBits.One;
                    break;
                case "OnePointFive":
                    serialPort.StopBits = StopBits.OnePointFive;
                    break;
                case "Two":
                    serialPort.StopBits = StopBits.Two;
                    break;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = string.Empty;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.textBox3.Text = string.Empty;
        }

        private void data_Recive(object sender, EventArgs e) 
        {
            /*Byte[] readBuffer = new Byte[serialPort.BytesToRead];
            serialPort.Read(readBuffer, 0, readBuffer.Length);*/
            string finalRecive = serialPort.ReadLine();
            if (this.checkBox2.Checked == false)
            {
                this.textBox2.Text += finalRecive;
                /*foreach (char a in readBuffer)
                {
                    this.textBox2.Text += a;
                }*/
            }
            else
            {
                
                this.textBox2.Text += string2Ascii(finalRecive) ;//ascii2String(serialPort.ReadLine());
                /* string s = string.Empty;
                 foreach (char a in readBuffer)
                 {
                     this.textBox2.Text += a;
                 }*/

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    if (this.checkBox1.Checked == true)
                    {
                        if (this.textBox1.Text != null)
                        {
                            timer.Interval = int.Parse(this.textBox1.Text);
                            timer.Elapsed += new System.Timers.ElapsedEventHandler(button5_Click); // 到时间后执行
                            timer.AutoReset = true; // 是否一直执行
                            timer.Enabled = true; // 是否执行
                            timer.Start(); // 开始
                        }
                        else
                        {
                            MessageBox.Show("请输入正确的时间间隔！");
                        }
                    }   
                    else
                    {
                        timer.AutoReset = false; // 是否一直执行
                        timer.Enabled = false;
                        timer.Stop();
                    }
                }
                else
                {
                    MessageBox.Show("未打开串口1！");
                    this.checkBox1.Checked = false;
                   
                }
                
            }
            catch (FormatException)
            {
                MessageBox.Show("请正确设置周期！");
            }
            
        }
        private string string2Ascii(string s)
        {
            byte[] b = Encoding.Default.GetBytes(s);
            string temp = string.Empty;
            foreach (byte c in b)
                temp += c.ToString("X");
            return temp;
        }

        private string ascii2String(string s)
        {
            if (s.Length % 2 != 0)
            {
                s += "0";
                char[] c = s.ToCharArray();
                c[c.Length - 1] = c[c.Length - 2];
                c[c.Length - 2] = '0';
                s = new string(c);
            }
            byte[] buff = new byte[s.Length / 2];
            int index = 0;
            for (int i = 0; i < s.Length; i += 2)
            {
                buff[index++] = Convert.ToByte(s.Substring(i, 2), 16);    //16进制转换
            }
            //string result = Encoding.ASCII.GetString(buff);
            //MessageBox.Show(result);
            return Encoding.ASCII.GetString(buff);
        }
        
    }
}

