using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopazApp.MYSQL
{

    public struct Column<T>
    {
        public string name;
        public T value;
    }

    public struct EfficiencyCompareQueryResult
    {
        public Column<object> testid;
        public Column<object> compareto;
        public Column<object> testname;
        public Column<object> dish_name;
        public Column<object> totalkj;
        public Column<object> avgfordb;
        public Column<object> avgrefdb;
        public Column<object> totalwatts;
        public Column<object> algoname;
        public Column<object> maxpower;
        public Column<object> time;
        public Column<object> mode;
        public Column<object> timediff;
        public Column<object> tempdiff;
    }

    public static partial class  Queris
    {

        static string myConnectionString;
        static Object m_lock = new Object();

        //public static string MySqlConnectionString = "server=localhost;database=topazdb;uid=root;pwd=;";
        public static string MySqlConnectionString = "server=192.168.10.64;database=topazdb;uid=root;pwd=1234;";

        static Queris()
        {

        }
        public static void Initialize(string serverIp, string userName, string password)
        {
            myConnectionString = string.Format("server={0};database=topazdb;uid={1};pwd={2};", serverIp, userName, password);
        }

        public static List<EfficiencyCompareQueryResult> RunCompareEfficiencyQuery(string query)
        {

            lock (m_lock)
            {
                string paramName = string.Empty;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                      
                        cn.Open();
                        List<EfficiencyCompareQueryResult> l = new List<EfficiencyCompareQueryResult>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            while (dataReader.Read())
                            {
                                EfficiencyCompareQueryResult q = new EfficiencyCompareQueryResult();
                                q.testid.value = int.Parse(dataReader["testid"].ToString());
                                q.testid.name = "testid";

                                int x;
                                int.TryParse(dataReader["compareto"].ToString(), out x);
                                q.compareto.value = x;
                                q.compareto.name = "compareto";

                                q.testname.value = dataReader["testname"].ToString();
                                q.testname.name = "testname";

                                q.dish_name.value = dataReader["dish_name"].ToString();
                                q.dish_name.name = "dish_name";

                                float f;
                                float.TryParse(dataReader["totalkj"].ToString(), out f);
                                q.totalkj.value = f;
                                q.totalkj.name = "totalkj";

                                float.TryParse(dataReader["avgfordb"].ToString(), out f);
                                q.avgfordb.value = f;
                                q.avgfordb.name = "avgfordb";

                                float.TryParse(dataReader["avgrefdb"].ToString(), out f);
                                q.avgrefdb.value = f;
                                q.avgrefdb.name = "avgrefdb";


                                float.TryParse(dataReader["totalwatts"].ToString(), out f);
                                q.totalwatts.value = f;
                                q.totalwatts.name = "totalwatts";

                                q.algoname.value = dataReader["algoname"].ToString();
                                q.algoname.name = "algoname";


                                q.maxpower.value = float.Parse(dataReader["maxpower"].ToString());
                                q.maxpower.name = "maxpower";

                                q.time.value = TimeSpan.Parse(dataReader["time"].ToString());
                                q.time.name = "time";
                                
                                int.TryParse(dataReader["mode"].ToString(), out x);
                                q.mode.value = x;
                                q.mode.name = "mode";

                                q.timediff.value = TimeSpan.Parse(dataReader["timediff"].ToString());
                                q.timediff.name = "timediff";

                                q.tempdiff.value = float.Parse(dataReader["tempdiff"].ToString());
                                q.tempdiff.name = "tempdiff";

                                l.Add(q);
                            }
                        }
                        cn.Close();
                        return l;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
    }
}
