using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace MonitoringMoney
{
    internal class DataBase
    {
        MySqlConnection mySqlConnection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=log_pass");

        public void OpenConnectionSQL()
        {
            if(mySqlConnection.State == System.Data.ConnectionState.Closed)
            {
                mySqlConnection.Open();
            }
        }
        
        public void CloseConnectionSQL()
        {
            if(mySqlConnection.State == System.Data.ConnectionState.Open)
            {
                mySqlConnection.Close();
            }
        }

        public MySqlConnection getConnection()
        {
            return mySqlConnection;
        }
    }
}
