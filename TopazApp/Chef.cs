using Manina.Windows.Forms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApi;
using TopazApp.Forms;

namespace TopazApp
{
    public partial class Chef : Form
    {
        string m_testStrStartDate;        
        string m_demoListName;
        object m_sstime = new object();
        struct APPSettings
        {
            public string signalVisaName;
            public bool useTime;
            public double drPower;
        }
        Thread m_totalCookginThread;
        AutoResetEvent m_totalTimeEvent = new AutoResetEvent(false);
        bool m_appRunning = true;

        ChefDemoDishes m_curTest;

        struct MsgQueue
        {
            public int code;
            public string msg;
            public int channel;
        }
        ConcurrentQueue<MsgQueue> m_queue = new ConcurrentQueue<MsgQueue>();
        bool m_paused = false;
        TimeSpan m_algoStartTime = new TimeSpan(0, 0, 0);
        double m_totalKj = 0;
        double m_totalWatts = 0;
        double m_grandTotalWatts = 0;
        double m_garndFordBm = 0;
        int m_grandPowerCount = 0;
        double m_garndRefdBm = 0;
        bool m_running = false;
        POCHeating m_heat;

        TimeSpan m_zeroTime = new TimeSpan(0, 0, 0);
        APPSettings m_appSet = new APPSettings();

