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
using Mysqlx.Crud;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace MonitoringMoney
{
    public partial class MainForm : Form
    {
        DB dB;
        DataTable dataTable;
        MySqlDataAdapter dataAdapter;
        MySqlCommand cmd;
        BindingSource bindingSource;
        DB_API dataBase;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //dB.CloseConnectionSQL();
            //Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dB = new DB();
            dataTable = new DataTable();
            dataAdapter = new MySqlDataAdapter();
            dB.mySqlConnection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=debtorddatabase");
            dataBase = new DB_API();
            allDataGridView.DataSource = dataBase.LoadAllData();
            ChangeColumnName();
            this.KeyPreview = true;
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
            dataBase = new DB_API();
            dataBase.Insert(dateOfReg.Text, clientNameT.Text, get_giveDropdown.Text, currency_Dropdown.Text, sumValue.Text, wellText.Text, cash_transfer.Text, descriptionText.Text,wellText.Enabled);

            allDataGridView.DataSource = dataBase.LoadAllData();
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

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("hello world");
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
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) addUserBtn.PerformClick();
        }
    }
}


