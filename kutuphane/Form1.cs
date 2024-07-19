using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kutuphane
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            islemler islem = new islemler();
            string kadi, sifre;
            kadi = textBox1.Text;
            sifre = textBox2.Text;
            if (kadi.Length == 0 || sifre.Length == 0)
            {
                MessageBox.Show("Boş alan bırakmayınız");
            }
            else
            {
                islem.giris(kadi, sifre);
            }

            if (kullanici_bilgileri.id != null)
            {
                kutuphane kutup = new kutuphane();
                kutup.Show();
                this.Hide();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.cenan.lovestoblog.com/kutuphane/index.php?i=1");
        }
    }
}
