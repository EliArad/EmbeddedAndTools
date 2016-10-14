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
    public partial class EditPAInfoForm : Form
    {
        OvenInfo m_ovenInfo;
        public EditPAInfoForm(OvenInfo info)
        {
            InitializeComponent();
            m_ovenInfo = info;

            lblAlias.Text = info.Alias;
            lblDescription.Text = info.description;
            lblCoupler.Text = info.strCouplerSerial;
            checkBox1.Checked = info.Circulator;
            txtCableFix.Text = info.cablefix;


        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                string coupler = string.Empty;
                switch (lblCoupler.Text)
                {
                    case "Lior Coupler - 01":
                        coupler = "Lior Coupler - 01";
                        break;
                    case "Green coupler - 99832":
                        coupler = "Green coupler - 99832";
                        break;
                }
                MySQLConnector.UpdateOvenInfo(m_ovenInfo.guid, lblDescription.Text, m_ovenInfo.userId,
                                              lblAlias.Text,
                                              coupler, checkBox1.Checked,
                                              txtCableFix.Text);

                Close();
            }
            catch (Exception ere)
            {
                MessageBox.Show(ere.Message);
            }
        }
    }
}
