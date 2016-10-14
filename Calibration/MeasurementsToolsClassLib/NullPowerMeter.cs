using System;
using System.Collections.Generic;
using System.Text;

namespace MeasurementsToolsClassLib
{
    public class NullPowerMeter : PowerMeter
    {

        public override bool IsInitialize()
        {
            return true;
        }
        public override bool Initialize(string ResourceName)
        {
            return true;
        }
        public override bool setExcpectedFrequency(double freq)
        {
            return true;
        }
        public bool Initiate(int time_in_mili)
        {
            return true;
        }
        public override void Close()
        {

        }
        public override double Read(int time_out)
        {
            return 50;
        }
        public override double Read(int Channel, int time_out)
        {
           return 50;
        }
    }
}
