using Bunifu.Charts.WinForms;
using Bunifu.Dataviz.WinForms;
using IronPython.Runtime.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static IronPython.Modules._ast;

namespace MonitoringMoney
{
    public partial class Profile : Form
    {
        Profile_DB_API db_api;
        private Dictionary<object,int> most,lowest;

        public Profile()
        {
            InitializeComponent();
            db_api = new Profile_DB_API();
        }
        private void Profile_Load(object sender, EventArgs e)
        {
            spendGridView.DataSource = db_api.Spends();
            ChangeColumn();
            FilerData();
            Render_BarChart();
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
                    showUserBtn.Location = new Point(4, btnY += 1);
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
                    showUserBtn.Location = new Point(4, btnY -= 1);
                }
                showUserBtn.Text = "Скрыть всех";
            }
        }

        private void Render_BarChart()
        {
            FilerData();
            //var all_data = db_api.Get_Name_And_Amount();
            int i = 0;
            foreach (var item in most)
            {
                columnChart.Series["Users"].Points.AddXY(item.Key, item.Value);
                columnChart.Series["Users"].Points[i].Label = item.Value.ToString();
                i++;
            }
            int a = 0;
            foreach (var item in lowest)
            {
                barChart.Series["Users"].Points.AddXY(item.Key, item.Value);
                barChart.Series["Users"].Points[a].Label = item.Value.ToString();
                a++;
            }

        }
        
        private void FilerData()
        {
            var all_data = db_api.Get_Name_And_Amount();
            most = new Dictionary<object, int>();
            lowest = new Dictionary<object, int>();
            //var sortedDict = from entry in all_data orderby entry.Value descending select entry;
            var sortedDict = all_data.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);


            int counts = sortedDict.Count <= 10? sortedDict.Count: counts = 15;

            for (int i=0 ; i < counts; i++)
            {
                var keyValuePair = sortedDict.ElementAt(i);
                if (i < 5)
                {
                    most.Add(keyValuePair.Key, keyValuePair.Value);
                }
                else
                {
                    lowest.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

        }

        private void bunifuLabel4_Click_1(object sender, EventArgs e)
        {

        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            Render_BarChart();
        }

        private void bunifuButton22_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(1);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(0);
        }
    }
}
