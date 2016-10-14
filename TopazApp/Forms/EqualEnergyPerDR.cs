using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApi;

namespace TopazApp.Forms
{
    public partial class EqualEnergyPerDR : Form
    {
        string m_algoName;
        int m_sid;
        int m_rowIndex;
        string path;
        public EqualEnergyPerDR(int sid, int rowindex, string algoName)
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            m_sid = sid;
            m_rowIndex = rowindex;
            m_algoName = algoName;
            path = DRIVE.Drive + @"TopazPOC\SolutionDRRange\";
            string fileName = path + m_sid + "_" + m_rowIndex + ".txt";
            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null)
                            break;
                        listBox1.Items.Add(line);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int x = listBox1.SelectedIndex;
            if (x != -1)
            {
                listBox1.Items.RemoveAt(x);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (numericUpDown1.Value >= numericUpDown2.Value)
            {
                MessageBox.Show("Cannot be the same or less DR values");
                return;                    
            }
            int percentage;
            bool b = int.TryParse(textBox1.Text, out percentage);
            if (b == false)
            {
                MessageBox.Show("Invalid percentage");
                return;                    
            }
            string s = string.Format("{0},{1},{2}" , numericUpDown1.Value , numericUpDown2.Value, textBox1.Text);
            listBox1.Items.Add(s);
        }

        private void button4_Click(object sender, EventArgs e)
        {

            string fileName = path + m_sid + "_" + m_rowIndex + ".txt";
            if (listBox1.Items.Count == 0)
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    string s;
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        s = listBox1.Items[i].ToString();
                        sw.WriteLine(s);
                    }
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
