using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TopazApi;
 
namespace TopazApp
{
    public struct QuerisInfo
    {
        public string queryname;
        public string description;
        public string datagrid_column;
        public string sqlquery;
        public int allowuser;
    }
    public struct MasterChefList
    {
        public int id;
        public int sid;
        public DISH_SOLUTION solution;
        public string groupname;
    }

    public class DISHInfo
    {
        public string dishName;
        public string picture1;
        public string picture2;
        public string description;
        public int dishScoreEnd;
        public DISHInfo()
        {

        }
        public DISHInfo(string name, string desc)
        {
            description = desc;
            dishName = name;
        }
    }
   
    public struct DISH_SOLUTION
    {
        public int id;
        public string name;
        public int createdBy;
        public string Dated;
        public string description;
        public int updatedBy;
        public string updated;
        public string utensilName;
        public string dishName;
        public string firstname;
        public string lastname;
        public TimeSpan totalTime;
        public float TotalKj;
        public float drpower;
        public string guid;
        public ushort drCycleTime;
        public string groupName;
        public int groupid;
    }

    public struct OvenInfo
    {
        public string guid;
        public DateTime added;
        public string description;
        public int userId;
        public string Alias;
        public string strCouplerSerial;
        public bool Circulator;
        public string cablefix;
        public User user;
    }

    public struct ChefDemoDishes
    {

        public string groupname;
        public int    testid;
        public string testName;
        public string picture1;
        public string picture2;
        public string finalDescription;
        public string dish_name;
        public string description;
        public TimeSpan totaltime;
    }

    public struct SolutionPictureInfo
    {
        public int sid;
        public string fullpicname;
        public bool show;
    }
    public class PictureInfo
    {
        public int testid;
        public string dbName;
        public string path;
        public string fullname;
        public string description;
        public string title;
        public string compip;
        public PictureInfo()
        {

        }
        public PictureInfo(string dbName, string fullname, string title, string description)
        {
            this.dbName = dbName;
            this.fullname = fullname;
            this.title = title;
            this.description = description;

        }
    }

    
    public struct AlgoThresholParams
    {
        public int mode;
        public float lowvalue;
        public float highvalue;
        public int RowIndex;
        public int sid;
        public int power_time_mili;
        public bool agc;
        public bool equaldrtime;
        public string freqtablefilename;
        public bool substractEmptyCavity;
    }

    public struct AlgoEqualEnergyParams
    {
        public int mode;
        public float lowvalue;
        public float highvalue;
        public int RowIndex;
        public int sid;
        public float acckj;
        public bool agc;
        public string freqtablefilename;
        public bool singlerepetition;
        public int toppercentage;
        public bool substractEmptyCavity;
    }

    public struct AlgoTopPercentageParams
    {
        public int RowIndex;
        public int sid;
        public int power_time_mili;
        public bool agc;
        public int toppercent;
        public bool equaldrtime;
        public string freqtablefilename;
        public bool substractEmptyCavity;
    }

    public struct FINAL_TEST_RESULT
    {
        public int testid;
        public string DRFileName;
        public string EnergyFileName;
        public string FinalDescription;
        public string PowerInfoFileName;
        public int PassFail;
        public float avgfordb;
        public float avgrefdb;
        public float totalwatts;
        public float totalkj;
    }

    public struct FinalResults
    {
        public int PassFail;
        public float totalkj;
        public float totalwatts;
        public float avgfordb;
        public float avgrefdb;
        public string FinalDescription;
    }
    public struct TestInfo
    {
        public int id;
        public int user_id;
        public DateTime StartDated;
        public DateTime StopDated;
        public string testname;
        public string email;
        public string remark;
        public string utensil_name;
        public string guid;
        public string firstName;
        public float drpower;
        public string lastName;
        public string description;
        public string dish_name;
        public ushort drCycleTime;
        public TimeSpan totalTime;
        public float totalKj;
        public string compare_reason;
        public int compareto;

        public FinalResults fres;
    }


    public struct TestInfo2
    {
        public int id;
        public int user_id;
        public DateTime StartDated;
        public DateTime StopDated;
        public string testname;
        public string email;
        public float totalwatts;
        public float totalkj;
        public string remark;
        public string utensil_name;
        public string firstName;
        public string lastName;
        public string dish_name;
        public string guid;
        public string compare_reason;
        public int compareto;
        
        public FinalResults fres;
    }
     

    public class SolutionAlgo
    { 
        public int id;
        public string algoname; 
        public TimeSpan time;
        public float maxpower;
        public int rowindex;
        public float kj;
        public float absorbed;

        public SolutionAlgo()
        {

        }   
        public SolutionAlgo(string algoname, TimeSpan time, float maxpower, int rowindex, float kj, float absorbed)
        {
            this.algoname = algoname;
            this.time = time;
            this.maxpower = maxpower;
            this.rowindex = rowindex;
            this.kj = kj;
            this.absorbed = absorbed;

        }
    }

    public static partial class MySQLConnector
    {

        static string myConnectionString;
        static Object m_lock = new Object();

        //public static string MySqlConnectionString = "server=localhost;database=topazdb;uid=root;pwd=;";
        public static string MySqlConnectionString = "server=192.168.10.64;database=topazdb;uid=root;pwd=1234;";

