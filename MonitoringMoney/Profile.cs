﻿using Bunifu.Charts.WinForms;
using Bunifu.Dataviz.WinForms;
using Bunifu.UI.WinForms;
using IronPython.Runtime.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
        private bool filter_ON;

        public Profile()
        {
            InitializeComponent();
            db_api = new Profile_DB_API();
        }
        private void Profile_Load(object sender, EventArgs e)
        {
            FilerData("Взял (одолжил)");
            Render_BarChart();
            Render_SecondPage_Charts();
            Change_Text_All_Time("Взял (одолжил)",label_all_spends,"-");
            spendGridView.DataSource = db_api.Spends_Income("Взял (одолжил)");
            incomeGridView.DataSource = db_api.Spends_Income("Дал (занял)");
            ChangeColumn(spendGridView);
            ChangeColumn(incomeGridView);
            SetMyCustomFormat();
        }
        public void SetMyCustomFormat()
        {
            // Set the Format type and the CustomFormat string.

            filer_DatePicker.Format = DateTimePickerFormat.Custom;
            filer_DatePicker.CustomFormat = "MMMM yyyy";
            filer_DatePicker.ShowUpDown = true;
        }

        private void ChangeColumn(DataGridView dataGridView)
        {
            string[] columnNames = { "ID", "Дата", "Клиент", "Обмен", "Валюта", "Сумма", "Курс", "Транзакция", "Описание" };

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                dataGridView.Columns[i].HeaderText = columnNames[i];
            }
        }

        private void showUserBtn_Click(object sender, EventArgs e)
        {
            int Y = 0;
            int btnY = 0;
            if (showUserBtn.Text == "Скрыть всех")
            {
              //  spendGridView.Visible = false;
                showUserBtn.Text = "Показать всех";
    
                //663
                while (dataPanel.Location.Y != 680)
                {
                    Y = dataPanel.Location.Y;
                    btnY = showUserBtn.Location.Y;
                    dataPanel.Location = new Point(0, Y+=1);
                    showUserBtn.Location = new Point(4, btnY += 1);
                }
            }
            else
            {
                //  spendGridView.Visible = true;
                while (dataPanel.Location.Y != 511)
                {
                    Y = dataPanel.Location.Y;
                    btnY = showUserBtn.Location.Y;
                    dataPanel.Location = new Point(0, Y -= 1);
                    showUserBtn.Location = new Point(4, btnY -= 1);
                }
                showUserBtn.Text = "Скрыть всех";
            }
        }

        private void Render_BarChart()
        {
            FilerData("Взял (одолжил)");


            Series series = columnChart.Series.FindByName("Users");
            if (series!=null)
            {
                columnChart.Series["Users"].Points.Clear();
                barChart.Series["Users"].Points.Clear();
            }

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
        
        private void Render_SecondPage_Charts()
        {
            FilerData("Дал (занял)");

            Series series = columnChart.Series.FindByName("Users");
            if (series != null)
            {
                chart1.Series["Users"].Points.Clear();
                chart2.Series["Users"].Points.Clear();
            }

            int i = 0;
            foreach (var item in most)
            {
                chart1.Series["Users"].Points.AddXY(item.Key, item.Value);
                chart1.Series["Users"].Points[i].Label = item.Value.ToString();
                i++;
            }
            int a = 0;
            foreach (var item in lowest)
            {
                chart2.Series["Users"].Points.AddXY(item.Key, item.Value);
                chart2.Series["Users"].Points[a].Label = item.Value.ToString();
                a++;
            }
        }

        private void FilerData(string data_to_get)
        {
            Dictionary<object, int> all_data; 


            if (filter_ON)
                all_data = db_api.Get_Data_By_DateTime(dateFrom.Value.Date, dateTo.Value.Date, data_to_get);
            else all_data = db_api.Get_Name_And_Amount(data_to_get);


            most = new Dictionary<object, int>();
            lowest = new Dictionary<object, int>();
            //var sortedDict = from entry in all_data orderby entry.Value descending select entry;
            var sortedDict = all_data.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);


            int counts = sortedDict.Count <= 10? sortedDict.Count: counts = 12;

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

        private int Get_All_Spends(string data_to_get)
        {
            int amount = 0;
            Dictionary<object, int> all_data; 

            if (filter_ON)
                all_data = db_api.Get_Data_By_DateTime(dateFrom.Value.Date, dateTo.Value.Date, data_to_get);
            else all_data = db_api.Get_Name_And_Amount(data_to_get);

            foreach (var item in all_data.Values)
            {
                amount += item;
            }
            return amount;
        }

        private void bunifuButton22_Click(object sender, EventArgs e)
        {
            //load next page
            main_menu.SetPage(1);
            ChangeColumn(incomeGridView);
            Change_Text_All_Time("Дал (занял)", whole_sum,"+");
        }

        private void label_all_spends_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label_all_spends.Text);
            bunifuSnackbar1.Show(this, "Скопирован");
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            int Y = 0;
            int btnY = 0;
            if (show_hide_btnp2.Text == "Скрыть всех")
            {
                //  spendGridView.Visible = false;
                show_hide_btnp2.Text = "Показать всех";
                //663
                while (income_data_panel.Location.Y != 680)
                {
                    Y = income_data_panel.Location.Y;
                    btnY = show_hide_btnp2.Location.Y;
                    income_data_panel.Location = new Point(0, Y += 1);
                    show_hide_btnp2.Location = new Point(4, btnY += 1);
                }
            }
            else
            {
                //  spendGridView.Visible = true;
                while (income_data_panel.Location.Y != 511)
                {
                    Y = income_data_panel.Location.Y;
                    btnY = show_hide_btnp2.Location.Y;
                    income_data_panel.Location = new Point(0, Y -= 1);
                    show_hide_btnp2.Location = new Point(4, btnY -= 1);
                }
                show_hide_btnp2.Text = "Скрыть всех";
            }
        }

        private void applyBtn2_Click(object sender, EventArgs e)
        {
            if(filter_ON) { filter_ON = false; }
            filter_ON = true;
            int spends = Get_All_Spends("Взял (одолжил)");
            whole_sum.Text = "-" + (spends).ToString("#,#", CultureInfo.InvariantCulture) + "$ | -" + $"{(spends * db_api.currency).ToString("#,#", CultureInfo.InvariantCulture)} сум";
            bunifuLabel2.Text = $"Ваш общий расход за {filer_DatePicker.Value.ToShortDateString()} состовляет: ";
            Render_BarChart();
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            filter_ON = false;
            bunifuLabel2.Text = "Ваш общий расход за всё время состовляет: ";
            Render_BarChart();
            Change_Text_All_Time("Взял (одолжил)", label_all_spends,"-");
        }

        private void Change_Text_All_Time(string get_text,BunifuLabel label,string plus_or_minus)
        {
            int spends = Get_All_Spends(get_text);
            label.Text = plus_or_minus + spends.ToString() + $"$ | {plus_or_minus}" + $"{(spends * db_api.currency).ToString("#,#", CultureInfo.InvariantCulture)} сум";
        }

        private void apply_btn_p2_Click(object sender, EventArgs e)
        {
            if (filter_ON) { filter_ON = false; }
            filter_ON = true;
            int spends = Get_All_Spends("Дал (занял)");
            whole_sum.Text = "+" + (spends).ToString("#,#", CultureInfo.InvariantCulture) + "$ | +" + $"{(spends * db_api.currency).ToString("#,#", CultureInfo.InvariantCulture)} сум";
            bunifuLabel15.Text = $"Ваш общий долг или прибыль за {filer_DatePicker.Value.ToShortDateString()} состовляет: ";
            Render_SecondPage_Charts();

        }

        private void reset_btn_p2_Click(object sender, EventArgs e)
        {
            filter_ON = false;
            bunifuLabel2.Text = "Ваш общий долг или прибыль за всё время состовляет: ";
            Render_SecondPage_Charts();
            Change_Text_All_Time("Дал (занял)",whole_sum,"+");
        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(0);
        }
    }
}
