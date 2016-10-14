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
    public partial class AddPictures : Form
    {
        int countAdded = 0;
        string m_strDate;
        int m_testId = -1;
        PictureInfo[] m_pictures = new PictureInfo[4];
        List<TestInfo2> list;
        bool m_before;
        public AddPictures(int userid, bool before, int testId)
        {
            InitializeComponent();
            m_testId = testId;

            m_before = before;
             

        }
        public PictureInfo [] GetPicturesList()
        {
            return m_pictures;
        }
        public string strDate
        {
            get
            {
                return m_strDate;
            }
        }
        public int TestId
        {
            get
            {
                return m_testId;
            }
        }
        public int AddedPic
        {
            get
            {
                return countAdded;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
 

                TextBox[] title = { textBox1, textBox2, textBox3, textBox4 };
                int i = 0;
                foreach (PictureInfo t in m_pictures)
                {
                    if (t == null || t.fullname == null)
                    {
                        i++;
                        continue;
                    }
                    string fileName = Path.GetFileName(t.fullname);
                    MySQLConnector.UpdatePictureDetails(m_testId, fileName, title[i].Text, t.description, m_before);
                    i++;
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            /*
            DialogResult d = MessageBox.Show("Are you sure you want do discard pictures loading?", "POC is the future", MessageBoxButtons.YesNo);
            if (d == System.Windows.Forms.DialogResult.No)
                return;
            */
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        void RemovePicture(int i)
        {
            PictureBox[] pbox = { pictureBox1, pictureBox2, pictureBox3, pictureBox4 };
            TextBox[] title = { textBox1, textBox2, textBox3, textBox4 };


            MySQLConnector.DeletePictureFromTest(m_testId, m_before, Path.GetFileName(pbox[i].ImageLocation));
            File.Delete(pbox[i].ImageLocation);
            m_pictures[i] = null;
            pbox[i].ImageLocation = string.Empty;
            button1.Text = "Add";
            title[i].Text = string.Empty;
            countAdded--;
        }
        string GetPrefix()
        {
            if (m_before)
                return "_B_";
            else
                return "_A_";
        }
        void AddPicture(int i)
        {
            PictureBox[] pbox = { pictureBox1, pictureBox2, pictureBox3, pictureBox4 };
            TextBox[] title = { textBox1, textBox2, textBox3, textBox4 };
            Button[] btn = { button1, button2, button3, button6 };


            string fileName;
            fileName = GetFileName();
            if (File.Exists(fileName))
            {
                pbox[i].ImageLocation = fileName;

                string dbName = m_testId + GetPrefix() + Path.GetFileName(fileName);
                m_pictures[i] = new PictureInfo(dbName, fileName, title[i].Text, string.Empty);
                btn[i].Text = "Remove";

                string savepath = DRIVE.Drive + @"TopazPOC\Pictures\" + m_strDate;
                Directory.CreateDirectory(savepath);
                PictureInfo tname =
                 new PictureInfo(dbName, fileName, title[i].Text, string.Empty);

                AddPictureToTest(m_testId, "192.168.10.64", savepath, m_before, tname);
                CopyFileToDestincation(savepath, m_testId, m_before, tname);
                countAdded++;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (button1.Text == "Remove")
                {
                    RemovePicture(0);

                }
                else
                {
                    AddPicture(0);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void UpdatePictureDetails(int testId, string fileName, string picname, string description, bool before)
        {
            MySQLConnector.UpdatePictureDetails(testId, fileName, picname, description, before);
        }
        void AddPictureToTest(int testId, string compip, string savepath, bool before, PictureInfo pic)
        {
            try
            {
                if (before)
                {
                    MySQLConnector.AddPictureBeforeToTest(testId, compip, savepath, pic);
                }
                else
                {
                    MySQLConnector.AddPictureAfterToTest(testId, compip, savepath, pic);
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }

        }

        void CopyFileToDestincation(string destPath, int tesId, bool before, PictureInfo s)
        {
            try
            {        
                File.Copy(s.fullname, destPath + "\\" + s.dbName);        
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        string GetFileName()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "JPEG Files (.jpg) |*.jpg|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Remove")
            {
                RemovePicture(1);
            }
            else
            {
                AddPicture(1);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (button3.Text == "Remove")
            {
                RemovePicture(2);
            }
            else
            {
                AddPicture(2);
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {

            PictureDescrptionForm a = new PictureDescrptionForm(pictureBox1.ImageLocation, m_pictures[0].description);
            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_pictures[0].description = a.Descrption;
            }
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {

            PictureDescrptionForm a = new PictureDescrptionForm(pictureBox2.ImageLocation, m_pictures[1].description);
            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_pictures[1].description = a.Descrption;
            }
        }

        private void pictureBox3_DoubleClick(object sender, EventArgs e)
        {

            PictureDescrptionForm a = new PictureDescrptionForm(pictureBox3.ImageLocation, m_pictures[2].description);
            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_pictures[2].description = a.Descrption;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (button6.Text == "Remove")
            {
                RemovePicture(3);
            }
            else
            {
                AddPicture(3);
            }
        }

        private void pictureBox4_DoubleClick(object sender, EventArgs e)
        {


            PictureDescrptionForm a = new PictureDescrptionForm(pictureBox4.ImageLocation, m_pictures[3].description);
            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_pictures[3].description = a.Descrption;
            }
        }
    }
}
