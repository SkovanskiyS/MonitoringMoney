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
namespace MonitoringMoney
{
    public partial class RegistrationForm : Form
    {
        DataBase dB;
        DataTable dataTable;
        MySqlDataAdapter dataAdapter;
        MySqlCommand cmd;

        public RegistrationForm()
        {
            InitializeComponent();
            dB = new DataBase();
            dataTable= new DataTable();
            dataAdapter= new MySqlDataAdapter();
            cmd = new MySqlCommand("SELECT * FROM `log_pass` WHERE `login` = @username AND `password` = @password", dB.getConnection());

        }

        private void bunifuUserControl1_Click(object sender, EventArgs e)
        {

        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            string user_name = this.user_box.Text;
            string user_password = this.password_box.Text;
            cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = user_name;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = user_password;
            dataAdapter.SelectCommand = cmd;
            dataAdapter.Fill(dataTable);
            
            if(dataTable.Rows.Count > 0 )
            {
                var mainForm = new Form1();
                this.Close();
                mainForm.Show();

            }
        }
    }
}
