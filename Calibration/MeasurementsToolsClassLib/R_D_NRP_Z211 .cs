using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
// IVI instrument class and inherent interfaces
using Ivi.PwrMeter.Interop;
// IVI inherent interfaces
using Ivi.Driver.Interop;
using MeasurementsToolsClassLib;
using InstrumentDrivers;

namespace MeasurementsToolsClassLib
{
    public enum NRP_Z211_Modes
    {
        BURSTED,
        NONBURSTED
    }; 
   
    public class NRP_Z211PowerMeter : PowerMeter
    {
        public const int NUMBER_OF_MEASUREMENTS = 100;
        struct  NRP_Z211BurstedSettings
        {
            public bool IDChecked;
            public bool ResetDevice;
            public string ResourceDescriptor;
            public int SensorModeTimeslot;
            public double correctionFrequency;
            public int TriggerSourceImmediate;
            public double timeslotWidth;
            public double excludeStart;
            public double triggerLevel;
            public int averageCount;
            public bool errorCheckState;
            public double excludeStop;
        }

        struct NRP_Z211NonBurstedSettings
        {
            public string ResourceDescriptor;
            public bool IDChecked;
            public bool ResetDevice;
            // Deactivates automatic determination of filter bandwidth
            public bool AutoEnabled;
            // Number of averages
            public int averageCount;
            // Set corr frequency
            public double correctionFrequency;
            // Set trigger source to immediate
            public int TriggerSourceImmediate;
            public double integrationTime;
            // Smoothing off
            public bool AvSmoothingEnabled;
            public bool CheckState;
        }                       
        NRP_Z211_Modes m_nrpMode = NRP_Z211_Modes.NONBURSTED;
        public double[] measResults = new double[NUMBER_OF_MEASUREMENTS];
        NRP_Z211BurstedSettings     m_nrpBurstedSettings = new NRP_Z211BurstedSettings();
        NRP_Z211NonBurstedSettings  m_nrpNonBurstedSettings = new NRP_Z211NonBurstedSettings();
        int m_Channel = 1;
        rsnrpz m_sensor = null;
        public NRP_Z211PowerMeter()
        {
           try
           {
              m_nrpBurstedSettings.IDChecked = true;
              m_nrpBurstedSettings.ResetDevice = true;
              m_nrpBurstedSettings.ResourceDescriptor = "*";
              m_nrpBurstedSettings.SensorModeTimeslot = rsnrpzConstants.SensorModeTimeslot;
              m_nrpBurstedSettings.correctionFrequency = 50;
              m_nrpBurstedSettings.TriggerSourceImmediate = rsnrpzConstants.TriggerSourceImmediate;
              m_nrpBurstedSettings.timeslotWidth = 577;
              m_nrpBurstedSettings.excludeStart = 2;
              m_nrpBurstedSettings.triggerLevel = -30;
              m_nrpBurstedSettings.averageCount = 1;
              m_nrpBurstedSettings.errorCheckState = false;
              m_nrpBurstedSettings.excludeStop = 100000;

              m_nrpNonBurstedSettings.ResourceDescriptor = "*";
              m_nrpNonBurstedSettings.IDChecked = true;
              m_nrpNonBurstedSettings.ResetDevice = true;
              // Deactivates automatic determinatio
              m_nrpNonBurstedSettings.AutoEnabled = false;
              // Number of averages
              m_nrpNonBurstedSettings.averageCount = 1;
              // Set corr frequency
              m_nrpNonBurstedSettings.correctionFrequency = 50;
              // Set trigger source to immediate
              m_nrpNonBurstedSettings.TriggerSourceImmediate = rsnrpzConstants.TriggerSourceImmediate;
              m_nrpNonBurstedSettings.integrationTime = 0.1;
              // Averaging Manual 1, increase numbe
              // Smoothing off
              m_nrpNonBurstedSettings.AvSmoothingEnabled = false;
              m_nrpNonBurstedSettings.CheckState = false;
           }
           catch (Exception err)
           {
               throw (new SystemException(err.Message));
           }
        }

        public static void GetSensorInfo(int Channel, 
                                   out string SensorType,
                                   out string SensorName,
                                   out string SensorSerial)
        {

           try
           {
              rsnrpz sensor = new rsnrpz("*",
                                          true,
                                          true);

              StringBuilder _sensorType = new StringBuilder(100);
              StringBuilder _sensorName = new StringBuilder(100);
              StringBuilder _sensorSerial = new StringBuilder(100);
              sensor.GetSensorInfo(Channel, _sensorName, _sensorType, _sensorSerial);
              SensorType = _sensorType.ToString();
              SensorName = _sensorName.ToString();
              SensorSerial = _sensorSerial.ToString();
              sensor.Dispose();
           }
           catch (Exception err)
           {
               throw (new SystemException(err.Message));
           }
        }        

