using MySql.Data.MySqlClient;

namespace MonitoringMoney
{
    public class DB
    {
        public MySqlConnection mySqlConnection;

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
