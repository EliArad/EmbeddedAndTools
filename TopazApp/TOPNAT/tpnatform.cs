using NetworkDrivesApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApi;
using TopazApp.AlgoParamsForms;
using TopazCommonDefs;
using ZedChartLib;
using ZedGraph;
 


namespace TopazApp.TOPNAT
{
    public partial class tpnatform : Form
    {
         
        int m_powerIndex = 0;
        int m_cdr = 0;
        int m_testId = -1;
        TestInfo m_testInfo;
        FINAL_TEST_RESULT m_finalResults;
        ZedChart m_zedWatts;
        ZedChart m_zedDr;
        ZedChart m_zedS11;
        ZedChart m_zedKj;
        PointPairList spl1;
        PointPairList spl2;
        PointPairList spl3;
        LineItem myKjCurve;
        LineItem myWattsCurve;
        LineItem myS11Curve;
        LineItem myForwardCurve;

        List<Tuple<TimeSpan, double>> m_totalEnergy = new List<Tuple<TimeSpan, double>>();
        List<Tuple<TimeSpan, double>> m_intermidiateEnergy = new List<Tuple<TimeSpan, double>>();

        List<Tuple<DateTime, DR[]>> m_AlldrList = new List<Tuple<DateTime, DR[]>>();

        List<PowerInfo> m_PowerList = new List<PowerInfo>();
        bool m_playdr = false;
        bool m_close = false;
        Tuple<int, string> m_solution;

        Thread m_drplayerThread;
        TimeSpan m_startTime;
        long m_lstartTime;
        string m_utensilName;
         
        public tpnatform()
        {
            InitializeComponent();
            label7.Text = "";
            Control.CheckForIllegalCrossThreadCalls = false;
            CreateZedForwardPowerGraph();
            checkBox1.Checked = true;
            label11.Text = string.Empty;
            InitDataGrid();
            viewPicturesToolStripMenuItem.Visible = false;
            //this.Height = Screen.PrimaryScreen.Bounds.Height - 50;


            MySQLConnector.Initialize("localhost", "root", "1234");

            try
            {
               //if (File.Exists("Y:\\TopazPOC\\readme.txt") == false)
                   // NetworkDrives.MapDrive("Y", "\\192.168.10.64\\Goji", "Hobart\\rfteam", "Helix123");
            }
            catch (Exception err)
            {

            }
            if (DRIVE.Drive[0] == 'Y')
            {
                if (File.Exists("Y:\\TopazPOC\\readme.txt") == false)
                {
                    MessageBox.Show("POC software needs to have a network drive Y mapped to RDB server");
                    m_close = true;
                }
            }
            
        }
        public class MyValue
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        void InitDataGrid()
        {
            IList<MyValue> values = new List<MyValue> { 
                new MyValue { id = 0, name = "equal energy" }, 
                new MyValue { id = 1, name = "Gamma Percentage" } ,
                new MyValue { id = 2, name = "RF Off" } ,
                new MyValue { id = 3, name = "DR Treshold" } ,
                new MyValue { id = 4, name = "Top Percentage" } ,
            
            };
            DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
            col.DataSource = values;
            col.DisplayMember = "name";
            col.DataPropertyName = "userid";
            col.ValueMember = "id";
            col.Width = 130;
            dataGridView1.Columns.Add(col);
            col.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            col.FlatStyle = FlatStyle.Popup;
            dataGridView1.Columns[0].Name = "Algo";

             

            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[1].Name = "Time";
            dataGridView1.Columns[1].Width = 60;

            dataGridView1.Columns[2].Name = "Power";
            dataGridView1.Columns[2].Width = 55;

            dataGridView1.Columns[3].Name = "Kj";
            dataGridView1.Columns[3].Width = 55;

            dataGridView1.Columns[4].Name = "absorbed";
            dataGridView1.Columns[4].Width = 55;

            dataGridView1.Enabled = true;

        }
        private void loadTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTestForm f = new LoadTestForm();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    m_testId = f.TestID;
                    m_testInfo = f.testInfo;
                    
                  

