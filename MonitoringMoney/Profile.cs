using Bunifu.Charts.WinForms;
using Bunifu.Dataviz.WinForms;
using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
            FilerData();
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

        private void showUserBtn_Click(object sender, EventArgs e)
        {

            if (showUserBtn.Text == "Скрыть всех")
            {
              //  spendGridView.Visible = false;
                showUserBtn.Text = "Показать всех";
                //663
                while (dataPanel.Location.Y!=682)
                {
                    int Y = dataPanel.Location.Y;
                    int btnY = showUserBtn.Location.Y;
                    dataPanel.Location = new Point(0, Y+=1);
                    showUserBtn.Location = new Point(0, btnY += 1);
                }

            }
            else
            {
                //  spendGridView.Visible = true;
                while (dataPanel.Location.Y != 511)
                {
                    int Y = dataPanel.Location.Y;
                    int btnY = showUserBtn.Location.Y;
                    dataPanel.Location = new Point(0, Y -= 1);
                    showUserBtn.Location = new Point(0, btnY -= 1);
                }
                showUserBtn.Text = "Скрыть всех";
            }
        }

        private void Render_BarChart()
        {
            var all_data = db_api.Get_Name_And_Amount();
           
            for (int i = 0; i < all_data.Count; i++)
            {
                barChart.Series["Users"].Points.AddXY(all_data);
            }
        }
        
        private void FilerData()
        {
            var all_data = db_api.Get_Name_And_Amount();

            

        }


    }
}
