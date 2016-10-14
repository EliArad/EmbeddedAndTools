using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopazApp;

namespace TopazApp
{
    public class User
    {
        public int ID;
        public string email;
        public string password;
        public string firstName;
        public string lastName;
        public string phoneNumber;
        public bool active;
    }

    public class usersDb  
    {
        public  int CheckAuthtintication(string userName, string password)
        {
            try
            {
                return MySQLConnector.CheckAuthtintication(userName, password);                
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public int GetUserID(string email)
        {
            try
            {
                return MySQLConnector.GetUserID(email);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public  bool IsUserActive(string userName, int typeOfUser)
        {
            return false;
        }
        public  bool IsUserExist(string userName, int typeOfUser)
        {
            return false;
        }
        public  List<User> ReadAllUsers()
        {
            try
            {
                return MySQLConnector.GetAllUsers();
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        public  void DeleteUser(int id)
        {
            try
            {
                MySQLConnector.DeleteUser(id);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public  void SaveUserChanges(User user)              
        {

            try
            {
                MySQLConnector.SaveUserChanges(user);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        public  void CreateNewUser(User user)              
        {

            try
            {
                MySQLConnector.CreateNewUser(user);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        public  void SuspendUser(int id, bool suspend)
        {
            try
            {
                MySQLConnector.SuspendUser(id, suspend);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
    }
}
