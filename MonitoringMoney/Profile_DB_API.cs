using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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


        public int WholeSpends()
        {
            string query = "select `Amount`` from debtordb";
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
                
            }

            connection.Close();
            return 0;
        }

    }
}
