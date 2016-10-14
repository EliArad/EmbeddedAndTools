using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace TopazMultiWCFApp
{
    public partial class LogoWindow : Form
    {
        public delegate int LogWindowCallback(byte code);

        protected LogWindowCallback m_pCallback = null;
        public LogoWindow()
        {
            InitializeComponent();
        
        }
        public void callback(LogWindowCallback b)
        {
            m_pCallback = b;
        }
        public void MessageBox(string msg)
        {
            bool wasVisible = false;
            if (Visible == true)
            {
                wasVisible = true;
                this.Hide();
            }
            System.Windows.Forms.MessageBox.Show(msg);
            if (wasVisible == true)
            {
                this.Show();
            }

        }
        public void setMessage(string message)
        {
            label1.Text = message;
        }

        public void setNoConnectionMessage()
        {
            label1.Text = "No connection" + Environment.NewLine + "Run ./start_goji.sh";
            if (label1.ForeColor == System.Drawing.Color.Red)
            {
                label1.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                label1.ForeColor = System.Drawing.Color.Red;
            }
            
         
        }
  
        private void button1_Click(object sender, EventArgs e)
        {
           if (m_pCallback != null)
               m_pCallback(0);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
           Application.DoEvents();
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
           Application.DoEvents();
        }
    }
}
