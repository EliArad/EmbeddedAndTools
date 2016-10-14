using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazApp.Forms
{
    public partial class PictureDescrptionForm : Form
    {

        string m_description;
        public PictureDescrptionForm(string url, string desc)
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            textBox1.Text = desc;
            pictureBox1.ImageLocation = url;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        public string Descrption
        {
            get
            {
                return m_description;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            m_description = textBox1.Text;
            Close();
        }
    }
}
