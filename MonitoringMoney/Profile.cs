using Bunifu.UI.WinForms;
using MonitoringMoney.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.InteropServices;

namespace MonitoringMoney
{
    public partial class Profile : Form
    {
        Profile_DB_API db_api;
        DB_API dataBase;
        private int user_id;
        private Dictionary<object, double> most, lowest;
        private bool filter_ON;
        private const int WM_CLOSE = 0x10;

        // Import the user32.dll library
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

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
            Change_Text_All_Time("Взял (одолжил)", label_all_spends, "-");
            spendGridView.DataSource = db_api.Spends_Income("Взял (одолжил)");
            incomeGridView.DataSource = db_api.Spends_Income("Дал (занял)");
            ChangeColumn(spendGridView);
            ChangeColumn(incomeGridView);
            SetMyCustomFormat();
            Load_Third_P_Data();
            Load_Fourth_Page();
            LoadPic();

        }
        public void SetMyCustomFormat()
        {
            // Set the Format type and the CustomFormat string.


        }

        private void LoadPic()
        {
            string path = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName + @"\picLoc.txt";

            using (var read = new StreamReader(path))
            {

                string loc = read.ReadToEnd();

                if (loc.Length!=0)
                {
                    profile_pic.ImageLocation = loc;
                    pictureBox5.ImageLocation = loc;

                }
                else
                {
                    Image myImage = Resources.profile;
                    pictureBox5.Image = myImage;
                    profile_pic.Image = myImage;
                }
            }
        }

        private void Load_Fourth_Page()
        {
            dataBase = new DB_API();

            allDataGridView.DataSource = dataBase.LoadAllData();
        }



        private void Load_Third_P_Data()
        {
            var dict = db_api.Load_Page_3();
            name.Text = dict[1];
            surname.Text = dict[2];
            company_name.Text = dict[3];
            username.Text = dict[4];
            budget_integer.Text = dict[4];

            if (dict[1].Length>0)
            {
                companyName.Text = dict[1];
            }
            else
            {
                companyName.Text = dict[3];
            }

            Profile_DB_API dB_API = new Profile_DB_API();
 
            double spends = Get_All_Spends("Взял (одолжил)");
            double get = Get_All_Spends("Дал (занял)");
            int currency = 0;
            string path = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName + @"\currency.txt";

            using (var reader = new StreamReader(path))
            {
                currency = int.Parse(reader.ReadToEnd());
            }

            double income = 0;
            double spends_ = 0;

            if (dict[6].Contains("сум"))
            {
               
                double budget = double.Parse(dict[6].Replace("сум", "").Replace(",", ""));

                budget_integer.Text = budget.ToString("#,#"+" сум", CultureInfo.InvariantCulture);
                income = (get * currency);
                spends_ = (spends * currency);

                get_text.Text = "+"+income.ToString("#,#"+" сум", CultureInfo.InvariantCulture);
                spends_text.Text = "-"+spends_.ToString("#,#"+" сум", CultureInfo.InvariantCulture);

                bunifbudget_integeruLabel27.Text = (budget + income - spends_).ToString("#,#"+" сум",CultureInfo.InvariantCulture);
            }
            else
            {

                double budget = double.Parse(dict[6].Replace("$", "").Replace(",", ""));

                income = (get);
                spends_ = (spends);

                budget_integer.Text = budget.ToString("#,#" + " $", CultureInfo.InvariantCulture);
                get_text.Text = "+"+income.ToString("#,#"+" $", CultureInfo.InvariantCulture);
                spends_text.Text = "-"+spends_.ToString("#,#"+" $", CultureInfo.InvariantCulture);

                bunifbudget_integeruLabel27.Text = (budget + get - spends).ToString("#,#"+" $", CultureInfo.InvariantCulture);
            }



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
                    dataPanel.Location = new Point(0, Y += 1);
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
            if (series != null)
            {
                columnChart.Series["Users"].Points.Clear();
                barChart.Series["Users"].Points.Clear();
            }

            //var all_data = db_api.Get_Name_And_Amount();
            int i = 0;
            foreach (var item in most)
            {
                columnChart.Series["Users"].Points.AddXY(item.Key, Math.Round(item.Value,2));
                columnChart.Series["Users"].Points[i].Label = Math.Round(item.Value,2).ToString();
                i++;
            }
            int a = 0;
            foreach (var item in lowest)
            {
                barChart.Series["Users"].Points.AddXY(item.Key, Math.Round(item.Value, 2));
                barChart.Series["Users"].Points[a].Label = Math.Round(item.Value, 2).ToString();
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
                chart1.Series["Users"].Points.AddXY(item.Key, Math.Round(item.Value, 2));
                chart1.Series["Users"].Points[i].Label = Math.Round(item.Value, 2).ToString();
                i++;
            }
            int a = 0;
            foreach (var item in lowest)
            {
                chart2.Series["Users"].Points.AddXY(item.Key, Math.Round(item.Value, 2));
                chart2.Series["Users"].Points[a].Label = Math.Round(item.Value, 2).ToString();
                a++;
            }
        }

