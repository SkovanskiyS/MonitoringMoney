using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MonitoringMoney
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void bunifuTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                if (e.KeyChar == ',')
                {
                    e.Handled = (sumValue.Text.Contains(","));
                }
                else
                    e.Handled = true;
            }
        }

        private void changeText()
        {
            //double value = 0;
            //string selectedItemw
            //{
            //    if (selectedItem == "SUM (UZS)")
            //    {
            //        value = Convert.ToDouble(sumValue.Text.Replace(",", "").Replace("сум", ""));
            //        sumValue.Text = value.ToString("#,#" + " сум", CultureInfo.InvariantCulture);
            //    }
            //    else
            //    {
            //        value = Convert.ToDouble(sumValue.Text.Replace(",", "").Replace("$", ""));
            //        sumValue.Text = value.ToString("#,#" + " $", CultureInfo.InvariantCulture);
            //    }
            //}
            //catch (Exception e)
            //{

            //    MessageBox.Show(e.Message,"Ошибка",MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Error)
            //}
            string selectedItem = Convert.ToString(currency_Dropdown.SelectedItem);
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
            wellText.Enabled = Convert.ToString(currency_Dropdown.SelectedItem) == "USD $" ? true : false;
        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {

        }
    }
}


