using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazCalibrationApi;
using System.Collections.Concurrent;
using CouplerLib;
using TopazCommonDefs;
using ZedGraph;

namespace TopazCalibrationApp
{
    public partial class Form1 : Form
    {
        bool m_running = false;
        LinearPowerCalibration m_tpcal;
        AutoResetEvent m_event = new AutoResetEvent(false);
        ConcurrentQueue<Tuple<int, string>> m_queue = new ConcurrentQueue<Tuple<int, string>>();
        ushort m_ddsStart;
        ushort m_ddsStop;
        ushort m_ddsStep;
        AssignPowerMeterForm.PowerMeterSelect[] pmSelect = new AssignPowerMeterForm.PowerMeterSelect[2];
        Thread msgQueueThread = null;
        LineItem[] myCurve = new LineItem[101];
        List<PointPairList> spl = new List<PointPairList>();
        

        bool appActive = true;
        public Form1()
        {

            InitializeComponent();
            msgQueueThread = new Thread(MsgQueueThread);
            msgQueueThread.Start();

            CreateZedForwardPowerGraph(zg1, "Topaz Linear calibration");


            InitDataGrid();
            TopazCalibration.MessageCallback p = new TopazCalibration.MessageCallback(CalMsgCallback);
            m_tpcal = new LinearPowerCalibration(p);
            m_tpcal.SetCoupler(COUPLER_SERIAL.GREEN_99832);
            Control.CheckForIllegalCrossThreadCalls = false;
            LoadSettings();
        }
        void MsgQueueThread()
        {
            while (appActive)
            {
                if (m_queue.Count == 0)
                    m_event.WaitOne();
                Tuple <int , string> t ;
                if (m_queue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                if (m_queue.TryDequeue(out t) == false)
                {
                    Thread.Sleep(100);
                    continue;
                }
                int code = t.Item1;
                string msg = t.Item2;

                switch (code)
                {
                    case 0:
                        button1.ForeColor = Color.Black;
                        button1.Enabled = true;
                        m_running = false;
                        break;
                    case 1:
                        button1.ForeColor = Color.Green;
                        button1.Enabled = false;
                        m_running = true;
                    break;
                    case 2:
                        button1.ForeColor = Color.Black;
                        button1.Enabled = false;
                        m_running = false;
                        MessageBox.Show("Finished successfully");
                    break;
                    case 40:
                    {
                        string[] s = msg.Split(new Char[] { ',' });
                        InvokeDataGrid(s);

                        double freq = double.Parse(s[0]);
                        int index = (int)(freq - 2400);
                        double power = double.Parse(s[2]);
                        double scode = double.Parse(s[1]);
                        Draw(index, scode, power);
                        RefresheZedGraphs();
                    }
                    break;
                }
            }            
        }
        void InitDataGrid()
        {

            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Frequency";
            dataGridView1.Columns[0].Width = 80;

            dataGridView1.Columns[1].Name = "DDS Code";
            dataGridView1.Columns[1].Width = 80;

            dataGridView1.Columns[2].Name = "Power";
            dataGridView1.Columns[2].Width = 80;
 
        }
        void InvokeDataGrid(string [] s)
        {
            if (this.dataGridView1.InvokeRequired == true)
                this.dataGridView1.Invoke((MethodInvoker)delegate
                {
                    dataGridView1.Rows.Add(s[0], s[1], s[2]);
                    this.dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;   
                });
        }

        void CalMsgCallback(int code, string msg)
        {
            Tuple<int, string> t = new Tuple<int, string>(code, msg);
            m_queue.Enqueue(t);
            m_event.Set();
        }

        private void RefresheZedGraphs()
        {
            // I add all three functions just to be sure it refeshes the plot.   
            zg1.AxisChange();
            zg1.Invalidate();
            zg1.Refresh();
        }
  
        void Draw(int iSeriesIndex, double xValue, double yValue)
        {
            PointPair p = new PointPair();
            p.X = xValue;
            p.Y = yValue;
            zg1.GraphPane.CurveList[iSeriesIndex].AddPoint(p);
        }
         
        private void CreateZedForwardPowerGraph(ZedGraph.ZedGraphControl zg, string title)
        {
            Random rand = new Random();
            for (int i = 0; i < 101; i++)
            {                 
               
                PointPairList p = new PointPairList();
                spl.Add(p);

                Color c = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255), rand.Next(255));


                myCurve[i] = zg1.GraphPane.AddCurve("Freq" + (2400 + i), spl[i], c, SymbolType.None);                 
                myCurve[i].Line.Width = 1;
            }
            zg1.GraphPane.Title.Text = "Goji MPFM Sensor";
            zg1.GraphPane.Legend.Position = ZedGraph.LegendPos.Right;
            
