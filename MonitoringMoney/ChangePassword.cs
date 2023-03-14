using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitoringMoney
{
    public partial class ChangePassword : Form
    {

        private string username;
        MySqlConnection connection;
        private string connection_text = @"server=localhost;port=3306;username=root;password=root;database=debtorddatabase";
        public ChangePassword()
        {
            InitializeComponent();
            connection = new MySqlConnection(connection_text);
        }

        public void Read_Table_Name()
        {
            string path = Directory.GetCurrentDirectory() + @"\username.txt";

            using (StreamReader reader2 = new StreamReader(path))
            {
                username = reader2.ReadToEnd().Replace("_data","");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Read_Table_Name();
            string password = "";
            string query = @"select Password from users where Username=@username";
            connection.Open();

            using (var cmd = new MySqlCommand(query,connection))
            {
                cmd.Parameters.AddWithValue("username", username);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        password = reader.GetString(0);
                    }
                }
            }

            if (bunifuTextBox1.Text==password)
            {
                string query2 = @"update `users` set `Password` = @password where Username=@username";

                using (var cmd = new MySqlCommand(query2, connection))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", bunifuTextBox2.Text);
                    if (cmd.ExecuteNonQuery()==1)
                    {
                        MessageBox.Show("Успешно изменён!");
                    }
                    else
                    {
                        MessageBox.Show("Что то пошло не так");
                    }
                }
            }
            else
            {
                MessageBox.Show("Вы ввели неверный пароль");
            }

            connection.Close();
        }
    }
}
