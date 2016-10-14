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

namespace TopazApp.AlgoParamsForms
{
    public partial class freqtableform1 : Form
    {
        bool[] m_frequencies = new bool[POCSET.MAXFREQ];
        string m_algoName;
        int m_sid;
        int m_rowIndex;
        string m_fileName;
        public freqtableform1(int sid, int rowindex, string algoName)
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
                        AlgoThresholParams? d = MySQLConnector.GetAlgoThreaholdParams(m_sid, m_rowIndex);
                        if (d != null)
                        {
                            if (d.Value.freqtablefilename.ToLower() == "all")
                            {
                                // it is alreay clreared.
                                listBox1.Items.Clear();
                            }
                            else
                            {
                                fileName = d.Value.freqtablefilename;
                            }
                        }
                    }
                break;
                case "equal energy":
                {
                    AlgoEqualEnergyParams? d = MySQLConnector.GetAlgoEqualEnergyParams(m_sid, m_rowIndex);
                    if (d != null)
                    {
                        if (d.Value.freqtablefilename.ToLower() == "all")
                        {
                            // it is alreay clreared.
                            listBox1.Items.Clear();
                        }
                        else
                        {
                            fileName = d.Value.freqtablefilename;
                        }
                    }
                }
                break;
                case "Top Percentage":
                {
                    AlgoTopPercentageParams? d = MySQLConnector.GetAlgoTopPercentageParams(m_sid, m_rowIndex);
                    if (d != null)
                    {
                        if (d.Value.freqtablefilename.ToLower() == "all")
                        {
                            // it is alreay clreared.
                            listBox1.Items.Clear();
                        }
                        else
                        {
                            fileName = d.Value.freqtablefilename;
                        }
                    }
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
          
        private void freqtableform1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
