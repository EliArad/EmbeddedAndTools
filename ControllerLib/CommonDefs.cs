using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopazCommonDefs
{
    public enum DETECTOR
    {
        FORWARD,
        REFLECTED
    }

    public struct DR
    {
        public ushort valid;
        public float value;
        public float freq;
    };
}
