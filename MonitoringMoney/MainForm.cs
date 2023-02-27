using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using MySql.Data.MySqlClient;

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
            countOfUsersLabel.Text = Convert.ToString(allDataGridView.RowCount);
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
            if (e.KeyCode == Keys.Enter) addBtn.PerformClick();
        }

        private void searchTextBox_TextChange(object sender, EventArgs e)
        {
            //dataBase = new DB_API(searchTextBox.Text);
            //if (searchTextBox.Text.Length > 0)
            //{
            //    allDataGridView.DataSource = dataBase.Search(searchTextBox.Text);
            //}
            //else
            //{
            //    allDataGridView.DataSource = dataBase.LoadAllData();
            //}

        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            dataBase = new DB_API(searchTextBox.Text);
            if (searchTextBox.Text.Length > 0)
            {
                allDataGridView.DataSource = dataBase.Search(searchTextBox.Text);
            }
            else
            {
                allDataGridView.DataSource = dataBase.LoadAllData();
            }
        }

        private void give_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            FilterCheckBox();
        }

        private void FilterCheckBox()
        {
            if(get_checkbox.Checked && give_checkbox.Checked||!get_checkbox.Checked&&!give_checkbox.Checked)
            {
                allDataGridView.DataSource = dataBase.LoadAllData();
            }
            else
            {
                allDataGridView.DataSource = get_checkbox.Checked? dataBase.FilterGetOrGive(true) : allDataGridView.DataSource = dataBase.FilterGetOrGive(false);
            }
        }

        private void get_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            FilterCheckBox();
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {

            allDataGridView.DataSource = dataBase.FilterByDate(dateFrom.Value.Date, dateTo.Value.Date, dataBase.Search("undefined"));


        }

        private void resetDate_Click(object sender, EventArgs e)
        {
            allDataGridView.DataSource = dataBase.LoadAllData();
            var dateToday = DateTime.Now;
            dateFrom.Value = DateTime.Parse(dateToday.ToShortDateString());
            dateTo.Value = DateTime.Parse(dateToday.ToShortDateString());
        }

        private void allDataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
           countOfUsersLabel.Text = Convert.ToString(allDataGridView.RowCount);
        }

        private void applyBtn2_Click(object sender, EventArgs e)
        {
            allDataGridView.DataSource = dataBase.FilterByDate(dateFrom.Value.Date, dateTo.Value.Date, dataBase.Search("undefined"));

        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            allDataGridView.DataSource = dataBase.LoadAllData();
            var dateToday = DateTime.Now;
            dateFrom.Value = DateTime.Parse(dateToday.ToShortDateString());
            dateTo.Value = DateTime.Parse(dateToday.ToShortDateString());
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            dataBase = new DB_API();
            dataBase.Insert(dateOfReg.Value.Date, clientNameT.Text, get_giveDropdown.Text, currency_Dropdown.Text, sumValue.Text, wellText.Text, cash_transfer.Text, descriptionText.Text, wellText.Enabled);
            allDataGridView.DataSource = dataBase.LoadAllData();
        }

        private void cleanData_Click(object sender, EventArgs e)
        {
            object[] textboxValues = { sumValue, clientNameT, wellText };
            foreach (BunifuTextBox item in textboxValues)
            {
                item.Text = "";
            }
            descriptionText.Text = "";
        }

        private void мойПрофильToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}


