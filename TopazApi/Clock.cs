using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TopazApi
{
    public class Clock
    {
        Thread m_clockThread;
        bool clockRunning = false;
        AutoResetEvent m_sleep = new AutoResetEvent(false);
        public delegate void ClockEvent(TimeSpan time, string strtime);
        ClockEvent m_clockEvent;
        DateTime m_startTime;
        TimeSpan m_pauseTime = new TimeSpan(0,0,0);
        DateTime m_pauseStart;
        ManualResetEvent m_pauseClockEvent = new ManualResetEvent(true);
        public Clock(ClockEvent ce)
        {
            m_clockEvent = ce;
        }
        public void Start()
        {
            m_sleep.Reset();
            m_pauseTime = new TimeSpan(0, 0, 0);
            if (m_clockThread == null || m_clockThread.IsAlive == false)
            {
                m_clockThread = new Thread(ClockThread);
                m_clockThread.Start();
            }
        }
        public void Stop()
        {
            clockRunning = false;
            m_sleep.Set();
            if (m_clockThread != null)
                m_clockThread.Join();
        }
        public void Pause(bool pause)
        {
            if (pause == true)
            {
                m_pauseClockEvent.Reset();
                m_pauseStart = DateTime.Now;
            }
            else
            {
                m_pauseClockEvent.Set();
                m_pauseTime += (DateTime.Now - m_pauseStart);
            }
        }
        void ClockThread()
        {
            m_startTime = DateTime.Now;
            m_clockEvent(new TimeSpan(0, 0, 0), "0:0:0");
            TimeSpan t;
            string str;
            clockRunning = true;
            while (clockRunning)
            {
                m_sleep.WaitOne(1000);
                t = DateTime.Now - m_startTime - m_pauseTime;
                str = string.Format("{0}:{1},{2}" , t.Hours, t.Minutes, t.Seconds);
                m_clockEvent(t, str);
                m_pauseClockEvent.WaitOne();
            }
        }
    }
}
