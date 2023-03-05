using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

            object[] data_collection = { name_textbox.Text, lastname_textbox.Text, companyName_textbox.Text, user_box.Text, password_box.Text };
            bool filled = false;
            try
            {

                for (int i = 0; i < data_collection.Length; i++)
                {
                    filled = string.IsNullOrWhiteSpace(data_collection[i].ToString())?false:true;
                }

                if (filled)
                {
                    DB_API.Add_User(data_collection[3].ToString().Trim(), data_collection[4].ToString().Trim(), data_collection[0].ToString(), data_collection[1].ToString(), data_collection[2].ToString());
                    name_textbox.Text = "";
                    lastname_textbox.Text = "";
                    companyName_textbox.Text = "";
                    user_box.Text = "";
                    password_box.Text = "";
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
    }
}
