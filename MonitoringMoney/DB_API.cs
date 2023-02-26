using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MonitoringMoney;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using System.Runtime.Remoting.Messaging;

namespace MonitoringMoney
{
    internal class DB_API
    {
        private MySqlConnection connection;
        private MySqlCommand cmd;
        private MySqlDataReader reader;
        private DataTable dataTable;
        private string name_to_search;
        private string connection_text = @"server=localhost;port=3306;username=root;password=root;database=debtorddatabase";
        public DB_API(string name = "Undefined")
        {
            Initialize();
            name_to_search = name;
        }

        private void Initialize()
        {
            connection = new MySqlConnection(connection_text);
        }
        public void Insert(string date_of_reg,string client_name,string get_dive,string currency_,string sumValue,string wellText,string cash_transfer,string descriptionText,bool textbox_status)
        {
            cmd = new MySqlCommand("INSERT INTO `debtordb`(`Date`, `Client`, `Exchange`, `Currency`, `Amount`, `Rate`, `Transaction`, `Description`) VALUES (@date,@client,@exchange,@currency,@amount,@rate,@transaction,@description)", connection);
            //cmd = new MySqlCommand("UPDATE `debtordb` SET `Date`=@date,`Client`=@client,`Exchange`=@exchange,`Currency`=@currency,`Amount`=@amount,`Rate`=@rate,`Transaction`=@transaction,`Description`=@description WHERE 1", dB.getConnection());

            //0 - date, 1 - client name, 2 - give or get, 3 - currency, 4 - amount(sum), 5 - rate(well), 6 - transaction(cash or transfer), 7 - description
            string[] collection_of_data = { date_of_reg, client_name, get_dive, currency_, sumValue, wellText, cash_transfer, descriptionText };
            string[] commands_to_add = { "@date", "@client", "@exchange", "@currency", "@amount", "@rate", "@transaction", "@description" };
            for (int i = 0; i < collection_of_data.Length; i++)
            {
                if (collection_of_data[5] == "" && textbox_status == false) collection_of_data[5] = "Пусто";
                if (collection_of_data[i] == null || collection_of_data[i].Length == 0)
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

        public void Refresh()
        {

        }

        public void Delete() { }


        public DataTable LoadAllData()
        {
            connection.Open();
            cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM `debtordb`";
            MySqlDataReader reader = cmd.ExecuteReader();
            dataTable= new DataTable();
            dataTable.Load(reader);
            connection.Close();
            return dataTable;
        }

        public DataTable Search()
        {
            var found_users = FindUsers(); 
            dataTable = new DataTable();
            DataTable emptyDataTable = new DataTable();   
            connection.Open();

            string query = "select * from debtordb where ID in (";
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
           // MessageBox.Show("Ничего не найдено!", "Пусто", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return dataTable;
        }

        private List<object> GetAllData()
        {
            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            string sql_cmd = @"SELECT `Client`,`ID` FROM debtordb";
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
            string sqlExpression = "SELECT COUNT(*) FROM debtordb";
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
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            string query = @"select * from debtordb where Exchange=@exchange";
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
    }
}
