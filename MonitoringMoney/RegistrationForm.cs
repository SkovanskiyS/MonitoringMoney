using MySql.Data.MySqlClient;
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
using MonitoringMoney;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace MonitoringMoney
{
    public partial class RegistrationForm : Form
    {
        DB dB;
        DataTable dataTable;
        MySqlDataAdapter dataAdapter;
        MySqlCommand cmd;
        Register_DB_API db_register;

        private bool isWindowOpened;
        public RegistrationForm()
        {
            InitializeComponent();

        }
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            dB = new DB();
            db_register = new Register_DB_API();
            dB.mySqlConnection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=debtorddatabase");
            dataTable = new DataTable();
            dataAdapter = new MySqlDataAdapter();
            
            cmd = new MySqlCommand("SELECT * FROM users WHERE `Username` = @username AND `Password` = @password", dB.getConnection());

            string user_name = this.user_box.Text;
            string user_password = this.password_box.Text;

            if (!db_register.UserExists(user_name))
            {
                MessageBox.Show("Данный пользователь не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = user_name;
                cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = user_password;

                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    string path = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName + @"\username.txt";

                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        writer.Write(user_name + "_data");
                    }
                    this.Hide();
                    var mainForm = new MainForm();
                    mainForm.ShowDialog();
                    dB.CloseConnectionSQL();


                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                }
            }
        }
        private void bunifuCheckBox1_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {
            password_box.PasswordChar = password_check.Checked ? '\0' : '●';
        }

        private void RegistrationForm_C(object sender, EventArgs e)
        {

        }

        private void dontHaveAccount_Click(object sender, EventArgs e)
        {

            isWindowOpened = false;


            Register register = new Register();
            register.ShowDialog();
           
        }

        private void RegistrationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {

        }
    }
}