        private void FilerData(string data_to_get)
        {
            Dictionary<object, double> all_data = new Dictionary<object, double>();


            if (filter_ON)
            {
                if (data_to_get == "Взял (одолжил)")
                    all_data = db_api.Get_Data_By_DateTime(dateFrom.Value.Date, dateTo.Value.Date, data_to_get);

                else if(data_to_get == "Дал (занял)")
                    all_data = db_api.Get_Data_By_DateTime(from_date_picker_p2.Value.Date, to_date_picker_p2.Value.Date, data_to_get);
            }
            else all_data = db_api.Get_Name_And_Amount(data_to_get);


            most = new Dictionary<object, double>();
            lowest = new Dictionary<object, double>();
            //var sortedDict = from entry in all_data orderby entry.Value descending select entry;
            var sortedDict = all_data.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);


            int counts = sortedDict.Count <= 10 ? sortedDict.Count : counts = 12;

            for (int i = 0; i < counts; i++)
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

        private double Get_All_Spends(string data_to_get)
        {
            double amount = 0;
            Dictionary<object, double> all_data = new Dictionary<object, double>();

            if (filter_ON)
            {
                if (data_to_get == "Взял (одолжил)")
                    all_data = db_api.Get_Data_By_DateTime(dateFrom.Value.Date, dateTo.Value.Date, data_to_get);

                else if (data_to_get == "Дал (занял)")
                    all_data = db_api.Get_Data_By_DateTime(from_date_picker_p2.Value.Date, to_date_picker_p2.Value.Date, data_to_get);
            }
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
            Change_Text_All_Time("Дал (занял)", whole_sum, "+");
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
            if (filter_ON) { filter_ON = false; }
            filter_ON = true;
            double spends = Get_All_Spends("Взял (одолжил)");
            if (spends == 0) label_all_spends.Text = "0";
            else
            {
                label_all_spends.Text = "-" + (spends).ToString("#,#", CultureInfo.InvariantCulture) + "$ | -" + $"{(spends * db_api.currency).ToString("#,#", CultureInfo.InvariantCulture)} сум";
                bunifuLabel2.Text = $"Ваш общий расход состовляет: ";
            }
            Render_BarChart();
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            filter_ON = false;
            bunifuLabel2.Text = "Ваш общий расход за всё время состовляет: ";

            Render_BarChart();
            Change_Text_All_Time("Взял (одолжил)", label_all_spends, "-");

            var dateToday = DateTime.Now;
            dateFrom.Value = DateTime.Parse(dateToday.ToShortDateString());
            dateTo.Value = DateTime.Parse(dateToday.ToShortDateString());
        }

        private void Change_Text_All_Time(string get_text, BunifuLabel label, string plus_or_minus)
        {
            double spends = Get_All_Spends(get_text);

            if (spends == 0) label.Text = "0";
            else label.Text = plus_or_minus + spends.ToString("#,#", CultureInfo.InvariantCulture) + $"$ | {plus_or_minus}" + $"{(spends * db_api.currency).ToString("#,#", CultureInfo.InvariantCulture)} сум";
        }

        private void apply_btn_p2_Click(object sender, EventArgs e)
        {
            if (filter_ON) { filter_ON = false; }
            filter_ON = true;
            double spends = Get_All_Spends("Дал (занял)");

            if (spends == 0) whole_sum.Text = "0";
            else
            {

                whole_sum.Text = "+" + (spends).ToString("#,#", CultureInfo.InvariantCulture) + "$ | +" + $"{(spends * db_api.currency).ToString("#,#", CultureInfo.InvariantCulture)} сум";
                bunifuLabel15.Text = $"Ваш общий долг или прибыль состовляет: ";
            }

            Render_SecondPage_Charts();

        }

