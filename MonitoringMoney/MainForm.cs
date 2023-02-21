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

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        string valor;

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

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            sumValue.Text = string.Format("{0:C}", double.Parse(sumValue.Text.Replace("R$", "")));
        }
    }
}
