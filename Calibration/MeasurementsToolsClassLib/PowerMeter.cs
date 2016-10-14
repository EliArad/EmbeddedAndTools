using System;
using System.Collections.Generic;
using System.Text;
using Ivi.PwrMeter.Interop;
using Ivi.Driver.Interop;


namespace MeasurementsToolsClassLib
{
    public abstract class PowerMeter
    {
        public PowerMeter()
        {
            m_initialize = false;
        }
        public abstract double Read(int time_out);
        public abstract double Read(int Channel, int numberOfReads);
        public abstract void   Close();
        public abstract bool   Initialize(string ResourceName);
        public abstract bool   IsInitialize();
        public abstract bool   setExcpectedFrequency(double freq);
        protected bool m_initialize; 
    }
}
