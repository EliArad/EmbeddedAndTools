using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TopazApp
{
    public partial class CreateUser : Form
    {
        public CreateUser()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            usersDb nu = new usersDb();
            

            if (textBox2.Text == string.Empty)
            {
                MessageBox.Show("Please enter password");
                return;
            }

            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Please enter user name");
                return;
            }

            if (textBox3.Text == string.Empty)
            {
                MessageBox.Show("Please enter first name");
                return;
            }

            if (textBox4.Text == string.Empty)
            {
                MessageBox.Show("Please enter last name");
                return;
            }

            if (textBox5.Text == string.Empty)
            {
                MessageBox.Show("Please enter phone number");
                return;
            }

            try
            {

                User u = new User
                {
                    email = textBox1.Text,
                    password = textBox2.Text,
                    firstName = textBox3.Text,
                    lastName = textBox4.Text,
                    phoneNumber = textBox5.Text,
                    active = true
                };
                nu.CreateNewUser(u);
                MessageBox.Show("User created");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
           
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.ToLower() == "admin")
            {
                button1.Enabled = false;
                return;
            }

            if (textBox2.Text != string.Empty && textBox1.Text != string.Empty)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }
     
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.ToLower() == "admin")
            {
                button1.Enabled = false;
                return;
            }

            if (textBox2.Text != string.Empty && textBox1.Text != string.Empty)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }
    }
}