        private void reset_btn_p2_Click(object sender, EventArgs e)
        {
            filter_ON = false;
            bunifuLabel2.Text = "Ваш общий долг или прибыль за всё время состовляет: ";
            Render_SecondPage_Charts();
            Change_Text_All_Time("Дал (занял)", whole_sum, "+");

            var dateToday = DateTime.Now;
            from_date_picker_p2.Value = DateTime.Parse(dateToday.ToShortDateString());
            to_date_picker_p2.Value = DateTime.Parse(dateToday.ToShortDateString());

        }

        private void bunifuButton23_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(2);
        }

        private void whole_sum_Click(object sender, EventArgs e)
        {

        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            dataBase = new DB_API(searchTextBox.Text);
            if (searchTextBox.Text.Length > 0)
            {
                allDataGridView.DataSource = dataBase.Search(searchTextBox.Text);
            }
            else
            {
                allDataGridView.DataSource = dataBase.LoadAllData();
            }

        }

        private void bunifuIconButton1_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            allDataGridView.DataSource = dataBase.LoadAllData();
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }



        private void bunifuButton24_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(3);
        }

        private void update_btn_Click(object sender, EventArgs e)
        {
            try
            {
                var selected_index = allDataGridView.CurrentCell.RowIndex;
            }
            catch (Exception) { MessageBox.Show("Пустое значение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
              

            db_api.Update_Data(user_id, dateOfReg.Value, clientNameT.Text, get_giveDropdown.Text, currency_Dropdown.Text, sumValue.Text, wellText.Text, cash_transfer.Text, descriptionText.Text);

            allDataGridView.DataSource = dataBase.LoadAllData();
        }

        private void allDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            var selected_index = allDataGridView.CurrentCell.RowIndex;

            object id = allDataGridView.Rows[selected_index].Cells[0].Value;
            object dateTime = allDataGridView.Rows[selected_index].Cells[1].Value;
            string client = allDataGridView.Rows[selected_index].Cells[2].Value.ToString();
            string exchange = allDataGridView.Rows[selected_index].Cells[3].Value.ToString();
            string currency = allDataGridView.Rows[selected_index].Cells[4].Value.ToString();
            string sum = allDataGridView.Rows[selected_index].Cells[5].Value.ToString();
            string well = allDataGridView.Rows[selected_index].Cells[6].Value.ToString();
            string transaction = allDataGridView.Rows[selected_index].Cells[7].Value.ToString();
            string description = allDataGridView.Rows[selected_index].Cells[8].Value.ToString();


            dateOfReg.Value = Convert.ToDateTime(dateTime);
            clientNameT.Text = client;
            get_giveDropdown.Text = exchange;
            currency_Dropdown.Text = currency;
            sumValue.Text = sum;
            wellText.Text = well;
            cash_transfer.Text = transaction;
            descriptionText.Text = description;
            user_id = Convert.ToInt32(id);
        }

        private void delete_btn_Click(object sender, EventArgs e)
        {

            db_api.Delete_Data(user_id);

            allDataGridView.DataSource = dataBase.LoadAllData();
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            
        }

        private void bunifuButton1_Click_1(object sender, EventArgs e)
        {
            using (var openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.InitialDirectory = @"D:\";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    profile_pic.ImageLocation = openFileDialog1.FileName;
                    pictureBox5.ImageLocation = openFileDialog1.FileName;
                    string path = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName + @"\picLoc.txt";

                    using (var write = new StreamWriter(path))
                    {
                        write.Write(openFileDialog1.FileName);
                    }

                }
            }
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            Image myImage = Resources.profile;
            pictureBox5.Image = myImage;
            profile_pic.Image = myImage;
        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {


            if (MessageBox.Show("Вы уверены?", "Удалить", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                string path = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName + @"\username.txt";

                try
                {
                    if (File.Exists(path))
                    {
                        IntPtr window1 = FindWindow(null, "Основная форма");
                        IntPtr window2 = FindWindow(null, "Статистика");

                        // Close the two windows
                        if (window1 != IntPtr.Zero)
                        {
                            SendMessage(window1, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        }

                        if (window2 != IntPtr.Zero)
                        {
                            SendMessage(window2, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        }

                        // Open the third window
                        RegistrationForm thirdForm = new RegistrationForm();
                        thirdForm.Show();

                        File.Delete(path);
                      
                    }
                }
                catch (Exception)
                {

                    throw;
                }


            }
        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(0);
        }
    }
}