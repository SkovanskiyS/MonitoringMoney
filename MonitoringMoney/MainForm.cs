using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MySql.Data.MySqlClient;
using Microsoft.Office.Interop;


namespace MonitoringMoney
{
    public partial class MainForm : Form
    {
        DB dB;
        DataTable dataTable;
        MySqlDataAdapter dataAdapter;

        BindingSource bindingSource;
        DB_API dataBase;
        private bool isWindowOpened;
        private int currency;
        public string user_data_table;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            dB = new DB();
            dataTable = new DataTable();
            dataAdapter = new MySqlDataAdapter();
            dB.mySqlConnection = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=debtorddatabase");
            dataBase = new DB_API();
            allDataGridView.DataSource = dataBase.LoadAllData();
            ChangeColumnName();
            countOfUsersLabel.Text = Convert.ToString(allDataGridView.RowCount);
            this.KeyPreview = true;
            GetCurrency();
        }
        public void GetCurrency()
        {
            try
            {
                var timeoutInMilliseconds = 5000;
                var uri = new Uri("https://bank.uz/currency");
                var doc = Supremes.Dcsoup.Parse(uri, timeoutInMilliseconds);
                var ratingSpan = doc.Select("span[class=medium-text]");
                double d_currency = double.Parse(ratingSpan.Text.Substring(23, 9).Replace(".", ","));
                currency = Convert.ToInt32(d_currency);
                wellText.Text = Convert.ToInt32(d_currency).ToString();
            }
            catch (Exception)
            {
                currency = 0;
                MessageBox.Show("Не удалось загрузить курс доллара","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }

        }

        private void bunifuTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                if (e.KeyChar == ',')
                {
                    e.Handled = (sumValue.Text.Contains(","));
                }
                else e.Handled = true;
            }
        }
        private void changeText()
        {
            string selectedItem = currency_Dropdown.Text;
            string clear_value = sumValue.Text.Trim();
            try
            {
                //filter
                string[] remove_elements = { ",", "сум", "$" };
                foreach (var item in remove_elements) clear_value = clear_value.Replace(item, "");
                double value = Convert.ToDouble(clear_value);
                sumValue.Text = selectedItem == "SUM (UZS)" ? value.ToString("#,#" + " сум", CultureInfo.InvariantCulture) : value.ToString("#,#" + " $", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                if (!(clear_value.Length == 0)) { MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error); }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            changeText();
        }

        private void wellText_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void cleanBtn_Click(object sender, EventArgs e)
        {
            object[] textboxValues = { sumValue, clientNameT, wellText};
            foreach (BunifuTextBox item in textboxValues)
            {
                item.Text = "";
            }
            descriptionText.Text = "";
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("hello world");
        }
        private void sumValue_Leave(object sender, EventArgs e)
        {
            try
            {
                changeText();
            }
            catch (Exception)
            {
                changeText();
            }
        }

        public void ChangeColumnName()
        {

            string[] columnNames = { "ID", "Дата", "Клиент", "Обмен", "Валюта", "Сумма", "Курс", "Транзакция","Описание"};

            if (allDataGridView.Columns.Count>0)
            {
                for (int i = 0; i < allDataGridView.Columns.Count; i++)
                {
                    allDataGridView.Columns[i].HeaderText = columnNames[i];
                }
            }

        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) addBtn.PerformClick();
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

        private void give_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            FilterCheckBox();
        }

        private void FilterCheckBox()
        {
            if(get_checkbox.Checked && give_checkbox.Checked||!get_checkbox.Checked&&!give_checkbox.Checked)
            {
                allDataGridView.DataSource = dataBase.LoadAllData();
            }
            else
            {
                allDataGridView.DataSource = get_checkbox.Checked? dataBase.FilterGetOrGive(true) : allDataGridView.DataSource = dataBase.FilterGetOrGive(false);
            }
        }

        private void get_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            FilterCheckBox();
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            allDataGridView.DataSource = dataBase.FilterByDate(dateFrom.Value.Date, dateTo.Value.Date, dataBase.Search("undefined"));
        }

        private void resetDate_Click(object sender, EventArgs e)
        {
            allDataGridView.DataSource = dataBase.LoadAllData();
            var dateToday = DateTime.Now;
            dateFrom.Value = DateTime.Parse(dateToday.ToShortDateString());
            dateTo.Value = DateTime.Parse(dateToday.ToShortDateString());
        }

        private void allDataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
           countOfUsersLabel.Text = Convert.ToString(allDataGridView.RowCount);
        }

        private void applyBtn2_Click(object sender, EventArgs e)
        {
            allDataGridView.DataSource = dataBase.FilterByDate(dateFrom.Value.Date, dateTo.Value.Date, dataBase.Search("undefined"));

        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            allDataGridView.DataSource = dataBase.LoadAllData();
            var dateToday = DateTime.Now;
            dateFrom.Value = DateTime.Parse(dateToday.ToShortDateString());
            dateTo.Value = DateTime.Parse(dateToday.ToShortDateString());
        }

        private void addBtn_Click(object sender, EventArgs e)
        {

            string path = Directory.GetCurrentDirectory() + @"\currency.txt";

            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            using (var write = new StreamWriter(path))
            {
                write.Write(wellText.Text);
            }
            changeText();
            dataBase = new DB_API();
            dataBase.Insert(dateOfReg.Value.Date, clientNameT.Text, get_giveDropdown.Text, currency_Dropdown.Text, sumValue.Text, wellText.Text, cash_transfer.Text, descriptionText.Text, wellText.Enabled);
            allDataGridView.DataSource = dataBase.LoadAllData();
        }

        private void cleanData_Click(object sender, EventArgs e)
        {
            object[] textboxValues = { sumValue, clientNameT, wellText };
            foreach (BunifuTextBox item in textboxValues)
            {
                item.Text = "";
            }
            descriptionText.Text = "";
        }

        private void мойПрофильToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!isWindowOpened)
            {
                Profile profile = new Profile();
                profile.Show();
                isWindowOpened = true;
            }
            isWindowOpened = false;

        }
        private void bunifuIconButton1_Click(object sender, EventArgs e)
        {
            searchTextBox.Text = "";
            allDataGridView.DataSource = dataBase.LoadAllData();
        }
        private void wellText_Click(object sender, EventArgs e)
        {

            wellText.Text = currency.ToString();
        }

        private void bunifuButton1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void сохранитьКакToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (allDataGridView.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel |*.xlsx";
                sfd.FileName = "Output.xlsx";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportToExcel(allDataGridView, sfd.FileName);

                }
            }
        }

        private void ExportToPDF(DataGridView dataGridView, string filePath)
        {
            // Create a Document object
            PdfPTable table = new PdfPTable(dataGridView.Columns.Count);

            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    table.AddCell(new Phrase(dataGridView.Rows[i].Cells[j].Value.ToString()));
                }
            }
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(table);
                pdfDoc.Close();
                stream.Close();
            }
        }

        private void ExportToExcel(DataGridView dataGridView, string filePath)
        {
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            int i = 0;
            int j = 0;

            for (i = 0; i <= dataGridView.RowCount - 1; i++)
            {
                for (j = 0; j <= dataGridView.ColumnCount - 1; j++)
                {
                    DataGridViewCell cell = dataGridView[j, i];
                    xlWorkSheet.Cells[i + 1, j + 1] = cell.Value;
                }
            }
            xlWorkBook.SaveAs(filePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            MessageBox.Show("Excel file created , you can find the file c:\\csharp.net-informations.xls");

        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void sumValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void statisticDropDown_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChangePassword password = new ChangePassword();
            password.ShowDialog();
        }

        private void wellText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}


