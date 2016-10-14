using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazMultiWCFApp
{
    public partial class Form1 : Form
    {
        Thread m_connectionThread;
        bool m_active = true;
        public Form1()
        {
            InitializeComponent();

            try
            {
                topazWCFControl1.Enabled = false;
                string ipAddress = Properties.Settings.Default.WebAddress;
                if (ipAddress == null || ipAddress == string.Empty)
                {
                    WebConfigForm f = new WebConfigForm();
                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ipAddress = Properties.Settings.Default.WebAddress;
                        topazWCFControl1.SetWebAddress(ipAddress);
                        if (topazWCFControl1.Connect() == false)
                        {
                            m_connectionThread = new Thread(Connection);
                            m_connectionThread.Start();
                        }
                        else
                        {
                            topazWCFControl1.Enabled = true;
                        }
                    }
                }
                else
                {
                    topazWCFControl1.SetWebAddress(ipAddress);
                    if (topazWCFControl1.Connect() == false)
                    {
                        m_connectionThread = new Thread(Connection);
                        m_connectionThread.Start();
                    }
                    else
                    {
                        topazWCFControl1.Enabled = true;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void Connection()
        {
            while (m_active)
            {
                Thread.Sleep(1000);
                if (topazWCFControl1.Connect() == true)
                {
                    topazWCFControl1.Enabled = true;
                    return;
                }
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_active = false;
            if (m_connectionThread != null)
                m_connectionThread.Join();
            topazWCFControl1.Close();
        }
    }
}
