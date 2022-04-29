using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace ScanQRcodeWithPython
{
    
    public partial class Form1 : Form
    {

        //private object syncGate = new object();
        private Process process;
        private StringBuilder output = new StringBuilder();
        private bool outputChanged;
        Form1.User user = new Form1.User();
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)// run file.py in one click
        {
            // lock (syncGate) // synchronized when scan qr and at the time result print in box
            // {
            //if (process != null) return; //if you use, you cannot open qrcode again after closing
            // }

            //output.Clear(); // use if want to clear after close box
            outputChanged = false;
            richTextBox1.Text = "";
            process = new Process();
            process.StartInfo.FileName = @"C:\Users\LENOVO\AppData\Local\Programs\Python\Python310\python.exe"; //change in other lap
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = Application.StartupPath + @"\scanqrcode.py"; // change in other lap
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += OnOutputDataReceived;
            process.Exited += OnProcessExited;
            process.Start();
            process.BeginOutputReadLine();
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //lock (syncGate) ensures that one thread is executing a piece of code at one time
            //{
            if (sender != process) return;
            output.Clear();
            output.AppendLine(e.Data);                
                if (outputChanged) return;
                outputChanged = true;
            BeginInvoke(new Action(OnOutputChanged));
           // richTextBox1.Text = output.ToString();
            //}
        }

        private void OnOutputChanged()
        {
            // lock (syncGate)
            //{
            //richTextBox1.Text = output.ToString();
            //Console.WriteLine(output.ToString());
            CheckQR(output.ToString());

            //await check(output.ToString(), user);
            //Console.WriteLine("abc");
            //richTextBox1.Text += "abc\n";
            //richTextBox1.Text += "cde";

            outputChanged = false;
            //}
        }
        private void OnProcessExited(object sender, EventArgs e) //check process exits 
        {
            //lock (syncGate)
           // {
                if (sender != process) return;
                process.Dispose();
                process = null;
            //}
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)//scroll text when new text appear
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;            
            richTextBox1.ScrollToCaret();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://hehelandfilm.com.vn"); //change to http of heheland
        }
        public async Task CheckQR(string QRCode)
        {
            //Initialize a Client
            HttpClient httpClient = new HttpClient();

            if (user.token == "")
            {
                //Admin Login Information
                string adminLogin = "{\"username\": \"" + user.username + "\", \"password\": \"" + user.password + "\"}";

                //Creating a new Request Message for Login
                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri("http://127.0.0.1:8000/api/QuanLyNguoiDung/login/");
                httpRequestMessage.Content = new StringContent(adminLogin, Encoding.UTF8, "application/json");

                //Send Login Information to the Server and get the Response (which includes Token)
                var res = await httpClient.SendAsync(httpRequestMessage);
                var resContent = await res.Content.ReadAsStringAsync();
                var resJSON = JObject.Parse(resContent);

                //Get Token from Response
                var token = resJSON.GetValue("key");
                if (token != null)
                {
                    user.token = token.ToString();
                }
                else
                {
                    throw new Exception("Wrong Username or Password");
                }
            }

            //Get QR Code Information
            string checkQRurl = $"http://127.0.0.1:8000/api/KiemTraDatVe?maQR={QRCode}";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var qrRes = await httpClient.GetAsync(checkQRurl);
            string qrResContent = await qrRes.Content.ReadAsStringAsync();

            if (qrResContent == "")
            {
                richTextBox1.Text = "QR Code is invalid";
            }
            else
            {
                var qrResJSON = JArray.Parse(qrResContent)[0].ToObject<JObject>();

                if (qrResJSON!.GetValue("isValid")!.ToString() == "False") {
                    richTextBox1.Text = ("This QR Code has already been scanned !!!!!\n");
                }
                else {
                    richTextBox1.Text = ($"QR Code: {qrResJSON.GetValue("maQR")}\n") +
                    ($"Account Name: {qrResJSON.GetValue("taiKhoanNguoiDat")}\n") +
                    ($"Total Price: {qrResJSON.GetValue("giaVe")}\n") +
                    ($"Chairs: {qrResJSON.GetValue("danhSachMaGhe")}\n") +
                    ($"Movie Name: {qrResJSON!.GetValue("tenPhim")}\n") +
                    ($"Theater Name: {qrResJSON!.GetValue("tenRap")}\n") +
                    ($"Date: {qrResJSON!.GetValue("ngayChieu")}\n") +
                    ($"Time: {qrResJSON!.GetValue("gioChieu")}\n")
                    ;

                    //Switch from valid to unvalid
                    string stopValidJSON = "{\"isValid\": false}";

                    //Creating a new Request Message to Server
                    var httpRequestMessage = new HttpRequestMessage();
                    httpRequestMessage.Method = HttpMethod.Put;
                    httpRequestMessage.RequestUri = new Uri($"http://127.0.0.1:8000/api/KiemTraDatVe/{QRCode}");
                    httpRequestMessage.Content = new StringContent(stopValidJSON, Encoding.UTF8, "application/json");

                    //Send Unvalid Message to Server
                    await httpClient.SendAsync(httpRequestMessage);
                }
            }
        }
        class User
        {
            public string username = "admin";
            public string password = "123456";
            public string token = "";
        }

    }

 }
