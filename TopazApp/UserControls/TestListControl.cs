using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazApp.UserControls
{
    public partial class TestListControl : UserControl
    {
        public delegate void SelectedTest(TestInfo t);
        SelectedTest pCallback;
        TestInfo m_test;
        public TestListControl(SelectedTest p)
        {
            InitializeComponent();
            pCallback = p;
        }
        public void setTest(TestInfo test)
        {
            label10.Text = test.testname;
            label9.Text = test.id.ToString();
            label10.Text = test.testname;
            label7.Text = test.totalTime.ToString();
            label6.Text = test.dish_name;
            label8.Text = test.fres.FinalDescription;
            m_test = test;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pCallback(m_test);
        }
    }
}
