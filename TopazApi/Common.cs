using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopazApi
{
     
    public struct EnergyData
    {
        public float freq;
        public double value;
    }
    public struct PowerInfo
    {
        public bool valid;
        public double freq;
        public double forward;
        public double reflected;
        public double mag_level;
        public long   timestamp;
        public double watts;
        public double fwwatts;
        public double rwwatts;
    }
     
    public class POCSET
    {

        const int maxFreq = 101;

        public static int MAXFREQ
        {
            get
            {
                return maxFreq;
            }
        }
    }

}
