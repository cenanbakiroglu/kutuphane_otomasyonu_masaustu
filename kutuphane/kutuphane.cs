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
    public partial class kutuphane : Form
    {
        public kutuphane()
        {
            InitializeComponent();
        }
        islemler islem = new islemler();
        


        private void kutuphane_Load(object sender, EventArgs e)
        {
            // profil sayfası işlemleri

            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            button2.Visible = false;

            label11.Text = kullanici_bilgileri.id;
            label12.Text = kullanici_bilgileri.ad;
            label13.Text = kullanici_bilgileri.soyad;
            label14.Text = kullanici_bilgileri.kullaniciadi;
            label15.Text = kullanici_bilgileri.mail;
            label16.Text = kullanici_bilgileri.sifre;
            label17.Text = kullanici_bilgileri.gorev;
            label18.Text = kullanici_bilgileri.telefon;
            label19.Text = kullanici_bilgileri.tcno;
            label20.Text = kullanici_bilgileri.adres;



            // kitaplar sayfası işlemleri

            groupBox2.Visible = false;
            groupBox3.Visible = false;
            kitap_tablo_doldurma();

            //kullaıcı işlemleri sayfası
            
            kul_tablo_doldurma();
            groupBox4.Visible= false;
            groupBox5.Visible= false;
            groupBox6.Visible= false;

            // personel işlemleri sayfası

            if (kullanici_bilgileri.gorev == "memur")
            {
                tabControl1.TabPages.Remove(tabPage4);
            }
            else
            {
                pers_tablo_doldurma();
                textBox24.Text=islem.pers_kadi();
                textBox28.Text = "Memur";
                textBox30.Text = "12345678";
                groupBox7.Visible = false;
                groupBox8.Visible = false;
            }

            //kayit işlemleri sayfası
            kayit_tablo_doldurma();


            //Kütüphane bilgileri ve duyuru sayfası işlemleri
            islem.kutupbilgi_veri_cekme();
            textBox34.Text = islem.hakkimizda;
            maskedTextBox1.Text=islem.telefon;
            maskedTextBox2.Text=islem.fax;
            textBox37.Text = islem.adres;
            groupBox13.Visible = false;


            //dilekçe sayfası işlemleri
            dilekce_tablo_doldurma(); 

        }
        //PROFİL SAYFASI İŞLEMLERİ BAŞLANGIÇ

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            label15.Visible = false;
            label16.Visible = false;
            label18.Visible = false;
            label20.Visible = false;

            textBox1.Text = kullanici_bilgileri.mail;
            textBox2.Text = kullanici_bilgileri.sifre;
            textBox3.Text = kullanici_bilgileri.telefon;
            textBox4.Text = kullanici_bilgileri.adres;

        }

        private void button2_Click(object sender, EventArgs e)
        {


            islem.profil_duzenle(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);

            textBox2.Visible = false;
            textBox1.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            button2.Visible = false;
            label15.Visible = true;
            label16.Visible = true;
            label18.Visible = true;
            label20.Visible = true;
            button1.Visible = true;
            label11.Text = kullanici_bilgileri.id;
            label12.Text = kullanici_bilgileri.ad;
            label13.Text = kullanici_bilgileri.soyad;
            label14.Text = kullanici_bilgileri.kullaniciadi;
            label15.Text = kullanici_bilgileri.mail;
            label16.Text = kullanici_bilgileri.sifre;
            label17.Text = kullanici_bilgileri.gorev;
            label18.Text = kullanici_bilgileri.telefon;
            label19.Text = kullanici_bilgileri.tcno;
            label20.Text = kullanici_bilgileri.adres;
        }
        // PROFİL SAYFASI İŞLEMLERİ SONU


        //KİTAPLAR SAYFASI İŞLEMLERİ BAŞLANGIÇ

        private void kitap_tablo_doldurma()
        {
            islem.kitap_tablo();
            dataGridView1.DataSource = islem.tablo;
            dataGridView1.Columns["id"].HeaderText = "KİTAP ID";
            dataGridView1.Columns["kitapadi"].HeaderText = "KİTABIN ADI";
            dataGridView1.Columns["yazaradi"].HeaderText = "YAZARIN ADI";
            dataGridView1.Columns["yazarsoyadi"].HeaderText = "YAZARIN SOYADI";
            dataGridView1.Columns["yayinevi"].HeaderText = "YAYIN EVİ";
            dataGridView1.Columns["kitapturu"].HeaderText = "KİTABIN TÜRÜ";
            dataGridView1.Columns["basimyili"].HeaderText = "BASIM YILI";
            dataGridView1.Columns["kira"].HeaderText = "KİRA DURUMU";

            dataGridView1.CellFormatting += islem.kitap_tablo_donusturme;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                islem.kitap_ekle(textBox5.Text, textBox6.Text, textBox7.Text, textBox8.Text, textBox9.Text, Convert.ToInt32(textBox10.Text));
            }
            catch
            {
                MessageBox.Show("Basım yılı değerini istenen türde giriniz lütfen");
            }
            kitap_tablo_doldurma();


        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                islem.kitap_sil(Convert.ToInt32(textBox11.Text));
                kitap_tablo_doldurma();
            }
            catch
            {
                MessageBox.Show("Lütfen istenen türde bir değer giriniz");
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                groupBox2.Visible = true;
                groupBox3.Visible = false;

            }
            else if (comboBox1.SelectedIndex == 1)
            {
                groupBox3.Visible = true;
                groupBox2.Visible = false;
            }
            else
            { }
        }


        //KİTAPLAR SAYFASI İŞLEMLERİ SONU

        //KULLANICI İŞLEMLERİ SAYFASI BAŞLANGIÇ

        private void kul_tablo_doldurma()
        {
            islem.kul_doldur();
            dataGridView2.DataSource = islem.tablo;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                dataGridView2.Rows[i].Cells[5].Value = "********";
            }
            dataGridView2.Columns["id"].HeaderText = "KULLANICI ID";
            dataGridView2.Columns["ad"].HeaderText = "AD";
            dataGridView2.Columns["soyad"].HeaderText = "SOYAD";
            dataGridView2.Columns["kullaniciadi"].HeaderText = "KULLANICI ADI";
            dataGridView2.Columns["mail"].HeaderText = "E-POSTA ADRESİ";
            dataGridView2.Columns["sifre"].HeaderText = "ŞİFRE";
            dataGridView2.Columns["telefon"].HeaderText = "TELEFON";
            dataGridView2.Columns["adres"].HeaderText = "ADRES";
            dataGridView2.Columns["tarih"].HeaderText = "KAYIT TARİHİ";
            dataGridView2.Columns["kod"].HeaderText = "ŞİFRE SIFIRLAMA KODU";
           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                islem.kul_kayit(textBox12.Text, textBox13.Text, textBox14.Text, textBox15.Text, textBox16.Text, textBox17.Text, textBox18.Text, textBox19.Text);
            }
            catch
            {
                MessageBox.Show("hata");
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            islem.kul_sil(textBox20.Text);
            kul_tablo_doldurma();
            textBox20.Clear();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            islem.kul_sifre(textBox21.Text);
            kul_tablo_doldurma();
            textBox21.Clear();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                groupBox4.Visible = true;
                groupBox5.Visible = false;
                groupBox6.Visible = false;
            }
            else if (comboBox2.SelectedIndex == 1)
            {
                groupBox4.Visible = false;
                groupBox5.Visible = true;
                groupBox6.Visible = false;
            }
            else if (comboBox2.SelectedIndex == 2)
            {
                groupBox4.Visible = false;
                groupBox5.Visible = false;
                groupBox6.Visible = true;
            }

        }

        //KULLANICI İŞLEMLERİ SAYFASI SONU

        //PERSONEL İŞLEMLERİ SAYFASI BAŞLANGIÇ

        private void pers_tablo_doldurma()
        {
            islem.pers_doldur();
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                dataGridView3.Rows[i].Cells[5].Value = "********";
            }
            dataGridView3.DataSource = islem.tablo;
            dataGridView3.Columns["id"].HeaderText = "PERSONEL ID";
            dataGridView3.Columns["ad"].HeaderText = "AD";
            dataGridView3.Columns["soyad"].HeaderText = "SOYAD";
            dataGridView3.Columns["kullaniciadi"].HeaderText = "KULLANICI ADI";
            dataGridView3.Columns["mail"].HeaderText = "E-POSTA";
            dataGridView3.Columns["sifre"].HeaderText = "ŞİFRE";
            dataGridView3.Columns["gorev"].HeaderText = "GÖREV";
            dataGridView3.Columns["telefon"].HeaderText = "TELEFON";
            dataGridView3.Columns["tcno"].HeaderText = "TC NO";
            dataGridView3.Columns["adres"].HeaderText = "ADRES";
            dataGridView3.Columns["kayit_tarih"].HeaderText = "İŞE BAŞLAMA TARİHİ";


        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == 0)
            {
                groupBox7.Visible = true;
                groupBox8.Visible = false;
            }
            else if (comboBox3.SelectedIndex == 1)
            {
                groupBox7.Visible = false;
                groupBox8.Visible = true;
            }
            else { }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            islem.pers_kayit(textBox22.Text, textBox23.Text, textBox24.Text, textBox25.Text, textBox26.Text, textBox27.Text, textBox28.Text, textBox29.Text, textBox30.Text);
            pers_tablo_doldurma();
            textBox24.Text = islem.pers_kadi();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            islem.pers_sil(textBox31.Text);
            pers_tablo_doldurma();
        }

        //PERSONEL İŞLEMLERİ SAYFASI SONU

        //KİRALAMA İŞLEMLERİ SAYFASI BAŞLANGIÇ

        private void kayit_tablo_doldurma()
        {
            islem.kayit_doldur();
            dataGridView4.DataSource = islem.tablo;
            dataGridView4.Columns["id"].HeaderText = "KAYIT ID";
            dataGridView4.Columns["kullaniciad"].HeaderText = "KULLANICI AD";
            dataGridView4.Columns["kullanicisoyad"].HeaderText = "KULLANICI SOYAD";
            dataGridView4.Columns["kullaniciadi"].HeaderText = "KULLANICI ADI";
            dataGridView4.Columns["kitapid"].HeaderText = "KİTAP ID";
            dataGridView4.Columns["kitapadi"].HeaderText = "KİTAP AD";
            dataGridView4.Columns["kayittarihi"].HeaderText = "KAYIT TARİHİ";
            dataGridView4.Columns["teslimtarihi"].HeaderText = "TESLİM TARİHİ";
            dataGridView4.Columns["teslimdurumu"].HeaderText = "TESLİM DURUMU";
            dataGridView4.Columns["personelad"].HeaderText = "KAYIT YAPAN PERSONEL AD";
            dataGridView4.Columns["personelsoyad"].HeaderText = "KAYIT YAPAN PERSONEL SOYAD";


        }

        private void button11_Click(object sender, EventArgs e)
        {
            islem.kitap_kirala(textBox32.Text, Convert.ToInt32(textBox33.Text));
            kayit_tablo_doldurma();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            islem.kitap_teslim(textBox32.Text, Convert.ToInt32(textBox33.Text));
            kayit_tablo_doldurma();
        }

        //KİRALAMA İŞLEMLERİ SAYFASI SONU

        //KÜTÜPHANE BİLGİLERİ VE DUYURU İŞLEMLERİ SAYFASI BAŞLANGIÇ

        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                islem.duyuru_sil(Convert.ToInt32(textBox38.Text));
                islem.duyuru_doldur();

                dataGridView5.DataSource = islem.tablo;
                dataGridView5.Columns["id"].HeaderText = "DUYURU ID";
                dataGridView5.Columns["baslik"].HeaderText = "DUYURU BAŞLIK";
                dataGridView5.Columns["icerik"].HeaderText = "DUYURU İÇERİK";
                dataGridView5.Columns[2].Width = 1000;
            }
            catch
            {
                MessageBox.Show("Lütfen istenen türde veri girişi yapınız");
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            islem.kutupbilgi_guncelleme(textBox34.Text, maskedTextBox1.Text, maskedTextBox2.Text, textBox37.Text);
            islem.kutupbilgi_veri_cekme();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            islem.kutupbilgi_guncelleme(textBox34.Text, maskedTextBox1.Text, maskedTextBox2.Text, textBox37.Text);
            islem.kutupbilgi_veri_cekme();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            islem.duyuru(textBox35.Text, textBox36.Text);
            textBox35.Clear();
            textBox36.Clear();

        }

        private void button15_Click(object sender, EventArgs e)
        {
            islem.duyuru_doldur();

            dataGridView5.DataSource = islem.tablo;
            dataGridView5.Columns["id"].HeaderText = "DUYURU ID";
            dataGridView5.Columns["baslik"].HeaderText = "DUYURU BAŞLIK";
            dataGridView5.Columns["icerik"].HeaderText = "DUYURU İÇERİK";
            dataGridView5.Columns[2].Width = 1000;

            groupBox13.Visible = true;

        }

        private void button17_Click(object sender, EventArgs e)
        {
            groupBox13.Visible = false;
        }

        //KÜTÜPHANE BİLGİLERİ VE DUYURU İŞLEMLERİ SAYFASI SONU

        //DİLEKÇELER SAYFASI İŞLEMLERİ BAŞLANGIÇ

        private void dilekce_tablo_doldurma()
        {
            islem.dilekce_doldur();
            dataGridView6.DataSource = islem.tablo;
            dataGridView6.Columns["id"].HeaderText = "DİLEKÇE ID";
            dataGridView6.Columns["dilekce"].HeaderText = "DİLEKÇCE";
            dataGridView6.Columns["dilekce_turu"].HeaderText = "DİLEKÇE TÜRÜ";
            dataGridView6.Columns["kullanici_kadi"].HeaderText = "GÖNDEREN KULLANICI";
            dataGridView6.Columns[1].Width = 1000;
        }

        //DİLEKÇELER SAYFASI İŞLEMLERİ SONU


    }
}
