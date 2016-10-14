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
using ZedChartLib;

namespace TopazApp.Forms.DebugForms
{
    public partial class EqualEnergyGraphForm : Form
    {
        bool allowclose = false;
        ZedChart m_zedDr;
        Thread m_thread;
        bool m_running = true;
        AutoResetEvent m_event = new AutoResetEvent(false);
        Queue<EnergyData[]> m_queue = new Queue<EnergyData[]>();
        public EqualEnergyGraphForm()
        {
            InitializeComponent();
            m_zedDr = new ZedChart(zg3.GraphPane, "Energy,", "Frequencies", "dB", "DR", 2, Color.Blue);
            m_thread = new Thread(ShowData);
            m_thread.Start();
        }
        void ShowData()
        {
            while (m_running)
            {
                if (m_queue.Count == 0)
                    m_event.WaitOne();
                if (m_running == false)
                    return;
                if (m_queue.Count == 0)
                    continue;
                EnergyData [] values = m_queue.Dequeue();
                m_zedDr.CreateGraph_EnergyGradientByZBars(zg3, values, POCSET.MAXFREQ, true);
                RefresheZedGraphs(zg3);
            }
        }
        public void SetData(EnergyData[] values)
        {
            m_queue.Enqueue(values);
            m_event.Set();

        }
        public void CloseThread()
        {
            allowclose = true;
            m_running = false;
            m_event.Set();
            if (m_thread != null)
                m_thread.Join();
            this.Close();
        }
        private void RefresheZedGraphs(ZedGraph.ZedGraphControl zg)
        {
            zg.AxisChange();
            zg.Invalidate();
            //zg.Refresh();
        }

        private void EqualEnergyGraphForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            e.Cancel = true;
        }
    }
}
