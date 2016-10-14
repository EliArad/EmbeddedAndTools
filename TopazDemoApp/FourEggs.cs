using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopazApi;

namespace TopazDemoApp
{
    public class FourEggs : GojiAlgoList
    {
        public FourEggs(POCHeating heat) : base(heat)
        {
           
        }
        public override void BuildAlgo()
        {

            List<DRRange> l = null;
          
            bool[] freqs = null;
                                
            EqualEnergyParams eqparams = new EqualEnergyParams();

            eqparams.mode = (ushort)DRMODES.HIGHER_THEN_DR;
            eqparams.highvalue = (ushort)(0.8f * 10);
            eqparams.lowvalue = 0 * 10;
            eqparams.maxpower = (ushort)(53.5f * 10);
            eqparams.acckj = 0;
            eqparams.targetkj = 55 * 10;
            eqparams.singlerepetition = 1;
            eqparams.toppercentage = 0;
            eqparams.agc = 1;
            eqparams.TotalSeconds = (ushort)(new TimeSpan(0, 2, 20).TotalSeconds);


            m_heat.Clear();

            m_heat.AddAlgo(ALGO_TYPES.EQUAL_ENERGY,
                            eqparams,
                            l,
                            null,
                            10, 
                            47 * 10);
            
            eqparams.mode = (ushort)DRMODES.BETWEEN_DR_RANGE;
            eqparams.highvalue = (ushort)(0.6f * 10);
            eqparams.lowvalue = 0 * 10;
            eqparams.maxpower = (ushort)(53.5f * 10);
            eqparams.acckj = 0;
            eqparams.targetkj = 55 * 10;
            eqparams.singlerepetition = 1;
            eqparams.toppercentage = 0;
            eqparams.agc = 1;
            eqparams.TotalSeconds = (ushort)(new TimeSpan(0, 2, 20).TotalSeconds);

            m_heat.AddAlgo(ALGO_TYPES.EQUAL_ENERGY,
                            eqparams,
                            l,
                            null,
                            10, 
                            47 * 10);


            m_heat.SendDataToController();

        }
    }
}
