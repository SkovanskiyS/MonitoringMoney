using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MonitoringMoney
{
    internal class DB_API
    {
        private MySqlConnection connection;
        private MySqlCommand cmd;
        private DataTable dataTable;
        private string name_to_search;
        private string connection_text = @"server=localhost;port=3306;username=root;password=root;database=debtorddatabase";

        private string table_name;

        public DB_API(string name = "Undefined")
        {
            Initialize();
            name_to_search = name;
        }

        private void Initialize()
        {
            connection = new MySqlConnection(connection_text);
        }
        public void Insert(DateTime date_of_reg,string client_name,string get_dive,string currency_,string sumValue,string wellText,string cash_transfer,string descriptionText,bool textbox_status)
        {
            Read_Table_Name();

            cmd = new MySqlCommand("INSERT INTO `"+table_name+"`(`Date`, `Client`, `Exchange`, `Currency`, `Amount`, `Rate`, `Transaction`, `Description`) VALUES (@date,@client,@exchange,@currency,@amount,@rate,@transaction,@description)", connection);

            //0 - date, 1 - client name, 2 - give or get, 3 - currency, 4 - amount(sum), 5 - rate(well), 6 - transaction(cash or transfer), 7 - description
            object[] collection_of_data = { date_of_reg.ToString("yyyy-MM-dd"), client_name, get_dive, currency_, sumValue, wellText, cash_transfer, descriptionText };
            string[] commands_to_add = { "@date", "@client", "@exchange", "@currency", "@amount", "@rate", "@transaction", "@description" };
            for (int i = 0; i < collection_of_data.Length; i++)
            {
                if (collection_of_data[5].ToString() == "" && textbox_status == false) collection_of_data[5] = "Пусто";
                if (collection_of_data[i] == null || collection_of_data[i].ToString().Length == 0)
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                else cmd.Parameters.Add(commands_to_add[i], MySqlDbType.VarChar).Value = collection_of_data[i];

            }
            connection.Open();
            try
            {
                if (cmd.ExecuteNonQuery() == 1) MessageBox.Show("Успешно добавлен", "Добавлен", MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка. Перепроверьте заполненные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }

        public DataTable LoadAllData()
        {

            Read_Table_Name();
            if (connection.State == ConnectionState.Closed) { connection.Open(); }
            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM "+ table_name;
            MySqlDataReader reader = cmd.ExecuteReader();
            dataTable= new DataTable();
            dataTable.Load(reader);
            connection.Close();
            return dataTable;
        }

        public void Read_Table_Name()
        {
            string path = Directory.GetCurrentDirectory()+ @"\username.txt";

            using (StreamReader reader2 = new StreamReader(path))
            {
                table_name = reader2.ReadToEnd();
            }
        }

        public DataTable Search(string a)
        {
            Read_Table_Name();

            var found_users = FindUsers(); 
            dataTable = new DataTable();
            DataTable emptyDataTable = new DataTable();   
            connection.Open();

            string query = "select * from "+table_name+" where ID in (";
            for (int i = 0; i < found_users.Count; i++)
            {
                query += "@id" + i.ToString() + ",";
            }
            //Remove the last comma
            query = query.Remove(query.Length - 1);
            query += ")";

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    for (int i = 0; i < found_users.Count; i++)
                    {
                        command.Parameters.AddWithValue("@id" + i.ToString(), found_users[i]);
                    }
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
            catch (Exception)
            {
            
            }
            return dataTable;
        }

        private List<object> GetAllData()
        {

            Read_Table_Name();

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            string sql_cmd = @"SELECT `Client`,`ID` FROM "+table_name;
            var cmd = new MySqlCommand(sql_cmd, connection);
            MySqlDataReader reader;
            reader = cmd.ExecuteReader(); 
            var values = new List<object>();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(reader.GetValue(i));
                }
            }
            connection.Close();
            return values;
        }

        private List<object> FindUsers()
        {
            var values = GetAllData();
            var ID = new List<object>();

            for (int i = 0; i < values.Count; i++)
            {
                int n;
                bool isNumeric = int.TryParse(Convert.ToString(values[i]), out n);

                if (!isNumeric)
                {
                    if (values[i].ToString().ToLower().Contains(name_to_search))
                    {
                        try
                        {
                            object index_of_ID = values[i + 1];
                            ID.Add(index_of_ID);

                        }
                        catch (Exception)
                        { }
                    }
                }
            }
            return ID;
        }

        public int GetCountOfObjects()
        {

            Read_Table_Name();

            string sqlExpression = "SELECT COUNT(*) FROM "+table_name;
            using (MySqlConnection connection = new MySqlConnection(connection_text))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(sqlExpression, connection);
                object count = command.ExecuteScalar();

                return Convert.ToInt32(count);

            }
        }

        public DataTable FilterGetOrGive(bool isGet)
        {

            Read_Table_Name();

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            string query = @"select * from "+table_name+" where Exchange=@exchange";
            dataTable = new DataTable();
            using (MySqlCommand command = new MySqlCommand(query,connection))
            {
                if (isGet)
                    command.Parameters.AddWithValue("@exchange", "Взял (одолжил)");
                else
                    command.Parameters.AddWithValue("@exchange", "Дал (занял)");
                using(MySqlDataReader reader = command.ExecuteReader())
                {
                     dataTable.Load(reader);
                }
            }
            connection.Close();
            return dataTable;
        }

        public DataTable FilterByDate(DateTime from,DateTime to,DataTable search)
        {

            Read_Table_Name();

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            string query = @"select * from "+table_name+" where Date between @from and @to";
            dataTable = search;
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@to", to.ToString("yyyy-MM-dd"));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
            }
            connection.Close();
            return dataTable;
        }
    }
}
