using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MonitoringMoney
{
    public partial class Register : Form
    {


        Register_DB_API DB_API;

        public Register()
        {
            InitializeComponent();
        }

        /*Личное пользование (Физическое лицо)
        Предприятия (Юридическое лицо)*/
        private void using_dropdown_SelectedValueChanged(object sender, EventArgs e)
        {
            if (using_dropdown.Text== "Личное пользование (Физическое лицо)")
            {
                companyName_textbox.Enabled = false;
                name_textbox.Enabled = true;
                lastname_textbox.Enabled = true;
            }
            else
            {
                name_textbox.Enabled = false;
                lastname_textbox.Enabled = false;
                companyName_textbox.Enabled = true;
            }
        }

        private void password_check_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {
            password_box.PasswordChar = password_check.Checked ? '\0' : '●';
        }

        private void Register_Load(object sender, EventArgs e)
        {
            using_dropdown_SelectedValueChanged(sender,e);
            DB_API = new Register_DB_API();
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {

            object[] data_collection = { name_textbox.Text, lastname_textbox.Text, companyName_textbox.Text, user_box.Text, password_box.Text , budgetValue .Text, currency_Dropdown .Text};
            bool filled = false;
            try
            {

                for (int i = 0; i < data_collection.Length-1; i++)
                {
                    filled = string.IsNullOrWhiteSpace(data_collection[i].ToString())?false:true;
                }

                if (filled)
                {
                    DB_API.Add_User(data_collection[3].ToString().Trim(), data_collection[4].ToString().Trim(), data_collection[0].ToString(), data_collection[1].ToString(), data_collection[2].ToString(), data_collection[5].ToString(), data_collection[6].ToString());
                    name_textbox.Text = "";
                    lastname_textbox.Text = "";
                    companyName_textbox.Text = "";
                    user_box.Text = "";
                    password_box.Text = "";
                    budgetValue.Text = "";
                    currency_Dropdown.Text = "";


                    this.Close();

                }
                else
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        private void sumValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void sumValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                if (e.KeyChar == ',')
                {
                    e.Handled = (budgetValue.Text.Contains(","));
                }
                else e.Handled = true;
            }
        }

        private void budgetValue_Leave(object sender, EventArgs e)
        {
            Change_Currency();
        }

        private void currency_Dropdown_SelectedValueChanged(object sender, EventArgs e)
        {
            Change_Currency();
        }

        private void Change_Currency()
        {
            string selectedItem = currency_Dropdown.Text;
            string clear_value = budgetValue.Text.Trim();
            try
            {
                //filter
                string[] remove_elements = { ",", "сум", "$" };
                foreach (var item in remove_elements) clear_value = clear_value.Replace(item, "");
                double value = Convert.ToDouble(clear_value);
                budgetValue.Text = selectedItem == "SUM (UZS)" ? value.ToString("#,#" + " сум", CultureInfo.InvariantCulture) : value.ToString("#,#" + " $", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                if (!(clear_value.Length == 0)) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error); }
            }
        }
    }
}
