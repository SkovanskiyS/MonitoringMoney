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
namespace MonitoringMoney
{
    internal class DB_API
    {
        private MySqlConnection connection;
        private MySqlCommand cmd;
        private DataTable dataTable;
        public DB_API()
        {
            Initialize();
        }

        private void Initialize()
        {
            connection = new MySqlConnection(@"server=localhost;port=3306;username=root;password=root;database=debtorddatabase");
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
                if (cmd.ExecuteNonQuery() == 1) MessageBox.Show("Успешно добавлен", "Добавлен", MessageBoxButtons.OK);
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

    }
}
