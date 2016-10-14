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
    public partial class PAInfoForm : Form
    {
        string m_guid;
        OvenInfo m_ovenInfo;
        public PAInfoForm(string guid)
        {
            InitializeComponent();
            m_guid = guid;

            label9.Text = guid;

            try
            {
                OvenInfo info = MySQLConnector.GetOvenInfo(guid);
                lblDescription.Text = info.description;
                lblAlias.Text = info.Alias;
                lblAdded.Text = info.added.ToString();
                lblBy.Text = info.user.firstName + info.user.lastName;
                lblCoupler.Text = info.strCouplerSerial;
                lblCirulator.Text = info.Circulator.ToString();
                lblCableFix.Text = info.cablefix;
                m_ovenInfo = info;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EditPAInfoForm f = new EditPAInfoForm(m_ovenInfo);
            f.ShowDialog();

            OvenInfo info = MySQLConnector.GetOvenInfo(m_guid);
            m_ovenInfo = info;

            lblAlias.Text = info.Alias;
            lblDescription.Text = info.description;
            lblCoupler.Text = info.strCouplerSerial;
            lblCirulator.Text = info.Circulator.ToString();
            lblCableFix.Text = info.cablefix;

        }
    }
}