        TimeSpan m_tsOneSecond = new TimeSpan(0, 0, 1);
        AutoResetEvent m_msgEvent = new AutoResetEvent(false);
        Clock m_clock;
        Thread m_msgThread;
        ulong m_totalCookingTimeInSeconds;
        List<ChefDemoDishes> m_fastChefList;
        int m_userId;
        OvenInfo m_ovenInfo;
        public Chef(int userid, string userName, OvenInfo ovenInfo)
        {
            InitializeComponent();
            m_ovenInfo = ovenInfo;
            m_userId = userid;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            label2.Text = string.Empty;
            try
            {
                LoadSettings();


                m_heat = new POCHeating();


                imageListView1.ItemClick += new ItemClickEventHandler(ItemClickEvent);
                imageListView1.ItemDoubleClick += new ItemDoubleClickEventHandler(ItemDoubleClickEvent);

                Control.CheckForIllegalCrossThreadCalls = false;
                Clock.ClockEvent ce = new Clock.ClockEvent(ClockEvents);
                m_clock = new Clock(ce);

                CallbackMessage p = new CallbackMessage(CallbackMessages);
                m_heat.SetCallback(p);

                m_demoListName = Properties.Settings.Default.ChefDemoGroupName;
                if (m_demoListName != string.Empty)
                {
                    m_fastChefList = MySQLConnector.GetChefDemoList(m_demoListName);
                    if (m_fastChefList.Count == 0)
                    {
                        MessageBox.Show("No solution in chef list" + m_demoListName);
                        btnStart.Enabled = false;
                        btnStop.Enabled = false;
                    }
                    else
                    {
                        LoadChefDemos();
                    }
                }

                m_msgThread = new Thread(MsgThread);
                m_msgThread.Start();

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void ClockEvents(TimeSpan time, string strtime)
        {
            ssMin.Value = time.Minutes.ToString();
            ssSec.Value = time.Seconds.ToString();

            m_totalCookingTimeInSeconds = (ulong)time.TotalSeconds;
            /*
            try
            {
                lock (m_sstime)
                {
                    if (m_algoStartTime.TotalSeconds > 0)
                    {
                        m_algoStartTime = m_algoStartTime.Subtract(m_tsOneSecond);
                        if (m_algoStartTime >= m_zeroTime)
                        {
                            sevenSegmentArray2.Value = m_algoStartTime.Minutes.ToString();
                            sevenSegmentArray1.Value = m_algoStartTime.Seconds.ToString();
                        }
                    }
                }
            }
            catch (Exception err)
            {

            }
             */
        }

        void ItemDoubleClickEvent(object sender, ItemClickEventArgs e)
        {

        }
        void ItemClickEvent(object sender, ItemClickEventArgs e)
        {
            TestSelected(e.Item.Index, e.Item.FileName);
        }
        void TestSelected(int index, string fileName)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.ImageLocation = fileName;

            m_curTest = m_fastChefList[index];
            label2.Text = m_curTest.description;
            
        }
        void LoadSettings()
        {
            m_appSet.useTime = Properties.Settings.Default.UseTime;
        }
        void LoadChefDemos()
        {
            string[] list = new string[m_fastChefList.Count];

            string pictures_path = DRIVE.Drive + @"TopazPOC\DishPictures\";

            int i = 0;
            foreach (ChefDemoDishes n in m_fastChefList)
            {
                list[i] = pictures_path + n.picture1;
                i++;
            }

            imageListView1.Items.AddRange(list);
            imageListView1.View = Manina.Windows.Forms.View.Thumbnails;
            imageListView1.ThumbnailSize = new Size(200, 200);
            imageListView1.Width = 250;

            TestSelected(0, pictures_path + m_fastChefList[0].picture1);

        }
        bool BuildHeatList()
        {
            try
            {
                int index = 0;
                m_heat.Clear();


                TestInfo testInfo = MySQLConnector.GetTestInfo(m_curTest.testid);

                m_heat.Clear();


                List<AlgoThresholParams> algoThsParamsList = MySQLConnector.GetTestAlgoThreaholdParams(m_curTest.testid);
                List<AlgoTopPercentageParams> AlgoTopPercentageParamsList = MySQLConnector.GetTestAlgoTopPercentageParams(m_curTest.testid);
                List<AlgoEqualEnergyParams> algoEEParamsList = MySQLConnector.GetTestAlgoEqualEnergyParams(m_curTest.testid);
                
                List<SolutionAlgo> salgo = MySQLConnector.GetTestSolutionAlgorithems(m_curTest.testid);

                foreach (SolutionAlgo testAlgo in salgo)
                {

                    CallbackMessage p = new CallbackMessage(CallbackMessages);
                    DRValueCallbackMsg p1 = null;
                    m_heat.SetDRValuesCallback(p1);
                    TimeSpan t = testAlgo.time;

                    switch (testAlgo.algoname)
                    {
                        case "equal energy":
                            {
                                List<DRRange> l = null;
                                bool found = false;
                                bool AGC = true;
                                bool[] freqs = null;
                                EqualEnergyParams eqparams = new EqualEnergyParams();

                                foreach (AlgoEqualEnergyParams d in algoEEParamsList)
                                {
                                    if (d.RowIndex == index)
                                    {
                                        
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

                                        freqs = GetTableFrequencies("equal energy", m_curTest.testid, d.RowIndex);

                                        l = GetDRRangeForEqualEnergy("equal energy", m_curTest.testid, d.RowIndex);
                                        AGC = d.agc;
                                         
                                        break;
                                    }
                                }
                                if (found == false)
                                {
                                    MessageBox.Show("Did not found parameters for equal energy at line: " + index);
                                    return false;
                                }
                                m_heat.AddAlgo(ALGO_TYPES.EQUAL_ENERGY,
                                                eqparams,
                                                l,
                                                freqs,
                                                (ushort)testInfo.drCycleTime, 47 * 1000);
                            }
                        break;
                        case "RF Off":
                            {
                                m_heat.AddAlgo(ALGO_TYPES.HEAT_OFF,
                                               t);


                                
                            }
                        break;
                        case "DR Treshold":
                        {
                            bool AGC = true;
                            bool[] freqs = null;
                            List<DRRange> l = null;
                            DRThresholdParams drTsalgoParams = new DRThresholdParams();
                            bool found = false;
                            foreach (AlgoThresholParams d in algoThsParamsList)
                            {
                                if (d.RowIndex == index)
                                {                                   
                                    found = true;
                                    drTsalgoParams.mode =  (ushort)d.mode;
                                    drTsalgoParams.highvalue =  (ushort)d.highvalue;
                                    drTsalgoParams.lowvalue = (ushort)d.lowvalue;
                                    drTsalgoParams.powertime = (ushort)d.power_time_mili;
                                    drTsalgoParams.maxpower = (ushort)testAlgo.maxpower;
                                    drTsalgoParams.targetkj = (ushort)testAlgo.kj;
                                    drTsalgoParams.absorbed = (ushort)testAlgo.absorbed;
                                    drTsalgoParams.equaldrtime = (ushort)(d.equaldrtime == true ? 1 : 0);
                                    freqs = GetTableFrequencies("DR Treshold", m_curTest.testid, d.RowIndex);
                                    drTsalgoParams.agc = (ushort)(d.agc == true? 1 : 0);
                                    l = GetDRRangeForEqualEnergy("equal energy", m_curTest.testid, d.RowIndex);
                                    break;
                                }                                    
                            }
                            if (found == false)
                            {
                                MessageBox.Show("Did not found parameters for DR Treshold at line: " + index);
                                return false;
                            }                          
                            m_heat.AddAlgo(ALGO_TYPES.DR_THREASHOLDS, 
                                            drTsalgoParams, 
                                            l, 
                                            freqs,
                                            (ushort)testInfo.drCycleTime, 47 * 1000);
                        }
                        break;
                        case "Top Percentage":
                        {
                            List<DRRange> l = null;
                            bool found = false;
                            bool[] freqs = null;
                            bool AGC = true;
                            TopPercentageParams toppParams = new TopPercentageParams();
                            foreach (AlgoTopPercentageParams d in AlgoTopPercentageParamsList)
                            {
                                if (d.RowIndex == index)
                                {
                                    
                                    found = true;                                    
                                    toppParams.powertime = (ushort)d.power_time_mili;
                                    toppParams.agc = 1;
                                    toppParams.maxpower = (ushort)testAlgo.maxpower;
                                    toppParams.toppercent = (ushort)d.toppercent;
                                    toppParams.targetkj = (ushort)testAlgo.kj;
                                    toppParams.absorbed = (ushort)testAlgo.absorbed;

                                    freqs = GetTableFrequencies("Top Percentage", m_curTest.testid, d.RowIndex);
                                    AGC = d.agc;
                                    break;
                                }
                            }
                            if (found == false)
                            {
                                MessageBox.Show("Did not found parameters for Top Percentage at line: " + index);
                                return false;
                            }
                            m_heat.AddAlgo(ALGO_TYPES.TOP_PERCENTAGE,
                                           toppParams,
                                           freqs,
                                           (ushort)testInfo.drCycleTime, 47 * 1000);
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

        List<DRRange> GetDRRangeForEqualEnergy(string algoName, int testid, int rowIndex)
        {
            string path = DRIVE.Drive + @"TopazPOC\TestDRRange\";
            string fileName = path + testid + "_" + rowIndex + ".txt";

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

        void CallbackMessages(int code, string msg)
        {
            if (code == 0)
            {
                Console.WriteLine("0");
            }
            MsgQueue q = new MsgQueue();
            q.code = code;
            q.msg = msg;
            m_queue.Enqueue(q);
            m_msgEvent.Set();
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
        void Start()
        {
            lock (this)
            {
                if (m_running == true)
                {
                    return;
                }

               

                btnStart.Enabled = false;
                   

                try
                {

                     
                    if (BuildHeatList() == false)
                    {
                        btnStart.Enabled = true;
                        return;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    btnStart.Enabled = true;
                    return;
                }
                try
                {
                    m_totalKj = 0;
                    m_totalWatts = 0;
                    m_grandTotalWatts = 0;
                    m_garndFordBm = 0;
                    m_grandPowerCount = 0;
                    m_garndRefdBm = 0;
                    m_algoStartTime = new TimeSpan(0, 0, 0);
                    m_paused = false;
                    btnPause.Font = new Font(btnPause.Font, m_paused == true ? FontStyle.Bold : FontStyle.Regular);
                    bool onetime = false;
                    if (m_curTest.totaltime.Minutes == 0 && m_curTest.totaltime.Seconds == 0)
                        onetime = true;

                    TimeSpan? t = m_curTest.totaltime;
                    m_heat.Start(onetime, t);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    btnStart.Enabled = true;
                }
            }
        }
        bool[] GetTableFrequencies(string algoName, int testid, int rowIndex)
        {
            bool[] freqs = new bool[POCSET.MAXFREQ];
            string fileName = "all";
            switch (algoName)
            {

                case "DR Treshold":
                    {
                        AlgoThresholParams? d = MySQLConnector.GetTestAlgoThreaholdParams(testid, rowIndex);
                        if (d != null)
                        {
                            if (d.Value.freqtablefilename.ToLower() == "all")
                            {
                                for (int i = 0; i < freqs.Length; i++)
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
                        AlgoEqualEnergyParams? d = MySQLConnector.GetAlgoEqualEnergyParams(testid, rowIndex);
                        if (d != null)
                        {
                            if (d.Value.freqtablefilename.ToLower() == "all")
                            {
                                for (int i = 0; i < freqs.Length; i++)
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
                        AlgoTopPercentageParams? d = MySQLConnector.GetAlgoTopPercentageParams(testid, rowIndex);
                        if (d != null)
                        {
                            if (d.Value.freqtablefilename.ToLower() == "all")
                            {
                                for (int i = 0; i < freqs.Length; i++)
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
        void TotalCookingTimeThread(int totalMiniute, int totalSeconds)
        {

            int c = totalMiniute + totalSeconds;
            if (c == 0)
                return;
            else
            {
                m_totalTimeEvent.Reset();
                m_totalTimeEvent.WaitOne(new TimeSpan(0, totalMiniute, totalSeconds));
                Stop();
            }
        }
        void StartTotalThread(int minutes, int seconds)
        {
            m_totalTimeEvent.Reset();
            if (m_totalCookginThread == null || m_totalCookginThread.IsAlive == false)
            {
                m_totalCookginThread = new Thread(() => TotalCookingTimeThread(minutes, seconds));
                m_totalCookginThread.Start();

            }
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
                            m_clock.Stop();
                            m_totalTimeEvent.Set();
                            Stop();
                            MessageBox.Show(q.msg);
                            btnStop.Font = new Font(btnStop.Font, FontStyle.Bold);
                            btnStart.ForeColor = Color.Black;

                            m_running = false;
                            btnStart.Enabled = true;
                        }
                        break;
                    case 1: // STARTED
                        StartTotalThread(0, 0);
                        btnStop.Font = new Font(btnStop.Font, FontStyle.Regular);
                        m_clock.Start();
                        btnStart.ForeColor = Color.Green;

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

                        }
                        break;
                    case 6:
                        {

                        }
                        break;
                    case 203:
                        {

                        }
                        break;
                    case 204:
                        {

                        }
                        break;
                    case 214:
                        {

                        }
                        break;
                    case 60:

                        break;
                    case 61: // KJ
                        {

                        }
                        break;
                    case 69:
                        {
                            string[] sdata = q.msg.Split(new Char[] { ',' });
                            /*
                            if (m_curSolution.TotalKj > 0 && m_totalKj >= m_curSolution.TotalKj)
                            {
                                Thread task = new Thread(() => Stop());
                                task.Start();
                            }*/
                            m_totalWatts = double.Parse(sdata[0]);
                            //label7.Text = m_totalWatts.ToString("0.00");
                            m_grandTotalWatts += m_totalWatts;
                            //label22.Text = sdata[1];
                        }
                        break;
                    case 64:
                        {
                            string[] sdata = q.msg.Split(new Char[] { ',' });
                            //label3.Text = sdata[0];
                            //label4.Text = sdata[1];
                            m_garndFordBm += double.Parse(sdata[0]);
                            m_garndRefdBm += double.Parse(sdata[1]);
                            m_grandPowerCount++;
                        }
                        break;
                    case 68:
                        {
                            m_totalKj += double.Parse(q.msg);
                            sevenSegmentArray1.Value = m_totalKj.ToString("0.00");                           
                            /*
                            if (m_curSolution.TotalKj > 0 && m_totalKj >= m_curSolution.TotalKj)
                            {
                                Thread task = new Thread(() => Stop());
                                task.Start();
                            }
                            */
                        }
                        break;
                    case 70:

                        break;
                    case 72:
                        {

                        }
                        break;
                }
            }
        }

        void Stop()
        {
            lock (this)
            {
                try
                {
                    m_totalTimeEvent.Set();
                    if (m_paused == true)
                    {
                        m_paused = !m_paused;
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
        void StopFromCallback()
        {
            m_clock.Stop();
            m_totalTimeEvent.Set();
            if (m_running == true)
            {
                btnStop.Font = new Font(btnStop.Font, FontStyle.Bold);
                btnStart.ForeColor = Color.Black;
                btnStart.Enabled = true;
            }
            m_running = false;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                Start();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Chef_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_appRunning = false;
            m_msgEvent.Set();
            m_msgThread.Join();

            if (m_heat.Running == true)
            {
                MessageBox.Show("Still cooking");
                e.Cancel = true;
                return;
            }
        }

        private void Chef_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "F2")
            {
                BuildChefDemo f = new BuildChefDemo();
                f.ShowDialog();
            }
            else
            if (e.KeyCode.ToString() == "F4")
            {
                Properties.Settings.Default.mode = "poc";
                Properties.Settings.Default.Save();
                Close();
            }
            else
            if (e.KeyCode.ToString() == "F10")
            {
                Close();
            }
        }
     
        private void Chef_Load(object sender, EventArgs e)
        {
            
        }

        private void btnStop_Click(object sender, EventArgs e)
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

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (m_running == false)
                return;
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
            m_clock.Pause(m_paused);
        }
    }
}