                    TimeSpan testTime = m_testInfo.StopDated - m_testInfo.StartDated;
                    sevenSegmentArray2.Value = testTime.Minutes.ToString();
                    sevenSegmentArray1.Value = testTime.Seconds.ToString();

                    label5.Text = m_testInfo.firstName + "  " + m_testInfo.lastName;

                    label16.Text = m_testInfo.testname + " (" + m_testId.ToString() + " )";
                    m_finalResults = MySQLConnector.GetFinalTestResults(m_testId);

                    label3.Text = m_finalResults.avgfordb.ToString();
                    label4.Text = m_finalResults.avgrefdb.ToString();
                    sskjtotal.Value = m_finalResults.totalkj.ToString("0.00");
                    lblTotalWatts.Text = m_finalResults.totalwatts.ToString();


                    LoadDRToGraph();
                    LoadEnergyToGraph();
                    LoadPowerToGraph();
                    ShowDRIndex(0, true);
                    ShowPowerIndex(0 + 1, true, true);

                    try
                    {
                        List<SolutionAlgo> list = MySQLConnector.GetTestSolutionAlgorithems(m_testId);                       
                        BuildDataGridFromSolution(list);
                        label11.Text = f.testInfo.testname;

                        viewPicturesToolStripMenuItem.Visible = true;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }    
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }
        void BuildDataGridFromSolution(List<SolutionAlgo> list)
        {

            dataGridView1.Rows.Clear();
            int index = 0;

            int rowIndex = 0;

            IList<MyValue> values = new List<MyValue> { 
                new MyValue { id = 0, name = "equal energy" }, 
                new MyValue { id = 1, name = "Gamma Percentage" } ,
                new MyValue { id = 2, name = "RF Off" }            ,
                new MyValue { id = 3, name = "DR Treshold" },
                new MyValue { id = 4, name = "Top Percentage" } 
            };


            IList<MyValue> uvalues = new List<MyValue>();
            List<string> l = MySQLConnector.GetUtensilList();
            int i = 0;
            foreach (string s in l)
            {
                MyValue v = new MyValue
                {
                    id = i,
                    name = s
                };
                uvalues.Add(v);
            }
            foreach (SolutionAlgo t in list)
            {
                switch (t.algoname)
                {
                    case "GammaPercentage":
                        index = 1;
                        break;
                    case "equal energy":
                        index = 0;
                        break;
                    case "RF Off":
                        index = 2;
                        break;
                    case "DR Treshold":
                        index = 3;
                        break;
                    case "Top Percentage":
                        index = 4;
                    break;
                    default:
                        {
                            throw (new SystemException("Unknown algo: " + t.algoname));
                        }
                }
                dataGridView1.Rows.Add();

                var c = dataGridView1[0, rowIndex] as DataGridViewComboBoxCell;
                if (c == null)
                    return;

                c.DataSource = values;
                c.Value = index;
                c.ValueMember = "id";
                c.DisplayMember = "name";
                dataGridView1.Rows[rowIndex].Cells[1].Value = t.time.ToString();
                dataGridView1.Rows[rowIndex].Cells[2].Value = t.maxpower;

                dataGridView1.Rows[rowIndex].Cells[3].Value = t.kj;
                dataGridView1.Rows[rowIndex].Cells[4].Value = t.absorbed;


                 

                rowIndex++;
            }         
        }

