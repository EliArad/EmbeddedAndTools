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

namespace TopazMultiWCFApp.AlgoParamsForms
{
    public partial class freqtableform : Form
    {
        bool[] m_frequencies = new bool[POCSET.MAXFREQ];
        string m_algoName;
        int m_sid;
        int m_rowIndex;
        string m_fileName;
        public freqtableform(int sid, int rowindex, string algoName)
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            m_sid = sid;
            m_rowIndex = rowindex;
            m_algoName = algoName;
            Array.Clear(m_frequencies, 0, m_frequencies.Length);
            try
            {
                ReadFrequencies();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        public string GetFileName()
        {
            return m_fileName;
        }
        void ReadFrequencies()
        {
            string fileName = "all";
            switch(m_algoName)
            {

                case "DR Treshold":
                    {
                         
                    }
                break;
                case "equal energy":
                {
                     
                }
                break;
                case "Top Percentage":
                {
                    
                }
                break;
            }

            if (fileName != "all")
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null)
                            break;
                        int freq = int.Parse(line);
                        m_frequencies[freq - 2400] = true;
                        listBox1.Items.Add(line);
                    }
                }
            }
            m_fileName = fileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && textBox2.Text != string.Empty)
            {
                int start = int.Parse(textBox1.Text);
                int stop = int.Parse(textBox2.Text);
                if (stop > 2500)
                {
                    MessageBox.Show("Max frequncies is 2500");
                    return;
                }
                if (start < 2400)
                {
                    MessageBox.Show("Initial frequency is 2400");
                    return;
                }
                for (int i = start; i <= stop; i++)
                {
                    int freqIndex = i - 2400;
                    m_frequencies[freqIndex] = true;
                }
                                    
                listBox1.Items.Clear();
                for (int i = 0 ; i < POCSET.MAXFREQ ;i++)
                {
                    if (m_frequencies[i] == true)
                        listBox1.Items.Add((2400 + i).ToString());
                }
            }
            else if (textBox1.Text != string.Empty && textBox2.Text == string.Empty)
            {
                int start = int.Parse(textBox1.Text);
                if (start < 2400 || start > 2500)
                {
                    MessageBox.Show("Frequencies range are 2400 - 2500");
                    return;
                }
                int freqIndex = start - 2400;
                m_frequencies[freqIndex] = true;
                listBox1.Items.Clear();
                for (int i = 0; i < POCSET.MAXFREQ; i++)
                {
                    if (m_frequencies[i] == true)
                        listBox1.Items.Add((2400 + i).ToString());
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (listBox1.Items.Count == 0)
            {
                m_fileName = "all";
            }
            else
            {
                string path = DRIVE.Drive + @"TopazPOC\SolutionFrequencies\" + m_algoName + @"\";
                Directory.CreateDirectory(path);
                string filename = m_sid + "_" + m_rowIndex;
                m_fileName = path + filename + ".txt";
                using (var writer = new StreamWriter(m_fileName))
                {
                    for (int i = 0; i < m_frequencies.Length; i++)
                    {
                        if (m_frequencies[i] == true)
                            writer.WriteLine((2400 + i).ToString());
                    }
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Note: clear means that you choose to work with all frequencies");
            listBox1.Items.Clear();
            Array.Clear(m_frequencies, 0, m_frequencies.Length);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int x = listBox1.SelectedIndex;
            if (x != -1)
            {
                int i = int.Parse(listBox1.Items[x].ToString()) - 2400;
                m_frequencies[i] = false;
                listBox1.Items.RemoveAt(x);
            }
        }
    }
}
