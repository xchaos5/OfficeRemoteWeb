using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSFTwifiAssistant
{
    public partial class MainForm : Form
    {
        //private static bool autoConfigOff = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tbSSID.Text = Environment.MachineName;
            tbPIN.Text = "12345678";
        }

        private string runCommand(string str)
        {
            string strOutput;

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/Q";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            
            p.StandardInput.WriteLine(str);
            p.StandardInput.WriteLine("exit");

            strOutput = p.StandardOutput.ReadToEnd();

            strOutput = Regex.Replace(strOutput, "Microsoft\\s+Windows.*reserved\\.", "", RegexOptions.Singleline);
            strOutput = strOutput.Replace(Environment.CurrentDirectory, "");
            strOutput = strOutput.Replace("\r\n>", "");
            p.WaitForExit();

            return strOutput;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ssid = tbSSID.Text;
            string pin = tbPIN.Text;
            if (String.IsNullOrEmpty(ssid))
            {
                MessageBox.Show("Please input SSID.");
                return;
            }
            if (String.IsNullOrEmpty(pin))
            {
                MessageBox.Show("Please input PIN.");
                return;
            }
            if (pin.Length < 8)
            {
                MessageBox.Show("PIN must be at least 8 charactors.");
                return;
            }

            string cmd = "netsh wlan set hostednetwork mode=allow ssid=" + ssid + " key=" + pin;
            string outStr = runCommand(cmd);
            appendLog(outStr);
        }

        private void appendLog(string outStr)
        {
            rtbLog.AppendText(String.Format("{0}: {1}\r\n", DateTime.Now, outStr));
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.ScrollToCaret();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string cmd = "netsh wlan start hostednetwork";
            string outStr = runCommand(cmd);
            appendLog(outStr);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            string cmd = "netsh wlan stop hostednetwork";
            string outStr = runCommand(cmd);
            appendLog(outStr);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbLog.Text = String.Empty;
        }

        private void btnTurnOff_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    var sc = new ServiceController("WLAN AutoConfig");
            //    sc.Stop();
            //    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
            //    if (sc.Status != ServiceControllerStatus.Stopped)
            //    {
            //        appendLog("Failed to turn on WLAN AutoConfig: Timeout");
            //    }
            //    else
            //    {
            //        autoConfigOff = true;
            //        appendLog("WLAN AutoConfig service stopped.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Failed to turn off WLAN AutoConfig service: " + ex.ToString());
            //}

            string cmd = "netsh wlan disconnect";
            string outStr = runCommand(cmd);
            appendLog(outStr);
        }

        private void btnTurnOn_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    var sc = new ServiceController("WLAN AutoConfig");
            //    sc.Start();
            //    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
            //    if (sc.Status != ServiceControllerStatus.Running)
            //    {
            //        appendLog("Failed to turn on WLAN AutoConfig: Timeout");
            //    }
            //    else
            //    {
            //        autoConfigOff = false;
            //        appendLog("WLAN AutoConfig service started.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Failed to turn on WLAN AutoConfig service: " + ex.ToString());
            //}
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (autoConfigOff)
            //{
            //    if (MessageBox.Show("You have turned off WLAN AutoConfig, are you sure not to turn on before quit?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No)
            //    {
            //        e.Cancel = true;
            //    }
            //}
        }
    }
}
