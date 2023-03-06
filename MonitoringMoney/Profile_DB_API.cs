using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using static IronPython.Modules._ast;
using System.Globalization;

namespace MonitoringMoney
{
    internal class Profile_DB_API
    {

        private string connection_text = @"server=localhost;port=3306;username=root;password=root;database=debtorddatabase";
        public int currency;
        private string table_name;
        MySqlConnection connection;
        DataTable table;
        DB_API read_Table_;

        public Profile_DB_API()
        {
            connection= new MySqlConnection(connection_text);
            table = new DataTable();
            read_Table_ = new DB_API();
        }

        public void Read_Table_Name()
        {
            string path = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName + @"\username.txt";

            using (StreamReader reader2 = new StreamReader(path))
            {
                table_name = reader2.ReadToEnd();
            }
        }

        public DataTable Spends_Income(string pathway)
        {
            table = new DataTable();

            Read_Table_Name();

            string query = "select * from "+table_name+" where Exchange=@exchange";
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@exchange", pathway);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            connection.Close();
            return table;
        }


        public void WholeSpends()
        {
            Read_Table_Name();
            string query = "select `Amount` from "+table_name;
            connection.Open();
            var all_amount = new List<object>();
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            all_amount.Add(reader.GetValue(i));
                        }
                    }
                }
            }
            connection.Close();

        }

        public void GetCurrency()
        {
            try
            {
                var timeoutInMilliseconds = 5000;
                var uri = new Uri("https://bank.uz/currency");
                var doc = Supremes.Dcsoup.Parse(uri, timeoutInMilliseconds);
                var ratingSpan = doc.Select("span[class=medium-text]");
                double d_currency = double.Parse(ratingSpan.Text.Substring(23, 9).Replace(".", ","));
                currency = Convert.ToInt32(d_currency);
            }
            catch (Exception e)
            {
                currency = 0;
                MessageBox.Show(e.Message);
            }
            
        }

        public Dictionary<object, int> Get_Name_And_Amount(string get_or_give)
        {
            if (currency == 0) GetCurrency();

            Read_Table_Name();

            string query = "select `Client`,`Amount` from "+table_name+" where Exchange=@exchange";
            Dictionary<object, int> data = new Dictionary<object, int>();
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@exchange", get_or_give);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int i = int.Parse(reader.GetString(1).Replace(",","").Replace("сум", "").Replace("$",""));
                        //int last_dollar = reader.GetString(1).Contains("сум")? i / currency : int.Parse(reader.GetString(1).Replace("$", "").Replace("сум",""));
                        int last_dollar = 0;
                        if (reader.GetString(1).Contains("сум"))
                        {
                            last_dollar = i / currency;
                        }
                        else
                        {
                            last_dollar = i;
                        }
                        
                        if (data.ContainsKey(reader.GetString(0).ToLower()))
                            data[reader.GetString(0).ToLower()] += last_dollar;
                        else
                            data.Add(reader.GetString(0).ToLower(), last_dollar);
                    }
                }
            }
            connection.Close();
            return data;
        }

        public Dictionary<object, int> Get_Data_By_DateTime(DateTime from,DateTime to,string get_or_give)
        {

            Read_Table_Name();

            string query = "select `Client`,`Amount` from "+table_name+" where Date between @from and @to and Exchange=@exchange";
            Dictionary<object, int> data = new Dictionary<object, int>();
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@from", from.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@to", to.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@exchange", get_or_give);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int i = int.Parse(reader.GetString(1).Replace(",", "").Replace("сум", "").Replace("$", ""));
                        //int last_dollar = reader.GetString(1).Contains("сум")? i / currency : int.Parse(reader.GetString(1).Replace("$", "").Replace("сум",""));
                        int last_dollar = 0;
                        if (reader.GetString(1).Contains("сум"))
                        {
                            last_dollar = i / currency;
                        }
                        else
                        {
                            last_dollar = i;
                        }

                        if (data.ContainsKey(reader.GetString(0).ToLower()))
                            data[reader.GetString(0).ToLower()] += last_dollar;
                        else
                            data.Add(reader.GetString(0).ToLower(), last_dollar);
                    }
                }
            }
            connection.Close();
            return data;
        }


    }
}