        void ShowPowerIndex(int index , bool showall, bool increase)
        {
            if (m_PowerList.Count == 0)
                return;
            PowerInfo values = m_PowerList[m_powerIndex];
            m_lstartTime = values.timestamp;


            zg2.GraphPane.CurveList[0].Clear();
            zg4.GraphPane.CurveList[0].Clear();
            zg5.GraphPane.CurveList[0].Clear();
            double s11;

            List<PointPair> s11List = new List<PointPair>();
            while(true)
            {
                if ((values.timestamp - m_lstartTime) > 10000)
                {
                    break;
                }

                PointPair p = new PointPair();
                PointPair p1 = new PointPair();
                PointPair p2 = new PointPair();                

                p2.Y = values.fwwatts;
                p2.X = values.timestamp - m_lstartTime;
                DrawPoint(zg2, p2);
                p.X = values.freq; // values.timestamp - m_lstartTime;
                p1.X = values.timestamp - m_lstartTime;
                s11 = values.reflected - values.forward;
                p.Y = s11;
                p1.Y = values.forward;
                s11List.Add(p);
                DrawPoint(zg5, p1);
                if (increase)
                    m_powerIndex = (m_powerIndex + 1) % m_PowerList.Count;
                else
                {
                    if (m_powerIndex > 0)
                        m_powerIndex--;
                    else
                    {
                        m_powerIndex = m_PowerList.Count - 1;
                    }
                } 

                if (m_powerIndex == 0)
                    break;
                values = m_PowerList[m_powerIndex];
            }
            s11List.Sort((s1, s2) => s1.X.CompareTo(s2.X));
           
            foreach (PointPair x in s11List)                                
                DrawPoint(zg4, x);

            RefresheZedGraphs(zg4);
            RefresheZedGraphs(zg5);
            RefresheZedGraphs(zg2);

        }
        void ShowDRIndex(int index, bool showall)
        {
            if (m_AlldrList.Count == 0)
                return;
            Tuple<DateTime, DR[]> values = m_AlldrList[index];
            if (index == 0)
            {
                m_startTime = values.Item1.TimeOfDay;
            }
            label7.Text = values.Item1.TimeOfDay.Subtract(m_startTime).ToString();

            m_zedDr.CreateGraph_DRGradientByZBars(zg3, values.Item2, POCSET.MAXFREQ, showall);
            RefresheZedGraphs(zg3);

        }
        void LoadDRToGraph()
        {
            string line;
            int i = 0;
            DateTime startTime = DateTime.Now;

            DR[] data = null;

            try
            {

                m_finalResults.DRFileName = m_finalResults.DRFileName.Replace("C:\\Goji", DRIVE.Drive);
                using (StreamReader sr = new StreamReader(m_finalResults.DRFileName))
                {
                    try
                    {
                        m_AlldrList.Clear();
                        while (true)
                        {
                            line = sr.ReadLine();
                            if (line == null)
                            {
                                if (i > 0)
                                {
                                    if (data == null)
                                    {
                                        throw (new SystemException("Error"));
                                    }
                                    Tuple<DateTime, DR[]> t = new Tuple<DateTime, DR[]>(startTime, data);
                                    m_AlldrList.Add(t);
                                }
                                label14.Text = m_AlldrList.Count().ToString();
                                return;
                            }
                            if (line == "+")
                            {
                                if (i > 0)
                                {
                                    if (data == null)
                                    {
                                        throw (new SystemException("Error"));
                                    }
                                    Tuple<DateTime, DR[]> t = new Tuple<DateTime, DR[]>(startTime, data);
                                    m_AlldrList.Add(t);
                                }
                                i = 0;
                                data = new DR[POCSET.MAXFREQ];
                                continue;
                            }

                            string[] s = line.Split(new Char[] { ',' });
                            startTime = DateTime.Parse(s[0]);

                            data[i].freq = float.Parse(s[1]);
                            data[i].value = float.Parse(s[2]);
                            data[i].valid = ushort.Parse(s[3]);

                            i++;
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Error reading energy data: " + err.Message);
                        return;
                    }
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }

        }

        void LoadPowerToGraph()
        {
            string line;
        
            try
            {
                m_finalResults.PowerInfoFileName = m_finalResults.PowerInfoFileName.Replace("C:\\Goji", DRIVE.Drive);
                using (StreamReader sr = new StreamReader(m_finalResults.PowerInfoFileName))
                {
                    try
                    {
                        m_PowerList.Clear();
                        while (true)
                        {
                            line = sr.ReadLine();
                            if (line == null)
                            {
                                m_PowerList = m_PowerList.OrderByDescending(x => x.timestamp).ToList();
                                m_PowerList.Reverse();                                                                
                                return;
                            }
                            PowerInfo data = new PowerInfo();
                            string[] s = line.Split(new Char[] { ',' });

                            data.timestamp = long.Parse(s[0]);
                            data.freq = float.Parse(s[1]);
                            data.forward = float.Parse(s[2]);
                            data.reflected = float.Parse(s[3]);
                            data.mag_level = float.Parse(s[4]);
                            data.watts = float.Parse(s[5]);
                            data.fwwatts = float.Parse(s[6]);
                            data.rwwatts = float.Parse(s[7]);

                            m_PowerList.Add(data);
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Error reading energy data: " + err.Message);
                        return;
                    }
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }

        }
        private void CreateZedForwardPowerGraph()
        {
            m_zedWatts = new ZedChart(zg2.GraphPane, "Channel 1,", "Heat entries", "Watts", "Watts", 2, Color.Blue);
            m_zedWatts.CreateSeries(100);

            m_zedKj = new ZedChart(zg1.GraphPane, "Channel 2", "Heat entries", "Kj", "Energy", 2, Color.Green);
            m_zedKj.CreateSeries(100);
            m_zedDr = new ZedChart(zg3.GraphPane, "DR,", "Frequencies", "dB", "DR", 2, Color.Blue);

            m_zedS11 = new ZedChart(zg1.GraphPane, "Channel 2", "Heat entries", "Kj", "Energy", 2, Color.Green);
            
            myKjCurve = zg1.GraphPane.AddCurve("Energy(Kj)", spl1, Color.Red, SymbolType.None);
            myWattsCurve = zg2.GraphPane.AddCurve("Watts", spl2, Color.Blue, SymbolType.None);

            myS11Curve = zg4.GraphPane.AddCurve("S11", spl3, Color.Red, SymbolType.None);
            myForwardCurve = zg5.GraphPane.AddCurve("Forward", spl3, Color.Blue, SymbolType.None);

        }
        void LoadEnergyToGraph()
        {
            string line;
            int i = 0;
            DateTime startTime = DateTime.Now;
            zg1.GraphPane.CurveList[0].Clear();
            zg2.GraphPane.CurveList[0].Clear();
            zg4.GraphPane.CurveList[0].Clear();
            //zg3.GraphPane.CurveList[0].Clear();

            double m_totalKj = 0;
            m_totalEnergy.Clear();
            m_intermidiateEnergy.Clear();
            bool inter = false;
            m_finalResults.EnergyFileName = m_finalResults.EnergyFileName.Replace("C:\\Goji", DRIVE.Drive);
            using (StreamReader sr = new StreamReader(m_finalResults.EnergyFileName))
            {
                try
                {
                    while (true)
                    {
                        line = sr.ReadLine();                         
                        if (line == null)
                            return;
                        string[] s = line.Split(new Char[] { ',' });

                         
                        DateTime dated = DateTime.Parse(s[0]);
                        if (i == 0)
                            startTime = dated;
                        float kj = float.Parse(s[1]);
                        i++;
                        TimeSpan diff = dated - startTime;

                        Tuple<TimeSpan, double> it = new Tuple<TimeSpan, double>(diff, kj);
                        m_intermidiateEnergy.Add(it);

                        m_totalKj += kj;
                        Tuple<TimeSpan, double> it1 = new Tuple<TimeSpan, double>(diff, m_totalKj);
                        m_totalEnergy.Add(it1);

                        PointPair p = new PointPair();                        
                        p.X = diff.TotalSeconds;
                        p.Y = kj;
                        DrawPoint(zg1, p);
                        RefresheZedGraphs(zg1);
                    }                    
                }
                catch (Exception err)
                {
                    MessageBox.Show("Error reading energy data: " + err.Message);
                    return;
                }
            }
        }
        private void DrawPoint(ZedGraph.ZedGraphControl zgc, PointPair p)
        {
            zgc.GraphPane.CurveList[0].AddPoint(p);
        }
        private void RefresheZedGraphs(ZedGraph.ZedGraphControl zg)
        {
            zg.AxisChange();
            zg.Invalidate();
            //zg.Refresh();
        }
        void DRPlayer()
        {
            m_playdr = true;
            m_powerIndex = 0;
            while (m_playdr == true)
            {
                m_powerIndex = 0;
                for (m_cdr = 0; m_cdr < m_AlldrList.Count; m_cdr++)
                {
                    ShowDRIndex(m_cdr, checkBox1.Checked);
                    ShowPowerIndex(m_cdr + 1, checkBox1.Checked, true);
                    if (m_playdr == false)
                        return;
                    try
                    {
                        Thread.Sleep(int.Parse(textBox1.Text));
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
           

            if (m_drplayerThread == null || m_drplayerThread.IsAlive == false)
            {
                m_drplayerThread = new Thread(DRPlayer);
                m_drplayerThread.Start();
            }
  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            m_playdr = false;
            if (m_drplayerThread != null)
                m_drplayerThread.Join();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            m_playdr = false;
            if (m_drplayerThread != null)
                m_drplayerThread.Join();


            m_cdr = (m_cdr + 1 ) % m_AlldrList.Count;
            ShowDRIndex(m_cdr, checkBox1.Checked);
            ShowPowerIndex(m_cdr + 1, checkBox1.Checked, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            m_playdr = false;
            if (m_drplayerThread != null)
                m_drplayerThread.Join();

            if ((m_cdr + 1) >= m_AlldrList.Count)
            {
                return;
            }
            else
                m_cdr++;
            ShowDRIndex(m_cdr, checkBox1.Checked);
            ShowPowerIndex(m_cdr + 1, checkBox1.Checked, true);

        }

        private void viewPicturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewNatPicturesForm f = new ViewNatPicturesForm(m_testId);
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            m_cdr = 0;
            m_powerIndex = 0;
            ShowDRIndex(0, true);
            ShowPowerIndex(0 + 1, true, true);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            zg1.GraphPane.CurveList[0].Clear();

            foreach (Tuple<TimeSpan, double> t in m_totalEnergy)
            {
                PointPair p = new PointPair();
                p.X = t.Item1.TotalSeconds;
                p.Y = t.Item2;
                DrawPoint(zg1, p);
            }
            RefresheZedGraphs(zg1);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
           
            zg1.GraphPane.CurveList[0].Clear();

            foreach (Tuple<TimeSpan, double> t in m_intermidiateEnergy)
            {
                PointPair p = new PointPair();
                p.X = t.Item1.TotalSeconds;
                p.Y = t.Item2;
                DrawPoint(zg1, p);
            }
            RefresheZedGraphs(zg1);
             
        }

        private void loadSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["Algo"].Value == null)
                    return;
                
                switch (dataGridView1.Rows[e.RowIndex].Cells["Algo"].Value.ToString())
                {
                    case "0":
                        {
                            try
                            {
                                AlgoEqualEnergyParams? d =
                                MySQLConnector.GetTestAlgoEqualEnergyParams(m_testId, e.RowIndex);
                                TopazApp.AlgoParamsForms.EqualEnergyAlgoForm1 f = new TopazApp.AlgoParamsForms.EqualEnergyAlgoForm1(d);
                                f.ShowDialog();
                            }
                            catch (Exception err)
                            {
                                MessageBox.Show(err.Message);
                            }
                        }
                        break;
                    case "1":
                        break;
                    case "2":
                        break;
                    case "3":
                        {
                            try
                            {
                                AlgoThresholParams? d =
                                MySQLConnector.GetTestAlgoThreaholdParams(m_testId, e.RowIndex);
                                DRThresholdAlgoForm1 f = new DRThresholdAlgoForm1(d);
                                f.ShowDialog();
                            }
                            catch (Exception err)
                            {
                                MessageBox.Show(err.Message);
                            }
                        }
                        break;
                    case "4":
                        {
                            try
                            {
                                AlgoTopPercentageParams? d =
                                MySQLConnector.GetTestAlgoTopPercentageParams(m_testId, e.RowIndex);
                                AlgoTopPercentageForm1 f = new AlgoTopPercentageForm1(d);
                                f.ShowDialog();
                            }
                            catch (Exception err)
                            {
                                MessageBox.Show(err.Message);
                            }
                        }
                        break;
                }
            }
        }

        private void queriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuerisForm q = new QuerisForm();
            this.Hide();
            q.ShowDialog();
            this.Show();
        }

        private void testSummeryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestSummeryForm f = new TestSummeryForm(m_testInfo);
            f.Show();
        }

        private void tpnatform_Load(object sender, EventArgs e)
        {
            if (m_close == true)
                Close(); 
        }      
    }
}
