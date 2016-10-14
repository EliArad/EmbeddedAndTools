using PowerMeterApp;
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
    public partial class Settings : Form
    {
        public struct SettingsData
        {
            public bool Debug;
            public double drPower;
        }
        SettingsData m_set = new SettingsData();
        string[] m_sensorsName = new string[2];
        public Settings()
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;             
        }

        private void button1_Click(object sender, EventArgs e)
        {
                    
        }

        void LoadAssigments(ref PowerMeterApp.PowerMeterForm.PowerMeterSelect[] pmSelect)
        {

            

        }
        void SaveAssigments(PowerMeterApp.PowerMeterForm.PowerMeterSelect[] pmSelect)
        {
           
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            ManagedUtensilsForm f = new ManagedUtensilsForm();
            f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {             
            m_set.Debug = checkBox1.Checked;             
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
        public SettingsData GetSettings()
        {
            return m_set;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
         
        }
    }
}