            zg1.GraphPane.Legend.FontSpec.Size = 8.0f;
            zg1.GraphPane.Legend.FontSpec.Family = "Arial, Narrow";

            zg1.GraphPane.YAxis.Title.FontSpec.Size = 10.0f;
            zg1.GraphPane.YAxis.Scale.FontSpec.Family = "Arial, Narrow";

            zg1.GraphPane.XAxis.Title.FontSpec.Size = 10.0f;
            zg1.GraphPane.XAxis.Scale.FontSpec.Family = "Arial, Narrow";

            zg1.GraphPane.Chart.Fill.Color = System.Drawing.Color.White;
            zg1.GraphPane.Legend.IsVisible = false;
             
            zg1.GraphPane.YAxis.Title.Text = "Power [dBm]";
            zg1.GraphPane.XAxis.Title.Text = "Frequency";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                LoadAssigments();
                m_tpcal.Initialize(pmSelect[0].sensorName , pmSelect[1].sensorName);

                for (int i = 0; i < zg1.GraphPane.CurveList.Count; i++)
                {
                    zg1.GraphPane.CurveList[i].Clear();
                }
            
                DETECTOR det = radioButton1.Checked ? DETECTOR.FORWARD : DETECTOR.REFLECTED;
                int id = (int)det;
                if (pmSelect[id].sensorName != null || pmSelect[id].sensorName != "" || pmSelect[id].sensorName.ToLower() != "none")
                {
                    m_tpcal.Configure(det, m_ddsStart, m_ddsStop, m_ddsStep);
                    m_tpcal.Start();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void SaveSettings()
        {
            Properties.Settings.Default.CalForward = radioButton1.Checked;
        }
        void LoadAssigments()
        {

            pmSelect[0].sensorName = Properties.Settings.Default.PowerMeter_forward;
            pmSelect[1].sensorName = Properties.Settings.Default.PowerMeter_reflected;

        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                m_tpcal.Stop();                
                button1.Enabled = true;
                button1.ForeColor = Color.Black;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void assignPowerMetersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssignPowerMeterForm f = new AssignPowerMeterForm();
            f.ShowDialog();
        }

        private void linearPowerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        void LoadSettings()
        {
            m_ddsStart = ushort.Parse(Properties.Settings.Default.DDSStart);
            m_ddsStop = ushort.Parse(Properties.Settings.Default.DDSStop);
            m_ddsStep = ushort.Parse(Properties.Settings.Default.DDSStep);
            radioButton1.Checked = Properties.Settings.Default.CalForward;
        }
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinearPowerCalSetting f = new LinearPowerCalSetting();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                appActive = false;
                m_event.Set();
                msgQueueThread.Join();
                m_tpcal.Close();
                SaveSettings();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                e.Cancel = true;
                return;
            }
        }

        private void openOutputFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(".");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Do you burn this calibration file to flash?", "Topaz Calibration", MessageBoxButtons.YesNo);
            if (d == System.Windows.Forms.DialogResult.Yes)
            {

                string fileCName;
                DETECTOR det;
                if (radioButton1.Checked)
                {
                    fileCName = "ab_forward_c.csv";
                    det = DETECTOR.FORWARD;
                }
                else
                {
                    fileCName = "ab_reflected_c.csv";
                    det = DETECTOR.REFLECTED;
                }
                try
                {
                    if (m_tpcal.Connect() == true)
                        m_tpcal.BurnABFile(fileCName, det);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }
    }
}
