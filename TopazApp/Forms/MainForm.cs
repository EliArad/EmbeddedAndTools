using Microsoft.Win32;
using RegistryClassApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApp;
using TopazApp.Forms;
using TopazApp.TOPNAT;
using TopazApp.MYSQL;
using NetworkDrivesApi;
using TopazApi;
 

namespace TopazApp
{

    public partial class MainForm : Form
    {
        usersDb m_users = new usersDb();
        OvenInfo m_ovenInfo;
        AutoResetEvent m_dayliEvent = new AutoResetEvent(false);
        Thread m_threadSend = null;
        bool m_debug = false;
        string m_mode;
        bool m_close = false;
        public MainForm()
        {
            InitializeComponent();
            comboBox1.Items.Add("Admin");
            try
            {
                MySQLConnector.Initialize("192.168.10.64", "root", "1234");
                Queris.Initialize("192.168.10.64", "root", "1234");
                m_mode = Properties.Settings.Default.mode;
                List<User> usersList = MySQLConnector.GetAllUsers();
                foreach (User u in usersList)
                {
                    comboBox1.Items.Add(u.email);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }

            try
            {
                string guid = Guid.NewGuid().ToString();
                clsRegistry reg = new clsRegistry();
                string fieldGuid = reg.GetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid");
                if (reg.strRegError != null)
                {
                    reg.SetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid", guid);
                }
                else
                {
                    if (fieldGuid == string.Empty || fieldGuid == null)
                    {
                        reg.SetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid", guid);
                    }
                }
                fieldGuid = reg.GetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid");
                label5.Text = fieldGuid;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            
            button1.Enabled = false;
            loadSettings();

            //BuildChefDemo f = new BuildChefDemo();
            //f.ShowDialog();

            linkLabel1.Visible = false;
            linkLabel2.Visible = false;
            //linkLabel4.Visible = false;

            char c = DRIVE.Drive[0];
            if (c == 'Y')
            {

                try
                {
                    if (File.Exists("Y:\\TopazPOC\\readme.txt") == false)
                        NetworkDrives.MapDrive("Y", "\\192.168.10.64\\Goji", "rfteam", "Helix123");
                }
                catch (Exception err)
                {

                }

                if (File.Exists("Y:\\TopazPOC\\readme.txt") == false)
                {
                    MessageBox.Show("POC software needs to have a network drive Y mapped to RDB server");
                    m_close = true;
                }
            }
        }
          
        void loadSettings()
        {
            comboBox1.Text = Properties.Settings.Default.LoginName;
        }

        DialogResult _MessageBox(string message, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {

            string caption = "POC Cooking";
            DialogResult result;
            // Displays the MessageBox.
            result = MessageBox.Show(this, message, caption, buttons);
            return result;
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (comboBox1.Text == string.Empty)
            {
                MessageBox.Show("Enter user name please");
                return;
            }
            if (textBox2.Text == string.Empty)
            {
                MessageBox.Show("Enter password please");
                return;
            }

            try
            {
                int userid = -1;
                if ((userid = m_users.CheckAuthtintication(comboBox1.Text, 
                                                           textBox2.Text)) != -1)
                {
                    m_mode = Properties.Settings.Default.mode;
                    if (m_mode == "chef")
                    {
                        GenerateGuid(userid, true);
                        Chef f = new Chef(userid, comboBox1.Text, m_ovenInfo);
                        f.ShowDialog();
                        this.Hide();
                        f.ShowDialog();
                        this.Show();
                    }
                    else
                    {
                        GenerateGuid(userid, true);
                        Form1 f = new Form1(userid,
                                            comboBox1.Text,
                                            m_debug, 
                                            m_ovenInfo);
                        this.Hide();
                        f.ShowDialog();
                        this.Show();
                    }
                   
                }
                else
                {
                    _MessageBox("User name or password are incorrect");
                }              
            }
            catch (Exception err)
            {
                _MessageBox("Error #100:" + err.Message);
                this.Show();


                if (err.Message.Contains("KMTronics HW Switches Initializtion error: Kmtronic : A device attached to the system is not functioning"))
                {                                       
                    RmoveKMTronic();
                    Thread.Sleep(2000);
                    RescanUSB();
                    Thread.Sleep(2000);
                } else 
                if (err.Message.Contains("KMTronics HW Switches Initializtion error: Kmtronic : The PortName cannot be empty"))
                {
                    RmoveKMTronic();
                    Thread.Sleep(2000);
                    RescanUSB();
                    Thread.Sleep(2000);
                }
            }
        }
         
        void RescanUSB()
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "devcon.exe";
                proc.StartInfo.Arguments = "rescan";
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        void RmoveKMTronic()
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "devcon.exe";
                proc.StartInfo.Arguments = @"/r remove USB\VID_04D8&PID_FEF9";
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_dayliEvent.Set();
            if (m_threadSend != null)
                m_threadSend.Join();

            Properties.Settings.Default.LoginName = comboBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            onChangeInputs();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            onChangeInputs();
        }
        bool GenerateGuid(int userid, bool save)
        {

            try
            {
                string guid = Guid.NewGuid().ToString();
                clsRegistry reg = new clsRegistry();
                string fieldGuid = reg.GetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid");
                if (reg.strRegError != null)
                {
                    reg.SetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid", guid);
                }
                else
                {
                    if (fieldGuid == string.Empty || fieldGuid == null)
                    {
                        reg.SetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid", guid);
                    }
                }
                if (save == true)
                {
                    try
                    {
                        m_ovenInfo = MySQLConnector.GetOvenInfo(fieldGuid);
                    }
                    catch (Exception err)
                    {
                        try
                        {
                            fieldGuid = reg.GetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid");
                            OvenInfo info = MySQLConnector.GetOvenInfo(fieldGuid);
                        }
                        catch (Exception err1)
                        {
                            if (err1.Message.ToLower() == "not found")
                            {
                                CreateNewPAInfoForm f = new CreateNewPAInfoForm(fieldGuid, userid);
                                f.ShowDialog();
                                m_ovenInfo = MySQLConnector.GetOvenInfo(fieldGuid);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }
        void onChangeInputs()
        {
            if (comboBox1.Text.ToLower() == "admin" && textBox2.Text == "Goji3838")
            {
                linkLabel2.Visible = true;
                linkLabel1.Visible = true;
                return;
            }
            else
            {
                linkLabel1.Visible = false;
                linkLabel2.Visible = false;
                //linkLabel4.Visible = false;
            }

            if (textBox2.Text != string.Empty && comboBox1.Text != string.Empty)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
             
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateUser c = new CreateUser();
            c.ShowDialog();

            comboBox1.Items.Clear();
            List<User> usersList = MySQLConnector.GetAllUsers();
            foreach (User u in usersList)
            {
                comboBox1.Items.Add(u.email);
            }

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            if (comboBox1.Text == string.Empty)
            {
                _MessageBox("Enter user name please");
                return;
            }
            if (textBox2.Text == string.Empty)
            {
                _MessageBox("Enter password please");
                return;
            }
            try
            {
                int userid;
                if ((userid = m_users.CheckAuthtintication(comboBox1.Text, textBox2.Text)) != -1)
                {
                    Form1 f = new Form1(userid, comboBox1.Text, m_debug, m_ovenInfo);
                    f.ShowDialog();
                }
                else
                {
                    _MessageBox("User name or password are incorrect");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                this.Show();
            }
        }

        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ListUsersOperators f = new ListUsersOperators();
            f.ShowDialog();
        }

        
        private void linkLabel4_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Settings s = new Settings();
            if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_debug = s.GetSettings().Debug;
                Properties.Settings.Default.Save();
            }
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tpnatform f = new tpnatform();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void label5_Click(object sender, EventArgs e)
        {

            clsRegistry reg = new clsRegistry();
            string fieldGuid = reg.GetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid");
            if (reg.strRegError == null)
            {
                PAInfoForm f = new PAInfoForm(fieldGuid);
                f.ShowDialog();
            }
            else
            {
                MessageBox.Show("Error getting guid");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            onChangeInputs();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (m_close == true)
            {
                Close();
            }
        }
    }
}
