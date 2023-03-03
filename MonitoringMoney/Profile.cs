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
    public partial class Profile : Form
    {
        Profile_DB_API db_api;
        public Profile()
        {
            InitializeComponent();
            db_api = new Profile_DB_API();
        }
        private void Profile_Load(object sender, EventArgs e)
        {
            spendGridView.DataSource = db_api.Spends();
            MessageBox.Show(db_api.WholeSpends().ToString());
            ChangeColumn();

        }

        private void bunifuLabel4_Click(object sender, EventArgs e)
        {

        }

        private void spendGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ChangeColumn()
        {
            string[] columnNames = { "ID", "Дата", "Клиент", "Обмен", "Валюта", "Сумма", "Курс", "Транзакция", "Описание" };

            for (int i = 0; i < spendGridView.Columns.Count; i++)
            {
                spendGridView.Columns[i].HeaderText = columnNames[i];
            }
        }

        private void bunifuLabel3_Click(object sender, EventArgs e)
        {

        }
    }
}
