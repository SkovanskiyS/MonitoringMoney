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
    public partial class password : Form
    {
        public bool status;
        public password()
        {
            InitializeComponent();
        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Check();

        }

        public void Check()
        {
            if (bunifuTextBox1.Text == "198814")
            {
                status = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Невереный пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                status = false;
            }

        }

        private void password_Load(object sender, EventArgs e)
        {

        }
    }
}
