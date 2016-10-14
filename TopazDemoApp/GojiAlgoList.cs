using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopazApi;

namespace TopazDemoApp
{
    public abstract class GojiAlgoList
    {
        protected POCHeating m_heat;
        public abstract void BuildAlgo();

        public GojiAlgoList(POCHeating heat)
        {
            m_heat = heat;
        }
        
    }
}
