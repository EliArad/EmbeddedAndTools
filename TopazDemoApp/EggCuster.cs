using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopazApi;

namespace TopazDemoApp
{
    public class EggCuster : GojiAlgoList
    {
        public EggCuster(POCHeating heat)
            : base(heat)
        {
           
        }
        public override void BuildAlgo()
        {

            TopPercentageParams tqparams = new TopPercentageParams();
            tqparams.powertime = 30000 / 25;
            tqparams.agc = 1;
            tqparams.maxpower = 53 * 10;
            tqparams.toppercent = 25;
            tqparams.targetkj = 0;
            tqparams.absorbed = 0;
            tqparams.TotalSeconds = (ushort)(new TimeSpan(0, 2, 20).TotalSeconds);            
                         
            m_heat.Clear();

            m_heat.AddAlgo(ALGO_TYPES.TOP_PERCENTAGE,
                           tqparams,
                           null,
                           10, 
                           47 * 10);
            
             
            m_heat.SendDataToController();

        }
    }
}
