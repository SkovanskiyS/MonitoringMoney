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

namespace MonitoringMoney
{
    internal class Profile_DB_API
    {

        private string connection_text = @"server=localhost;port=3306;username=root;password=root;database=debtorddatabase";

        MySqlConnection connection;
        DataTable table;
        public Profile_DB_API()
        {
            connection= new MySqlConnection(connection_text);
            table = new DataTable();
        }

        public DataTable Spends()
        {
            string query = "select * from debtordb where Exchange=@exchange";
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@exchange", "Дал (занял)");

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            connection.Close();
            return table;
        }


        public object WholeSpends()
        {
            string query = "select `Amount` from debtordb";
            connection.Open();
            var all_amount = new List<object>();
            int sum_all_elems = 0;
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

            for (int i = 0; i < all_amount.Count; i++)
            {
               // MessageBox.Show(all_amount[i].ToString());
            }
            connection.Close();
            return GetCurrency();
        }



        private double GetCurrency()
        {
            try
            {
                var timeoutInMilliseconds = 5000;
                var uri = new Uri("https://bank.uz/currency");
                var doc = Supremes.Dcsoup.Parse(uri, timeoutInMilliseconds);
                var ratingSpan = doc.Select("span[class=medium-text]");
                double currency = double.Parse(ratingSpan.Text.Substring(23, 9).Replace(".", ","));
                return currency;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return 0;

        }


        public Dictionary<object, List<object>> Get_Name_And_Amount()
        {
            string query = "select `Client`,`Amount` from debtordb";
            Dictionary<object, List<object>> data = new Dictionary<object, List<object>>();

            connection.Open();
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (data.ContainsKey(reader.GetString(0)))
                        {
                            data[reader.GetString(0)].Add(reader.GetString(1));
                        }
                        else
                        {
                            data.Add(reader.GetString(0), new List<object>() { reader.GetString(1) });
                        }
                    }
                }
            }
            return data;
        }


    }
}
