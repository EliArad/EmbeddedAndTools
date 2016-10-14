using CouplerLib;
using MeasurementsToolsClassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopazCommonDefs;
using TopazControllerLib;

namespace TopazCalibrationApi
{
   
    public class TopazCalibration
    {
        Thread m_connectionThread = null;
        protected DiolanController m_con = new DiolanController();
        bool m_tryConnect = true;
        protected bool m_running = false;
        protected bool m_connected = false;
        protected NRP_Z211PowerMeter[] m_powerMeter = { null, null };
        string m_powerMeterForward , m_powerMeterReflected;
        public delegate void MessageCallback(int code, string msg);

        protected CouplerReader m_coupler;
        protected COUPLER_SERIAL m_serial = COUPLER_SERIAL.NONE;
        protected MessageCallback pCallback;
        public TopazCalibration(MessageCallback p)
        {
            pCallback = p;
        }
        public void SetCoupler(COUPLER_SERIAL c)
        {
            m_serial = c;
        }
        public void Initialize(string powerMeterForward , 
                               string powerMeterReflected, 
                               int timeout = 5)
        {


            try
            {
                if (m_serial != COUPLER_SERIAL.NONE)
                {
                    m_coupler = new CouplerReader(m_serial);
                }
                else
                {
                    throw (new SystemException("Please select coupler serial first"));
                }
            }
            catch (Exception err)
            {
                throw (new SystemException("Coupler error: " + err.Message));
            }

            try
            {
                m_powerMeterForward = powerMeterForward;
                m_powerMeterReflected = powerMeterReflected;
                int count = 0;
                if (m_powerMeter[0] == null)
                {
                    if (powerMeterForward.ToLower() != "none")
                    {
                        bool b = AllocatePowerMeterResources(0, powerMeterForward);
                        if (b == true)
                            count++;
                        else
                        {
                            throw (new SystemException("Failed to allocate reflected power meter"));
                        }
                    }
                }
                else
                {
                    count++;
                }

                if (m_powerMeter[1] == null)
                {
                    if (powerMeterReflected.ToLower() != "none")
                    {
                        bool b = AllocatePowerMeterResources(1, powerMeterReflected);
                        if (b == true)
                            count++;
                        else
                        {
                            throw (new SystemException("Failed to allocate reflected power meter"));
                        }
                    }
                }
                else
                {
                    count++;
                }
                if (count == 0)
                {
                    throw (new SystemException("Allocation of power meter failed"));
                }
            }
            catch (Exception err)
            {
                throw (new SystemException("Allocation of power meter failed "  + err.Message));
            }
            if (m_connected == false)
            {
                m_connectionThread = new Thread(Connection);
                m_connectionThread.Start();
                int to = timeout;
                while (m_connected == false)
                {
                    Thread.Sleep(1000);
                    to--;
                    if (to == 0)
                        throw (new SystemException("Not connected to controller I2C master"));
                }
            }
        }
        public bool Connect()
        {
            if (m_connected == false)
            {
                m_connectionThread = new Thread(Connection);
                m_connectionThread.Start();
                int to = 5;
                while (m_connected == false)
                {
                    Thread.Sleep(1000);
                    to--;
                    if (to == 0)
                        throw (new SystemException("Not connected to controller I2C master"));
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        public void BurnABFile(string fileName, DETECTOR det)
        {
            
            m_con.BurnABFile(fileName, det);
        }
        public void Close()
        {
            if (m_running == true)
            {
                throw (new SystemException("Still running"));
            }
            for (int i = 0 ; i < 2; i++)
            {
                if (m_powerMeter[i] != null)
                {
                    m_powerMeter[i].Close();
                    m_powerMeter[i] = null;
                }
            }
            try
            {
                m_con.Close();
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        bool AllocatePowerMeterResources(int index, string pname)
        {
            lock (this)
            {
                string str = string.Empty;

                if (m_powerMeter[index] != null)
                    m_powerMeter[index].Close();
                m_powerMeter[index] = null;
                if (pname.ToLower() != "none")
                {
                    m_powerMeter[index] = new NRP_Z211PowerMeter();
                    NRP_Z211PowerMeter m_nrp = (NRP_Z211PowerMeter)m_powerMeter[index];
                    m_nrp.Mode = NRP_Z211_Modes.NONBURSTED;
                    return m_powerMeter[index].Initialize(pname);
                }
            }
            return false;
        }
        void Connection()
        {
            while (m_tryConnect == true)
            {
                try
                {
                    m_con.connect(string.Empty, 115200, true);
                    m_connected = true;
                    break;
                }
                catch (Exception err)
                {
                    Thread.Sleep(1000);
                }
            }
        }      
    }
}
