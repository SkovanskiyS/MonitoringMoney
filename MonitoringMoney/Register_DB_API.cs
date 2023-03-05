using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static IronPython.Runtime.Profiler;

namespace MonitoringMoney
{
    internal class Register_DB_API
    {
        private string connection_text = @"server=localhost;port=3306;username=root;password=root;database=debtorddatabase";
        MySqlConnection connection;

        public Register_DB_API()
        {
            connection = new MySqlConnection(connection_text);
        }

        public void Add_User(string username, string password,string name,string surname,string company_name)
        {
            string sqlQuery = "INSERT INTO `users`(`Name`, `Surname`, `Company_Name`, `Username`, `Password`) VALUES (@name,@surname,@company_name,@username,@password)";

            if (UserExists(username))
            {
                MessageBox.Show("Данный пользователь уже зарегистрирован!","Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                using (MySqlCommand cmd = new MySqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("surname", surname);
                    cmd.Parameters.AddWithValue("company_name", company_name);
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password);

                    if (cmd.ExecuteNonQuery() == 1) MessageBox.Show("Успешно добавлен", "Добавлен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }


            string sqlQuery_check = "SELECT * FROM users WHERE username = @username";
            string table_name = username + "_data";

            using (MySqlCommand cmd2 = new MySqlCommand(sqlQuery_check, connection))
            {
                cmd2.Parameters.AddWithValue("@username", username);

                // Use a while loop to read through the results and close the reader
                using (MySqlDataReader reader = cmd2.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reader.Close();
                        // Get the table count for the user
                        string sqlQuery_check_count = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = @database AND table_name = @table";
                        using (MySqlCommand cmd3 = new MySqlCommand(sqlQuery_check_count, connection))
                        {
                            cmd3.Parameters.AddWithValue("@database", "mydatabase");
                            cmd3.Parameters.AddWithValue("@table", table_name);

                            // Execute the query to get the count
                            int count = Convert.ToInt32(cmd3.ExecuteScalar());

                            if (count == 0)
                            {
                                // Create the user-specific table
                                string sqlQuery_create_table = "CREATE TABLE " + table_name + " (ID INT NOT NULL AUTO_INCREMENT, Date date NOT NULL, Client VARCHAR(255),Exchange VARCHAR(255),	Currency VARCHAR(100),	Amount VARCHAR(100),Rate VARCHAR(100),Transaction VARCHAR(100),Description text,PRIMARY KEY (id))";
                                using (MySqlCommand cmd4 = new MySqlCommand(sqlQuery_create_table, connection))
                                {
                                    cmd4.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    // Close the first data reader
                    
                }
            }
            connection.Close();
        }

        private bool UserExists(string username)
        {
            bool userExists = false;
            connection.Open();
            string sqlQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
            cmd.Parameters.AddWithValue("@username", username);
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count > 0)
            {
                userExists = true;
            }

            // Close database objects
            cmd.Dispose();

            return userExists;
        }

    }
}
