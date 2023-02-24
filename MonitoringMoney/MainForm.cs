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

namespace MonitoringMoney
{
    public partial class MainForm : Form
    {
        DB dB;
        DataTable dataTable;
        MySqlDataAdapter dataAdapter;
        MySqlCommand cmd;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dB = new DB();
            dataTable = new DataTable();
            dataAdapter = new MySqlDataAdapter();
            dB.mySqlConnection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=debtorddatabase");
        }

        private void bunifuTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                if (e.KeyChar == ',')
                {
                    e.Handled = (sumValue.Text.Contains(","));
                }
                else
                    e.Handled = true;
            }
        }

        private void changeText()
        {
            //double value = 0;
            //string selectedItemw
            //{
            //    if (selectedItem == "SUM (UZS)")
            //    {
            //        value = Convert.ToDouble(sumValue.Text.Replace(",", "").Replace("сум", ""));
            //        sumValue.Text = value.ToString("#,#" + " сум", CultureInfo.InvariantCulture);
            //    }
            //    else
            //    {
            //        value = Convert.ToDouble(sumValue.Text.Replace(",", "").Replace("$", ""));
            //        sumValue.Text = value.ToString("#,#" + " $", CultureInfo.InvariantCulture);
            //    }
            //}
            //catch (Exception e)
            //{

            //    MessageBox.Show(e.Message,"Ошибка",MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Error)
            //}
            string selectedItem = currency_Dropdown.Text;
            string clear_value = sumValue.Text.Trim();
            try
            {
                //filter
                string[] remove_elements = { ",", "сум", "$" };
                foreach (var item in remove_elements) clear_value = clear_value.Replace(item, "");
                double value = Convert.ToDouble(clear_value);
                sumValue.Text = selectedItem == "SUM (UZS)" ? value.ToString("#,#" + " сум", CultureInfo.InvariantCulture) : value.ToString("#,#" + " $", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                if (!(clear_value.Length == 0)) { MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error); }

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            changeText();
        }

        private void wellText_MouseClick(object sender, MouseEventArgs e)
        {
            wellText.Enabled = Convert.ToString(currency_Dropdown.SelectedItem) == "USD $" ? true : false;
        }

        private void addUserBtn_Click(object sender, EventArgs e)
        {
            //string dateOfAdd = dateOfReg.Text;
            //string clientName = clientNameT.Text;
            //string get_give = get_giveDropdown.Text;
            //string currency = currency_Dropdown.Text;
            //string amountOfMoney = sumValue.Text;
            //string well = wellText.Text;
            //string cash_transferText = cash_transfer.Text;
            //string description = descriptionText.Text;
            //cmd.Parameters.Add("@date", MySqlDbType.VarChar).Value = dateOfAdd;
            //cmd.Parameters.Add("@client", MySqlDbType.VarChar).Value = clientName;
            //cmd.Parameters.Add("@exchange", MySqlDbType.VarChar).Value = get_give;
            //cmd.Parameters.Add("@currency", MySqlDbType.VarChar).Value = currency;
            //cmd.Parameters.Add("@amount", MySqlDbType.VarChar).Value = amountOfMoney;
            //cmd.Parameters.Add("@rate", MySqlDbType.VarChar).Value = well;
            //cmd.Parameters.Add("@transcation", MySqlDbType.VarChar).Value = cash_transferText;
            //cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = description;
            cmd = new MySqlCommand("INSERT INTO `debtordb`(`Date`, `Client`, `Exchange`, `Currency`, `Amount`, `Rate`, `Transaction`, `Description`) VALUES (@date,@client,@exchange,@currency,@amount,@rate,@transaction,@description)", dB.getConnection());

            //0 - date, 1 - client name, 2 - give or get, 3 - currency, 4 - amount(sum), 5 - rate(well), 6 - transaction(cash or transfer), 7 - description
            string[] collection_of_data = { dateOfReg.Text, clientNameT.Text, get_giveDropdown.Text, currency_Dropdown.Text, sumValue.Text, wellText.Text, cash_transfer.Text, descriptionText.Text };
            string[] commands_to_add = { "@date", "@client", "@exchange", "@currency", "@amount", "@rate", "@transaction", "@description" };

            MessageBox.Show(collection_of_data.Length.ToString());
            MessageBox.Show(commands_to_add.Length.ToString());


            for (int i = 0; i < collection_of_data.Length; i++)
            {
                if (collection_of_data[5] =="" && wellText.Enabled == false)
                {
                    collection_of_data[5] = "Пусто";
                }
                if (collection_of_data[i]==null || collection_of_data[i].Length==0)
                {
                    MessageBox.Show("Заполните все поля!","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                else
                {
                    cmd.Parameters.Add(commands_to_add[i], MySqlDbType.VarChar).Value = collection_of_data[i];
                }
            }
            dB.OpenConnectionSQL();

            try
            {
                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Успешно добавлен", "Добавлен", MessageBoxButtons.OK);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Возникла ошибка. Перепроверьте заполненные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dB.CloseConnectionSQL();

        }

        private void cleanBtn_Click(object sender, EventArgs e)
        {
            clientNameT.Text = "";
        }
    }
}