        static MySQLConnector()
        {

        }
        public static void Initialize(string serverIp , string userName, string password)
        {

            myConnectionString = string.Format("server={0};database=topazdb;uid={1};pwd={2};" , serverIp, userName, password);
        }

     
        public static void DeleteUser(int id)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = "DELETE from users  where id = " + id;
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void UpdateWhightLoss(int testId , float Weight, bool before)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        if (before == true)
                        {
                            query = @" INSERT INTO weightloss (testid,WeightStart, WeightEnd)
                                                              VALUES(@testid,@WeightStart, @WeightEnd)";
                        }
                        else
                        {
                            query = @"UPDATE weightloss SET WeightEnd = @Weight
                                                            WHERE testId = @testId";
                        }
                        cn.Open();
                        List<string> l = new List<string>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testId);
                            if (before == true)
                            {
                                cmd.Parameters.AddWithValue("@WeightStart", Weight);
                                cmd.Parameters.AddWithValue("@WeightEnd", 0);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@WeightEnd", Weight);
                            }                       
                            newId = cmd.ExecuteNonQuery();                            
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
             

        }

        public static void BuildChefList(List<Tuple<int, string>> list, string groupName)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {

                        string query1 = @"
                        SET SQL_SAFE_UPDATES = 0;
                        delete from masterchef where groupname = @groupname;
                        SET SQL_SAFE_UPDATES = 1";

                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query1, cn))
                        {
                             
                            cmd.Parameters.AddWithValue("@groupname", groupName);
                            cmd.ExecuteNonQuery();                            
                        }
                        cn.Close();

                        string query = @" INSERT INTO masterchef (testid, groupname, description)
                                          VALUES(@testid,@groupname, @description)";
                        cn.Open();
                   
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            foreach (Tuple<int, string> x in list)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@testid", x.Item1);
                                cmd.Parameters.AddWithValue("@description", x.Item2);
                                cmd.Parameters.AddWithValue("@groupname", groupName);
                                newId = cmd.ExecuteNonQuery();
                            }
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
             
        }

        public static int InsertDefrost_temp8(int testId, float [] temp , float frozen, float cooked, string whenmeasured)
        {
             
            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @" INSERT INTO defrost_temp8 (testid,temp1,temp2,temp3,temp4,temp5,temp6,temp7,frozen_part, cooked_part,whenmeasured)
                                                              VALUES(@testid,@temp1,@temp2,@temp3,@temp4,@temp5,@temp6,@temp7,@frozen_part, @cooked_part, @whenmeasured)";
                        cn.Open();
                        List<string> l = new List<string>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testId);
                            cmd.Parameters.AddWithValue("@temp1", temp[0]);
                            cmd.Parameters.AddWithValue("@temp2", temp[1]);
                            cmd.Parameters.AddWithValue("@temp3", temp[2]);
                            cmd.Parameters.AddWithValue("@temp4", temp[3]);
                            cmd.Parameters.AddWithValue("@temp5", temp[4]);
                            cmd.Parameters.AddWithValue("@temp6", temp[5]);
                            cmd.Parameters.AddWithValue("@temp7", temp[6]);
                            cmd.Parameters.AddWithValue("@frozen_part", frozen);
                            cmd.Parameters.AddWithValue("@cooked_part", cooked);
                            cmd.Parameters.AddWithValue("@whenmeasured", whenmeasured);
                            newId = cmd.ExecuteNonQuery();                            
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
             
        }

        public static void AddNewGroupNames(List<string> toadd)
        {

            lock (m_lock)
            {
                string paramName = string.Empty;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @" INSERT INTO test_group_names (groupname)
                                            VALUES(@names)";
                        cn.Open();
                        List<string> l = new List<string>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            foreach (string s in toadd)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@names",s);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static string GetGroupName(int id)
        {
            lock (m_lock)
            {
                string groupname = "none";
                 
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @" SELECT groupname FROM test_group_names where id = @id";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);

                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.Read())
                            {
                                groupname = dataReader["groupname"].ToString();
                            }
                        }
                        cn.Close();
                        return groupname;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int GetGroupId(string groupname)
        {
            lock (m_lock)
            {
                int id = -1;
                string paramName = string.Empty;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @" SELECT id FROM test_group_names where groupname = @groupname";
                        cn.Open();
                       
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@groupname", groupname);
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.Read())
                            {
                                id = int.Parse(dataReader["id"].ToString());
                            }
                        }
                        cn.Close();
                        return id;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static List<string> GetGroupNames()
        {

            lock (m_lock)
            {
                string paramName = string.Empty;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @" SELECT * FROM test_group_names";
                        cn.Open();
                        List<string> l = new List<string>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            while (dataReader.Read())
                            {
                                string name = dataReader["groupname"].ToString();
                                l.Add(name);
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
        public static TestInfo ? getLastTestInfo()
        {
             
            lock (m_lock)
            {
                string paramName = string.Empty;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @" SELECT * FROM topazdb.test
                                          order by StartDated desc
                                          limit 1 offset 1";
                        cn.Open();
                        List<MasterChefList> l = new List<MasterChefList>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.Read())
                            {
                                TestInfo d = new TestInfo();
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.user_id = int.Parse(dataReader["user_id"].ToString());
                                d.StartDated = DateTime.Parse(dataReader["StartDated"].ToString());
                                d.StopDated = DateTime.Parse(dataReader["StopDated"].ToString());
                                d.dish_name = dataReader["dish_name"].ToString();
                                d.testname = dataReader["testname"].ToString();
                                d.remark = dataReader["remark"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.utensil_name = dataReader["utensil_name"].ToString();
                                d.drpower = float.Parse(dataReader["drpower"].ToString());
                                d.totalTime = TimeSpan.Parse(dataReader["totaltime"].ToString());
                                d.totalKj = float.Parse(dataReader["totalkj"].ToString());
                                d.compare_reason = dataReader["compare_reason"].ToString();

                                bool b = int.TryParse(dataReader["compareto"].ToString(), out d.compareto);
                                if (b == false)
                                {
                                    d.compareto = -1;
                                }
                                
                                cn.Close();
                                return d;
                            }
                        }
                        cn.Close();
                        return null;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static List<QuerisInfo> GetAllQueris()
        {

            lock (m_lock)
            {
                string paramName = string.Empty;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"SELECT * FROM queris_tree_view";
                                           
                        cn.Open();
                        List<QuerisInfo> l = new List<QuerisInfo>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                             
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.Read())
                            {
                                QuerisInfo q = new QuerisInfo();
                                q.allowuser = int.Parse(dataReader["allowuser"].ToString());
                                q.datagrid_column = dataReader["datagrid_column"].ToString();
                                q.queryname = dataReader["queryname"].ToString();
                                q.description = dataReader["description"].ToString();
                                q.datagrid_column = dataReader["datagrid_column"].ToString();
                                q.sqlquery = dataReader["sqlquery"].ToString();
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

        public static void UpdateDishPictureFileName(string dishName, string filename, int picnum)
        {

            lock (m_lock)
            {

                string query;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        if (picnum == 1)
                        {
                            query = @"UPDATE dish_names SET picture1 = @filename
                                         WHERE dish_name = @dishName";
                        }
                        else 
                        {
                            query = @"UPDATE dish_names SET picture2 = @filename
                                         WHERE dish_name = @dishName";
                        }
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {                           
                            cmd.Parameters.AddWithValue("@dishName", dishName);
                            cmd.Parameters.AddWithValue("@filename", filename);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
         
        public static void UpdateDishScoreEnd(string dishName, int scoreEnds)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"UPDATE dish_names SET score_range_end = @scoreEnds
                                         WHERE dish_name = @dishName";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@scoreEnds", scoreEnds);
                            cmd.Parameters.AddWithValue("@dishName", dishName);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }

        public static void UpdateDishDescription(string dishName, string description)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"UPDATE dish_names SET description = @description
                                         WHERE dish_name = @dishName";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@description", description);
                            cmd.Parameters.AddWithValue("@dishName", dishName);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }
        public static string GetDishNameParametersName(string dishname)
        {

            lock (m_lock)
            {
                string paramName = string.Empty;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"SELECT * FROM dish_names 
                                          where dish_name = @dishname";
                        cn.Open();
                        List<MasterChefList> l = new List<MasterChefList>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@dishname", dishname);
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.Read())
                            {
                                paramName = dataReader["parameters"].ToString();
                            }
                        }
                        cn.Close();
                        return paramName;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static List<MasterChefList> GetDemoSolutionList(string groupname)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"SELECT * FROM masterchef 
                                          inner join dish_solution
                                          on   masterchef.sid = dish_solution.id
                                          where groupname = @groupname";
                        cn.Open();
                        List<MasterChefList> l = new List<MasterChefList>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@groupname", groupname);
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            while (dataReader.Read())
                            {
                                MasterChefList d = new MasterChefList();
                                d.solution = new DISH_SOLUTION();
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.sid = int.Parse(dataReader["sid"].ToString());
                                d.groupname = dataReader["groupname"].ToString();
                                d.solution.description = dataReader["description"].ToString();
                                d.solution.dishName = dataReader["dish_name"].ToString();
                                d.solution.name = dataReader["name"].ToString();
                                d.solution.TotalKj = float.Parse(dataReader["TotalKj"].ToString());
                                d.solution.utensilName = dataReader["utensilName"].ToString();
                                d.solution.totalTime = TimeSpan.Parse(dataReader["totalTime"].ToString());

                                l.Add(d);
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

        public static List<string> GetDemoList (int userid)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"SELECT * FROM demo";
                        cn.Open();
                        List<string> l = new List<string>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@userid", userid);
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            while (dataReader.Read())
                            {
                                string name = dataReader["groupname"].ToString();
                                l.Add(name);
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
        public static List<OvenInfo> GetAllOvenList()
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"SELECT * FROM topazes";
                        cn.Open();
                        List<OvenInfo> l = new List<OvenInfo>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            while (dataReader.Read())
                            {
                                OvenInfo d = new OvenInfo();
                                d.guid = dataReader["guid"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.userId = int.Parse(dataReader["userId"].ToString());
                                d.added = DateTime.Parse(dataReader["added"].ToString());
                                d.Alias = dataReader["Alias"].ToString();
                                d.strCouplerSerial = dataReader["coupler"].ToString();
                                d.cablefix = dataReader["cablefix"].ToString();
                                d.Circulator = int.Parse(dataReader["Circulator"].ToString()) == 1 ? true : false;
                                l.Add(d);
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
        public static OvenInfo GetOvenInfo(string guid)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"SELECT * FROM topazes 
                                        inner join users
                                        on users.id = topazes.userid
                                        where guid = @guid";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@guid", guid);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                OvenInfo d = new OvenInfo();
                                d.user = new User();
                                d.guid = dataReader["guid"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.Alias = dataReader["Alias"].ToString();
                                d.userId = int.Parse(dataReader["userId"].ToString());
                                d.added = DateTime.Parse(dataReader["added"].ToString());
                                bool x = int.Parse(dataReader["Circulator"].ToString()) == 1 ? true : false;
                                d.Circulator = x;
                                d.strCouplerSerial = dataReader["coupler"].ToString();
                                d.cablefix = dataReader["cablefix"].ToString();

                                d.user.firstName = dataReader["firstname"].ToString();
                                d.user.lastName = dataReader["lastname"].ToString();
                                cn.Close();
                                return d;
                            }                           
                        }
                        cn.Close();
                        throw (new SystemException("Not found"));
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void UpdateOvenInfo(string guid, 
                                          string description, 
                                          int userid, 
                                          string Alias,
                                          string coupler,
                                          bool circulator,
                                          string cablefix)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"UPDATE topazes SET description = @description, 
                                                            userid = @userid , 
                                                            Alias = @Alias, 
                                                            coupler = @coupler , 
                                                            circulator = @circulator, 
                                                            cablefix = @cablefix
                                                            where guid = @guid";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@guid", guid);
                            cmd.Parameters.AddWithValue("@description", description);

                            string addedDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                            cmd.Parameters.AddWithValue("@added", addedDate);
                            cmd.Parameters.AddWithValue("@Alias", Alias);
                            cmd.Parameters.AddWithValue("@userid", userid);

                            cmd.Parameters.AddWithValue("@coupler", coupler);
                            cmd.Parameters.AddWithValue("@circulator", circulator == true? 1 : 0);
                            cmd.Parameters.AddWithValue("@cablefix", cablefix);
                            cmd.ExecuteNonQuery();
                            
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static void UpdateWaterTemperatureWeightParams(int testid, float tempend, float efficiency)
        {
            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"UPDATE temperatures_params SET tempend = @tempend, efficiency = @efficiency
                                         WHERE testid = @testid";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@tempend", tempend);
                            cmd.Parameters.AddWithValue("@efficiency", efficiency);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }


        public static int SaveFourCupTemperatures(int testid, float [] temp , bool before)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        if (before == true)
                        {
                             query = @"INSERT INTO fourcup_temp_results (testid, cup1_temp_start,cup2_temp_start,cup3_temp_start,cup4_temp_start )
                                         VALUES(@testid, @temp1, @temp2, @temp3,@temp4)";
                        }
                        else
                        {
                            query = @"UPDATE fourcup_temp_results SET cup1_temp_end = @temp1 ,cup2_temp_end = @temp2 ,cup3_temp_end = @temp3 ,cup4_temp_end = @temp4   
                                      where testid = @testid";
                        }
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testid", testid);
                            
                            cmd.Parameters.AddWithValue("@temp1", temp[0]);
                            cmd.Parameters.AddWithValue("@temp2", temp[1]);
                            cmd.Parameters.AddWithValue("@temp3", temp[2]);
                            cmd.Parameters.AddWithValue("@temp4", temp[3]);
                             
                            cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int SaveThreeCupTemperatures(int testid, float[] temp, bool before)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        if (before == true)
                        {
                            query = @"INSERT INTO fourcup_temp_results (testid, cup1_temp_start,cup2_temp_start,cup3_temp_start )
                                         VALUES(@testid, @temp1, @temp2, @temp3)";
                        }
                        else
                        {
                            query = @"UPDATE fourcup_temp_results SET cup1_temp_end = @temp1 ,cup2_temp_end = @temp2
                                        ,cup3_temp_end = @temp3 
                                        where testid = @testid";
                        }
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testid", testid);
                             
                            cmd.Parameters.AddWithValue("@temp1", temp[0]);
                            cmd.Parameters.AddWithValue("@temp2", temp[1]);
                            cmd.Parameters.AddWithValue("@temp3", temp[2]);
                            
                            cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int SaveTwoCupTemperatures(int testid, float[] temp, bool before)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        if (before == true)
                        {
                            query = @"INSERT INTO fourcup_temp_results (testid, cup1_temp_start,cup2_temp_start )
                                         VALUES(@testid, @temp1, @temp2)";
                        }
                        else
                        {
                            query = @"UPDATE fourcup_temp_results SET cup1_temp_end = @temp1 ,cup2_temp_end = @temp2
                                        where testid = @testid";
                        }
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testid", testid);
                            
                            cmd.Parameters.AddWithValue("@temp1", temp[0]);
                            cmd.Parameters.AddWithValue("@temp2", temp[1]);
                            
                            cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int SaveFiveCupTemperatures(int testid, float[] temp, bool before)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        if (before == true)
                        {
                            query = @"INSERT INTO fourcup_temp_results (testid, cup1_temp_start,cup2_temp_start,cup3_temp_start,cup4_temp_start,cup5_temp_start )
                                         VALUES(@testid, @temp1, @temp2, @temp3,@temp4,@temp5)";
                        }
                        else
                        {
                            query = @"UPDATE fourcup_temp_results SET cup1_temp_end = @temp1 ,cup2_temp_end = @temp2
                                        ,cup3_temp_end = @temp3 ,cup4_temp_end = @temp4   ,cup5_temp_end = @temp5
                                        where testid = @testid";
                        }
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testid", testid);

                            cmd.Parameters.AddWithValue("@temp1", temp[0]);
                            cmd.Parameters.AddWithValue("@temp2", temp[1]);
                            cmd.Parameters.AddWithValue("@temp3", temp[2]);
                            cmd.Parameters.AddWithValue("@temp4", temp[3]);
                            cmd.Parameters.AddWithValue("@temp5", temp[4]);
                            
                            cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int SaveNineCupTemperatures(int testid, float[] temp, bool before)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        if (before == true)
                        {
                            query = @"INSERT INTO fourcup_temp_results (testid, cup1_temp_start,cup2_temp_start,cup3_temp_start,cup4_temp_start,cup5_temp_start,cup6_temp_start,cup7_temp_start,cup8_temp_start,cup9_temp_start )
                                         VALUES(@testid, @temp1, @temp2, @temp3,@temp4,@temp5,@temp6,@temp7,@temp8, temp9)";
                        }
                        else
                        {
                            query = @"UPDATE fourcup_temp_results SET cup1_temp_end = @temp1 ,cup2_temp_end = @temp2
                                        ,cup3_temp_end = @temp3 ,cup4_temp_end = @temp4   ,cup5_temp_end = @temp5,
                                        ,cup6_temp_end = @temp6 ,cup7_temp_end = @temp7   ,cup8_temp_end = @temp8,cup9_temp_end = @temp9
                                        where testid = @testid";
                        }
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testid", testid);

                            cmd.Parameters.AddWithValue("@temp1", temp[0]);
                            cmd.Parameters.AddWithValue("@temp2", temp[1]);
                            cmd.Parameters.AddWithValue("@temp3", temp[2]);
                            cmd.Parameters.AddWithValue("@temp4", temp[3]);
                            cmd.Parameters.AddWithValue("@temp5", temp[4]);
                            cmd.Parameters.AddWithValue("@temp6", temp[5]);
                            cmd.Parameters.AddWithValue("@temp7", temp[6]);
                            cmd.Parameters.AddWithValue("@temp8", temp[7]);
                            cmd.Parameters.AddWithValue("@temp9", temp[8]);
                            
                             
                            cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int SaveEightCupTemperatures(int testid, float[] temp, bool before)
        {

            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        if (before == true)
                        {
                            query = @"INSERT INTO fourcup_temp_results (testid, cup1_temp_start,cup2_temp_start,cup3_temp_start,cup4_temp_start,cup5_temp_start,cup6_temp_start,cup7_temp_start,cup8_temp_start)
                                         VALUES(@testid, @temp1, @temp2, @temp3,@temp4,@temp5,@temp6,@temp7,@temp8)";
                        }
                        else
                        {
                            query = @"UPDATE fourcup_temp_results SET cup1_temp_end = @temp1 ,cup2_temp_end = @temp2
                                        ,cup3_temp_end = @temp3 ,cup4_temp_end = @temp4   ,cup5_temp_end = @temp5,
                                        ,cup6_temp_end = @temp6 ,cup7_temp_end = @temp7   ,cup8_temp_end = @temp8
                                        where testid = @testid";
                        }
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testid", testid);

                            cmd.Parameters.AddWithValue("@temp1", temp[0]);
                            cmd.Parameters.AddWithValue("@temp2", temp[1]);
                            cmd.Parameters.AddWithValue("@temp3", temp[2]);
                            cmd.Parameters.AddWithValue("@temp4", temp[3]);
                            cmd.Parameters.AddWithValue("@temp5", temp[4]);
                            cmd.Parameters.AddWithValue("@temp6", temp[5]);
                            cmd.Parameters.AddWithValue("@temp7", temp[6]);
                            cmd.Parameters.AddWithValue("@temp8", temp[7]);
                             
                            
                            cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int SaveWaterTemperatureWeightParams(int testid, float starttemp , float weight)
        {
            lock (m_lock)
            {
                int newId = -1;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"INSERT INTO temperatures_params (testid, tempstart, tempend, weight)
                                         VALUES(@testid, @tempstart, @tempend, @weight)";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@tempstart", starttemp);
                            cmd.Parameters.AddWithValue("@tempend", 0);
                            cmd.Parameters.AddWithValue("@weight", weight);
                            cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }

        public static void AddNewOven(string guid, 
                                      string description, 
                                      int userid, 
                                      string Alias,
                                      string coupler,
                                      bool circulator,
                                      string cablefix)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"INSERT INTO topazes (guid, added , description,userid, Alias,coupler , circulator, cablefix)
                                         VALUES(@guid, @added, @description,@userid,@Alias,@coupler , @circulator, @cablefix)";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@guid", guid);
                            cmd.Parameters.AddWithValue("@description", description);

                            string addedDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                            cmd.Parameters.AddWithValue("@added", addedDate);
                            cmd.Parameters.AddWithValue("@Alias", Alias);
                            cmd.Parameters.AddWithValue("@userid", userid);

                            cmd.Parameters.AddWithValue("@coupler", coupler);
                            cmd.Parameters.AddWithValue("@circulator", circulator == true? 1 : 0);
                            cmd.Parameters.AddWithValue("@cablefix", cablefix);
                            cmd.ExecuteNonQuery();
                            
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
        public static void AddPicturesBeforeToTest(int testId, string compip, string savepath, PictureInfo [] pictures)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    { 
                        string query = @"INSERT INTO test_pictures_before (testid, filename,description,picname, compip,savepath)
                                         VALUES(@testid, @filename, @description,@picname, @compip, @savepath)";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            foreach (PictureInfo n in pictures)
                            {
                                cmd.Parameters.Clear();                                    
                                cmd.Parameters.AddWithValue("@testid", testId);
                                cmd.Parameters.AddWithValue("@filename", n.dbName);
                                cmd.Parameters.AddWithValue("@description", n.description);
                                cmd.Parameters.AddWithValue("@picname", n.title);

                                cmd.Parameters.AddWithValue("@savepath", savepath);
                                cmd.Parameters.AddWithValue("@compip", compip);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }
        public static void AddPictureBeforeToTest(int testId, string compip, string savepath, PictureInfo n)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"INSERT INTO test_pictures_before (testid, filename,description,picname, compip,savepath)
                                         VALUES(@testid, @filename, @description,@picname, @compip, @savepath)";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@testid", testId);
                            cmd.Parameters.AddWithValue("@filename", n.dbName);
                            cmd.Parameters.AddWithValue("@description", n.description);
                            cmd.Parameters.AddWithValue("@picname", n.title);

                            cmd.Parameters.AddWithValue("@savepath", savepath);
                            cmd.Parameters.AddWithValue("@compip", compip);

                            cmd.ExecuteNonQuery();                            
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }
        public static void AddPictureAfterToTest(int testId, string compip, string savepath, PictureInfo n)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"INSERT INTO test_pictures_after (testid, filename,description,picname, compip,savepath)
                                         VALUES(@testid, @filename, @description,@picname, @compip, @savepath)";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@testid", testId);
                            cmd.Parameters.AddWithValue("@filename", n.dbName);
                            cmd.Parameters.AddWithValue("@description", n.description);
                            cmd.Parameters.AddWithValue("@picname", n.title);

                            cmd.Parameters.AddWithValue("@savepath", savepath);
                            cmd.Parameters.AddWithValue("@compip", compip);

                            cmd.ExecuteNonQuery();                            
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void AddPicturesAfterToTest(int testId, string compip, string savepath, PictureInfo[] pictures)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = @"INSERT INTO test_pictures_after (testid, filename,description,picname, compip,savepath)
                                         VALUES(@testid, @filename, @description,@picname, @compip, @savepath)";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            foreach (PictureInfo n in pictures)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@testid", testId);
                                cmd.Parameters.AddWithValue("@filename", n.dbName);
                                cmd.Parameters.AddWithValue("@description", n.description);
                                cmd.Parameters.AddWithValue("@picname", n.title);

                                cmd.Parameters.AddWithValue("@savepath", savepath);
                                cmd.Parameters.AddWithValue("@compip", compip);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
        public static string  GetDishDescription(string dishName)
        {
            lock (m_lock)
            {
                string desc = "";
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        query = @"SELECT * FROM dish_names where dish_name = @dishName";

                        List<Tuple<string, string>> l = new List<Tuple<string, string>>();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@dishName", dishName);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                desc = dataReader["description"].ToString();
                            }
                        }
                        cn.Close();
                        return desc;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
     
        public static DISHInfo GetDishInfo(string dishName, out bool found)
        {
            lock (m_lock)
            {
                DISHInfo d = new DISHInfo();
                found = false;             
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        query = @"SELECT * FROM dish_names where dish_name = @dishName";

                        List<Tuple<string, string>> l = new List<Tuple<string, string>>();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@dishName", dishName);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            if (dataReader.Read())
                            {
                              
                                d.description = dataReader["description"].ToString();
                                d.dishName = dataReader["dish_name"].ToString();
                                d.picture1 = dataReader["picture1"].ToString();
                                d.picture2 = dataReader["picture2"].ToString();
                                d.dishScoreEnd = int.Parse(dataReader["score_range_end"].ToString());
                                found = true;                                
                            }
                        }
                        cn.Close();
                        return d;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static List<DISHInfo> GetAllDishNames()
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        query = @"SELECT * FROM dish_names order by dish_name";

                        List<DISHInfo> l = new List<DISHInfo>();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                DISHInfo d = new DISHInfo();
                                d.dishName = dataReader["dish_name"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.picture1 = dataReader["picture1"].ToString();
                                d.picture2 = dataReader["picture2"].ToString();
                                d.dishScoreEnd = int.Parse(dataReader["score_range_end"].ToString());
                                l.Add(d);
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
        public static FINAL_TEST_RESULT GetFinalTestResults(int testid)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        query = @"SELECT *, final_results.totalkj as tkj FROM test
                                inner join users 
                                on test.user_id = users.id
                                inner join final_results 
                                on test.id = final_results.testid where test.id =" + testid;

                        FINAL_TEST_RESULT d = new FINAL_TEST_RESULT();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                          
                            while (dataReader.Read())
                            {                                 
                                d.testid = int.Parse(dataReader["testid"].ToString());
                                d.DRFileName = dataReader["DRFileName"].ToString();
                                d.EnergyFileName = dataReader["EnergyFileName"].ToString();
                                d.FinalDescription = dataReader["FinalDescription"].ToString();
                                d.PassFail = int.Parse(dataReader["PassFail"].ToString());
                                d.PowerInfoFileName = dataReader["PowerInfoFileName"].ToString();
                                d.totalwatts = float.Parse(dataReader["totalwatts"].ToString());
                                d.totalkj = float.Parse(dataReader["tkj"].ToString());
                                d.avgfordb = float.Parse(dataReader["avgfordb"].ToString());
                                d.avgrefdb = float.Parse(dataReader["avgrefdb"].ToString());
                            }
                        }
                        cn.Close();
                        return d;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static void UpdatePictureDetails(int testId, string fileName, string picname, string description , bool before)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query;
                        if (before == true)
                        {
                            query = @"UPDATE  test_pictures_before
                                         SET picname = @picname , description = @description
                                         where testid = @testId and filename = @fileName";
                        }
                        else
                        {
                            query = @"UPDATE  test_pictures_after
                                         SET picname = @picname , description = @description
                                         where testid = @testId and filename = @fileName";

                        }
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@picname", picname);
                            cmd.Parameters.AddWithValue("@description", description);
                            cmd.Parameters.AddWithValue("@testId", testId);
                            cmd.Parameters.AddWithValue("@fileName", fileName);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
        public static void UpdateAlgoEqualEnergyParams(int sid, int rowindex, AlgoEqualEnergyParams p)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"UPDATE  algo_equalenergy_params 
                                         SET lowvalue = @lowvalue , highvalue = @highvalue, agc=@agc ,
                                         substractEmptyCavity = @substractEmptyCavity,
                                         freqtablefilename = @freqtablefilename,
                                         mode = @mode , acckj = @acckj, singlerepetition = @singlerepetition,
                                         toppercentage = @toppercentage
                                         where sid = @sid and rowindex = @rowindex";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@substractEmptyCavity", p.substractEmptyCavity);
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@rowindex", p.RowIndex);
                            cmd.Parameters.AddWithValue("@acckj", p.acckj);
                            cmd.Parameters.AddWithValue("@agc", p.agc == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@lowvalue", p.lowvalue);                            
                            cmd.Parameters.AddWithValue("@highvalue", p.highvalue);
                            cmd.Parameters.AddWithValue("@mode", p.mode);
                            cmd.Parameters.AddWithValue("@singlerepetition", p.singlerepetition == true ? 1 : 0);
                            if (p.freqtablefilename == null || p.freqtablefilename == "")
                                p.freqtablefilename = "all";
                            cmd.Parameters.AddWithValue("@freqtablefilename", p.freqtablefilename);
                            cmd.Parameters.AddWithValue("@toppercentage", p.toppercentage);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void UpdateAlgoThresholdParams(int sid, int rowindex, AlgoThresholParams p)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"UPDATE  algo_drthreshold_params SET lowvalue = @lowvalue , 
                                        freqtablefilename = @freqtablefilename,
                                        substractEmptyCavity = @substractEmptyCavity,
                                        highvalue = @highvalue, mode = @mode , powertime=@powertime , agc=@agc , equaldrtime=@equaldrtime
                                         where sid = @sid and rowindex = @rowindex";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@equaldrtime", p.equaldrtime == true ? 1 : 0);


                            cmd.Parameters.AddWithValue("@substractEmptyCavity", p.substractEmptyCavity);

                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@rowindex", p.RowIndex);
                            if (p.freqtablefilename == null || p.freqtablefilename == "")
                                p.freqtablefilename = "all";
                            cmd.Parameters.AddWithValue("@freqtablefilename", p.freqtablefilename);
                            cmd.Parameters.AddWithValue("@agc", p.agc == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@lowvalue", p.lowvalue);
                            cmd.Parameters.AddWithValue("@powertime", p.power_time_mili);
                            cmd.Parameters.AddWithValue("@highvalue", p.highvalue);
                            cmd.Parameters.AddWithValue("@mode", p.mode);

                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void UpdateAlgoTopPercentageParams(int sid, int rowindex, AlgoTopPercentageParams p)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"UPDATE  algo_top_percentage_params SET powertime=@powertime , 
                                        substractEmptyCavity = @substractEmptyCavity,
                                        agc=@agc , toppercent = @toppercent , 
                                         freqtablefilename = @freqtablefilename,
                                         equaldrtime = @equaldrtime
                                         where sid = @sid and rowindex = @rowindex";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@substractEmptyCavity", p.substractEmptyCavity);
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@agc", p.agc == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@toppercent", p.toppercent);
                            cmd.Parameters.AddWithValue("@equaldrtime", p.equaldrtime);                            
                            cmd.Parameters.AddWithValue("@rowindex", p.RowIndex);
                            cmd.Parameters.AddWithValue("@powertime", p.power_time_mili);
                            if (p.freqtablefilename == null || p.freqtablefilename == "")
                                p.freqtablefilename = "all";
                            cmd.Parameters.AddWithValue("@freqtablefilename", p.freqtablefilename);

                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static void SaveAlgoTopPercentageParams(int sid, int rowindex, AlgoTopPercentageParams p)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO algo_top_percentage_params (sid, rowindex,powertime,agc,toppercent, equaldrtime,freqtablefilename, substractEmptyCavity)
                                         VALUES(@sid, @rowindex, @powertime,@agc,@toppercent,@equaldrtime,@freqtablefilename, @substractEmptyCavity)";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            
                            cmd.Parameters.AddWithValue("@substractEmptyCavity", p.substractEmptyCavity);
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@rowindex", p.RowIndex);
                            cmd.Parameters.AddWithValue("@toppercent", p.toppercent);
                            cmd.Parameters.AddWithValue("@equaldrtime", p.equaldrtime);
                            if (p.freqtablefilename == null || p.freqtablefilename == string.Empty)
                                p.freqtablefilename = "all";
                            cmd.Parameters.AddWithValue("@freqtablefilename", p.freqtablefilename);
                            cmd.Parameters.AddWithValue("@agc", p.agc == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@powertime", p.power_time_mili);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        } 
        public static void AddTestAlgoTopPercentageParams(int testid, 
                                                          int rowindex,
                                                          AlgoTopPercentageParams p)
        {
             
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {

                        // save the freqtable under test name
                        if (p.freqtablefilename != "all")
                        {
                            string filename = DRIVE.Drive + @"TopazPOC\TestFrequencies\" + testid + "_" + rowindex + ".txt";
                            File.Copy(p.freqtablefilename, filename);
                            p.freqtablefilename = filename;
                        }

                        string query = @"INSERT INTO test_algo_top_percentage_params (testid, rowindex,powertime,agc,toppercent, equaldrtime,freqtablefilename)
                                         VALUES(@testid, @rowindex, @powertime,@agc,@toppercent,@equaldrtime,@freqtablefilename)";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {                             
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);
                            cmd.Parameters.AddWithValue("@toppercent", p.toppercent);
                            cmd.Parameters.AddWithValue("@equaldrtime", p.equaldrtime);
                            cmd.Parameters.AddWithValue("@agc", p.agc == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@powertime", p.power_time_mili);
                            if (p.freqtablefilename == null || p.freqtablefilename == string.Empty)
                                p.freqtablefilename = "all";
                            cmd.Parameters.AddWithValue("@freqtablefilename", p.freqtablefilename);
                            cmd.ExecuteNonQuery();
                             
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static void AddNewDishName(string name, string description)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO dish_names (dish_name,description)
                                         VALUES(@dish_name,@description)";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@dish_name", name);
                            cmd.Parameters.AddWithValue("@description", description);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void SaveAlgoEqualEnergyParams(int sid, int rowindex, AlgoEqualEnergyParams p)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO algo_equalenergy_params (sid, rowindex, lowvalue , 
                                        highvalue, mode, acckj,agc,singlerepetition,freqtablefilename, toppercentage, substractEmptyCavity)
                                        VALUES(@sid, @rowindex, @lowvalue , @highvalue, @mode, @acckj, @agc,@singlerepetition,@freqtablefilename, @toppercentage, @substractEmptyCavity)";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@substractEmptyCavity", p.substractEmptyCavity);
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@rowindex", p.RowIndex);
                            cmd.Parameters.AddWithValue("@acckj", p.acckj);
                            cmd.Parameters.AddWithValue("@agc", p.agc == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@lowvalue", p.lowvalue);
                            cmd.Parameters.AddWithValue("@highvalue", p.highvalue);
                            cmd.Parameters.AddWithValue("@mode", p.mode);
                            if (p.freqtablefilename == null || p.freqtablefilename == string.Empty)
                                p.freqtablefilename = "all";
                            cmd.Parameters.AddWithValue("@freqtablefilename", p.freqtablefilename);
                            cmd.Parameters.AddWithValue("@singlerepetition", p.singlerepetition == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@toppercentage", p.toppercentage);

                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void AddTestAlgoEqualEnergyParams(int testid,
                                                        int rowindex,
                                                        AlgoEqualEnergyParams p)                                                        
        {
            
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {

                        // save the freqtable under test name
                        if (p.freqtablefilename != "all")
                        {
                            string filename = DRIVE.Drive + @"TopazPOC\TestFrequencies\" + testid + "_" + rowindex + ".txt";
                            File.Copy(p.freqtablefilename, filename);
                            p.freqtablefilename = filename;
                        }

                        string query = @"INSERT INTO test_algo_equalenergy_params (testid, rowindex, lowvalue , 
                                        highvalue, mode, acckj,agc,singlerepetition, toppercentage, substractEmptyCavity)
                                        VALUES(@testid, @rowindex, @lowvalue , @highvalue, @mode, @acckj, @agc,@singlerepetition,@toppercentage, @substractEmptyCavity)";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {                             
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@substractEmptyCavity", p.substractEmptyCavity);
                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);
                            cmd.Parameters.AddWithValue("@acckj", p.acckj);
                            cmd.Parameters.AddWithValue("@agc", p.agc == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@lowvalue", p.lowvalue);
                            cmd.Parameters.AddWithValue("@highvalue", p.highvalue);
                            cmd.Parameters.AddWithValue("@mode", p.mode);
                            cmd.Parameters.AddWithValue("@singlerepetition", p.singlerepetition == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@toppercentage", p.toppercentage);
                            cmd.ExecuteNonQuery();                            
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void SaveAlgoThresholdParams(int sid, int rowindex, AlgoThresholParams p)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO algo_drthreshold_params (sid, rowindex, lowvalue , 
                                        highvalue, mode,powertime, agc, equaldrtime, freqtablefilename, substractEmptyCavity)
                                         VALUES(@sid, @rowindex, @lowvalue , @highvalue, @mode,@powertime, @agc , @equaldrtime, @freqtablefilename, @substractEmptyCavity)";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@substractEmptyCavity", p.substractEmptyCavity);
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@equaldrtime", p.equaldrtime == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@rowindex", p.RowIndex);
                            cmd.Parameters.AddWithValue("@agc", p.agc == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@lowvalue", p.lowvalue);
                            cmd.Parameters.AddWithValue("@highvalue", p.highvalue);
                            cmd.Parameters.AddWithValue("@powertime", p.power_time_mili);
                            if (p.freqtablefilename == null || p.freqtablefilename == string.Empty)
                                p.freqtablefilename = "all";
                            cmd.Parameters.AddWithValue("@freqtablefilename", p.freqtablefilename);
                            cmd.Parameters.AddWithValue("@mode", p.mode);

                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void AddTestAlgoRFOffParams(int testid, 
                                                  int rowindex,
                                                  TimeSpan time)
        {
             
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO test_algo_rfoff_params (testid, rowindex, time)                                        
                                         VALUES(@testid, @rowindex, @time)";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                             
                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@time", time);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static void AddTestAlgoThresholdParams(int testid, 
                                                      int rowindex,
                                                      AlgoThresholParams p)
        {
             
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {

                        // save the freqtable under test name
                        if (p.freqtablefilename != "all")
                        {
                            string filename = DRIVE.Drive + @"TopazPOC\TestFrequencies\" + testid + "_" + rowindex + ".txt";
                            File.Copy(p.freqtablefilename, filename, true);
                            p.freqtablefilename = filename;
                        }

                        string query = @"INSERT INTO test_algo_drthreshold_params (testid, rowindex, lowvalue , 
                                        highvalue, mode,powertime, agc, equaldrtime, substractEmptyCavity)
                                         VALUES(@testid, @rowindex, @lowvalue , @highvalue, @mode,@powertime, @agc , @equaldrtime, @substractEmptyCavity)";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                             
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@substractEmptyCavity", p.substractEmptyCavity);
                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@equaldrtime", p.equaldrtime == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);
                            cmd.Parameters.AddWithValue("@agc", p.agc == true ? 1 : 0);
                            cmd.Parameters.AddWithValue("@lowvalue", p.lowvalue);
                            cmd.Parameters.AddWithValue("@highvalue", p.highvalue);
                            cmd.Parameters.AddWithValue("@powertime", p.power_time_mili);
                            cmd.Parameters.AddWithValue("@mode", p.mode);
                            cmd.ExecuteNonQuery();
                             
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }



        public static AlgoEqualEnergyParams? GetAlgoEqualEnergyParams(int solutionid, int rowindex)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.algo_equalenergy_params
                                 where sid = @solutionid and RowIndex = @rowindex";
                        
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@solutionid", solutionid);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);

                            MySqlDataReader dataReader = cmd.ExecuteReader();
                          
                            while (dataReader.Read())
                            {
                                AlgoEqualEnergyParams d = new AlgoEqualEnergyParams();
                                d.highvalue = float.Parse(dataReader["highvalue"].ToString());
                                d.lowvalue = float.Parse(dataReader["lowvalue"].ToString());
                                d.acckj = float.Parse(dataReader["acckj"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = (x == 1) ? true : false;
                                d.mode = int.Parse(dataReader["mode"].ToString());
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                d.sid = int.Parse(dataReader["sid"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                d.singlerepetition = int.Parse(dataReader["singlerepetition"].ToString()) == 1 ? true : false;
                                d.toppercentage = int.Parse(dataReader["toppercentage"].ToString());
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;
                                cn.Close();
                                return d;
                            }
                        }
                        cn.Close();
                        return null;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static AlgoThresholParams ? GetAlgoThreaholdParams(int solutionid, int rowindex)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.algo_drthreshold_params
                                 where sid = @solutionid and RowIndex = @rowindex";
                        
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@solutionid", solutionid);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);

                            MySqlDataReader dataReader = cmd.ExecuteReader();
                          
                            while (dataReader.Read())
                            {
                                AlgoThresholParams d = new AlgoThresholParams();
                                d.highvalue = float.Parse(dataReader["highvalue"].ToString());
                                d.lowvalue = float.Parse(dataReader["lowvalue"].ToString());
                                d.mode = int.Parse(dataReader["mode"].ToString());
                                d.power_time_mili = int.Parse(dataReader["powertime"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = (x == 1) ? true : false;
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                x = int.Parse(dataReader["equaldrtime"].ToString());
                                d.equaldrtime = (x == 1) ? true : false;
                                
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;

                                d.sid = int.Parse(dataReader["sid"].ToString());
                                cn.Close();
                                return d;
                            }
                        }
                        cn.Close();
                        return null;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static AlgoTopPercentageParams? GetAlgoTopPercentageParams(int solutionid, int rowindex)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.algo_top_percentage_params
                                 where sid = @solutionid and RowIndex = @rowindex";

                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@solutionid", solutionid);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoTopPercentageParams d = new AlgoTopPercentageParams();
                                d.power_time_mili = int.Parse(dataReader["powertime"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = (x == 1) ? true : false;
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                d.toppercent = int.Parse(dataReader["toppercent"].ToString());
                                d.sid = int.Parse(dataReader["sid"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                try
                                {
                                    x = int.Parse(dataReader["equaldrtime"].ToString());
                                    d.equaldrtime = (x == 1) ? true : false;
                                }
                                catch (Exception err)
                                {
                                    d.equaldrtime = true;
                                }
                                
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;

                                cn.Close();
                                return d;
                            }
                        }
                        cn.Close();
                        return null;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static List<AlgoThresholParams> GetAlgoThreaholdParams(int solutionid)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.algo_drthreshold_params
                                 where sid = @solutionid";

                        cn.Open();
                        List<AlgoThresholParams> l = new List<AlgoThresholParams>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@solutionid", solutionid);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoThresholParams d = new AlgoThresholParams();
                                d.highvalue = float.Parse(dataReader["highvalue"].ToString());
                                d.lowvalue = float.Parse(dataReader["lowvalue"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = x == 1 ? true : false; 
                                d.mode = int.Parse(dataReader["mode"].ToString());
                                d.power_time_mili = int.Parse(dataReader["powertime"].ToString());
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                d.sid = int.Parse(dataReader["sid"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                x = int.Parse(dataReader["equaldrtime"].ToString());
                                d.equaldrtime = x == 1 ? true : false;

                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = x == 1 ? true : false; 

                                l.Add(d);
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

        public static AlgoThresholParams ? GetTestAlgoThreaholdParams(int testid, int rowindex)
        {

            lock (m_lock)
            {
                AlgoThresholParams ?d1 = null;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.test_algo_drthreshold_params
                                 where testid = @testid  and RowIndex = @rowindex";

                        cn.Open();
                       
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoThresholParams d = new AlgoThresholParams();
                                d.highvalue = float.Parse(dataReader["highvalue"].ToString());
                                d.lowvalue = float.Parse(dataReader["lowvalue"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = x == 1 ? true : false;
                                d.mode = int.Parse(dataReader["mode"].ToString());
                                d.power_time_mili = int.Parse(dataReader["powertime"].ToString());
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                d.sid = int.Parse(dataReader["testid"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                x = int.Parse(dataReader["equaldrtime"].ToString());
                                d.equaldrtime = x == 1 ? true : false;
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;

                                d1 = d;
                            }
                        }
                        cn.Close();
                        return d1;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static List<AlgoThresholParams> GetTestAlgoThreaholdParams(int testid)
        {

            lock (m_lock)
            {
               
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.test_algo_drthreshold_params
                                 where testid = @testid";

                        cn.Open();

                        List<AlgoThresholParams> l = new List<AlgoThresholParams>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testid);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoThresholParams d = new AlgoThresholParams();
                                d.highvalue = float.Parse(dataReader["highvalue"].ToString());
                                d.lowvalue = float.Parse(dataReader["lowvalue"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = x == 1 ? true : false;
                                d.mode = int.Parse(dataReader["mode"].ToString());
                                d.power_time_mili = int.Parse(dataReader["powertime"].ToString());
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                d.sid = int.Parse(dataReader["testid"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                x = int.Parse(dataReader["equaldrtime"].ToString());
                                d.equaldrtime = x == 1 ? true : false;
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;
                                l.Add(d);
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

        public static List<AlgoEqualEnergyParams> GetAlgoEqualEnergyParams(int solutionid)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.algo_equalenergy_params
                                 where sid = @solutionid";

                        cn.Open();
                        List<AlgoEqualEnergyParams> l = new List<AlgoEqualEnergyParams>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@solutionid", solutionid);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoEqualEnergyParams d = new AlgoEqualEnergyParams();
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = (x == 1) ? true : false;
                                d.acckj = float.Parse(dataReader["acckj"].ToString());
                                d.sid = int.Parse(dataReader["sid"].ToString());
                                d.mode = int.Parse(dataReader["mode"].ToString());
                                d.highvalue = float.Parse(dataReader["highvalue"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                d.lowvalue = float.Parse(dataReader["lowvalue"].ToString());
                                d.singlerepetition = int.Parse(dataReader["singlerepetition"].ToString()) == 1 ? true : false;
                                d.toppercentage = int.Parse(dataReader["toppercentage"].ToString());
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;
                                l.Add(d);
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

        public static AlgoEqualEnergyParams ? GetTestAlgoEqualEnergyParams(int testid, int rowindex)
        {

            lock (m_lock)
            {
                AlgoEqualEnergyParams? d1 = null;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.test_algo_equalenergy_params
                                 where testid = @testid  and RowIndex = @rowindex";

                        cn.Open();
                        List<AlgoEqualEnergyParams> l = new List<AlgoEqualEnergyParams>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoEqualEnergyParams d = new AlgoEqualEnergyParams();
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = (x == 1) ? true : false;
                                d.mode = int.Parse(dataReader["mode"].ToString());
                                d.acckj = float.Parse(dataReader["acckj"].ToString());
                                d.highvalue = float.Parse(dataReader["highvalue"].ToString());
                                d.lowvalue = float.Parse(dataReader["lowvalue"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                d.singlerepetition = int.Parse(dataReader["singlerepetition"].ToString()) == 1 ? true : false;
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;
                                try
                                {
                                    if (d.mode == 3)
                                        d.toppercentage = int.Parse(dataReader["toppercentage"].ToString());
                                    else
                                        d.toppercentage = 0;
                                }
                                catch (Exception err)
                                {
                                    d.toppercentage = 0;
                                }
                                d1 = d;
                            }
                        }
                        cn.Close();
                        return d1;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static List<AlgoEqualEnergyParams> GetTestAlgoEqualEnergyParams(int testid)
        {

            lock (m_lock)
            {
                List<AlgoEqualEnergyParams> l = new List<AlgoEqualEnergyParams>();
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.test_algo_equalenergy_params
                                 where testid = @testid";

                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testid);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoEqualEnergyParams d = new AlgoEqualEnergyParams();
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = (x == 1) ? true : false;
                                d.mode = int.Parse(dataReader["mode"].ToString());
                                d.acckj = float.Parse(dataReader["acckj"].ToString());
                                d.highvalue = float.Parse(dataReader["highvalue"].ToString());
                                d.lowvalue = float.Parse(dataReader["lowvalue"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                d.singlerepetition = int.Parse(dataReader["singlerepetition"].ToString()) == 1 ? true : false;
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;
                                try
                                {
                                    if (d.mode == 3)
                                        d.toppercentage = int.Parse(dataReader["toppercentage"].ToString());
                                    else
                                        d.toppercentage = 0;
                                }
                                catch (Exception err)
                                {
                                    d.toppercentage = 0;
                                }
                                l.Add(d);
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

        public static List<AlgoTopPercentageParams> GetAlgoTopPercentageParams(int solutionid)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.algo_top_percentage_params
                                 where sid = @solutionid";

                        cn.Open();
                        List<AlgoTopPercentageParams> l = new List<AlgoTopPercentageParams>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@solutionid", solutionid);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoTopPercentageParams d = new AlgoTopPercentageParams();
                                d.power_time_mili = int.Parse(dataReader["powertime"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = (x == 1) ? true : false;
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                d.toppercent = int.Parse(dataReader["toppercent"].ToString());
                                d.sid = int.Parse(dataReader["sid"].ToString());
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;
                                l.Add(d);
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



        public static TestInfo GetTestInfo(int testid)
        {

            lock (m_lock)
            {
                TestInfo d = new TestInfo();
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        query = @"SELECT *,final_results.totalkj as tkj FROM test                            
                                 inner join final_results on final_results.testid = test.id
                                 where id= @testid";

                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            
                            cmd.Parameters.AddWithValue("@testid", testid);                          
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                          
                            while (dataReader.Read())
                            {
                                 
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.user_id = int.Parse(dataReader["user_id"].ToString());
                                d.StartDated = DateTime.Parse(dataReader["StartDated"].ToString());
                                d.StopDated = DateTime.Parse(dataReader["StopDated"].ToString());
                                d.dish_name = dataReader["dish_name"].ToString();
                                d.testname = dataReader["testname"].ToString();
                                //d.email = dataReader["email"].ToString();
                                d.remark = dataReader["remark"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.utensil_name = dataReader["utensil_name"].ToString();
                                //d.firstName = dataReader["firstName"].ToString();
                                //d.lastName = dataReader["lastName"].ToString();
                                d.drpower = float.Parse(dataReader["drpower"].ToString());
                                d.totalTime = TimeSpan.Parse(dataReader["totaltime"].ToString());
                                d.totalKj = float.Parse(dataReader["totalkj"].ToString());

                                d.compare_reason = dataReader["compare_reason"].ToString();
                                bool b = int.TryParse(dataReader["compareto"].ToString(), out d.compareto);
                                if (b == false)
                                    d.compareto = -1;


                                d.fres = new FinalResults();
                                d.fres.FinalDescription = dataReader["FinalDescription"].ToString();
                                b = int.TryParse(dataReader["PassFail"].ToString(), out d.fres.PassFail);
                                b = float.TryParse(dataReader["avgfordb"].ToString(), out d.fres.avgfordb);
                                b = float.TryParse(dataReader["avgrefdb"].ToString(), out d.fres.avgrefdb);
                                b = float.TryParse(dataReader["tkj"].ToString(), out d.fres.totalkj);
                                b = float.TryParse(dataReader["totalwatts"].ToString(), out d.fres.totalwatts);
                            }
                        }
                        cn.Close();
                        return d;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
 
        public static List<TestInfo> GetTestList(string firstName, 
                                                 string lastName, 
                                                 string orderby , ASCD acdc)
        {

             lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        bool addParams = false;
                        string query = string.Empty;
                        if (firstName == string.Empty && lastName == string.Empty)
                        {
                            query = @"SELECT *, final_results.totalkj as tkj  FROM test
                                                inner join users 
                                                on test.user_id = users.id
                                                inner join final_results on final_results.testid = test.id";
                        }
                        else
                        {
                            addParams = true;
                            query = @"SELECT *, final_results.totalkj as tkj FROM test
                                        inner join users 
                                        on test.user_id = users.id
                                        inner join final_results on final_results.testid = test.id
                                        where users.firstName =@firstName and users.lastName = @lastName ";
                        }
                        orderby = orderby.ToLower();
                        if (orderby == "id") orderby = "test.id";
                        if (orderby == "date") orderby = "test.StartDated";
                        if (orderby == "test name") orderby = "test.testname";
                           

                        if (acdc == ASCD.DESC)
                        {
                            query += " order by " + orderby + "  desc";
                        }
                        else
                        {
                            query += " order by " + orderby + "  asc";
                        }
                        List<TestInfo> l = new  List<TestInfo>();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            if (addParams)
                            {
                                cmd.Parameters.AddWithValue("@firstName", firstName);
                                cmd.Parameters.AddWithValue("@lastName", lastName);
                            }
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                          
                            while (dataReader.Read())
                            {
                                
                                TestInfo d = new TestInfo();
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.user_id = int.Parse(dataReader["user_id"].ToString());
                                d.StartDated = DateTime.Parse(dataReader["StartDated"].ToString());
                                d.StopDated = DateTime.Parse(dataReader["StopDated"].ToString());
                                d.dish_name = dataReader["dish_name"].ToString();
                                d.testname = dataReader["testname"].ToString();
                                d.email = dataReader["email"].ToString();
                                d.remark = dataReader["remark"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.utensil_name = dataReader["utensil_name"].ToString();
                                d.firstName = dataReader["firstName"].ToString();
                                d.lastName = dataReader["lastName"].ToString();
                                d.drpower = float.Parse(dataReader["drpower"].ToString());
                                d.totalTime = TimeSpan.Parse(dataReader["totaltime"].ToString());
                                d.totalKj = float.Parse(dataReader["totalkj"].ToString());    
                                d.compare_reason = dataReader["compare_reason"].ToString();
                                bool b = int.TryParse(dataReader["compareto"].ToString(), out d.compareto);
                                if (b == false)
                                    d.compareto = -1;

                                d.fres = new FinalResults();
                                d.fres.FinalDescription = dataReader["FinalDescription"].ToString();
                                b = int.TryParse(dataReader["PassFail"].ToString(), out d.fres.PassFail);
                                b = float.TryParse(dataReader["avgfordb"].ToString(), out d.fres.avgfordb);
                                b = float.TryParse(dataReader["avgrefdb"].ToString(), out d.fres.avgrefdb);
                                b = float.TryParse(dataReader["tkj"].ToString(), out d.fres.totalkj);
                                b = float.TryParse(dataReader["totalwatts"].ToString(), out d.fres.totalwatts);



                                l.Add(d);
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


        public static List<TestInfo> GetTestListByNearTestName(int userid, string orderby, ASCD acdc,string names, int skip, int limit)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;
                        if (userid != -1)
                        {
                            query = @"SELECT  * , final_results.totalkj as tkj FROM test
                            inner join users 
                            on test.user_id = users.id
                            inner join final_results on final_results.testid = test.id
                            where users.id = @userid and 
                            testname like @names  ";
                        }
                        else
                        {
                            query = @"SELECT  * , final_results.totalkj as tkj FROM test
                            inner join users 
                            on test.user_id = users.id
                            inner join final_results on final_results.testid = test.id
                            where testname like @names  ";

                        }
                        orderby = orderby.ToLower();
                        if (orderby == "id") orderby = "test.id ";
                        if (orderby == "date") orderby = "test.StartDated ";
                        if (orderby == "test name") orderby = "test.testname ";


                        if (acdc == ASCD.DESC)
                        {
                            query += " order by " + orderby + "  desc  ";
                        }
                        else
                        {
                            query += " order by " + orderby + "  asc  ";
                        }

                        if (limit > 0)
                            query += "limit " + limit;
                        if (skip > 0)
                            query += "   offset " + skip;

                        List<TestInfo> l = new List<TestInfo>();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            if (userid != -1)
                                cmd.Parameters.AddWithValue("@userid", userid);
                            cmd.Parameters.AddWithValue("@names", "%" + names + "%");
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {

                                TestInfo d = new TestInfo();
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.user_id = int.Parse(dataReader["user_id"].ToString());
                                d.StartDated = DateTime.Parse(dataReader["StartDated"].ToString());
                                d.StopDated = DateTime.Parse(dataReader["StopDated"].ToString());
                                d.dish_name = dataReader["dish_name"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.testname = dataReader["testname"].ToString();
                                d.email = dataReader["email"].ToString();
                                d.remark = dataReader["remark"].ToString();
                                d.utensil_name = dataReader["utensil_name"].ToString();
                                d.firstName = dataReader["firstName"].ToString();
                                d.lastName = dataReader["lastName"].ToString();
                                d.guid = dataReader["topazguid"].ToString();
                                d.drpower = float.Parse(dataReader["drpower"].ToString());
                                d.totalTime  = TimeSpan.Parse(dataReader["totaltime"].ToString());
                                d.totalKj = float.Parse(dataReader["totalkj"].ToString());
                                d.compare_reason = dataReader["compare_reason"].ToString();
                                d.fres = new FinalResults();
                                d.fres.FinalDescription = dataReader["FinalDescription"].ToString();
                                bool b = int.TryParse(dataReader["PassFail"].ToString(), out d.fres.PassFail);
                                b = float.TryParse(dataReader["avgfordb"].ToString(), out d.fres.avgfordb);
                                b = float.TryParse(dataReader["avgrefdb"].ToString(), out d.fres.avgrefdb);
                                b = float.TryParse(dataReader["tkj"].ToString(), out d.fres.totalkj);
                                b = float.TryParse(dataReader["totalwatts"].ToString(), out d.fres.totalwatts);

                                l.Add(d);
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

        public static List<TestInfo> GetMyTestList(int userid, string orderby, ASCD acdc, int skip, int limit)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                       
                        string query = string.Empty;                        
                        query = @"SELECT * FROM test
                                    inner join users 
                                    on test.user_id = users.id
                                    where users.id = @userid";

                        orderby = orderby.ToLower();
                        if (orderby == "id") orderby = "test.id ";
                        if (orderby == "date") orderby = "test.StartDated ";
                        if (orderby == "test name") orderby = "test.testname ";


                        if (acdc == ASCD.DESC)
                        {
                            query += " order by " + orderby + "  desc  ";
                        }
                        else
                        {
                            query += " order by " + orderby + "  asc  ";
                        }

                        if (limit > 0)
                            query += "limit " + limit;
                        if (skip > 0)
                            query += "   offset " + skip;

                        List<TestInfo> l = new List<TestInfo>();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@userid", userid);                            
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {

                                TestInfo d = new TestInfo();
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.user_id = int.Parse(dataReader["user_id"].ToString());
                                d.StartDated = DateTime.Parse(dataReader["StartDated"].ToString());
                                d.StopDated = DateTime.Parse(dataReader["StopDated"].ToString());
                                d.dish_name = dataReader["dish_name"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.testname = dataReader["testname"].ToString();
                                d.email = dataReader["email"].ToString();
                                d.remark = dataReader["remark"].ToString();
                                d.utensil_name = dataReader["utensil_name"].ToString();
                                d.firstName = dataReader["firstName"].ToString();
                                d.lastName = dataReader["lastName"].ToString();
                                d.guid = dataReader["topazguid"].ToString();
                                d.drpower = float.Parse(dataReader["drpower"].ToString());
                                d.totalTime  = TimeSpan.Parse(dataReader["totaltime"].ToString());
                                d.totalKj = float.Parse(dataReader["totalkj"].ToString());
                                d.compare_reason = dataReader["compare_reason"].ToString();

                                l.Add(d);
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

        public static List<TestInfo> GetMyTestListWithResults(int userid, 
                                                              string orderby, 
                                                              ASCD acdc, 
                                                              int skip, 
                                                              int limit)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {

                        string query = string.Empty;
                        query = @"SELECT * , final_results.totalkj as tkj FROM test
                                    inner join users 
                                    on test.user_id = users.id
                                    inner join final_results on final_results.testid = test.id
                                    where users.id = @userid";

                        orderby = orderby.ToLower();
                        if (orderby == "id") orderby = "test.id ";
                        if (orderby == "date") orderby = "test.StartDated ";
                        if (orderby == "test name") orderby = "test.testname ";


                        if (acdc == ASCD.DESC)
                        {
                            query += " order by " + orderby + "  desc  ";
                        }
                        else
                        {
                            query += " order by " + orderby + "  asc  ";
                        }

                        if (limit > 0)
                            query += "limit " + limit;
                        if (skip > 0)
                            query += "   offset " + skip;

                        List<TestInfo> l = new List<TestInfo>();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@userid", userid);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {

                                TestInfo d = new TestInfo();
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.user_id = int.Parse(dataReader["user_id"].ToString());
                                d.StartDated = DateTime.Parse(dataReader["StartDated"].ToString());
                                d.StopDated = DateTime.Parse(dataReader["StopDated"].ToString());
                                d.dish_name = dataReader["dish_name"].ToString();
                                d.drCycleTime = ushort.Parse(dataReader["drcycletime"].ToString());
                                d.description = dataReader["description"].ToString();
                                d.testname = dataReader["testname"].ToString();
                                d.email = dataReader["email"].ToString();
                                d.remark = dataReader["remark"].ToString();
                                d.utensil_name = dataReader["utensil_name"].ToString();
                                d.firstName = dataReader["firstName"].ToString();
                                d.lastName = dataReader["lastName"].ToString();
                                d.guid = dataReader["topazguid"].ToString();
                                d.drpower = float.Parse(dataReader["drpower"].ToString());
                                d.totalTime = TimeSpan.Parse(dataReader["totaltime"].ToString());
                                d.totalKj = float.Parse(dataReader["totalkj"].ToString());
                                d.compare_reason = dataReader["compare_reason"].ToString();
                                d.fres = new FinalResults();
                                d.fres.FinalDescription = dataReader["FinalDescription"].ToString();
                                bool b = int.TryParse(dataReader["PassFail"].ToString(), out d.fres.PassFail);
                                b = float.TryParse(dataReader["avgfordb"].ToString(), out d.fres.avgfordb);
                                b = float.TryParse(dataReader["avgrefdb"].ToString(), out d.fres.avgrefdb);
                                b = float.TryParse(dataReader["tkj"].ToString(), out d.fres.totalkj);
                                b = float.TryParse(dataReader["totalwatts"].ToString(), out d.fres.totalwatts);
                                 
                                l.Add(d);
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


        public static List<TestInfo2> GetAllMyTestNames(int userid)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        
                        string query = string.Empty;

                        query = @"SELECT *, final_results.totalkj as tkj FROM test 
                                inner join final_results on final_results.testid = test.id
                                where test.user_id = @userid";
                                
                        
                        List<TestInfo2> l = new List<TestInfo2>();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            
                            cmd.Parameters.AddWithValue("@userid", userid);
                            
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                TestInfo2 d = new TestInfo2();
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.user_id = int.Parse(dataReader["user_id"].ToString());
                                d.StartDated = DateTime.Parse(dataReader["StartDated"].ToString());
                                d.StopDated = DateTime.Parse(dataReader["StopDated"].ToString());
                                d.dish_name = dataReader["dish_name"].ToString();
                                d.testname = dataReader["testname"].ToString();
                                bool b = float.TryParse(dataReader["totalkj"].ToString(), out  d.totalkj);                                
                                d.remark = dataReader["remark"].ToString();
                                d.utensil_name = dataReader["utensil_name"].ToString();

                                d.fres = new FinalResults();
                                d.fres.FinalDescription = dataReader["FinalDescription"].ToString();
                                b = int.TryParse(dataReader["PassFail"].ToString(), out d.fres.PassFail);
                                b = float.TryParse(dataReader["avgfordb"].ToString(), out d.fres.avgfordb);
                                b = float.TryParse(dataReader["avgrefdb"].ToString(), out d.fres.avgrefdb);
                                b = float.TryParse(dataReader["tkj"].ToString(), out d.fres.totalkj);
                                b = float.TryParse(dataReader["totalwatts"].ToString(), out d.fres.totalwatts);

                                l.Add(d);
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

        public static void DeleteUtensilName(string name)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"DELETE FROM utensil WHERE utensil_name = @name";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            // Now we can start using the passed values in our parameters:

                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static void UpdateTestResults(int  testid, 
                                             int passfail , 
                                             string desc, 
                                             string enfile , string drfile, string powerfile)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO final_results (testid, DRFileName, EnergyFileName, FinalDescription, PassFail, PowerInfoFileName)
                                         VALUES(@testid, @drfile, @enfile, @desc, @passfail, @PowerInfoFileName)";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@drfile", drfile);
                            cmd.Parameters.AddWithValue("@enfile", enfile);
                            cmd.Parameters.AddWithValue("@desc", desc);
                            cmd.Parameters.AddWithValue("@PowerInfoFileName", powerfile);
                            cmd.Parameters.AddWithValue("@passfail", passfail);

                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void AddNewUtensil(string name)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO utensil (utensil_name)
                                         VALUES(@name)";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            // Now we can start using the passed values in our parameters:

                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void UpdateFinalResultStopTime(int testid)
        {
            lock (m_lock)
            {
               
            try
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {   
                    string query = @"UPDATE test SET 
                                            StopDated = @StopDated where id = " + testid;
                    cn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, cn))
                    {
                        string StopDated = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                        cmd.Parameters.AddWithValue("@StopDated", StopDated);
                        cmd.ExecuteNonQuery();
                    }
                    cn.Close();
                }
            }
            catch (MySqlException err)
            {
                throw (new SystemException(err.Message));
            }
           }
        }
        public static void UpdateEnergyInfoAndTestStop(int testid, 
                                                        float totalkj,
                                                        float totalwatts, 
                                                        float avgfordb, 
                                                        float avgrefdb)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {                    
                    try
                    {
                        string query = @"UPDATE final_results SET 
                           totalkj = @totalkj ,
                           totalwatts = @totalwatts, 
                           avgfordb = @avgfordb,
                           avgrefdb = @avgrefdb
                           where testid = " + testid;
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@avgfordb", avgfordb);
                            cmd.Parameters.AddWithValue("@avgrefdb", avgrefdb);
                            cmd.Parameters.AddWithValue("@totalkj", totalkj);
                            cmd.Parameters.AddWithValue("@totalwatts", totalwatts);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }                     
                }
            }
        }

        public static void UpdateSolutionInfo(DISH_SOLUTION d, 
                                              int userId)
                                            
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    
 
                    try
                    {
                        string query = @"UPDATE dish_solution SET 
                           description = @description ,  utensilname = @utensilName, 
                           totalkj = @totalKj,
                           drpower  = @dr_power,
                           dish_name = @dish_name,
                           totaltime = @totaltime,
                           topaz_guid = @guid,
                           UpdatedBy = @userId, updated=@updated  where name = @sname";
                        cn.Open();
                       
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@dish_name", d.dishName);
                            cmd.Parameters.AddWithValue("@sname", d.name);
                            cmd.Parameters.AddWithValue("@totalKj", d.TotalKj);
                            cmd.Parameters.AddWithValue("@totaltime", d.totalTime);
                            cmd.Parameters.AddWithValue("@dr_power", d.drpower);
                            cmd.Parameters.AddWithValue("@guid", d.guid);
                            cmd.Parameters.AddWithValue("@description", d.description);
                            cmd.Parameters.AddWithValue("@utensilName", d.utensilName);
                            cmd.Parameters.AddWithValue("@userId", userId);
                            string updatedate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                            cmd.Parameters.AddWithValue("@updated", updatedate);

                            // Execute the query
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static Tuple<int, string> GetLastSolution(int userId)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        cn.Open();
                        string query = @"SELECT * FROM lastsolutions
                                        inner join dish_solution
                                        on lastsolutions.sid = dish_solution.id
                                        where lastsolutions.userid = @userId";

                        int sid = -1;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                
                                sid = int.Parse(dataReader["sid"].ToString());
                                string name = dataReader["name"].ToString();
                                Tuple<int, string> t = new Tuple<int, string>(sid, name);
                                cn.Close();
                                return t;
                            }
                        }
                        return null;
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void SaveLastSolution(int sid, int userId)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO lastsolutions (sid, userid)
                                         VALUES(@sid, @userid)";
                                                     
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@userid", userId);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        try
                        {
                            string query1 = @"UPDATE lastsolutions SET sid = @sid
                                         where userid = @userId";

                            using (MySqlCommand cmd = new MySqlCommand(query1, cn))
                            {
                                cmd.Parameters.AddWithValue("@sid", sid);
                                cmd.Parameters.AddWithValue("@userid", userId);
                                cmd.ExecuteNonQuery();
                            }
                            cn.Close();
                        }
                        catch (Exception err1)
                        {
                            throw (new SystemException(err1.Message));
                        }
                    }
                }
            }
        }


        public static void SaveUserChanges(User u)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    // Here we have to create a "try - catch" block, this makes sure your app
                    // catches a MySqlException if the connection can't be opened, 
                    // or if any other error occurs.


                    SHA256Managed crypt = new SHA256Managed();
                    string passwordhash = String.Empty;
                    byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(u.password), 0, Encoding.ASCII.GetByteCount(u.password));
                    foreach (byte theByte in crypto)
                    {
                        passwordhash += theByte.ToString("x2");
                    }

                    try
                    {
                        string query = "UPDATE users SET email = @email , typeofuser = @typeofuser ,  firstname = @firstname , lastname = @lastname ,phone = @phone , active = @active    where id = " + u.ID;
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            // Now we can start using the passed values in our parameters:

                            cmd.Parameters.AddWithValue("@email", u.email);
                            cmd.Parameters.AddWithValue("@firstname", u.firstName);
                            cmd.Parameters.AddWithValue("@lastname", u.lastName);
                            cmd.Parameters.AddWithValue("@phone", u.phoneNumber);
                            cmd.Parameters.AddWithValue("@active", u.active);

                            // Execute the query
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void SuspendUser(int id, bool suspend)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                   
                    try
                    {
                        string query = "UPDATE users SET active = @active where id = @id";

                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            int active = suspend == true ? 0 : 1;
                            cmd.Parameters.AddWithValue("@active", active);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void CreateNewUser(User u)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    // Here we have to create a "try - catch" block, this makes sure your app
                    // catches a MySqlException if the connection can't be opened, 
                    // or if any other error occurs.


                    SHA256Managed crypt = new SHA256Managed();
                    string passwordhash = String.Empty;
                    byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(u.password), 0, Encoding.ASCII.GetByteCount(u.password));
                    foreach (byte theByte in crypto)
                    {
                        passwordhash += theByte.ToString("x2");
                    }

                    try
                    {
                        string query = @"INSERT INTO users (email, hashpassword, firstname, lastname, phonenumber, active)
                        VALUES (@email, @hashpassword,@firstname, @lastname, @phone, @active);";

                        cn.Open();

                        // Yet again, we are creating a new object that implements the IDisposable
                        // interface. So we create a new using statement.

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@email", u.email);
                            cmd.Parameters.AddWithValue("@hashpassword", passwordhash);
                            cmd.Parameters.AddWithValue("@firstname", u.firstName);
                            cmd.Parameters.AddWithValue("@lastname", u.lastName);
                            cmd.Parameters.AddWithValue("@phone", u.phoneNumber);
                            cmd.Parameters.AddWithValue("@active", u.active);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void UpdateSolution(string sname,
                                          float totalKj,
                                          float drpower,
                                          TimeSpan totaltime,
                                          int userId,
                                          List<SolutionAlgo> list, 
                                          string guid, 
                                          int cycletime, 
                                          int groupid)
        {
            int newId = -1;
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                                     
                    try
                    {
                        string query = @"UPDATE dish_solution SET updatedBy = @userId , updated = @ndate,
                                                                  totalkj = @totalKj , totaltime = @totaltime ,
                                                                  drcycletime = @cycletime,
                                                                  drpower=@drpower , 
                                                                  groupid = @groupid  ,
                                                                  topaz_guid = @guid
                                                                  WHERE name = @sname";

                        cn.Open();

                        string ndate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

                     
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@groupid", groupid);
                            cmd.Parameters.AddWithValue("@cycletime", cycletime);
                            cmd.Parameters.AddWithValue("@guid", guid);
                            cmd.Parameters.AddWithValue("@drpower", drpower);
                            cmd.Parameters.AddWithValue("@totaltime", totaltime);
                            cmd.Parameters.AddWithValue("@totalKj", totalKj);
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@sname", sname);
                            cmd.Parameters.AddWithValue("@ndate", ndate);
                            cmd.ExecuteNonQuery();
                        }

                        query = @"SELECT id FROM dish_solution WHERE name = @sname";

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sname", sname);

                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            newId = -1;
                            while (dataReader.Read())
                            {
                                newId = int.Parse(dataReader["id"].ToString());
                            }
                        }
                        if (newId == -1)
                        {
                            throw (new SystemException("Error , cannot find Id for solution_algorithems"));
                        }

                        cn.Close();
                        cn.Open();

                        query = @"DELETE FROM solution_algorithems WHERE sid = @newId";
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@newId", newId);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();

                        
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }

                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        cn.Open();

                        string query = @"INSERT INTO solution_algorithems (sid, algoname, time, maxpower, rowindex, kj , absorbed)
                        VALUES (@sid, @algoname, @time, @maxpower, @rowindex,@kj , @absorbed)";

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            foreach (SolutionAlgo t in list)
                            {
                                cmd.Parameters.AddWithValue("@sid", newId);
                                cmd.Parameters.AddWithValue("@algoname", t.algoname);
                                cmd.Parameters.AddWithValue("@time", t.time.ToString());
                                cmd.Parameters.AddWithValue("@maxpower", t.maxpower);
                                cmd.Parameters.AddWithValue("@rowindex", t.rowindex);
                                cmd.Parameters.AddWithValue("@kj", t.kj);
                                cmd.Parameters.AddWithValue("@absorbed", t.absorbed);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }

                        cn.Close();
                    }
                    catch (Exception err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int UpdateSolution(string sname,
                                          string description,
                                          string utensilname,
                                          string dish_name,
                                          int createdby,                                          
                                          float totalKj,
                                          TimeSpan totaltime,
                                          float drpower,
                                          string guid,
                                          List<SolutionAlgo> list)
        {
            int newId = -1;
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {


                    int sid = -1;
                    try
                    {

                        string query = @"select id from dish_solution where dish_name = @dish_name limit 1";

                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@dish_name", dish_name);
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                           
                            while (dataReader.Read())
                            {
                                sid = int.Parse(dataReader["id"].ToString());
                            }

                        }
                        cn.Close();


                        query = @"UPDATE dish_solution SET updatedBy = @userId , 
                                                            updated = @ndate,
                                                            totalkj = @totalKj , 
                                                            totaltime = @totaltime , 
                                                            drpower=@drpower , 
                                                            Dated = @datednow,
                                                            topaz_guid = @guid
                                                            WHERE id = @sid";

                        cn.Open();

                        string ndate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");


                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@guid", guid);
                            cmd.Parameters.AddWithValue("@drpower", drpower);
                            cmd.Parameters.AddWithValue("@totaltime", totaltime);
                            cmd.Parameters.AddWithValue("@totalKj", totalKj);
                            cmd.Parameters.AddWithValue("@userId", createdby);
                            cmd.Parameters.AddWithValue("@sname", sname);
                            cmd.Parameters.AddWithValue("@datednow", ndate);
                            cmd.Parameters.AddWithValue("@ndate", ndate);
                            cmd.ExecuteNonQuery();
                        }

                        query = @"SELECT id FROM dish_solution WHERE name = @sname";

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sname", sname);

                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            newId = -1;
                            while (dataReader.Read())
                            {
                                newId = int.Parse(dataReader["id"].ToString());
                            }
                        }
                        if (newId == -1)
                        {
                            throw (new SystemException("Error , cannot find Id for solution_algorithems"));
                        }

                        cn.Close();
                        cn.Open();

                        query = @"DELETE FROM solution_algorithems WHERE sid = @newId";
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@newId", newId);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();


                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }

                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        cn.Open();

                        string query = @"INSERT INTO solution_algorithems (sid, algoname, time, maxpower, rowindex, kj , absorbed)
                        VALUES (@sid, @algoname, @time, @maxpower, @rowindex,@kj , @absorbed)";

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            foreach (SolutionAlgo t in list)
                            {
                                cmd.Parameters.AddWithValue("@sid", newId);
                                cmd.Parameters.AddWithValue("@algoname", t.algoname);
                                cmd.Parameters.AddWithValue("@time", t.time.ToString());
                                cmd.Parameters.AddWithValue("@maxpower", t.maxpower);
                                cmd.Parameters.AddWithValue("@rowindex", t.rowindex);
                                cmd.Parameters.AddWithValue("@kj", t.kj);
                                cmd.Parameters.AddWithValue("@absorbed", t.absorbed);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (Exception err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
         

        public static List<SolutionAlgo> GetTestSolutionAlgorithems(int testid)
        {
             
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                                     
                    try
                    {

                        string query = @"SELECT * FROM topazdb.test_algo
                                        where testid = @testid 
                                        order by rowindex ";

                        cn.Open();

                        List<SolutionAlgo> list = new List<SolutionAlgo>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testid", testid);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                SolutionAlgo d = new SolutionAlgo();

                                d.absorbed = float.Parse(dataReader["absorbed"].ToString());
                                d.algoname = dataReader["algoname"].ToString();
                                d.kj = float.Parse(dataReader["kj"].ToString());
                                d.maxpower = float.Parse(dataReader["maxpower"].ToString());
                                d.rowindex = int.Parse(dataReader["rowindex"].ToString());
                                d.id = int.Parse(dataReader["testid"].ToString());
                                d.time = TimeSpan.Parse(dataReader["time"].ToString());
                                list.Add(d);
                            }
                        }
                        cn.Close();
                        return list;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }

        public static List <SolutionAlgo> GetSolutionAlgorithems(int solutionid)
        {
             
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                                     
                    try
                    {
                    
                        string query = @"SELECT * FROM topazdb.solution_algorithems
                                        inner join dish_solution
                                        on dish_solution.id = solution_algorithems.sid
                                        where sid = @solutionid";

                        cn.Open();

                        List<SolutionAlgo> list = new List<SolutionAlgo>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@solutionid", solutionid);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                SolutionAlgo d = new SolutionAlgo();

                                d.absorbed = float.Parse(dataReader["absorbed"].ToString());
                                d.algoname = dataReader["algoname"].ToString();
                                d.kj = float.Parse(dataReader["kj"].ToString());
                                d.maxpower = float.Parse(dataReader["maxpower"].ToString());
                                d.rowindex = int.Parse(dataReader["rowindex"].ToString());
                                d.id = int.Parse(dataReader["sid"].ToString());
                                d.time = TimeSpan.Parse(dataReader["time"].ToString());
                                list.Add(d);
                            }
                        }
                        cn.Close();
                        return list;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        } 

        public static int SaveSolution(string sname, 
                                        string desc,
                                        string utensil,
                                        string dishName,
                                        int userId,
                                        List<SolutionAlgo> list,
                                        TimeSpan totaltime, 
                                        float totalkj,
                                        float drpower,
                                        string guid, 
                                        int drCycleTime,
                                        int groupid)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                                     
                    try
                    {
                        string query = @"INSERT INTO dish_solution (name, createdBy, updated, updatedBy, Dated, description, utensilname, dish_name, totaltime, totalkj,topaz_guid,drpower, drcycletime, groupid)
                        VALUES (@sname, @userid, @updated, @updatedBy , @ndate, @desc, @utensilname, @dish_name, @totaltime, @totalkj,@topaz_guid,@drpower, @drcycletime, @groupid)";

                        cn.Open();

                        string ndate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

                        int newId = -1;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@groupid", groupid);
                            cmd.Parameters.AddWithValue("@drpower", drpower);
                            cmd.Parameters.AddWithValue("@dish_name", dishName);
                            cmd.Parameters.AddWithValue("@sname", sname);
                            cmd.Parameters.AddWithValue("@userid", userId);

                            cmd.Parameters.AddWithValue("@drcycletime", drCycleTime);
                            cmd.Parameters.AddWithValue("@totaltime", totaltime);
                            cmd.Parameters.AddWithValue("@totalkj", totalkj);

                            cmd.Parameters.AddWithValue("@utensilname", utensil);
                            cmd.Parameters.AddWithValue("@updatedBy", userId);
                            cmd.Parameters.AddWithValue("@updated", ndate);

                            if (desc == null)
                            {
                                desc = string.Empty;
                            }
                            cmd.Parameters.AddWithValue("@desc", desc);
                            cmd.Parameters.AddWithValue("@ndate", ndate);
                            cmd.Parameters.AddWithValue("@topaz_guid", guid);
                           

                            newId = cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }

                        cn.Close();
                        cn.Open();

                        query = @"INSERT INTO solution_algorithems (sid, algoname, time, maxpower , rowindex, kj, absorbed)
                        VALUES (@sid, @algoname1, @time1, @maxpower1, @rowindex1 ,@kj1 , @absorbed1)";


                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandTimeout = 5000;
                            foreach (SolutionAlgo t in list)
                            {
                                cmd.Parameters.AddWithValue("@sid", newId);
                                cmd.Parameters.AddWithValue("@algoname1", t.algoname);
                                cmd.Parameters.AddWithValue("@time1", t.time);
                                cmd.Parameters.AddWithValue("@maxpower1", t.maxpower);
                                cmd.Parameters.AddWithValue("@rowindex1", t.rowindex);
                                cmd.Parameters.AddWithValue("@kj1", t.kj);
                                cmd.Parameters.AddWithValue("@absorbed1", t.absorbed);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }

                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int UpdateTestAsSolution(string sname,
                                            string desc,
                                            string utensil,
                                            string dishName,
                                            int userId,
                                            List<SolutionAlgo> list,
                                            TimeSpan totaltime,
                                            float totalkj,
                                            float drpower,
                                            string guid)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {


                        string q = @"select id from dish_solution where name = @sname";
                        cn.Open();
                        int sid = -1;
                        using (MySqlCommand cmd = new MySqlCommand(q, cn))
                        {
                            cmd.Parameters.AddWithValue("@sname", sname);
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.Read())
                            {
                                sid = int.Parse(dataReader["id"].ToString());
                            }
                        }

                        cn.Close();
                        cn.Open();
                        string query = @"UPDATE  dish_solution SET 
                                        updated = @updated,
                                        updatedBy = @updatedBy, 
                                        Dated = @ndate, 
                                        description = @desc, 
                                        utensilname =  @utensilname, 
                                        dish_name = @dish_name, 
                                        totaltime = @totaltime, 
                                        totalkj = @totalkj,
                                        topaz_guid = @topaz_guid,
                                        drpower = @drpower
                                        WHERE name=@sname and createdBy = @userid";
                        

                        string ndate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

                        
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@drpower", drpower);
                            cmd.Parameters.AddWithValue("@dish_name", dishName);
                            cmd.Parameters.AddWithValue("@sname", sname);
                            cmd.Parameters.AddWithValue("@userid", userId);

                            cmd.Parameters.AddWithValue("@totaltime", totaltime);
                            cmd.Parameters.AddWithValue("@totalkj", totalkj);

                            cmd.Parameters.AddWithValue("@utensilname", utensil);
                            cmd.Parameters.AddWithValue("@updatedBy", userId);
                            cmd.Parameters.AddWithValue("@updated", ndate);

                            if (desc == null)
                            {
                                desc = string.Empty;
                            }
                            cmd.Parameters.AddWithValue("@desc", desc);
                            cmd.Parameters.AddWithValue("@ndate", ndate);
                            cmd.Parameters.AddWithValue("@topaz_guid", guid);


                            cmd.ExecuteNonQuery();
                          
                        }

                        cn.Close();


                        cn.Open();

                        query = @"DELETE FROM solution_algorithems WHERE sid = @sid";
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();


                        cn.Open();



                        query = @"INSERT INTO solution_algorithems (sid, algoname, time, maxpower , rowindex, kj, absorbed)
                        VALUES (@sid, @algoname1, @time1, @maxpower1, @rowindex1 ,@kj1 , @absorbed1)";


                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandTimeout = 5000;
                            foreach (SolutionAlgo t in list)
                            {
                                cmd.Parameters.AddWithValue("@sid", sid);
                                cmd.Parameters.AddWithValue("@algoname1", t.algoname);
                                cmd.Parameters.AddWithValue("@time1", t.time);
                                cmd.Parameters.AddWithValue("@maxpower1", t.maxpower);
                                cmd.Parameters.AddWithValue("@rowindex1", t.rowindex);
                                cmd.Parameters.AddWithValue("@kj1", t.kj);
                                cmd.Parameters.AddWithValue("@absorbed1", t.absorbed);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }

                        cn.Close();
                        return sid;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static  DISH_SOLUTION  GetSolutionInfo(string sname, string dishName, int userid)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        if (dishName == "ALL")
                        {
                            query = @"SELECT * FROM dish_solution 
                                          inner join users 
                                          on users.id = dish_solution.createdBy
                                          left join test_group_names on test_group_names.id = dish_solution.groupid
                                          where name=@sname and createdBy=@userid";
                        }
                        else
                        {
                            query = @"SELECT * FROM dish_solution 
                                          inner join users 
                                          on users.id = dish_solution.createdBy
                                          left join test_group_names on test_group_names.id = dish_solution.groupid
                                          where name=@sname and createdBy=@userid and dish_name=@dishName";
                        }
                        DISH_SOLUTION d = new DISH_SOLUTION();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sname", sname);
                            cmd.Parameters.AddWithValue("@userid", userid);
                            if (dishName  != "ALL")
                            {
                                cmd.Parameters.AddWithValue("@dishName", dishName);
                            }
                            MySqlDataReader dataReader = cmd.ExecuteReader();
                          
                            while (dataReader.Read())
                            {
                                bool b = int.TryParse(dataReader["groupid"].ToString(), out d.groupid);
                                if (b == false)
                                    d.groupid = 1;
                                d.groupName = dataReader["groupName"].ToString();
                                d.dishName = dataReader["dish_name"].ToString();
                                d.name = dataReader["name"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.utensilName = dataReader["utensilname"].ToString();
                                d.totalTime = TimeSpan.Parse(dataReader["totaltime"].ToString());
                                d.utensilName = dataReader["utensilname"].ToString();
                                d.TotalKj = float.Parse(dataReader["totalkj"].ToString());
                                d.createdBy = int.Parse(dataReader["createdBy"].ToString());
                                d.Dated = dataReader["Dated"].ToString();
                                d.drpower = float.Parse(dataReader["drpower"].ToString());
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.guid = dataReader["topaz_guid"].ToString();
                                d.lastname = dataReader["lastname"].ToString();
                                d.firstname = dataReader["firstname"].ToString();
                                d.updated = dataReader["updated"].ToString();
                                d.drCycleTime = ushort.Parse(dataReader["drcycletime"].ToString());
                                    

                                break;
                            }
                        }
                        cn.Close();
                        return d;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static DISH_SOLUTION GetSolutionInfo(int sid)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT *, dish_solution.id as did  
                                FROM dish_solution 
                                inner join users
                                on dish_solution.createdBy = users.id
                                where dish_solution.id=@sid";

                        DISH_SOLUTION d = new DISH_SOLUTION();
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                d.id = int.Parse(dataReader["did"].ToString());
                                d.name = dataReader["name"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.utensilName = dataReader["utensilname"].ToString();
                                d.dishName = dataReader["dish_name"].ToString();                                                                
                                d.createdBy = int.Parse(dataReader["createdBy"].ToString());
                                d.firstname = dataReader["firstname"].ToString();
                                d.lastname = dataReader["lastname"].ToString();
                                d.Dated = dataReader["Dated"].ToString();
                                d.guid = dataReader["topaz_guid"].ToString();
                                d.TotalKj = float.Parse(dataReader["totalkj"].ToString());
                                d.totalTime = TimeSpan.Parse(dataReader["totalTime"].ToString());
                                d.drpower = float.Parse(dataReader["drpower"].ToString());

                                break;
                            }
                        }
                        cn.Close();
                        return d;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

            
        public static List<string> GetUtensilList()
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = "SELECT utensil_name FROM utensil";

                        cn.Open();
                        List<string> list = new List<string>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                string name = dataReader["utensil_name"].ToString();
                                list.Add(name);
                            }
                        }
                        cn.Close();
                        return list;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static List<SolutionAlgo> LoadSolutions(int solutionId)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = "SELECT * FROM solution_algorithems WHERE sid = @solutionId";
                         
                        cn.Open();
                        List<SolutionAlgo> list = new List<SolutionAlgo>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@solutionId", solutionId);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                TimeSpan time = TimeSpan.Parse(dataReader["time"].ToString());
                                float maxpower = float.Parse(dataReader["maxpower"].ToString());
                                int rowindex = int.Parse(dataReader["rowindex"].ToString());
                                float kj = float.Parse(dataReader["kj"].ToString());
                                float absorbed = float.Parse(dataReader["absorbed"].ToString());

                                SolutionAlgo t =
                                    new SolutionAlgo(dataReader["algoname"].ToString(), time, maxpower, rowindex, kj, absorbed);
                                t.id = int.Parse(dataReader["sid"].ToString());
                                list.Add(t);
                            }
                        }
                        cn.Close();
                        return list;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void CheckAllDRThresholdParametersAreSet(int sid)
                                                                                                    
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"select * from algo_drthreshold_params
                                right join solution_algorithems
                                on algo_drthreshold_params.sid = solution_algorithems.sid and 
                                algo_drthreshold_params.rowindex = solution_algorithems.rowindex 
                                where solution_algorithems.sid = @sid 
                                order by  solution_algorithems.rowindex";

                        cn.Open();

                         
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            int index = 0;
                            while (dataReader.Read())
                            {
                                try
                                {
                                    int rowindex = int.Parse(dataReader["RowIndex"].ToString());
                                    index++;
                                }
                                catch (Exception err)
                                {
                                    throw (new SystemException("Configuration parameters for row " + (index + 1) + " is missing"));
                                }                                    
                            }
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }



        public static List<SolutionAlgo> LoadSolutions(string sname, out int solutionId)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT id  FROM dish_solution
                                 where name= @sname";

                        cn.Open();

                        solutionId = -1;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sname", sname);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                solutionId = int.Parse(dataReader["id"].ToString()); 
                            }
                            if (solutionId == -1)
                            {
                                throw (new SystemException("error 1123"));
                            }
                        }
                        cn.Close();

                        cn.Open();

                        query = "SELECT * FROM solution_algorithems WHERE sid = @solutionId";

                        List<SolutionAlgo> list = new List<SolutionAlgo>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@solutionId", solutionId);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                TimeSpan time = TimeSpan.Parse(dataReader["time"].ToString());
                                float maxpower = float.Parse(dataReader["maxpower"].ToString());
                                int rowindex = int.Parse(dataReader["rowindex"].ToString());
                                float kj = float.Parse(dataReader["kj"].ToString());
                                float absorbed = float.Parse(dataReader["absorbed"].ToString());

                                SolutionAlgo t =
                                    new SolutionAlgo(dataReader["algoname"].ToString(), time, maxpower, rowindex, kj, absorbed);
                                list.Add(t);
                            }
                        }
                        cn.Close();
                        return list;
                    }
                    catch (MySqlException err)
                    {
                        solutionId = -1;
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static Dictionary<string, DISH_SOLUTION> getSolutionsList(int userId, bool mysolution, string dishName, string guid)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        if (mysolution == false)
                        {
                            if (dishName == "ALL")
                            {
                                if (guid == string.Empty)
                                    query = "SELECT * FROM dish_solution";
                                else
                                    query = "SELECT * FROM dish_solution where topaz_guid = @guid";
                            }
                            else
                            {
                                if (guid == string.Empty)
                                    query = "SELECT * FROM dish_solution where dish_name = @dishName";
                                else
                                    query = "SELECT * FROM dish_solution where dish_name = @dishName  and topaz_guid = @guid";
                            }
                        }
                        else
                        {
                            if (dishName == "ALL")
                            {
                                if (guid == string.Empty)
                                    query = "SELECT * FROM dish_solution WHERE createdBy = @userid";
                                else
                                    query = "SELECT * FROM dish_solution WHERE createdBy = @userid and topaz_guid = @guid";
                            }
                            else
                            {
                                if (guid == string.Empty)
                                    query = "SELECT * FROM dish_solution WHERE createdBy = @userid and dish_name = @dishName";
                                else
                                    query = "SELECT * FROM dish_solution WHERE createdBy = @userid and dish_name = @dishName and topaz_guid = @guid";
                            }
                        }

                        cn.Open();
                        Dictionary<string, DISH_SOLUTION> list = new Dictionary<string, DISH_SOLUTION>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            if (guid != string.Empty)
                            {
                                cmd.Parameters.AddWithValue("@guid", guid);
                            }

                            cmd.Parameters.AddWithValue("@userid", userId);
                            if (dishName != "ALL")
                            {
                                cmd.Parameters.AddWithValue("@dishName", dishName);
                            }
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                DISH_SOLUTION d = new DISH_SOLUTION();
                                d.id = int.Parse(dataReader["id"].ToString());
                                d.name = dataReader["name"].ToString();
                                d.dishName = dataReader["dish_name"].ToString();
                                d.Dated = dataReader["Dated"].ToString();
                                d.utensilName = dataReader["utensilname"].ToString();
                                d.createdBy = int.Parse(dataReader["createdBy"].ToString());
                                d.groupid = 1;
                                bool b = int.TryParse(dataReader["groupid"].ToString(), out d.groupid);
                                if (b == false)
                                    d.groupid = 1;
                                d.description = dataReader["description"].ToString();
                                d.updatedBy = int.Parse(dataReader["updatedBy"].ToString());
                                d.updated = dataReader["updated"].ToString();

                                d.totalTime = TimeSpan.Parse(dataReader["totalTime"].ToString());
                                d.TotalKj = float.Parse(dataReader["TotalKj"].ToString());
                                d.drpower = float.Parse(dataReader["drpower"].ToString());
                                list.Add(dataReader["name"].ToString(), d);

                                 


                            }
                        }
                        cn.Close();
                        return list;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
        public static List<User> GetAllUsers()
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {

                        string query = "SELECT * FROM users";
                        cn.Open();
                        List<User> U = new List<User>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                User u = new User();
                                u.ID = int.Parse(dataReader["ID"].ToString());
                                u.email = dataReader["email"].ToString();
                                u.firstName = dataReader["firstname"].ToString();
                                u.lastName =  dataReader["lastname"].ToString();
                                u.phoneNumber = dataReader["phonenumber"].ToString();
                                if (int.Parse(dataReader["active"].ToString()) == 1)
                                    u.active = true;
                                else
                                    u.active = false;
                                U.Add(u);
                            }
                        }
                        cn.Close();
                        return U;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int CheckAuthtintication(string email, string password)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    

                    try
                    {

                        SHA256Managed crypt = new SHA256Managed();
                        string passwordhash = String.Empty;
                        byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(password), 0, Encoding.ASCII.GetByteCount(password));
                        foreach (byte theByte in crypto)
                        {
                            passwordhash += theByte.ToString("x2");
                        }


                        string query = "SELECT ID FROM users where email = @email and hashpassword = @password";

                        cn.Open();


                        int ID = -1;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@password", passwordhash);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            //Read the data and store them in the list
                            while (dataReader.Read())
                            {
                                ID = int.Parse(dataReader["ID"].ToString());
                            }
                        }
                        cn.Close();
                        return ID;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int GetUserID(string email)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {


                    try
                    {
                        string query = "SELECT ID FROM users where email= @email";

                        cn.Open();

                        int ID = -1;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@email", email);



                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            //Read the data and store them in the list
                            while (dataReader.Read())
                            {
                                ID = int.Parse(dataReader["ID"].ToString());
                            }
                        }
                        cn.Close();
                        return ID;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static TimeSpan GetTotalWorkTime()
        {

            lock (m_lock)
            {

        
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {

                        int hour;
                        int days;
                        int minute;
                        int id;
                        int seconds;

                        string query = "SELECT * FROM total_work";

                        cn.Open();

                        bool IsExist;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            //Read the data and store them in the list
                            IsExist = dataReader.Read();
                            if (IsExist)
                            {
                                hour = int.Parse(dataReader["hour"].ToString());
                                minute = int.Parse(dataReader["minute"].ToString());
                                seconds = int.Parse(dataReader["seconds"].ToString());
                                days = int.Parse(dataReader["days"].ToString());
                                id = int.Parse(dataReader["ID"].ToString());

                                TimeSpan t = new TimeSpan(days, hour, minute, seconds);
                                cn.Close();
                                return t;
                            }
                            else
                            {
                                return new TimeSpan(0, 0, 0, 0);
                            }                          
                        }
                    }
                    catch (Exception err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
        public static int UpdateTotalWork(DateTime start, DateTime stop)
        {
            lock (m_lock)
            {

                TimeSpan timeToAdd = stop - start;

                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {

                        int hour ;
                        int days ;
                        int minute;
                        int id;
                        int seconds; 

                        string query = "SELECT * FROM total_work";

                        cn.Open();

                        bool IsExist;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            //Read the data and store them in the list
                            IsExist = dataReader.Read();
                            if (IsExist)
                            {
                                hour = int.Parse(dataReader["hour"].ToString());
                                minute = int.Parse(dataReader["minute"].ToString());
                                seconds = int.Parse(dataReader["seconds"].ToString());
                                days = int.Parse(dataReader["days"].ToString());
                                id = int.Parse(dataReader["ID"].ToString());


                                TimeSpan t = new TimeSpan(days, hour, minute, seconds);
                                timeToAdd = t + timeToAdd;

                            }
                            cn.Close();
                        }

                        cn.Open();

                        if (IsExist == false)
                        {
                            query = "INSERT INTO total_work (hour, minute, days,seconds, id) VALUES (@hour, @minute,@days, @seconds, @id)";
                        }
                        else
                        {
                            query = @"UPDATE total_work  SET days = @days, hour = @hour, minute = @minute, seconds = @seconds WHERE ID = @id";
                        }
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            // Here we already start using parameters in the query to prevent
                            // SQL injection.
                          

                       

                            // Yet again, we are creating a new object that implements the IDisposable
                            // interface. So we create a new using statement.

                            hour = timeToAdd.Hours;
                            days = timeToAdd.Days;
                            minute = timeToAdd.Minutes;
                            seconds = timeToAdd.Seconds;


                            Int32 newId = 0;

                            // Now we can start using the passed values in our parameters:

                            cmd.Parameters.AddWithValue("@hour", hour);
                            cmd.Parameters.AddWithValue("@minute", minute);
                            cmd.Parameters.AddWithValue("@days", days);
                            cmd.Parameters.AddWithValue("@seconds", seconds);
                            cmd.Parameters.AddWithValue("@id", 1);

                            //newId = (Int32)cmd.ExecuteScalar();
                            // Execute the query
                            newId = cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;

                            cn.Close();
                            return newId;
                        }                        
                         
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static void UpdateTestRemarkAndName(int testId, string testRemark , string testName, string compareReason , int compareTo)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"UPDATE test SET testname = @testName  , 
                                        compare_reason = @compareReason,
                                        compareto = @compareTo,
                                        remark = @testRemark
                                        WHERE id = @testId";

                        cn.Open();
                       
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testName", testName);
                            cmd.Parameters.AddWithValue("@testRemark", testRemark);
                            cmd.Parameters.AddWithValue("@testId", testId);
                            cmd.Parameters.AddWithValue("@compareTo", compareTo);
                            cmd.Parameters.AddWithValue("@compareReason", compareReason);
                            cmd.ExecuteNonQuery();

                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {/*
                        if (err.Message.Contains("Duplicate entry") == true)
                        {
                            string query = @"DELETE FROM test
                                           WHERE id = @testId";

                            using (MySqlCommand cmd = new MySqlCommand(query, cn))
                            {
                                cmd.Parameters.AddWithValue("@testId", testId);
                                cmd.ExecuteNonQuery();
                            }
                            cn.Close();
                        }*/
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }

        public static void DeleteTestById(int testId)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"SET SQL_SAFE_UPDATES = 0; 
                                        DELETE FROM  test WHERE id = @testId";

                        cn.Open();
                        
                        int newId = -1;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testId", testId);
                             
                            newId = cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void UpdateTestStartTime(int testid)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"UPDATE test SET StartDated = @startDate 
                                         WHERE id = @testid";

                        cn.Open();
                        string startDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");                                          
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@startDate", startDate);
                            cmd.ExecuteNonQuery();                            
                        }
                        cn.Close();
                        return;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int AddNewTest(string testname,  
                                     int uid, string utensil_name, 
                                     string remark, out string _startDate,
                                     DISH_SOLUTION solution)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO test (testname, user_id, StartDated, 
                                         StopDated, utensil_name, remark, dish_name, drpower,totaltime, 
                                         description, topazguid, totalkj, groupid,drcycletime)
                        VALUES (@testname, @user_id, 
                                @StartDated, @StopDated, @utensil_name, @remark, 
                                @dish_name, @drpower,@totaltime, @description, @guid, @TotalKj, @groupid,@drcycletime)";


                        cn.Open();

                        DateTime date = DateTime.Now;
                        string startDate = date.ToString("yyyy-MM-dd H:mm:ss");
                        _startDate = date.ToString("yyyy-MM-dd");                        

                        int newId = -1;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@groupid", solution.groupid);
                            cmd.Parameters.AddWithValue("@drcycletime", solution.drCycleTime);
                            cmd.Parameters.AddWithValue("@guid", solution.guid);
                            cmd.Parameters.AddWithValue("@testname", testname);
                            cmd.Parameters.AddWithValue("@user_id", uid);
                            cmd.Parameters.AddWithValue("@StartDated", startDate);
                            cmd.Parameters.AddWithValue("@StopDated", startDate);
                            cmd.Parameters.AddWithValue("@utensil_name", utensil_name);
                            cmd.Parameters.AddWithValue("@remark", remark);


                            cmd.Parameters.AddWithValue("@dish_name", solution.dishName);
                            cmd.Parameters.AddWithValue("@drpower", solution.drpower);
                            cmd.Parameters.AddWithValue("@totaltime", solution.totalTime);
                            cmd.Parameters.AddWithValue("@TotalKj", solution.TotalKj);
                            cmd.Parameters.AddWithValue("@description", solution.description);


                            newId = cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;
                        }                         
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static int AddSolutionAlgoToTest(int testID, List<SolutionAlgo> solutionAlgo)
        {
            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query = @"INSERT INTO test_algo (testid,absorbed, algoname, kj, maxpower,time,rowindex)
                                        VALUES (@testid,@absorbed, @algoname, @kj, @maxpower,@time,@rowindex)";

                        cn.Open();

                       
                        int newId = -1;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            foreach (SolutionAlgo t in solutionAlgo)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@absorbed", t.absorbed);
                                cmd.Parameters.AddWithValue("@algoname", t.algoname);
                                cmd.Parameters.AddWithValue("@kj", t.kj);
                                cmd.Parameters.AddWithValue("@maxpower", t.maxpower);
                                cmd.Parameters.AddWithValue("@testid", t.id);
                                cmd.Parameters.AddWithValue("@time", t.time);
                                cmd.Parameters.AddWithValue("@rowindex", t.rowindex);
                                newId = cmd.ExecuteNonQuery();
                                newId = (Int32)cmd.LastInsertedId;
                            }
                        }
                        cn.Close();
                        return newId;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }


        public static void DeletePictureFromTest(int testId , bool before , string filename)
        {

            lock (m_lock)
            {

                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                     
                    try
                    {
                        string query;
                        if (before)
                            query = @"DELETE FROM test_pictures_before WHERE testid = @testId and filename = @filename" ;
                        else
                            query = @"DELETE FROM test_pictures_after WHERE testid = @testId and filename = @filename";
                       
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testId", testId);
                            cmd.Parameters.AddWithValue("@filename", filename);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();                       
                    }
                    catch (Exception err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }

        public static List<PictureInfo> GetPictures(int testId, bool before)
        {
            lock (m_lock)
            {


                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query;
                        if (before == true)
                            query = "SELECT * FROM test_pictures_before where testid = @testId";
                        else
                            query = "SELECT * FROM test_pictures_after where testid = @testId";

                        cn.Open();

                        List<PictureInfo> l = new List<PictureInfo>();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@testId", testId);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                PictureInfo d = new PictureInfo();
                                d.testid = int.Parse(dataReader["testid"].ToString());
                                d.dbName = dataReader["filename"].ToString();
                                d.description = dataReader["description"].ToString();
                                d.title =  dataReader["picname"].ToString();
                                d.compip = dataReader["compip"].ToString();
                                d.path = dataReader["savepath"].ToString();
                                d.fullname = d.path + "\\" + d.dbName;
                                l.Add(d);
                            }
                        }
                        cn.Close();
                        return l;
                    }
                    catch (Exception err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void DeleteSolutionPicture(int sid)
        {
            lock (m_lock)
            {
                SolutionPictureInfo? d = null;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query;
                        query = "DELETE FROM solution_pictures where sid = @sid";

                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (Exception err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }

        }
        public static void UpdateShowHideSolutionPicture(int sid , bool show)
        {

            lock (m_lock)
            {
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query;

                        query = @"UPDATE  solution_pictures
                                        SET showhide = @show 
                                        where sid = @sid";


                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@show", show == true ? 1 : 0);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
        public static SolutionPictureInfo ? GetSolutionPicture(int sid)
        {
            lock (m_lock)
            {
                SolutionPictureInfo ?d = null;            
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query;
                        query = "SELECT * FROM solution_pictures where sid = @sid";

                        cn.Open();
                        
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                SolutionPictureInfo d1 = new SolutionPictureInfo();    
                                d1.fullpicname = dataReader["fullpicname"].ToString();
                                d1.show = int.Parse(dataReader["showhide"].ToString()) == 1 ? true : false;
                                d = d1;
                            }
                        }
                        cn.Close();
                        return d;
                    }
                    catch (Exception err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static List<ChefDemoDishes> GetChefDemoList(string demoname)
        {
            using (MySqlConnection cn = new MySqlConnection(myConnectionString))
            {
                    
                try
                {

                    cn.Open();

                    string query = @"SELECT * FROM topazdb.masterchef
                                    natural join solution_pictures
                                    inner join test
                                    on test.id = masterchef.testid
                                    inner join final_results  on final_results.testid = test.id
                                    inner join dish_names  on dish_names.dish_name = test.dish_name
                                    where groupname = @mydemo";


                    List<ChefDemoDishes> list = new List<ChefDemoDishes>();
                    using (MySqlCommand cmd = new MySqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@mydemo", demoname);
                        MySqlDataReader dataReader = cmd.ExecuteReader();

                       
                       while (dataReader.Read())
                       {
                            ChefDemoDishes d = new ChefDemoDishes();
                            
                            d.groupname = dataReader["groupname"].ToString();
                            d.testid = int.Parse(dataReader["testid"].ToString());
                            d.testName = dataReader["testname"].ToString();
                            d.totaltime = TimeSpan.Parse(dataReader["totaltime"].ToString());
                            d.picture1 = dataReader["picture1"].ToString();
                            d.picture2 = dataReader["picture2"].ToString();
                            d.finalDescription = dataReader["finalDescription"].ToString();
                            d.description = dataReader["description"].ToString();
                            d.dish_name = dataReader["dish_name"].ToString();
                            list.Add(d);
                        }
                        cn.Close();
                        return list;
                    }
                }
                catch (MySqlException err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public static void UpdateSolutionPicture(int sid, string fileName)
        {
            lock (m_lock)
            {
                bool update = true;
                SolutionPictureInfo ? r = GetSolutionPicture(sid);
                if (r == null)
                {
                    update = false;
                }

                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {

                    try
                    {
                        string query;

                        if (update == true)
                        {
                            query = @"UPDATE  solution_pictures
                                        SET fullpicname = @fileName 
                                        where sid = @sid";

                        }
                        else
                        {
                            query = @"INSERT INTO solution_pictures(sid, fullpicname, showhide) 
                                        VALUES (@sid, @fileName, @showhide)";



                        }
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@fileName", fileName);

                            if (update == false)
                            {
                                cmd.Parameters.AddWithValue("@showhide", 1);
                            }
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }
        public static void UpdateAlgoFrequnecyParams(string algoName, 
                                                     int sid, 
                                                     int rowIndex,
                                                     string FrequenciesFileName)
        {

            lock (m_lock)
            {

                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {


                    try
                    {
                        string query = string.Empty;
                        switch (algoName)
                        {
                            case "DR Treshold" :
                                query = @"UPDATE algo_drthreshold_params SET freqtablefilename = @FrequenciesFileName
                                         where sid = @sid and RowIndex = @rowindex";
                                
                            break;
                            case "equal energy":
                                query = @"UPDATE algo_equalenergy_params SET freqtablefilename = @FrequenciesFileName
                                             where sid = @sid and RowIndex = @rowindex";
                            break;
                            case "Gamma Percentage":
                            break;
                            case "Top Percentage":
                            query = @"UPDATE algo_top_percentage_params SET freqtablefilename = @FrequenciesFileName
                                             where sid = @sid and RowIndex = @rowindex";
                            break;
                            default:
                                throw (new SystemException("Unknown algorithem"));
                        }

                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@rowindex", rowIndex);
                            cmd.Parameters.AddWithValue("@FrequenciesFileName", FrequenciesFileName);

                            cmd.ExecuteNonQuery();

                            
                        }
                        cn.Close();
                    }
                    catch (Exception err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static void DeleteAlgorithemParams(int sid , int rowindex, string algoname)
        {
            if (algoname == "RF Off") return;
            string tableName = string.Empty;
            lock (m_lock)
            {
                string freqFile = string.Empty;
                switch (algoname)
                {
                    case "equal energy":
                    {
                        tableName = "algo_equalenergy_params";
                        AlgoEqualEnergyParams? d = GetAlgoEqualEnergyParams(sid, rowindex);
                        if (d != null)
                            freqFile = d.Value.freqtablefilename;
                    }
                    break;
                    case "DR Treshold":
                    {
                        tableName = "algo_drthreshold_params";
                        AlgoThresholParams? d = GetAlgoThreaholdParams(sid, rowindex);
                        if (d != null)
                            freqFile = d.Value.freqtablefilename;
                    }
                    break;
                    case "Top Percentage":
                    {
                        tableName = "algo_top_percentage_params";
                        AlgoTopPercentageParams? d = GetAlgoTopPercentageParams(sid, rowindex);
                        if (d != null)
                            freqFile = d.Value.freqtablefilename;
                    }
                    break;
                    default:
                        throw (new SystemException("Unknown algo name"));
                }

                if (freqFile != "all" && File.Exists(freqFile))
                {
                    File.Delete(freqFile);
                }

                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                       
                       

                        string query = "DELETE from  " + tableName + "  where sid = @sid and RowIndex = @rowindex ";
                        cn.Open();

                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Close();
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static AlgoTopPercentageParams? GetTestAlgoTopPercentageParams(int testid, int rowindex)
        {

            lock (m_lock)
            {
                AlgoTopPercentageParams? d1 = null;
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.test_algo_top_percentage_params
                                 where testid = @testid and RowIndex = @rowindex";

                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testid);
                            cmd.Parameters.AddWithValue("@rowindex", rowindex);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoTopPercentageParams d = new AlgoTopPercentageParams();
                                d.power_time_mili = int.Parse(dataReader["powertime"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = (x == 1) ? true : false;
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                d.toppercent = int.Parse(dataReader["toppercent"].ToString());
                                d.sid = int.Parse(dataReader["testid"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;
                                try
                                {
                                    x = int.Parse(dataReader["equaldrtime"].ToString());
                                    d.equaldrtime = (x == 1) ? true : false;
                                }
                                catch (Exception err)
                                {
                                    d.equaldrtime = true;
                                }
                                d1 = d;
                            }
                        }
                        cn.Close();
                        return d1;
                    }
                    catch (MySqlException err)
                    {
                        throw (new SystemException(err.Message));
                    }
                }
            }
        }

        public static List<AlgoTopPercentageParams> GetTestAlgoTopPercentageParams(int testid)
        {

            lock (m_lock)
            {
                List<AlgoTopPercentageParams> l = new List<AlgoTopPercentageParams>();
                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        string query = string.Empty;

                        query = @"SELECT * FROM topazdb.test_algo_top_percentage_params
                                 where testid = @testid";

                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@testid", testid);

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                AlgoTopPercentageParams d = new AlgoTopPercentageParams();
                                d.power_time_mili = int.Parse(dataReader["powertime"].ToString());
                                int x = int.Parse(dataReader["agc"].ToString());
                                d.agc = (x == 1) ? true : false;
                                d.RowIndex = int.Parse(dataReader["RowIndex"].ToString());
                                d.toppercent = int.Parse(dataReader["toppercent"].ToString());
                                d.sid = int.Parse(dataReader["testid"].ToString());
                                d.freqtablefilename = dataReader["freqtablefilename"].ToString();
                                x = 0;
                                int.TryParse(dataReader["substractEmptyCavity"].ToString(), out x);
                                d.substractEmptyCavity = (x == 1) ? true : false;
                                try
                                {
                                    x = int.Parse(dataReader["equaldrtime"].ToString());
                                    d.equaldrtime = (x == 1) ? true : false;
                                }
                                catch (Exception err)
                                {
                                    d.equaldrtime = true;
                                }
                                l.Add(d);
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

        public static int UpdateUsrtWorkTime(int userId, DateTime start, DateTime stop)
        {
            lock (m_lock)
            {

                TimeSpan timeToAdd = stop - start;

                using (MySqlConnection cn = new MySqlConnection(myConnectionString))
                {
                    
                    try
                    {

                        int hour;
                        int days;
                        int minute;
                        int seconds;

                        string query = "SELECT * FROM user_worktime where userid = " + userId;

                        cn.Open();

                        bool IsExist;
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {

                            MySqlDataReader dataReader = cmd.ExecuteReader();

                            //Read the data and store them in the list
                            IsExist = dataReader.Read();
                            if (IsExist)
                            {
                                hour = int.Parse(dataReader["hour"].ToString());
                                minute = int.Parse(dataReader["minutes"].ToString());
                                seconds = int.Parse(dataReader["seconds"].ToString());
                                days = int.Parse(dataReader["days"].ToString());

                                TimeSpan t = new TimeSpan(days, hour, minute, seconds);
                                timeToAdd = t + timeToAdd;

                            }
                            cn.Close();
                        }

                        cn.Open();

                        if (IsExist == false)
                        {
                            query = "INSERT INTO user_worktime (hour, minutes, days,seconds, userid) VALUES (@hour, @minutes,@days, @seconds, @userid)";
                        }
                        else
                        {
                            query = @"UPDATE user_worktime  SET days = @days, hour = @hour, minutes = @minutes, seconds = @seconds WHERE userid = @userid";
                        }
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            hour = timeToAdd.Hours;
                            days = timeToAdd.Days;
                            minute = timeToAdd.Minutes;
                            seconds = timeToAdd.Seconds;

                            Int32 newId = 0;

                            cmd.Parameters.AddWithValue("@hour", hour);
                            cmd.Parameters.AddWithValue("@minutes", minute);
                            cmd.Parameters.AddWithValue("@days", days);
                            cmd.Parameters.AddWithValue("@seconds", seconds);                            
                            cmd.Parameters.AddWithValue("@userid", userId);

                            //newId = (Int32)cmd.ExecuteScalar();
                            // Execute the query
                            newId = cmd.ExecuteNonQuery();
                            newId = (Int32)cmd.LastInsertedId;

                            cn.Close();
                            return newId;
                        }

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
