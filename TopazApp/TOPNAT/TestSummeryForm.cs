using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazApp.TOPNAT
{
    public partial class TestSummeryForm : Form
    {
        public TestSummeryForm(TestInfo testInfo)
        {
            InitializeComponent();

            this.Text = "Test " + testInfo.testname + " summery:";
             

            textBox2.Text = testInfo.remark;
            label3.Text = testInfo.utensil_name;
            label4.Text = testInfo.drpower.ToString();
            label7.Text = testInfo.dish_name;
            label11.Text = testInfo.fres.PassFail.ToString();
            textBox3.Text = testInfo.fres.FinalDescription;
            label13.Text = testInfo.StartDated.ToString();
            TimeSpan tdiff = testInfo.StopDated - testInfo.StartDated;
            label14.Text = tdiff.ToString();
            label16.Text = testInfo.fres.totalkj.ToString();
            label18.Text = testInfo.fres.avgfordb.ToString();



        }
    }
}
