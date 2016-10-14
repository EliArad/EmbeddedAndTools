using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopazApi;

namespace TopazDemoApp
{
    public class HeatWater : GojiAlgoList
    {
        public HeatWater(POCHeating heat)
            : base(heat)
        {
           
        }
        public override void BuildAlgo()
        {

            List<DRRange> l = null;
            
            bool[] freqs = null;

            DRThresholdParams dqparams = new DRThresholdParams();

            dqparams.mode = (ushort)DRMODES.HIGHER_THEN_DR;
            dqparams.highvalue = (ushort)(0.8f * 10);
            dqparams.lowvalue = 0 * 10;
            dqparams.maxpower = (ushort)(53.5f * 10);
            dqparams.targetkj = 55 * 10;
            dqparams.powertime = 100;
            dqparams.equaldrtime = 0;
            dqparams.agc = 1;
            dqparams.TotalSeconds = (ushort)(new TimeSpan(0, 2, 20).TotalSeconds);
            
             
            m_heat.Clear();

            m_heat.AddAlgo(ALGO_TYPES.DR_THREASHOLDS,
                            dqparams,
                            l,
                            null,
                            10, 
                            47 * 10);
            
             
            m_heat.SendDataToController();

        }
    }
}