        public NRP_Z211_Modes Mode
        {
            set
            {
                m_nrpMode = value;
            }
        }
        public static int GetSensorCount()
        {
            try
            {
                rsnrpz sensor = new rsnrpz("*",
                                            true,
                                            true);

                int Sensorscount = 0;
                sensor.GetSensorCount(out Sensorscount);
                sensor.Dispose();
                return Sensorscount;
            }
            catch (Exception err)
            {
               if (err.Message != "Unknown Error Code (0xC0000002)")
               {
                  throw (new SystemException(err.Message));
               }
               return 0;
            }           
        }
        bool InitializeNrpBurstedMode(string ResourceName)
        {
            try
            {
                m_sensor = new rsnrpz(ResourceName,
                                      m_nrpBurstedSettings.IDChecked,
                                      m_nrpBurstedSettings.ResetDevice);

           
                double triggerLevel_W;
                //sensor = new rsnrpz(ResourceDescriptor.Text, IDQuery.Checked, ResetDevice.Checked);
                // Timeslot: The power is measured simultaneously in a number of timeslots
                m_sensor.chan_mode(m_Channel, rsnrpzConstants.SensorModeTimeslot);
                // Set corr frequency
                m_sensor.chan_setCorrectionFrequency(m_Channel, m_nrpBurstedSettings.correctionFrequency * 1000000.0);
                // Set trigger source to internal
                m_sensor.trigger_setSource(m_Channel, rsnrpzConstants.TriggerSourceImmediate);

                m_sensor.tslot_configureTimeSlot(m_Channel, 1, m_nrpBurstedSettings.timeslotWidth / 1000000.0);
                m_sensor.timing_configureExclude(m_Channel, m_nrpBurstedSettings.excludeStart / 1000000.0, m_nrpBurstedSettings.excludeStop / 1000000.0);

                // Set trigger level
                triggerLevel_W = Math.Pow(10, (m_nrpBurstedSettings.triggerLevel) / 10.0) / 1000.0;
                m_sensor.trigger_setLevel(m_Channel, triggerLevel_W);

                // Averaging Manual 1, increase number for better repeatability
                m_sensor.avg_configureAvgManual(m_Channel, m_nrpBurstedSettings.averageCount);

                m_sensor.errorCheckState(m_nrpBurstedSettings.errorCheckState);
                m_initialize = true;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
            return true; 
        }
        private bool InitializeNrpNonBurstedMode(string ResourceName)
        {
            try
            {
                m_sensor = new rsnrpz(ResourceName,
                                      m_nrpNonBurstedSettings.IDChecked,
                                      m_nrpNonBurstedSettings.ResetDevice);

                // Deactivates automatic determination of filter bandwidth
                m_sensor.avg_setAutoEnabled(m_Channel, m_nrpNonBurstedSettings.AutoEnabled);
                // Number of averages
                m_sensor.avg_setCount(m_Channel, m_nrpNonBurstedSettings.averageCount);
                // Set corr frequency
                m_sensor.chan_setCorrectionFrequency(m_Channel, m_nrpNonBurstedSettings.correctionFrequency * 1000000.0);
                // Set trigger source to immediate
                m_sensor.trigger_setSource(m_Channel, m_nrpNonBurstedSettings.TriggerSourceImmediate);
                m_sensor.chan_setContAvAperture(m_Channel, m_nrpNonBurstedSettings.integrationTime / 1000.0);
                // Averaging Manual 1, increase number for better repeatability
                m_sensor.avg_configureAvgManual(m_Channel, m_nrpNonBurstedSettings.averageCount);
                // Smoothing off
                m_sensor.chan_setContAvSmoothingEnabled(m_Channel, false);

                m_sensor.errorCheckState(m_nrpNonBurstedSettings.CheckState);
                m_initialize = true; 
            }
            catch (Exception err)
            {
               throw (new SystemException(err.Message));
            }
            return true;
        }
        public void SetContAvAperture(double apt)
        {
            m_sensor.chan_setContAvAperture(m_Channel, apt);
        }
        public override bool IsInitialize()
        {
            return m_initialize; 
        }
        public override bool Initialize(string ResourceName)
        {
           try
           {
              if (m_nrpMode == NRP_Z211_Modes.BURSTED)
              {
                 return InitializeNrpBurstedMode(ResourceName);
              }
              return InitializeNrpNonBurstedMode(ResourceName);
           }
           catch (Exception err)
           {
              throw (new SystemException(err.Message));
           }
        }
        public override bool setExcpectedFrequency(double freq)
        {
            return false;
        }

        public override void Close()
        {
            if (m_sensor != null)
                m_sensor.Dispose();
        }
        public bool Connected 
        {
            get
            {
                return false;
            }
        }
        public override double Read(int numberOfReads)
        {
            lock (this)
            {
                try
                {
                    if (m_nrpMode == NRP_Z211_Modes.BURSTED)
                    {
                        return ReadBursted(numberOfReads);
                    }
                    return ReadNonBursted(numberOfReads);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public override double Read(int Channel, int numberOfReads)
        {
            lock (this)
            {
                try
                {
                    if (m_nrpMode == NRP_Z211_Modes.BURSTED)
                    {
                        return ReadBursted(numberOfReads);
                    }
                    return ReadNonBursted(Channel, numberOfReads);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public void setContAvAperture(int Channel, double ContAv_Aperture)
        {
            double cav;
            m_sensor.chan_getContAvAperture(Channel, out   cav);
           m_sensor.chan_setContAvAperture(Channel, ContAv_Aperture);
        }
        private double ReadNonBursted(int numberOfReads)
        {
           try
           {
              double meas_value = 0;
              for (int i = 0; i < numberOfReads; i++)
              {
                 bool meas_complete;
                 m_sensor.chans_initiate();
                 System.DateTime tout = System.DateTime.Now.AddSeconds(2);
                 do
                 {
                    m_sensor.chan_isMeasurementComplete(1, out meas_complete);

                    //System.Threading.Thread.Sleep(0);
                 } while (meas_complete == false);  // while (!meas_complete && (System.DateTime.Now.CompareTo(tout) < 0));
                
                 if (meas_complete)
                 {
                    double result;

                    m_sensor.meass_fetchMeasurement(1, out result);
                    meas_value = 10 * Math.Log(Math.Abs(result)) / Math.Log(10) + 30.0;
                    measResults[i] = meas_value;
                 }
                 else
                 {
                    throw new System.Runtime.InteropServices.ExternalException("Measurement Timeout Occured", 0);
                 }                 
              }
              return meas_value;
           }
           catch (Exception err)
           {
              throw (new SystemException(err.Message));
           }
        }

        private double ReadNonBursted(int Channel, int numberOfReads)
        {
           try
           {
              double meas_value = 0;
              for (int i = 0; i < numberOfReads; i++)
              {
                 bool meas_complete;
                 m_sensor.chan_initiate(Channel);
                 System.DateTime tout = System.DateTime.Now.AddSeconds(2);
                 do
                 {
                    m_sensor.chan_isMeasurementComplete(1, out meas_complete);
                    //System.Threading.Thread.Sleep(0);
                 } while (!meas_complete && (System.DateTime.Now.CompareTo(tout) < 0));
                
                 if (meas_complete)
                 {
                    double result;

                    m_sensor.meass_fetchMeasurement(1, out result);
                    meas_value = 10 * Math.Log(Math.Abs(result)) / Math.Log(10) + 30.0;
                    measResults[i] = meas_value;
                 }
                 else
                 {
                    throw new System.Runtime.InteropServices.ExternalException("Measurement Timeout Occured", 0);
                 }                  
              }
              return meas_value;
           }
           catch (Exception err)
           {
              throw (new SystemException(err.Message));
           }
        }

        public int setInitContinuousEnabled(int Channel, bool Continuous_Initiate)
        {
           return m_sensor.chan_setInitContinuousEnabled(Channel, Continuous_Initiate);
        }
        private double ReadBursted(int numberOfReads)
        {
           try
           {
              double meas_value = 0;
              for (int i = 0; i < numberOfReads; i++)
              {
                 bool meas_complete;
                 m_sensor.chans_initiate();
                 System.DateTime tout = System.DateTime.Now.AddSeconds(2);
                 do
                 {
                    m_sensor.chan_isMeasurementComplete(1, out meas_complete);
                    System.Threading.Thread.Sleep(0);
                 } while (!meas_complete && (System.DateTime.Now.CompareTo(tout) < 0));

                 if (meas_complete)
                 {
                    double[] result = new double[100];
                    int count = 0;

                    m_sensor.meass_fetchBufferMeasurement(1, 100, result, out count);
                    if (count > 0)
                    {
                       meas_value = (10 * Math.Log(Math.Abs(result[0])) / Math.Log(10)) + 30.0;
                       measResults[i] = meas_value;
                    }
                    else
                    {
                       throw new System.Runtime.InteropServices.ExternalException("Measurement Error Occured", 0);
                    }
                 }
                 else
                 {
                    throw new System.Runtime.InteropServices.ExternalException("Measurement Timeout Occured", 0);
                 }
              }
              return meas_value;
           }
           catch (Exception err)
           {
              throw (new SystemException(err.Message));
           }
        }

    }
}
