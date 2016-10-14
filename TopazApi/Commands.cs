using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopazApiDefsCommon;
using TopazControllerLib;

namespace TopazApi
{
    public class Commands
    {
        ITopazControllerApi m_con;
        public Commands(ITopazControllerApi con)
        {
            m_con = con;
        }
        public void WriteTopazHwInfo(TopazHwInfo info)
        {
            m_con.WriteTopazHwInfo(info);
        }
        public TopazHwInfo ReadTopazHwInfo()
        {
            return m_con.ReadTopazHwInfo();
        }
        public void setFrequency(double frequency)
        {
            m_con.setFrequency(frequency);
        }
    }
}
