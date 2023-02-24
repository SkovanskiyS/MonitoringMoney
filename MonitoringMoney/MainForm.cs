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
namespace MonitoringMoney
{
    public partial class MainForm : Form
    {
        DB dB;
        DataTable dataTable;
        MySqlDataAdapter dataAdapter;
        MySqlCommand cmd;
        BindingSource bindingSource;
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

            // MessageBox.Show(GetWellOfDollar.BankUz());
            dB = new DB();
            dataTable = new DataTable();
            dataAdapter = new MySqlDataAdapter();
            dB.mySqlConnection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=debtorddatabase");

            LoadAllData();
            ChangeColumnName();
        }

        private void bunifuTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                if (e.KeyChar == ',')
                {
                    e.Handled = (sumValue.Text.Contains(","));
                }
                else e.Handled = true;
            }
        }
        private void changeText()
        {
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
            cmd = new MySqlCommand("INSERT INTO `debtordb`(`Date`, `Client`, `Exchange`, `Currency`, `Amount`, `Rate`, `Transaction`, `Description`) VALUES (@date,@client,@exchange,@currency,@amount,@rate,@transaction,@description)", dB.getConnection());
            //cmd = new MySqlCommand("UPDATE `debtordb` SET `Date`=@date,`Client`=@client,`Exchange`=@exchange,`Currency`=@currency,`Amount`=@amount,`Rate`=@rate,`Transaction`=@transaction,`Description`=@description WHERE 1", dB.getConnection());

            //0 - date, 1 - client name, 2 - give or get, 3 - currency, 4 - amount(sum), 5 - rate(well), 6 - transaction(cash or transfer), 7 - description
            string[] collection_of_data = { dateOfReg.Text, clientNameT.Text, get_giveDropdown.Text, currency_Dropdown.Text, sumValue.Text, wellText.Text, cash_transfer.Text, descriptionText.Text };
            string[] commands_to_add = { "@date", "@client", "@exchange", "@currency", "@amount", "@rate", "@transaction", "@description" };
            for (int i = 0; i < collection_of_data.Length; i++)
            {
                if (collection_of_data[5] =="" && wellText.Enabled == false) collection_of_data[5] = "Пусто";
                if (collection_of_data[i]==null || collection_of_data[i].Length==0)
                {
                    MessageBox.Show("Заполните все поля!","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                else cmd.Parameters.Add(commands_to_add[i], MySqlDbType.VarChar).Value = collection_of_data[i];

            }
            dB.OpenConnectionSQL();
            try
            {
                if (cmd.ExecuteNonQuery() == 1) MessageBox.Show("Успешно добавлен", "Добавлен", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show("Возникла ошибка. Перепроверьте заполненные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dB.CloseConnectionSQL();
            LoadAllData();
        }

        private void cleanBtn_Click(object sender, EventArgs e)
        {
            object[] textboxValues = { sumValue, clientNameT, wellText};
            foreach (BunifuTextBox item in textboxValues)
            {
                item.Text = "";
            }
            descriptionText.Text = "";
        }

        private void LoadAllData()
        {
            dB.OpenConnectionSQL();
            cmd = dB.mySqlConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM `debtordb`";

            MySqlDataReader reader = cmd.ExecuteReader();
            dataTable.Load(reader);
            allDataGridView.DataSource = dataTable;
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("hello world");
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void sumValue_Leave(object sender, EventArgs e)
        {
            try
            {
                changeText();
            }
            catch (Exception)
            {}
        }

        private void ChangeColumnName()
        {

            string[] columnNames = { "ID", "Дата", "Клиент", "Обмен", "Валюта", "Сумма", "Курс", "Транзакция","Описание"};

            for (int i = 0; i < allDataGridView.Columns.Count; i++)
            {
                allDataGridView.Columns[i].HeaderText = columnNames[i];
            }
        }
    }
}


