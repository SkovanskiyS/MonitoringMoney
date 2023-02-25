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

namespace MonitoringMoney
{
    public partial class RegistrationForm : Form
    {
        DB dB;
        DataTable dataTable;
        MySqlDataAdapter dataAdapter;
        MySqlCommand cmd;
        public RegistrationForm()
        {
            InitializeComponent();
        }
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            dB = new DB();
            dB.mySqlConnection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=log_pass");
            dataTable = new DataTable();
            dataAdapter = new MySqlDataAdapter();
            
            cmd = new MySqlCommand("SELECT * FROM `log_pass` WHERE `login` = @username AND `password` = @password", dB.getConnection());

            string user_name = this.user_box.Text;
            string user_password = this.password_box.Text;

            cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = user_name;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = user_password;

            dataAdapter.SelectCommand = cmd;
            dataAdapter.Fill(dataTable);

            if(dataTable.Rows.Count > 0 )
            {
                this.Hide();
                var mainForm = new MainForm();
                mainForm.ShowDialog();
                dB.CloseConnectionSQL();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!","Ошибка",MessageBoxButtons.RetryCancel,MessageBoxIcon.Error);
            }
        }
        private void bunifuCheckBox1_CheckedChanged(object sender, Bunifu.UI.WinForms.BunifuCheckBox.CheckedChangedEventArgs e)
        {
            password_box.PasswordChar = password_check.Checked ? '\0' : '●';
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {

        }
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Determine if text has changed in the textbox by comparing to original text.
            Application.Exit();
        }
    }
}