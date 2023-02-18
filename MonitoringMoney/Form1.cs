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
    public partial class Form1 : Form
    {
        DataBase data = new DataBase();

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
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
                }
            }
        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(0);
        }

        private void bunifuButton22_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(1);
        }

        private void bunifuButton23_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(2);
        }

        private void bunifuButton24_Click(object sender, EventArgs e)
        {
            main_menu.SetPage(3);
        }

        private void bunifuPanel5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

