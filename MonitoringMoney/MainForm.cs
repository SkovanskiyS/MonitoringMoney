using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
        DataBase dB;
        DataTable dataTable;
        MySqlDataAdapter dataAdapter;
        MySqlCommand cmd;

        string dateOfAdd,clientName, get_give, currency, amountOfMoney, well, cash_transferText;
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
            dateOfAdd = dateOfReg.Text;
            clientName = clientNameT.Text;
            get_give = this.get_giveDropdown.Text;
            currency = currency_Dropdown.Text;
            amountOfMoney = sumValue.Text;
            well = wellText.Text;
            cash_transferText = cash_transfer.Text;

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

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            //dB = new DataBase();
            //dataTable = new DataTable();
            //dataAdapter = new MySqlDataAdapter();
            //cmd = new MySqlCommand("SELECT * FROM `log_pass` WHERE `login` = @username AND `password` = @password", dB.getConnection());

            //string user_name = this.user_box.Text;
            //string user_password = this.password_box.Text;

            //cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = user_name;
            //cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = user_password;
            MessageBox.Show(cash_transfer.Text);
            //dataAdapter.SelectCommand = cmd;
        }
    }
}


