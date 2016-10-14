using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApi;
using System.Collections.Concurrent;
using TopazApp.Forms;
using ZedChartLib;
using ZedGraph;
using System.IO;
using TopazApp.AlgoParamsForms;
using ScreenShotDemo;
using System.Drawing.Imaging;
using Microsoft.Win32;
using RegistryClassApi;
using TopazApp.Forms.DishNamesParameters;
using TopazApp.Forms.DebugForms;
using TopazCommonDefs;

namespace TopazApp
{
   
        
    public partial class Form1 : Form
    {
        int m_loadedTestId = -1;
        bool m_startDR = false;
        DR[] m_emptyCavityDR = null;
        //List<IAlgo> m_currentPlan = new List<IAlgo>();
        TestSoundPlayer m_soundPlayer = new TestSoundPlayer();
        DISH_SOLUTION m_curSolution;
        string m_testStrStartDate;
        string m_testName;
        string m_testRemark;
        PointPairList spl1;
        PointPairList spl2;
        PointPairList spl3;
        LineItem myKjCurve;
        LineItem myWattsCurve;
        LineItem mydBMDirectCurve;
        TimeSpan m_zeroTime = new TimeSpan(0, 0, 0);
        List<SolutionAlgo> m_solutionAlgo;
        TimeSpan m_TotalRunningTime = new TimeSpan(0, 0, 0);
        EqualEnergyGraphForm m_equalEnergyForm;
        bool m_paused = false;

        
        Tuple<int, string> m_solutionId = new Tuple<int, string>(-1, string.Empty);

        bool m_running = false;
        struct APPSettings
        {
            public string signalVisaName;
            public bool useTime;
            public float drPower;
        }
        
        ZedChart m_zedWatts;
        ZedChart m_zedDirect;
        ZedChart m_zedDr;
        ZedChart m_zedKj;
        object m_sstime = new object();

        double m_totalKj = 0;
        double m_totalWatts = 0;
        double m_grandTotalWatts = 0;
        double m_garndFordBm = 0;
        int m_grandPowerCount = 0;
        double m_garndRefdBm = 0;

        APPSettings m_appSet = new APPSettings();
        TimeSpan m_tsOneSecond = new TimeSpan(0, 0, 1);
        AutoResetEvent m_msgEvent = new AutoResetEvent(false);
        POCHeating  m_heat;  

        Thread m_msgThread;
        TimeSpan m_algoStartTime = new TimeSpan(0, 0, 0);
        ulong m_totalCookingTimeInSeconds;
        int m_testId;
        struct MsgQueue
        {
            public int code;
            public string msg;
            public int channel;
        }
        bool m_appRunning = true;
        ConcurrentQueue<MsgQueue> m_queue = new ConcurrentQueue<MsgQueue>();
        int m_userId;
        bool m_debug = false;
        string m_userName;
        string m_ovenGuid;
        OvenInfo m_ovenInfo;

        public Form1(int userid, string userName, bool debug, OvenInfo ovenInfo)
        {
            InitializeComponent();
            m_ovenInfo = ovenInfo;
            CreateZedForwardPowerGraph();
            label11.Text = string.Empty;
            m_userName = userName;
            m_debug = debug;
            label15.Text = string.Empty;

            label30.Visible = false;
            label31.Text = "";
            m_heat = new POCHeating(POCHeating.I2C_MASTER_COMPANY.DIOLAN);

            int timeout = 5;
            while (m_heat.Connected == false)
            {
                Thread.Sleep(1000);
                timeout--;
                if (timeout == 0)
                {
                    MessageBox.Show("Timeout connecting controller device");
                    return;
                }
            }

            clsRegistry reg = new clsRegistry();
            m_ovenGuid = reg.GetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid");           
            if (reg.strRegError != null)
            {
                throw (new SystemException("No oven guid"));
            }

            try
            {
                InitDataGrid();
                LoadSettings();
                m_userId = userid;
                Tuple<int, string> tsidp;
                if ((tsidp = MySQLConnector.GetLastSolution(m_userId)) != null)
                {
                    OpenSolution(tsidp.Item2, tsidp.Item1);
                    var t = new Thread(ContinuesLoading);
                    t.Start();
                }
                else
                {
                    preferencesToolStripMenuItem.Enabled = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error loading application settings");
            }
            
            Control.CheckForIllegalCrossThreadCalls = false;
            this.Text = "Hello " + userName + " let's cook";

            this.Text += "      " + m_ovenInfo.Alias;
            
            m_msgThread = new Thread(MsgThread);
            m_msgThread.Start();

            CallbackMessage p = new CallbackMessage(CallbackMessages);           
            m_heat.SetCallback(p);


            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\DishPictures");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\SolutionDRRange");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\TestDRRange");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\SolutionFrequencies");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\FinalScreenshots");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\TestFrequencies");

            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\Energy");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\DR");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\Power");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\KJPerFreq");
            


            zgdirect.Visible = false;
            checkBox3.Visible = true;// m_debug;
            label32.Text = "";
        }
        void ContinuesLoading()
        {

            try
            {
                SolutionPictureInfo? d = MySQLConnector.GetSolutionPicture(m_solutionId.Item1);

                if (d != null)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.ImageLocation = d.Value.fullpicname;
                    showToolStripMenuItem.Visible = d.Value.show;
                    showToolStripMenuItem.Visible = File.Exists(pictureBox1.ImageLocation);
                    showToolStripMenuItem.Text = d.Value.show == true ? "Hide" : "Show";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
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
                //new MyValue { id = 1, name = "Gamma Percentage" } ,
                new MyValue { id = 2, name = "RF Off" } ,
                new MyValue { id = 3, name = "DR Treshold" } ,
                new MyValue { id = 4, name = "Top Percentage" } ,
            
            };
            DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
            col.DataSource = values;
            col.DisplayMember = "name";
            col.DataPropertyName = "userid";
            col.ValueMember = "id";
            col.Width = 140;
            dataGridView1.Columns.Add(col);
            col.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            col.FlatStyle = FlatStyle.Popup;
            dataGridView1.Columns[0].Name = "Algo";


    

            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Arial", 15.5F, GraphicsUnit.Pixel);

            
            

            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[1].Name = "Time";
            dataGridView1.Columns[1].Width = 60;

            dataGridView1.Columns[2].Name = "Power";
            dataGridView1.Columns[2].Width = 55;

            dataGridView1.Columns[3].Name = "Kj";
            dataGridView1.Columns[3].Width = 55;

            dataGridView1.Columns[4].Name = "absorbed";
            dataGridView1.Columns[4].Width = 65;

            /*
            IList<MyValue> uvalues = new List<MyValue>();
            List<string> l = MySQLConnector.GetUtensilList();
            int i = 0;
            foreach(string s in l)
            {
                MyValue v = new MyValue{
                    id = i, name = s
                };
                uvalues.Add(v);
            }
            DataGridViewComboBoxColumn col1 = new DataGridViewComboBoxColumn();
            col1.DataSource = uvalues;
            col1.DisplayMember = "name";
            col1.DataPropertyName = "userid";
            col1.ValueMember = "id";
            dataGridView1.Columns.Add(col1);
            dataGridView1.Columns[3].Name = "Utensil";
             */

            dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);


            dataGridView1.ColumnHeadersDefaultCellStyle.Font =
           new Font(dataGridView1.Font, FontStyle.Bold);


        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox combo = e.Control as ComboBox;
            if (combo != null)
            {
                combo.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);
                combo.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            string item = cb.Text;
            return;
            if (item != null)
            {
                if (item == "RF Off")
                {
                    Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                    dataGridView1.Rows[selectedRowCount].Cells["kj"].Value = 0;
                    dataGridView1.Rows[selectedRowCount].Cells["power"].Value = 0;
                    dataGridView1.Rows[selectedRowCount].Cells["absorbed"].Value = 0;
                    dataGridView1.Rows[selectedRowCount].Cells["kj"].ReadOnly = true;
                }
                else
                {
                    Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                    dataGridView1.Rows[selectedRowCount].Cells["kj"].ReadOnly = false;
                }
            }
                
        }
        void LoadSettings()
        {
            m_appSet.useTime = Properties.Settings.Default.UseTime;
            checkBox2.Checked = m_appSet.useTime;

            try
            {
                LoadEmptyCavity();
            }
            catch (Exception err)
            {

            }
        }
        void SaveSettings()
        {
            m_appSet.useTime = checkBox2.Checked;

            Properties.Settings.Default.UseTime = m_appSet.useTime;
            Properties.Settings.Default.Save();
        }
        void CaptureScreen()
        {
            try
            {
                
                ScreenCapture sc = new ScreenCapture();
                // capture entire screen, and save it to a file
                Image img = sc.CaptureScreen();

                string fileName = DRIVE.Drive + @"TopazPOC\FinalScreenshots\" + m_testId + ".jpg";
                if (File.Exists(fileName) == true)
                {
                    File.Delete(fileName);
                }
                sc.CaptureWindowToFile(this.Handle, fileName, ImageFormat.Jpeg);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        void EQNGValuesCallbackMsg(EnergyData [] data)
        {
            /*
            if (m_equalEnergyForm != null)
                m_equalEnergyForm.SetData(data);
            else
            {
                this.BeginInvoke((Action)(() =>
                {
                    if (m_equalEnergyForm == null)
                        m_equalEnergyForm = new EqualEnergyGraphForm();

                    m_equalEnergyForm.Show();
                    m_equalEnergyForm.SetData(data);
                })); 

            }*/
        }
        void DRValuesCallback(DR [] values)
        {
            if (checkBox4.Checked)
            {
                for (int i = 0 ; i < values.Length;i++)
                {
                    values[i].value -= m_emptyCavityDR[i].value;
                }
            }

            //m_zedDr.CreateGraph_DRGradientByZBars(zg3, values, POCSET.MAXFREQ, checkBox1.Checked);
            RefresheZedGraphs(zg3);
                    
        }
        void MsgThread()
        {            
            while (m_appRunning)
            {
                if (m_queue.Count == 0)
                    m_msgEvent.WaitOne();
                if (m_appRunning == false)
                    return;
                MsgQueue q;
                if (m_queue.TryDequeue(out q) == false)
                {
                    continue;
                }

                switch (q.code)
                {
                    case 100:
                    {
                        Stop();
                        MessageBox.Show(q.msg);
                        MySQLConnector.UpdateTestResults(m_testId, 0, "Stop due error:" + q.msg, string.Empty, string.Empty, string.Empty);
                        btnStop.Font = new Font(btnStop.Font, FontStyle.Bold);
                        btnStart.ForeColor = Color.Black;
                        dataGridView1.Enabled = true;
                        m_running = false;
                        btnStart.Enabled = true;
                    }
                    break;
                    case 1: // STARTED
                        if (m_startDR == true)
                           break;
                        btnStop.Font = new Font(btnStop.Font, FontStyle.Regular);
                        MySQLConnector.UpdateTestStartTime(m_testId);
                        btnStart.ForeColor = Color.Green;
                        dataGridView1.Enabled = false;
                        m_running = true;
                        btnStart.Enabled = false;
                    break;
                    case 0:
                        StopFromCallback();                                          
                    break;                   
                    case 4:
                    {
                        lock (m_sstime)
                        {
                            m_algoStartTime = TimeSpan.Parse(q.msg);
                        }
                    }
                    break;
                    case 5:
                    {
                        dataGridView1.Rows[int.Parse(q.msg)].DefaultCellStyle.ForeColor = Color.Green;
                    }
                    break;
                    case 6:
                    {
                        dataGridView1.Rows[int.Parse(q.msg)].DefaultCellStyle.ForeColor = Color.Black;
                    }
                    break;
                    case 203:
                    {
                        label15.Text = q.msg;
                        label14.Text = "0";
                    }
                    break;
                    case 204:
                    {
                        label14.Text = q.msg;
                        label15.Text = "";
                    }
                    break;
                    case 214:
                    {
                        label25.Text = q.msg;
                    }
                    break;
                    case 60:
                        label3.Text = q.msg;
                    break;
                    case 61: // KJ
                    {
                        label21.Text = q.msg;
                    }
                    break;
                    case 69: 
                    {
                        string[] sdata = q.msg.Split(new Char[] { ',' });
                        PointPair p = new PointPair();
                        p.X = m_totalCookingTimeInSeconds;
                        p.Y = m_totalKj;
                        DrawPoint(zg1, p);
                        RefresheZedGraphs(zg1);

                        if (m_curSolution.TotalKj > 0 && m_totalKj >= m_curSolution.TotalKj)
                        {
                            Thread task = new Thread(() => Stop());
                            task.Start();
                        }

                        m_totalWatts = double.Parse(sdata[0]);
                        
                        label7.Text = m_totalWatts.ToString("0.00");
                        m_grandTotalWatts += m_totalWatts;

                        p = new PointPair();
                        p.X = m_totalCookingTimeInSeconds;
                        p.Y = m_totalWatts;
                        DrawPoint(zg2, p);
                        RefresheZedGraphs(zg2);
                        label22.Text = sdata[1];
                    }
                    break;
                    case 64:
                    {
                        string[] sdata = q.msg.Split(new Char[] { ',' });
                        label3.Text = sdata[0];
                        label4.Text = sdata[1];
                        m_garndFordBm += double.Parse(sdata[0]);
                        m_garndRefdBm += double.Parse(sdata[1]);
                        m_grandPowerCount++;
                    }
                    break;
                    case 68:
                    {
                        m_totalKj = double.Parse(q.msg);
                        label5.Text = m_totalKj.ToString("0.00");
                        sskjtotal.Value = m_totalKj.ToString("0.00");

                        if (m_curSolution.TotalKj > 0 && m_totalKj >= m_curSolution.TotalKj)
                        {
                            Thread task = new Thread(() => Stop());
                            task.Start();
                        }
                    }
                    break;
                    case 90:
                    {
                        string[] sdata = q.msg.Split(new Char[] { ',' });
                        ssMin.Value = sdata[1];
                        ssSec.Value = sdata[0];
                        m_TotalRunningTime = new TimeSpan(0, int.Parse(ssMin.Value), int.Parse(ssSec.Value)); 
                    }
                    break;
                    case 95:
                    {
                        TimeSpan t = TimeSpan.FromSeconds(int.Parse(q.msg));
                        sevenSegmentArray2.Value = t.Minutes.ToString("00");
                        sevenSegmentArray1.Value = t.Seconds.ToString("00");
                    }
                    break;
                    case 600:
                        label32.Text = q.msg;
                    break;
                    case 507:
                        label25.Text = q.msg;
                    break;
                    case 70:
                        label17.Text = q.msg;
                    break;
                    case 72:
                    {
                        label19.Text = q.msg;
                    }
                    break;
                    case 866:
                        ColorLablesForAGCType(q.msg == "True" ? true : false);
                    break;
                    case 287:

                    this.BeginInvoke((Action)(() =>
                    {
                        if (m_equalEnergyForm == null)
                            m_equalEnergyForm = new EqualEnergyGraphForm();

                        m_equalEnergyForm.Show();
                    })); 

                       
                    break;
                }
            }
        }
        void CallbackMessages(int code , string msg)
        {
            MsgQueue q = new MsgQueue();
            q.code = code;
            q.msg = msg;
            m_queue.Enqueue(q);
            m_msgEvent.Set();
        }

        bool CheckIfMissingParams()
        {
            int index = 0;
            string algo = string.Empty;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((dataGridView1.Rows.Count - 1) == index)
                {
                    return true;
                }
                try
                {
                    algo = row.Cells["Algo"].EditedFormattedValue.ToString();
                }
                catch (Exception err)
                {
                    MessageBox.Show("Algo type error in row: " + index);
                    return false;
                }

                switch (algo)
                {
                    case "equal energy":
                    {
                        bool found = false;
                        List<AlgoEqualEnergyParams> algoEEParamsList = MySQLConnector.GetAlgoEqualEnergyParams(m_solutionId.Item1);
                        foreach (AlgoEqualEnergyParams d in algoEEParamsList)
                        {
                            if (d.RowIndex == index)
                            {
                                found = true;
                            }
                        }
                        if (found == false)
                        {
                            MessageBox.Show("Did not found parameters for equal energy at line: " + index);
                            return false;
                        }
                    }
                    break;
                    case "DR Treshold":
                    {
                        List<AlgoThresholParams> algoThsParamsList = MySQLConnector.GetAlgoThreaholdParams(m_solutionId.Item1);
                        bool found = false;
                        foreach (AlgoThresholParams d in algoThsParamsList)
                        {
                            if (d.RowIndex == index)
                            {
                                found = true;
                            }
                        }
                        if (found == false)
                        {
                            MessageBox.Show("Did not found parameters for equal energy at line: " + index);
                            return false;
                        }
              
                    }
                    break;
                    case "Top Percentage":
                    {
                        List<AlgoTopPercentageParams> AlgoTopPercentageParamsList = MySQLConnector.GetAlgoTopPercentageParams(m_solutionId.Item1);
                        bool found = false;
                        foreach (AlgoTopPercentageParams d in AlgoTopPercentageParamsList)
                        {
                            if (d.RowIndex == index)
                            {
                                found = true;
                            }
                        }
                        if (found == false)
                        {
                            MessageBox.Show("Did not found parameters for equal energy at line: " + index);
                            return false;
                        }
                    }
                    break;
                }
                index++;
            }
            return false;
        }

        bool BuildHeatList(int testid)
        {

            try
            {
                //m_currentPlan.Clear();
                int index = 0;
                m_heat.Clear();


                List<AlgoThresholParams> algoThsParamsList = MySQLConnector.GetAlgoThreaholdParams(m_solutionId.Item1);
                List<AlgoTopPercentageParams> AlgoTopPercentageParamsList = MySQLConnector.GetAlgoTopPercentageParams(m_solutionId.Item1);
                List<AlgoEqualEnergyParams> algoEEParamsList = MySQLConnector.GetAlgoEqualEnergyParams(m_solutionId.Item1);
                m_solutionAlgo = new List<SolutionAlgo>();
               
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if ((dataGridView1.Rows.Count - 1) == index)
                    {
                        break;
                    }
                    SolutionAlgo testAlgo = new SolutionAlgo();
                    TimeSpan t = new TimeSpan(0, 0, 0);
                    string algo = string.Empty;
                    float maxpower;
                    float kj;
                    float absorbed;
                    try
                    {
                        t = TimeSpan.Parse(row.Cells["Time"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Time format error in row: " + index);
                        return false;
                    }

                    try
                    {
                        maxpower = float.Parse(row.Cells["Power"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Time format error in row: " + index);
                        return false;
                    }

                    try
                    {
                        absorbed = float.Parse(row.Cells["absorbed"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("absorbed format error in row: " + index);
                        return false;
                    }

                    try
                    {
                        kj = float.Parse(row.Cells["kj"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Target KJ format error in row: " + index);
                        return false;
                    }

                    try
                    {
                        algo = row.Cells["Algo"].EditedFormattedValue.ToString();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Algo type error in row: " + index);
                        return false;
                    }

                    testAlgo.rowindex = index;
                    testAlgo.id = m_testId;
                    testAlgo.kj = kj;
                    testAlgo.algoname = algo;
                    testAlgo.absorbed = absorbed;
                    testAlgo.maxpower = maxpower;
                    testAlgo.time = t;
                    m_solutionAlgo.Add(testAlgo);
                  
                    CallbackMessage p = new CallbackMessage(CallbackMessages);
                    DRValueCallbackMsg p1 = new DRValueCallbackMsg(DRValuesCallback);
                    m_heat.SetDRValuesCallback(p1);
                    bool allFreqs = false;
                    switch (algo)
                    {
                        case "equal energy":
                            {
                                List<DRRange> l = null;
                                bool found = false;
                                //al = new EqualEnergyAlgo(p,p1);
                                bool AGC = true;
                                bool[] freqs = null;
                                EqualEnergyParams eqparams = new EqualEnergyParams();
                                //m_currentPlan.Add(al);
                                if (checkBox3.Checked)
                                {
                                    EQNGValueCallbackMsg eqc = new EQNGValueCallbackMsg(EQNGValuesCallbackMsg);
                                    m_heat.SetEqualEnergyDataCallback(eqc);
                                }
                                foreach (AlgoEqualEnergyParams d in algoEEParamsList)
                                {
                                    if (d.RowIndex == index)
                                    {

                                        if (testid != -1)
                                            MySQLConnector.AddTestAlgoEqualEnergyParams(testid, index, d);

                                        found = true;

                                        eqparams.mode = (ushort)d.mode;
                                        eqparams.highvalue = (ushort)(d.lowvalue * 10);
                                        eqparams.lowvalue = (ushort)(d.lowvalue * 10);
                                        eqparams.maxpower = (ushort)(testAlgo.maxpower * 10);
                                        eqparams.acckj = (ushort)d.acckj;
                                        eqparams.targetkj = (ushort)(testAlgo.kj * 10);
                                        eqparams.singlerepetition = 1;
                                        eqparams.toppercentage = 0;
                                        eqparams.agc = 1;
                                        eqparams.TotalSeconds = (ushort)(t.TotalSeconds);
                                        
                                        freqs = GetTableFrequencies("equal energy", m_solutionId.Item1, d.RowIndex, out allFreqs);
                                        //al.SetFrequenciesTable(freqs);

                                        l = GetDRRangeForEqualEnergy("equal energy", m_solutionId.Item1, d.RowIndex);
                                        //al.SetDREqualEnergyRange(l);
                                        //al.AGC = d.agc;
                                        AGC = d.agc;
                                        //al.setParameters(parameters);

                                        if (l.Count > 0 && testid > 0)
                                        {
                                            string dfilename = DRIVE.Drive + @"TopazPOC\TestDRRange\" + testid + "_" + index + ".txt";
                                            string sfilename = DRIVE.Drive + @"TopazPOC\SolutionDRRange\" + m_solutionId.Item1 + "_" + index + ".txt";
                                            File.Copy(sfilename, dfilename, true);
                                        }

                                        break;
                                    }
                                }
                                if (found == false)
                                {
                                    MessageBox.Show("Did not found parameters for equal energy at line: " + index);
                                    return false;
                                }
                                freqs = allFreqs == true ? null : freqs;
                                m_heat.AddAlgo(ALGO_TYPES.EQUAL_ENERGY,
                                                eqparams,
                                                l,
                                                freqs,
                                                m_curSolution.drCycleTime, 47 * 1000);
                            }
                        break;
                        case "Gamma Percentage":
                            {
                            }
                        break;
                        case "RF Off":
                            {
                                //al = new HeatOff(p, p1);
                                //m_currentPlan.Add(al);
                                //al.Time = t;
                                //al.drCycleTime = m_curSolution.drCycleTime;
                                //al.SetTestID(m_testId);
                                //al.AGC = false;
                                //m_heat.AddAlgo(al);

                                m_heat.AddAlgo(ALGO_TYPES.HEAT_OFF,
                                               t);


                                if (testid != -1)
                                    MySQLConnector.AddTestAlgoRFOffParams(testid, index, t);
                            }
                        break;
                        case "DR Treshold":
                        {
                            //al = new DRThresholdAlgo(p, p1);
                            //m_currentPlan.Add(al);
                            bool AGC = true;
                            bool[] freqs = null;
                            List<DRRange> l = null;
                            if (checkBox3.Checked)
                            {
                                EQNGValueCallbackMsg eqc = new EQNGValueCallbackMsg(EQNGValuesCallbackMsg);
                                m_heat.SetEqualEnergyDataCallback(eqc);
                            }
                            DRThresholdParams drTsalgoParams = new DRThresholdParams();
                            bool found = false;
                            foreach (AlgoThresholParams d in algoThsParamsList)
                            {
                                if (d.RowIndex == index)
                                {
                                    if (testid != -1)
                                        MySQLConnector.AddTestAlgoThresholdParams(testid, index, d);

                                    found = true;
                                    drTsalgoParams.mode = (ushort)d.mode;
                                    drTsalgoParams.highvalue = (ushort)d.highvalue;
                                    drTsalgoParams.lowvalue = (ushort)d.lowvalue;
                                    drTsalgoParams.powertime = (ushort)d.power_time_mili;
                                    drTsalgoParams.maxpower = (ushort)testAlgo.maxpower;
                                    drTsalgoParams.targetkj = (ushort)testAlgo.kj;
                                    drTsalgoParams.absorbed = (ushort)testAlgo.absorbed;
                                    drTsalgoParams.equaldrtime = (ushort)(d.equaldrtime == true ? 1 : 0);
                                    drTsalgoParams.agc = 1;
                                    freqs = GetTableFrequencies("DR Treshold", m_solutionId.Item1, d.RowIndex, out allFreqs);
                                    //al.SetFrequenciesTable(freqs);
                                    AGC = d.agc;
                                    l = GetDRRangeForEqualEnergy("equal energy", m_solutionId.Item1, d.RowIndex);
                                    //al.SetDREqualEnergyRange(l);
                                    //al.setParameters(parameters);
                                    break;
                                }                                    
                            }
                            if (found == false)
                            {
                                MessageBox.Show("Did not found parameters for DR Treshold at line: " + index);
                                return false;
                            }                          
                            //al.Time = t;
                            //al.drCycleTime = m_curSolution.drCycleTime;
                            //al.SetTestID(m_testId);
                            freqs = allFreqs == true ? null : freqs;
                            m_heat.AddAlgo(ALGO_TYPES.DR_THREASHOLDS, 
                                            drTsalgoParams, 
                                            l, 
                                            freqs,
                                            m_curSolution.drCycleTime, 47 * 1000);
                        }
                        break;
                        case "Top Percentage":
                        {
                            List<DRRange> l = null;
                            bool found = false;
                            bool[] freqs = null;
                            //al = new TopPercentageAlgo(p, p1);
                            //m_currentPlan.Add(al);
                            if (checkBox3.Checked)
                            {
                                EQNGValueCallbackMsg eqc = new EQNGValueCallbackMsg(EQNGValuesCallbackMsg);
                                m_heat.SetEqualEnergyDataCallback(eqc);
                            }
                            TopPercentageParams toppParams = new TopPercentageParams();
                            foreach (AlgoTopPercentageParams d in AlgoTopPercentageParamsList)
                            {
                                if (d.RowIndex == index)
                                {
                                    if (testid != -1)
                                        MySQLConnector.AddTestAlgoTopPercentageParams(testid, index, d);

                                    found = true;
                                   
                                    toppParams.powertime = (ushort)d.power_time_mili;

                                    toppParams.maxpower = (ushort)maxpower;
                                    toppParams.toppercent = (ushort)d.toppercent;
                                    toppParams.targetkj = (ushort)kj;
                                    toppParams.absorbed = (ushort)absorbed;
                                    freqs = GetTableFrequencies("Top Percentage", m_solutionId.Item1, d.RowIndex, out allFreqs);
                                    //al.SetFrequenciesTable(freqs);
                                    toppParams.agc = (ushort)(d.agc == true ? 1: 0);
                                    //al.setParameters(parameters);
                                    break;
                                }
                            }
                            if (found == false)
                            {
                                MessageBox.Show("Did not found parameters for Top Percentage at line: " + index);
                                return false;
                            }

                            freqs = allFreqs == true ? null : freqs;
                            m_heat.AddAlgo(ALGO_TYPES.TOP_PERCENTAGE,
                                           toppParams,
                                           freqs,
                                           m_curSolution.drCycleTime, 47 * 1000);
                        }
                        break;
                    }
                    index++;
                }
                return true;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        List<SolutionAlgo> BuildDataGridList()
        {

            try
            {
                int index = 0;
                m_heat.Clear();
                List<SolutionAlgo> list = new List<SolutionAlgo>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if ((dataGridView1.Rows.Count - 1) == index)
                    {
                        break;
                    }
                    TimeSpan t = new TimeSpan(0, 0, 0);
                    string algo = string.Empty;
                    float maxpower;
                    float absorbed;
                    float kj;
                    try
                    {
                        t = TimeSpan.Parse(row.Cells["Time"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Time format error in row: " + index);
                        return null;
                    }

                    try
                    {
                        maxpower = float.Parse(row.Cells["Power"].Value.ToString());
                        if (maxpower > 54.3)
                        {
                            MessageBox.Show("max power cannot be greated then 54.3 dBm");
                            return null;
                        }
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Time format error in row: " + index);
                        return null;
                    }

                    try
                    {
                        if (row.Cells["absorbed"].Value == null)
                            row.Cells["absorbed"].Value = 0;
                        absorbed = float.Parse(row.Cells["absorbed"].Value.ToString());
                        if (absorbed > 250)
                        {
                            MessageBox.Show("absorbed cannot be greated then 250 watts");
                            return null;
                        }
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Time format error in row: " + index);
                        return null;
                    }

                    try
                    {
                        kj = float.Parse(row.Cells["kj"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Time format error in row: " + index);
                        return null;
                    }

                    try
                    {
                        algo = row.Cells["Algo"].EditedFormattedValue.ToString();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Algo type error in row: " + index);
                        return null;
                    }

                    /*
                    try
                    {
                        uname = row.Cells["Utensil"].EditedFormattedValue.ToString();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Utensil name error in row: " + index);
                        return null;
                    }
                    */
                

                   
                    CallbackMessage p = new CallbackMessage(CallbackMessages);
                    DRValueCallbackMsg p1 = new DRValueCallbackMsg(DRValuesCallback);
                    switch (algo)
                    {
                        case "equal energy":
                            {
                                
                                SolutionAlgo tpl = new SolutionAlgo(
                                   "equal energy", t, maxpower, index, kj, absorbed);
                               list.Add(tpl);
                            }
                            break;
                        case "Gamma Percentage":
                            {
                                SolutionAlgo tpl = new SolutionAlgo(
                                  "Gamma Percentage", t, maxpower, index, kj, absorbed);
                                list.Add(tpl);
                            }
                            break;
                        case "RF Off":
                            {
                                SolutionAlgo tpl = new SolutionAlgo(
                                  "RF Off", t, maxpower, index, kj, absorbed);
                                list.Add(tpl);
                            }
                        break;
                        case "DR Treshold":
                        {
                            SolutionAlgo tpl = new SolutionAlgo(
                              "DR Treshold", t, maxpower, index, kj, absorbed);
                            list.Add(tpl);
                        }
                        break;
                        case "Top Percentage":
                        {
                            SolutionAlgo tpl = new SolutionAlgo(
                              "Top Percentage", t, maxpower, index, kj, absorbed);
                            list.Add(tpl);
                        }
                        break;
                    }
                    index++;
                }
                return list;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        void BuildDataGridFromSolution(string sname, List<SolutionAlgo> list)
        {
            m_heat.Clear();
            dataGridView1.Rows.Clear();
            CallbackMessage p = new CallbackMessage(CallbackMessages);
            int index = 0;

            int rowIndex = 0;

            IList<MyValue> values = new List<MyValue> { 
                new MyValue { id = 0, name = "equal energy" }, 
                //new MyValue { id = 1, name = "Gamma Percentage" } ,
                new MyValue { id = 2, name = "RF Off" }     ,
                new MyValue { id = 3, name = "DR Treshold" } , 
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
            

            DRValueCallbackMsg p1 = new DRValueCallbackMsg(DRValuesCallback);
            foreach (SolutionAlgo t in list)
            {

                switch (t.algoname)
                {
                    case "Gamma Percentage":

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

                /*
                int uid = 0;
                for (uid = 0; uid < uvalues.Count; uid++)
                {
                    if (l[uid] == t.Item4)
                    {
                        break;
                    }
                }

                var cell = dataGridView1[3, rowIndex] as DataGridViewComboBoxCell;
                if (cell == null)
                    return;
                cell.DataSource = uvalues;
                cell.Value = uid;
                cell.ValueMember = "id";
                cell.DisplayMember = "name";                
                */

                rowIndex++;
            }
            label11.Text = m_solutionId.Item2;
        }
        string getRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (m_running == true)
                {
                    return;
                }

                if (m_startDR == true)
                {
                    m_heat.Stop();
                    linkLabel1.LinkColor = Color.Red;
                }

                if (UserSaveSolution() == false)
                {
                    return;
                }
                btnStart.Enabled = false;
                if (m_solutionId.Item1 == -1)
                {
                    MessageBox.Show("Cooking solution is not open");
                    btnStart.Enabled = true;
                    return;
                }
                 
                try
                {

                    if (CheckIfMissingParams() == false)
                    {
                        btnStart.Enabled = true;
                        return;
                    }
 
                    // if the test does not start we need to delete it!!
                    m_testId = -1;
                    if (m_solutionId.Item1 != -1)
                    {
                        if (m_curSolution.groupid == 0)
                            m_curSolution.groupid = 1;
                         
                        m_testId = MySQLConnector.AddNewTest("rnd_" + getRandomString(),
                                                            m_userId,
                                                            m_curSolution.utensilName,
                                                            string.Empty, out m_testStrStartDate,
                                                            m_curSolution);


                        if (m_userName != "fastcook")
                        {

                            TestInfo ?lastTestInfo = MySQLConnector.getLastTestInfo();

                            TestRemark tr = new TestRemark(m_userId, true, m_testId, lastTestInfo, m_curSolution.groupName, m_loadedTestId);
                            if (tr.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                m_testRemark = tr.Remark;
                                m_testName = tr.TestName;
                                MySQLConnector.UpdateTestRemarkAndName(m_testId, m_testRemark, m_testName, tr.CompareReason, tr.CompareTo);
                            }
                            else
                            {
                                MySQLConnector.DeleteTestById(m_testId);
                                btnStart.Enabled = true;
                                return;
                            }
                            
                        }
                        label30.Visible = true;
                        label31.Text = m_testName + " ( " + m_testId + " )";

                        ShowDishNameParmeters(m_curSolution.dishName, true);
                        
                    }
                    if (BuildHeatList(m_testId) == false)
                    {
                        btnStart.Enabled = true;
                        return;
                    }

                    MySQLConnector.AddSolutionAlgoToTest(m_testId, m_solutionAlgo);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    btnStart.Enabled = true;
                    return;
                }
                try
                {
                    SetDebug();
                    GuiOnStart();
                    m_totalKj = 0;
                    m_totalWatts = 0;
                    m_grandTotalWatts = 0;
                    m_garndFordBm = 0;
                    m_grandPowerCount = 0;
                    m_garndRefdBm = 0;
                    m_algoStartTime = new TimeSpan(0, 0, 0);
                    m_paused = false;
                    btnPause.Font = new Font(btnPause.Font, m_paused == true ? FontStyle.Bold : FontStyle.Regular);
                    CallbackMessage p = new CallbackMessage(DirectCallbackMsg);
                    m_heat.SetDirectCallback(p);
                    label15.Text = string.Empty;

                    m_heat.SendDataToController();

                    TimeSpan t = new TimeSpan(0, int.Parse(textBox2.Text), int.Parse(textBox3.Text));
                    m_heat.Start(!checkBox2.Checked, t);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    btnStart.Enabled = true;
                }
            }
        }
        void GuiOnStart()
        {

            myKjCurve.Clear();
            myWattsCurve.Clear();
            mydBMDirectCurve.Clear();

            sskjtotal.Value = "0";
            label7.Text = "0";
            label22.Text = "0";
            label3.Text = "0";
            label4.Text = "0";
            label21.Text = "0";
            label14.Text = "0";
            label17.Text = "0";
            label25.Text = "0";
            label19.Text = "0";

            DR[] values = new DR[POCSET.MAXFREQ];
            float f = 2400;
            for (int i = 0 ; i < POCSET.MAXFREQ; i++)
            {
                values[i].value = 0;
                values[i].freq = f;
                f += 1;
            }
            m_zedDr.CreateGraph_DRGradientByZBars(zg3, values, POCSET.MAXFREQ, checkBox1.Checked);
            RefresheZedGraphs(zg3);


        }
        void Stop()
        {
            lock (this)
            {
                try
                {
                    if (m_paused == true)
                    {
                        m_paused = !m_paused;
                        if (m_paused == true)
                        {
                            btnPause.Text = "Resume";
                        }
                        else
                        {
                            btnPause.Text = "Pause";
                        }
                        m_heat.Pause(m_paused);
                        btnPause.Font = new Font(btnPause.Font, m_paused == true ? FontStyle.Bold : FontStyle.Regular);
                    }
                    m_heat.Stop();
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Thread task = new Thread(() => Stop());
                task.Start();
               
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_heat.Running == true)
            {
                MessageBox.Show("Still cooking");
                e.Cancel = true;
                return;
            }

            if (m_startDR == true)
            {
                m_heat.Stop();
                linkLabel1.LinkColor = Color.Red;
            }

            if (UserSaveSolution() == false)
            {

            }
            if (m_equalEnergyForm != null)
                m_equalEnergyForm.CloseThread();

            try
            {
                button2_Click(sender, e);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            m_appRunning = false;
            m_msgEvent.Set();
            m_msgThread.Join();            
            m_heat.Stop();
            SaveSettings();
        }

        private void saveDishSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSolutionName s = new SaveSolutionName();
                if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                     
                    SaveSolution(s.SolutionName, s.Description, s.Utensil, s.DishName, s.DRPower, s.DRCycleTime);
                    m_curSolution.drCycleTime = s.DRCycleTime;
                    m_curSolution.description = s.Description;
                    m_curSolution.utensilName = s.Utensil;
                    m_curSolution.drpower = s.DRPower;
                    m_curSolution.dishName = s.DishName;
                    label34.Text = s.DishName;
                    m_appSet.drPower = m_curSolution.drpower;
                    preferencesToolStripMenuItem.Enabled = true;
                    saveDishSolutionToolStripMenuItem1.Enabled = true;
                    /*
                    try
                    {
                        if (BuildHeatList(-1) == false)
                            return;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                        return;
                    }
                     */
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void SaveSolution(string sname, string desc, string utensil, string dishName, float drpower, int drCycleTime)
        {
            try
            {
                TimeSpan totaltime;
                try
                {
                    totaltime = new TimeSpan(0, int.Parse(textBox2.Text), int.Parse(textBox3.Text));
                }
                catch (Exception  err)
                {
                    totaltime = new TimeSpan(0, 0, 0);
                }
                float totalKj = 0;
                try
                {
                    totalKj = float.Parse(textBox1.Text);
                }
                catch (Exception err)
                {
                    textBox1.Text = "0";
                }

                List<SolutionAlgo> list = BuildDataGridList();
                if (list == null)
                    return;
                if (list.Count == 0)
                {
                    MessageBox.Show("Please add one algo step into the dish solution and then save it");
                    return;
                }

                int sid = MySQLConnector.SaveSolution(sname, 
                                                      desc,
                                                      utensil, 
                                                      dishName, 
                                                      m_userId, 
                                                      list, 
                                                      totaltime,
                                                      totalKj, 
                                                      drpower,
                                                      m_ovenGuid,
                                                      drCycleTime, 
                                                      1);                
                m_solutionId = new Tuple<int, string>(sid, sname);
                label11.Text = sname;
                label34.Text = dishName;
                m_curSolution = MySQLConnector.GetSolutionInfo(sid);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void UpdateSolution(string sname)
        {
            try
            {
                List<SolutionAlgo> list = BuildDataGridList();
                if (list.Count == 0)
                {
                    MessageBox.Show("Please add one algo step into the dish solution and then save it");
                    return;
                }
                TimeSpan totaltime;
                try
                {
                    totaltime = new TimeSpan(0, int.Parse(textBox2.Text), int.Parse(textBox3.Text));
                }
                catch (Exception err)
                {
                    totaltime = new TimeSpan(0, 0, 0);
                }
                float totalKj = 0;
                try
                {
                    totalKj = float.Parse(textBox1.Text);
                }
                catch (Exception err)
                {
                    textBox1.Text = "0";
                }
                m_curSolution.totalTime = totaltime;
                m_curSolution.TotalKj = totalKj;
                label34.Text = m_curSolution.dishName;
                MySQLConnector.UpdateSolution(sname, 
                                              totalKj, m_appSet.drPower, 
                                              totaltime , m_userId, list, 
                                              m_ovenGuid,
                                              m_curSolution.drCycleTime,
                                              m_curSolution.groupid);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void newSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_heat.Running)
            {
                MessageBox.Show("Stop cooking to create new dish");
                return;
            }
            dataGridView1.Rows.Clear();
            preferencesToolStripMenuItem.Enabled = false;
            m_solutionId = new Tuple<int,string>(-1, string.Empty);
            label11.Text = "No solution";
            saveDishSolutionToolStripMenuItem1.Enabled = false;
            m_heat.Clear();
        }

        private void openDishSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSolutionsForm f = new OpenSolutionsForm(m_userId, m_ovenGuid);
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (f.SolutionName == string.Empty)
                {
                    MessageBox.Show("Solution not selected");
                    return;
                }
                else
                {
                    OpenSolution(f.SolutionName, f.SolutionId);
                    m_solutionId = new Tuple<int, string>(f.SolutionId, f.SolutionName);
                    MySQLConnector.SaveLastSolution(f.SolutionId, m_userId);
                    saveDishSolutionToolStripMenuItem1.Enabled = true;
                }
            }
        }
        void OpenSolution(string sname, int sid)
        {

            try
            {
                m_curSolution = MySQLConnector.GetSolutionInfo(sname, "ALL", m_userId);
                m_appSet.drPower = m_curSolution.drpower;
                label34.Text = m_curSolution.dishName;
                List<SolutionAlgo> list = MySQLConnector.LoadSolutions(sid);
                BuildDataGridFromSolution(sname, list);
                UpdateGuiSolutionInfo(sid, sname);
                preferencesToolStripMenuItem.Enabled = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }            
        }
        void UpdateGuiSolutionInfo(int sid, string sname)
        {
            m_solutionId = new Tuple<int, string>(sid, sname);
            label11.Text = sname;

            if (m_curSolution.totalTime == new TimeSpan(0, 0, 0))
            {
                textBox2.Text = "0";
                textBox3.Text = "0";
                checkBox2.Checked = false;
            }
            else
            {
                textBox2.Text = m_curSolution.totalTime.Minutes.ToString();
                textBox3.Text = m_curSolution.totalTime.Seconds.ToString();
                checkBox2.Checked = true;
            }
            textBox1.Text = m_curSolution.TotalKj.ToString();
            SolutionPictureInfo? d = MySQLConnector.GetSolutionPicture(sid);

            if (d != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.ImageLocation = d.Value.show == true ? d.Value.fullpicname : null;
                showToolStripMenuItem.Visible = File.Exists(pictureBox1.ImageLocation);
            }
            else
            {
                pictureBox1.ImageLocation = null;
            }
        }
        void OpenSolution(string sname)
        {
            try
            {
                int sid;
                List<SolutionAlgo> list = MySQLConnector.LoadSolutions(sname, out sid);
                m_solutionId = new Tuple<int, string>(sid, sname);

                DISH_SOLUTION d = MySQLConnector.GetSolutionInfo(sname, "ALL", m_userId);

                BuildDataGridFromSolution(sname, list);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }  

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void saveDishSolutionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (UserSaveSolution() == false)
            {

            }
        }

        bool UserSaveSolution()
        {
            try
            {
                try
                {
                    if (BuildHeatList(-1) == false)
                        return false;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return false;
                }

                if (m_solutionId.Item2 == string.Empty || m_solutionId.Item1 == -1)
                {
                    MessageBox.Show("Solution not loaded");
                    return false;
                }
                UpdateSolution(m_solutionId.Item2);
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return false;
            }
        }

   
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        
        private void CreateZedForwardPowerGraph()
        {
            m_zedDirect = new ZedChart(zgdirect.GraphPane, "dBm,", "Forward power in dBm", "Power dBm", "dBm", 2, Color.Blue);
            m_zedWatts = new ZedChart(zg2.GraphPane, "Watts,", "Forward power in watts", "Watts", "Watts", 2, Color.Blue);
            m_zedWatts.CreateSeries(1);
            m_zedDirect.CreateSeries(1);

            m_zedKj = new ZedChart(zg1.GraphPane, "Channel 2", "Heat entries", "Kj", "Energy", 2, Color.Green);
            m_zedKj.CreateSeries(1);
            m_zedDr = new ZedChart(zg3.GraphPane, "DR,", "Frequencies", "dB", "DR", 2, Color.Blue);
                                
            myKjCurve = zg1.GraphPane.AddCurve("Energy(Kj)", spl1, Color.Red, SymbolType.None);
            myWattsCurve = zg2.GraphPane.AddCurve("Watts", spl2, Color.Blue, SymbolType.None);

            mydBMDirectCurve = zgdirect.GraphPane.AddCurve("dBm forward", spl3, Color.Blue, SymbolType.None);

            

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

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences s = new Preferences(m_curSolution, m_userId);
            if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DISH_SOLUTION d = s.GetDishSolution();
                m_curSolution = d;
                m_appSet.drPower = d.drpower;
                UpdateSolution(m_solutionId.Item2);
            }           
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                try
                {
                    DeleteRowDataFromDB(index, dataGridView1.Rows[index].Cells["Algo"].EditedFormattedValue.ToString());
                    string path = DRIVE.Drive + @"TopazPOC\SolutionDRRange\";
                    string fileName = path + m_curSolution.id + "_" + index + ".txt";
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                dataGridView1.ClearSelection();
            }
        }

        void SaveTopPercentageAlgoParams(int sid, AlgoTopPercentageParams ?p, int rowindex)
        {
            try
            {                 
                if (p != null)
                {
                    MySQLConnector.DeleteAlgorithemParams(m_solutionId.Item1, rowindex, "Top Percentage");
                    MySQLConnector.SaveAlgoTopPercentageParams(m_solutionId.Item1, rowindex, p.Value);
                }
                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        void SaveDRTresholdAlgoParams(int sid, AlgoThresholParams ? p, int rowindex)
        {
            try
            {

                if (p != null)
                {
                    MySQLConnector.DeleteAlgorithemParams(m_solutionId.Item1, rowindex, "DR Treshold");
                    MySQLConnector.SaveAlgoThresholdParams(m_solutionId.Item1, rowindex, p.Value);
                }
                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        void SaveEqualEnergyAlgoParams(int sid, AlgoEqualEnergyParams? p, int rowindex)
        {
            try
            {

                if (p != null)
                {
                    MySQLConnector.DeleteAlgorithemParams(m_solutionId.Item1, rowindex, "equal energy");
                    MySQLConnector.SaveAlgoEqualEnergyParams(sid, rowindex, p.Value);
                }
                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                try
                {
                    TimeSpan time = new TimeSpan(0, 0, 0);
                    if (dataGridView1.Rows[e.RowIndex].Cells["Time"].Value != null)
                    {
                        time = TimeSpan.Parse(dataGridView1.Rows[e.RowIndex].Cells["Time"].Value.ToString());
                    }
                    TimeForm f = new TimeForm(time);
                    f.ShowDialog();
                    time = f.Time;
                    dataGridView1.Rows[e.RowIndex].Cells["Time"].Value = time.ToString();
                    
                    return;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return;
                }

            }
            else
            if (e.ColumnIndex == 2)
            {
                try
                {
                    popupPowerSelectForm f = new popupPowerSelectForm();
                    f.ShowDialog();
                    dataGridView1.Rows[e.RowIndex].Cells["Power"].Value = f.Power.ToString();
                    return;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return;
                }
            } else 
            if (e.ColumnIndex == -1)
            {
                if (m_solutionId.Item1 == -1)
                {
                    MessageBox.Show("Please save the solution in a new name first");
                    return;
                }
                if (dataGridView1.Rows[e.RowIndex].Cells["Algo"].Value == null)
                    return;
                switch (dataGridView1.Rows[e.RowIndex].Cells["Algo"].Value.ToString())
                {
                    case "0":
                        {
                            try
                            {
                                AlgoEqualEnergyParams? d =
                                MySQLConnector.GetAlgoEqualEnergyParams(m_solutionId.Item1, e.RowIndex);
                                EqualEnergyAlgoForm f = new EqualEnergyAlgoForm(d);
                                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    AlgoEqualEnergyParams p = f.GetParams();
                                    p.sid = m_solutionId.Item1;
                                    p.RowIndex = e.RowIndex;
                                    // save to DB for m_sid and e.rowindex
                                    if (d == null)
                                    {
                                        MySQLConnector.SaveAlgoEqualEnergyParams(m_solutionId.Item1, e.RowIndex, p);
                                    }
                                    else
                                    {
                                        MySQLConnector.UpdateAlgoEqualEnergyParams(m_solutionId.Item1, e.RowIndex, p);
                                    }
                                }

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
                                MySQLConnector.GetAlgoThreaholdParams(m_solutionId.Item1, e.RowIndex);
                                DRThresholdAlgoForm f = new DRThresholdAlgoForm(d);
                                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    AlgoThresholParams p = f.GetParams();
                                    p.sid = m_solutionId.Item1;
                                    p.RowIndex = e.RowIndex;
                                    // save to DB for m_sid and e.rowindex
                                    if (d == null)
                                    {
                                        MySQLConnector.SaveAlgoThresholdParams(m_solutionId.Item1, e.RowIndex, p);
                                    }
                                    else
                                    {
                                        MySQLConnector.UpdateAlgoThresholdParams(m_solutionId.Item1, e.RowIndex, p);
                                    }
                                }

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
                                MySQLConnector.GetAlgoTopPercentageParams(m_solutionId.Item1, e.RowIndex);
                                AlgoTopPercentageForm f = new AlgoTopPercentageForm(d);
                                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    AlgoTopPercentageParams p = f.GetParams();
                                    p.sid = m_solutionId.Item1;
                                    p.RowIndex = e.RowIndex;
                                    // save to DB for m_sid and e.rowindex
                                    if (d == null)
                                    {
                                        MySQLConnector.SaveAlgoTopPercentageParams(m_solutionId.Item1, e.RowIndex, p);
                                    }
                                    else
                                    {
                                        MySQLConnector.UpdateAlgoTopPercentageParams(m_solutionId.Item1, e.RowIndex, p);
                                    }
                                }

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

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void copySolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySolution f = new CopySolution(m_userId, m_ovenGuid);
            f.ShowDialog();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (m_paused == false)
            {
                DialogResult d = MessageBox.Show("Asking you again , do you want to pause?", "Topaz POC", MessageBoxButtons.YesNo);
                if (d == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }            
            m_paused = !m_paused;            
            m_heat.Pause(m_paused);
            btnPause.Font = new Font(btnPause.Font, m_paused == true ? FontStyle.Bold : FontStyle.Regular);

            if (m_paused == true)
            {
                btnPause.Text = "Resume";
            }
            else
            {
                btnPause.Text = "Pause";
            }
        }

        private void beforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                AddPicturesOffline a = new AddPicturesOffline(m_userId, true);
                a.ShowDialog();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void InvokePictureOnEnd()
        {
            this.BeginInvoke((Action)(() =>
            {
                AddPictures a = new AddPictures(m_userId, false, m_testId);
                a.ShowDialog();               
            }));            
        }
        private void afterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                AddPicturesOffline a = new AddPicturesOffline(m_userId, false);
                a.ShowDialog();
                 
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        bool[] GetTableFrequencies(string algoName, int sid, int rowIndex, out bool allFreqs)
        {
            bool [] freqs = new bool[POCSET.MAXFREQ];
            string fileName = "all";
            allFreqs = false;
            switch(algoName)
            {

                case "DR Treshold":
                    {
                        AlgoThresholParams? d = MySQLConnector.GetAlgoThreaholdParams(sid, rowIndex);
                        if (d != null)
                        {
                            if (d.Value.freqtablefilename.ToLower() == "all")
                            {
                                allFreqs = true;
                                for (int i = 0 ; i < freqs.Length;i++)
                                {
                                    freqs[i] = true;
                                }
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
                    AlgoEqualEnergyParams? d = MySQLConnector.GetAlgoEqualEnergyParams(sid, rowIndex);
                    if (d != null)
                    {
                        if (d.Value.freqtablefilename.ToLower() == "all")
                        {
                            allFreqs = true;
                            for (int i= 0 ; i < freqs.Length;i++)
                            {
                                freqs[i] = true;
                            }
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
                    AlgoTopPercentageParams? d = MySQLConnector.GetAlgoTopPercentageParams(sid, rowIndex);
                    if (d != null)
                    {
                        if (d.Value.freqtablefilename.ToLower() == "all")
                        {
                            allFreqs = true;
                            for (int i =0 ; i < freqs.Length;i++)
                            {
                                freqs[i] = true;
                            }
                        }
                        else
                        {
                            fileName = d.Value.freqtablefilename;
                        }
                    }
                }
                break;
            }
            if (fileName == "")
            {
                allFreqs = true;
                fileName = "all";
                for (int i = 0 ; i < freqs.Length;i++)
                {
                    freqs[i] = true;
                }
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
                        freqs[freq - 2400] = true;
                    }
                }
            }
            return freqs;
        }

        List<DRRange> GetDRRangeForEqualEnergy(string algoName, int sid, int rowIndex)
        {
            string path = DRIVE.Drive + @"TopazPOC\SolutionDRRange\";
            string fileName = path + sid + "_" + rowIndex + ".txt";

            List<DRRange> f = new List<DRRange>();
            if (File.Exists(fileName) == false)
                return f;
            using (StreamReader sw = new StreamReader(fileName))
            {
                string s;
                while (true)
                {
                    string line = sw.ReadLine();
                    if (line == null)
                        break;
                    DRRange d = new DRRange();
                    string[] sdata = line.Split(new Char[] { ',' });
                    d.drlow = float.Parse(sdata[0]);
                    d.drhigh = float.Parse(sdata[1]);
                    d.percentage = int.Parse(sdata[2]);
                    f.Add(d);
                }
            }  

            
            return f;
        }
        void ExportSolution()
        {
            StreamWriter sw = null;
            try
            {
                DISH_SOLUTION sdata = MySQLConnector.GetSolutionInfo(m_solutionId.Item1);
                string filename = sdata.name + "_" + sdata.id + "_" + sdata.dishName + "_" + sdata.firstname + "_" + sdata.lastname + ".cfg";
                sw = new StreamWriter(filename);
                sw.WriteLine("solution:" + sdata.name);                
                sw.WriteLine("{");
                sw.WriteLine("id:" + sdata.id);
                sw.WriteLine("dish_name:" + sdata.dishName);
                sw.WriteLine("firstname:" + sdata.firstname);
                sw.WriteLine("lastname:" + sdata.lastname);
                sw.WriteLine("createdBy:" + sdata.createdBy);
                sw.WriteLine("description:" + sdata.description);
                sw.WriteLine("utensilName:" + sdata.utensilName);
                sw.WriteLine("}");


                List<AlgoThresholParams> algoThsParamsList = MySQLConnector.GetAlgoThreaholdParams(m_solutionId.Item1);
                List<AlgoTopPercentageParams> AlgoTopPercentageParamsList = MySQLConnector.GetAlgoTopPercentageParams(m_solutionId.Item1);
                List<AlgoEqualEnergyParams> algoEEParamsList = MySQLConnector.GetAlgoEqualEnergyParams(m_solutionId.Item1);
               
                int index = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if ((dataGridView1.Rows.Count - 1) == index)
                    {
                        break;
                    }
                    TimeSpan t = new TimeSpan(0, 0, 0);
                    string algo = string.Empty;
                    float maxpower;
                    float kj;
                    float absorbed;
                    try
                    {
                        t = TimeSpan.Parse(row.Cells["Time"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Time format error in row: " + index);
                        sw.Close();
                        return;
                    }

                    try
                    {
                        maxpower = float.Parse(row.Cells["Power"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Time format error in row: " + index);
                        sw.Close();
                        return;
                    }

                    try
                    {
                        absorbed = float.Parse(row.Cells["absorbed"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("absorbed format error in row: " + index);
                        sw.Close();
                        return;
                    }

                    try
                    {
                        kj = float.Parse(row.Cells["kj"].Value.ToString());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show("Target KJ format error in row: " + index);
                        sw.Close();
                        return;
                    }

                    try
                    {
                        algo = row.Cells["Algo"].EditedFormattedValue.ToString();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Algo type error in row: " + index);
                        sw.Close();
                        return;
                    }
                     
                    CallbackMessage p = new CallbackMessage(CallbackMessages);
                    DRValueCallbackMsg p1 = new DRValueCallbackMsg(DRValuesCallback);
                    switch (algo)
                    {
                        case "equal energy":
                            {
                                bool found = false;

                                sw.WriteLine("algo:equal energy");
                                sw.WriteLine("{");
                                sw.WriteLine("time:" + t);
                                foreach (AlgoEqualEnergyParams d in algoEEParamsList)
                                {
                                    if (d.RowIndex == index)
                                    {
                                        found = true;
                                        Parameter[] parameters = new Parameter[9];
                                        parameters[0] = new IntParameter("mode", d.mode);
                                        parameters[1] = new FloatParameter("highvalue", d.highvalue);
                                        parameters[2] = new FloatParameter("lowvalue", d.lowvalue);
                                        parameters[3] = new FloatParameter("maxpower", maxpower);
                                        parameters[4] = new FloatParameter("acckj", d.acckj);
                                        parameters[5] = new FloatParameter("targetkj", kj);
                                        parameters[6] = new FloatParameter("absorbed", absorbed);
                                        parameters[7] = new BoolParameter("singlerepetition", d.singlerepetition);
                                        parameters[8] = new IntParameter("toppercentage", d.toppercentage);
                                        bool allFreqs = false;
                                        bool[] freqs = GetTableFrequencies("equal energy", m_solutionId.Item1, d.RowIndex, out allFreqs);
                                        for (int i = 0; i < parameters.Length; i++)
                                        {
                                            sw.WriteLine("{0}:{1}" , parameters[i].GetName(), parameters[i].GetValue().ToString());
                                        }
                                        WriteFreqs(sw, freqs);
                                        break;
                                    }
                                }
                                sw.WriteLine("}");
                                if (found == false)
                                {
                                    MessageBox.Show("Did not found parameters for equal energy at line: " + index);
                                    sw.Close();
                                    return;
                                }
                            }
                        break;
                        case "Gamma Percentage":
                            {
                                                           
                            }
                        break;
                        case "RF Off":
                            {                                 
                                                          
                            }
                        break;
                        case "DR Treshold":
                        {
                            sw.WriteLine("algo:DR Treshold");
                            sw.WriteLine("{");
                            sw.WriteLine("time:" + t);
                            bool found = false;
                            foreach (AlgoThresholParams d in algoThsParamsList)
                            {
                                if (d.RowIndex == index)
                                {
                                    found = true;
                                    Parameter[] parameters = new Parameter[8];
                                    parameters[0] = new IntParameter("mode", d.mode);
                                    parameters[1] = new FloatParameter("highvalue", d.highvalue);
                                    parameters[2] = new FloatParameter("lowvalue", d.lowvalue);
                                    parameters[3] = new IntParameter("powertime", d.power_time_mili);
                                    parameters[4] = new FloatParameter("maxpower", maxpower);
                                    parameters[5] = new FloatParameter("targetkj", kj);
                                    parameters[6] = new FloatParameter("absorbed", absorbed);
                                    parameters[7] = new BoolParameter("equaldrtime", d.equaldrtime);
                                    bool allFreqs = false;
                                    bool[] freqs = GetTableFrequencies("DR Treshold", m_solutionId.Item1 , d.RowIndex, out allFreqs);
                                    if (allFreqs == false)
                                    {
                                        for (int i = 0; i < parameters.Length; i++)
                                        {
                                            sw.WriteLine("{0}:{1}", parameters[i].GetName(), parameters[i].GetValue().ToString());
                                        }
                                    }
                                    else
                                    {
                                        sw.WriteLine("all");
                                    }
                                  
                                    break;
                                }                                    
                            }
                            sw.WriteLine("}");
                            if (found == false)
                            {
                                MessageBox.Show("Did not found parameters for DR Treshold at line: " + index);
                                sw.Close();
                                return;
                            }                            
                        }
                        break;
                        case "Top Percentage":
                        {
                            bool found = false;
                            sw.WriteLine("algo:Top Percentage");
                            sw.WriteLine("{");
                            sw.WriteLine("time:" + t);
                            foreach (AlgoTopPercentageParams d in AlgoTopPercentageParamsList)
                            {
                                if (d.RowIndex == index)
                                {
                                    found = true;
                                    Parameter[] parameters = new Parameter[5];
                                    parameters[0] = new IntParameter("powertime", d.power_time_mili);
                                    parameters[1] = new FloatParameter("maxpower", maxpower);
                                    parameters[2] = new IntParameter("toppercent", d.toppercent);
                                    parameters[3] = new FloatParameter("targetkj", kj);
                                    parameters[4] = new FloatParameter("absorbed", absorbed);
                                    bool allFreqs = false;
                                    bool[] freqs = GetTableFrequencies("Top Percentage", m_solutionId.Item1, d.RowIndex, out allFreqs);
                                    if (allFreqs == false)
                                    {
                                        for (int i = 0; i < parameters.Length; i++)
                                        {
                                            sw.WriteLine("{0}:{1}", parameters[i].GetName(), parameters[i].GetValue().ToString());
                                        }
                                    }
                                    else
                                    {
                                        sw.WriteLine("all");
                                    }                                    
                                    break;
                                }
                            }
                            sw.WriteLine("}");
                            if (found == false)
                            {
                                MessageBox.Show("Did not found parameters for Top Percentage at line: " + index);
                                sw.Close();
                                return;
                            }
                        }
                        break;
                    }
                    index++;
                }
                sw.Close();
                MessageBox.Show("Done\nFile created by name:" + Environment.NewLine + filename);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                if (sw != null)
                    sw.Close();
            }
        }
        void WriteFreqs(StreamWriter sw, bool [] freqs)
        {
            bool b = true;
            for (int i = 0; i < freqs.Length; i++)
            {
                if (freqs[i] == false)
                {
                    b = false;
                    break;
                }
            }
            if (b == true)
            {
                sw.WriteLine("freqs:all");
            }
            else
            {
                for (int i = 0; i < freqs.Length; i++)
                {
                    if (freqs[i] == true)
                    {
                        sw.WriteLine("freqs:" + (2400 + i));
                    }
                }
            }
        }
        private void exportSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task task = new Task(() => { ExportSolution(); });
            task.Start();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm(m_ovenGuid);
            f.TopMost = true;
            this.Hide();
            f.ShowDialog();
            this.Show();
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
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string fileName;
                Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\SolutionPictures");
                if ((fileName = GetFileName()) != string.Empty)
                {
                    string savepath = DRIVE.Drive + @"TopazPOC\SolutionPictures\";
                    try
                    {
                        File.Copy(fileName, savepath + Path.GetFileName(fileName), true);
                    }
                    catch (Exception err)
                    {

                    }
                    MySQLConnector.UpdateSolutionPicture(m_solutionId.Item1, fileName);
                    pictureBox1.ImageLocation = fileName;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void removePictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = null;
            MySQLConnector.DeleteSolutionPicture(m_solutionId.Item1);
        }

        void DeleteRowDataFromDB(int rowindex , string algoname)
        {
            try
            {
                MySQLConnector.DeleteAlgorithemParams(m_solutionId.Item1, rowindex, algoname);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {

             try
             {
                 int index = dataGridView1.SelectedRows[0].Index;

                 try
                 {
                     DeleteRowDataFromDB(index, dataGridView1.Rows[index].Cells["Algo"].EditedFormattedValue.ToString());
                     string path = DRIVE.Drive + @"TopazPOC\SolutionDRRange\";
                     string fileName = path + m_curSolution.id + "_" + index + ".txt";
                     if (File.Exists(fileName))
                         File.Delete(fileName);
                 }
                 catch (Exception err)
                 {
                     MessageBox.Show(err.Message);
                 }
                 dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                 dataGridView1.ClearSelection();
             }
             catch (Exception err)
             {

             }
        }
    
        private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
            if (rowIndex != -1)
            {

                var c = dataGridView1[0, rowIndex] as DataGridViewComboBoxCell;
                if (c == null)
                    return;

                dataGridView1.Rows.Add();                
                var c1 = dataGridView1[0, rowIndex + 1] as DataGridViewComboBoxCell;
                if (c1 == null)
                    return;

                c1.DataSource = c.DataSource;
                c1.Value = c.Value;
                c1.ValueMember = "id";
                c1.DisplayMember = "name";
                dataGridView1.Rows[rowIndex + 1].Cells[1].Value = dataGridView1.Rows[rowIndex].Cells[1].Value;
                dataGridView1.Rows[rowIndex + 1].Cells[2].Value = dataGridView1.Rows[rowIndex].Cells[2].Value;

                dataGridView1.Rows[rowIndex + 1].Cells[3].Value = dataGridView1.Rows[rowIndex].Cells[3].Value;
                dataGridView1.Rows[rowIndex + 1].Cells[4].Value = dataGridView1.Rows[rowIndex].Cells[4].Value;
                

            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = checkBox2.Checked;
            textBox3.Enabled = checkBox2.Checked;
        }
        void DirectCallbackMsg(int code, string msg)
        {
            switch (code)
            {
                case 64:
                    PointPair p = new PointPair();
                    p.X = m_totalCookingTimeInSeconds;
                    p.Y = double.Parse(msg);
                    DrawPoint(zgdirect, p);
                    RefresheZedGraphs(zgdirect);
                break;
                case 203:
                    label15.Text = msg;
                    Application.DoEvents();
                break; 
                case 634:
                    label28.Text = msg;
                break;
            }
        }

        void SetDebug()
        {
            EQNGValueCallbackMsg eqc = null;
            if (checkBox3.Checked)
            {
                zgdirect.Visible = true;
                CallbackMessage p = new CallbackMessage(DirectCallbackMsg);
                m_heat.SetDirectCallback(p);
                eqc = new EQNGValueCallbackMsg(EQNGValuesCallbackMsg);
            }
            else
            {
                zgdirect.Visible = false;
                m_heat.SetDirectCallback(null);
            }
                         
            m_heat.SetEqualEnergyDataCallback(eqc);
             
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            SetDebug();
        }
        void StopFromCallback()
        {

            if (m_startDR == true)
            {
                m_startDR = false;
                return;
            }
            if (m_running == true)
            {
                try
                {
                    MySQLConnector.UpdateFinalResultStopTime(m_testId);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }

                for (int i = 0; i < 3; i++)
                {                   
                    m_soundPlayer.playSound(TestSoundPlayer.SOUND_TYPES.END_OF_COOKING);
                    Thread.Sleep(400);
                }
                string description = "fastcook";
                int passfail = 0;
                if (m_userName != "fastcook")
                {
                    bool found;
                    DISHInfo di = MySQLConnector.GetDishInfo(m_curSolution.dishName, out found);
                    PassFailForm f = new PassFailForm(di.dishScoreEnd);
                    f.ShowDialog();
                    description = f.Description;
                    passfail = f.PassFail;
                }

                string energyFileName = DRIVE.Drive + @"TopazPOC\Energy\" + m_testId + "_energy.csv";
                string DRFileName = DRIVE.Drive + @"TopazPOC\DR\" + m_testId + "_dr.csv";
                string PowerInfoFileName = DRIVE.Drive + @"TopazPOC\Power\" + m_testId + "_dr.csv";

                try
                {
                    MySQLConnector.UpdateTestResults(m_testId, passfail, description, energyFileName, DRFileName, PowerInfoFileName);
                    if (m_userName != "fastcook")
                    {
                        InvokePictureOnEnd();
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }

                try
                {
                    MySQLConnector.UpdateEnergyInfoAndTestStop(m_testId,
                                                    (float)m_totalKj,
                                                    (float)m_grandTotalWatts,
                                                    (float)(m_garndFordBm / m_grandPowerCount),
                                                    (float)(m_garndRefdBm / m_grandPowerCount));
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }

                ShowDishNameParmeters(m_curSolution.dishName, false);

                btnStop.Font = new Font(btnStop.Font, FontStyle.Bold);
                btnStart.ForeColor = Color.Black;
                dataGridView1.Enabled = true;
                btnStart.Enabled = true;
                try
                {
                    CaptureScreen();
                }
                catch (Exception err)
                {

                }                
            }
            m_running = false;
        }

        void ColorLablesForAGCType(bool type)
        {
            System.Windows.Forms.Label[] l = { label3, label4, label7, label22 };
            foreach (System.Windows.Forms.Label x in l )
            {
                if (type == true)
                {
                    x.ForeColor = Color.Blue;
                }
                else
                {
                    x.ForeColor = Color.Black;
                }
            }
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "F4")
            {
                Properties.Settings.Default.mode = "chef";
                Properties.Settings.Default.Save();
                Close();
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showToolStripMenuItem.Text == "Hide")
            {
                pictureBox1.ImageLocation = null;
                showToolStripMenuItem.Text = "Show";
                MySQLConnector.UpdateShowHideSolutionPicture(m_solutionId.Item1, true);
            }
            else
            {

                SolutionPictureInfo? d = MySQLConnector.GetSolutionPicture(m_solutionId.Item1);

                if (d != null)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.ImageLocation = d.Value.fullpicname;
                    showToolStripMenuItem.Visible = File.Exists(pictureBox1.ImageLocation);
                }

                showToolStripMenuItem.Text = "Hide";
                MySQLConnector.UpdateShowHideSolutionPicture(m_solutionId.Item1, true);
            }
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Are you sure?", "Topaz POC", MessageBoxButtons.YesNo);
            if (DialogResult == System.Windows.Forms.DialogResult.No)
                return;

            try
            {
                int index = dataGridView1.SelectedRows[0].Index;
                try
                {
                    DeleteRowDataFromDB(index, dataGridView1.Rows[index].Cells["Algo"].EditedFormattedValue.ToString());
                    string path = DRIVE.Drive + @"TopazPOC\SolutionDRRange\";
                    string fileName = path + m_curSolution.id + "_" + index + ".txt";
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
                dataGridView1.Rows.RemoveAt(index);
                dataGridView1.ClearSelection();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void loadTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                LoadTest f = new LoadTest(m_userId);
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TestInfo testInfo = f.TestInformation;
                    m_curSolution = f.Solution;
                    m_curSolution.name = f.TestInformation.testname + ".sln";
                    
                    List<SolutionAlgo> list = MySQLConnector.GetTestSolutionAlgorithems(testInfo.id);
                    m_loadedTestId = testInfo.id;

                    int sid = -1;
                    try
                    {
                        sid = MySQLConnector.SaveSolution(m_curSolution.name,
                                                            m_curSolution.description,
                                                            m_curSolution.utensilName,
                                                            m_curSolution.dishName,
                                                            m_userId,
                                                            list,
                                                            m_curSolution.totalTime,
                                                            m_curSolution.TotalKj,
                                                            m_curSolution.drpower,
                                                            m_ovenGuid, 
                                                            m_curSolution.drCycleTime, 
                                                            m_curSolution.groupid);

                        UpdateGuiSolutionInfo(sid, m_curSolution.name);
                        m_curSolution.id = sid;                      
                        BuildDataGridFromSolution(m_curSolution.name, list);
                    }
                    catch (Exception err)
                    {
                        try
                        {

                           sid = MySQLConnector.UpdateTestAsSolution(m_curSolution.name,
                                                                      m_curSolution.description,
                                                                      m_curSolution.utensilName,
                                                                      m_curSolution.dishName,
                                                                      m_userId,
                                                                      list,
                                                                      m_curSolution.totalTime,
                                                                      m_curSolution.TotalKj,
                                                                      m_curSolution.drpower,
                                                                      m_ovenGuid);

                           m_curSolution.id = sid;
                           UpdateGuiSolutionInfo(m_curSolution.id, m_curSolution.name);
                           BuildDataGridFromSolution(m_curSolution.name, list);
                        }
                        catch (Exception err1)
                        {
                            MessageBox.Show(err1.Message);
                        }
                    }

                    int index = 0;
                    foreach (SolutionAlgo s in list)
                    {
                        switch (s.algoname)
                        {
                            case "Top Percentage":
                            {
                                AlgoTopPercentageParams ? p = MySQLConnector.GetTestAlgoTopPercentageParams(testInfo.id, index);
                                if (p != null && p.Value.freqtablefilename != "all")
                                {
                                    string filename = DRIVE.Drive + @"TopazPOC\SolutionFrequencies\" + sid + "_" + index + ".txt";
                                    File.Copy(p.Value.freqtablefilename, filename, true);
                                    AlgoTopPercentageParams p1 = new AlgoTopPercentageParams();
                                    p1 = p.Value;
                                    p1.freqtablefilename = filename;
                                    p = p1;
                                }
                                SaveTopPercentageAlgoParams(m_curSolution.id, p, index);
                            }
                            break;
                            case "DR Treshold":
                            {
                                AlgoThresholParams? p = MySQLConnector.GetTestAlgoThreaholdParams(testInfo.id, index);
                                if (p != null && p.Value.freqtablefilename != "all")
                                {
                                    string filename = DRIVE.Drive + @"TopazPOC\SolutionFrequencies\" + sid + "_" + index + ".txt";
                                    File.Copy(p.Value.freqtablefilename, filename, true);
                                    AlgoThresholParams p1 = new AlgoThresholParams();
                                    p1 = p.Value;
                                    p1.freqtablefilename = filename;
                                    p = p1;
                                }
                                SaveDRTresholdAlgoParams(m_curSolution.id, p, index);
                            }
                            break;
                            case "equal energy":
                            {
                                AlgoEqualEnergyParams? p = MySQLConnector.GetTestAlgoEqualEnergyParams(testInfo.id, index);
                                if (p != null && p.Value.freqtablefilename != "all")
                                {
                                    string filename = DRIVE.Drive + @"TopazPOC\SolutionFrequencies\" + sid + "_" + index + ".txt";
                                    File.Copy(p.Value.freqtablefilename, filename, true);
                                    AlgoEqualEnergyParams p1 = new AlgoEqualEnergyParams();
                                    p1 = p.Value;
                                    p1.freqtablefilename = filename;
                                    p = p1;
                                }
                                SaveEqualEnergyAlgoParams(m_curSolution.id, p, index);                                 
                            }
                            break;
                        }
                        index++;
                    }
                    m_solutionId = new Tuple<int, string>(sid, m_curSolution.name);
                    label11.Text = m_curSolution.name;
                    label31.Text = string.Empty;
                    label34.Text = m_curSolution.dishName;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void ShowDishNameParmeters(string dishname, bool start)
        {

            try
            {
                string p = MySQLConnector.GetDishNameParametersName(dishname);
                switch (p)
                {
                    case "temperatures_params":
                        {
                            WaterTempStartStopEffForm f = new WaterTempStartStopEffForm(start, m_testId);
                            f.ShowDialog();
                        }
                    break;
                    case "threecups":
                    {
                        ThreeCupsTemperaturesForm f = new ThreeCupsTemperaturesForm(m_testId, start);
                        f.ShowDialog();
                    }
                    break;
                    case "fourcups":
                    {
                        FourCupsTemperaturesForm f = new FourCupsTemperaturesForm(m_testId, start);
                        f.ShowDialog();
                    }
                    break;
                    case "fivecups":
                    {
                        FiveCupsTemperaturesForm f = new FiveCupsTemperaturesForm(m_testId, start);
                        f.ShowDialog();
                    }
                    break;
                    case "eightcups":
                    {
                        EightCupsTemperaturesForm f = new EightCupsTemperaturesForm(m_testId, start);
                        f.ShowDialog();
                    }
                    break;
                    case "ninecups":
                    {
                        NineCupsTemperaturesForm f = new NineCupsTemperaturesForm(m_testId, start);
                        f.ShowDialog();
                    }
                    break;
                    case "mincedbeef3kg":
                    {
                        if (start == false)
                        {
                            DefrostingMincedBeef3kgForm f = new DefrostingMincedBeef3kgForm(m_curSolution.dishName, m_testId);
                            f.ShowDialog();
                        }
                    }
                    break;
                    case "cake butter":
                    {
                        WeightlossForm f = new WeightlossForm(m_testId, start);
                        f.ShowDialog();                
                    }
                    break;
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        void LoadEmptyCavity()
        {
            m_emptyCavityDR = new DR[POCSET.MAXFREQ];
            for (int i = 0; i < m_emptyCavityDR.Length; i++)
            {
                m_emptyCavityDR[i].value = 0;
            }

            string fileName = @"C:\Goji\Cavity\DR Empty cavity.csv";
            using (StreamReader  sr = new StreamReader(fileName))
            {
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        break;
                    
                    string[] sdata = line.Split(new Char[] { ',' });
                    int freq = (int)(double.Parse(sdata[1]) - 2400);
                    m_emptyCavityDR[freq].freq = float.Parse(sdata[1]);
                    m_emptyCavityDR[freq].value = float.Parse(sdata[2]);

                }
            }
        }

        private void openEnergyGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_equalEnergyForm != null)
            {
                m_equalEnergyForm.Show();
                m_equalEnergyForm.WindowState = FormWindowState.Normal;
            }
            else
            {
                if (m_equalEnergyForm == null)
                {
                    m_equalEnergyForm = new EqualEnergyGraphForm();
                    m_equalEnergyForm.Show();
                    m_equalEnergyForm.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (m_startDR == false)
            {

               
                /*
                CallbackMessage p = new CallbackMessage(CallbackMessages);
                DRValueCallbackMsg p1 = new DRValueCallbackMsg(DRValuesCallback);
                al = new DROnlyAlgo(p, p1);
                al.Time = new TimeSpan(0, 0, 0);
                m_heat.AddAlgo(al);
                m_heat.Start(0, true, null);
                m_startDR = true;
                linkLabel1.LinkColor = Color.LightGreen;
                 */
            }
            else
            {
                m_heat.Stop();               
                linkLabel1.LinkColor = Color.Red;
            }
        }

        private void dishesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DishesForm f = new DishesForm();
            f.ShowDialog();
        }        
    }                        
}
