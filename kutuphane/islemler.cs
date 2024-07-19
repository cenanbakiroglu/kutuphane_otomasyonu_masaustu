using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using System.Windows.Forms.VisualStyles;
using System.Security.AccessControl;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.RegularExpressions;

namespace kutuphane
{
    internal class islemler
    {
        MySqlConnection conn;
        MySqlCommand komut;
        MySqlDataReader oku;
        MySqlDataAdapter liste;
        public DataTable tablo;

        public void giris(string kullanici_adi, string sifre)
        {


            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.CommandText = "SELECT * FROM personel WHERE kullaniciadi=\"" + kullanici_adi + "\" AND personeldurum=0";
            komut.Connection = conn;
            oku = komut.ExecuteReader();
            if (oku.Read() == true)
            {
                if (oku["sifre"].ToString() == MD5Hash(sifre))
                {
                    kullanici_bilgileri.id = oku["id"].ToString();
                    kullanici_bilgileri.ad = oku["ad"].ToString();
                    kullanici_bilgileri.soyad = oku["soyad"].ToString();
                    kullanici_bilgileri.kullaniciadi = oku["kullaniciadi"].ToString();
                    kullanici_bilgileri.mail = oku["mail"].ToString();
                    kullanici_bilgileri.sifre = sifre;
                    kullanici_bilgileri.gorev = oku["gorev"].ToString();
                    kullanici_bilgileri.telefon = oku["telefon"].ToString();
                    kullanici_bilgileri.tcno = oku["tcno"].ToString();
                    kullanici_bilgileri.adres = oku["adres"].ToString();
                    kullanici_bilgileri.kayit_tarihi = oku["kayit_tarih"].ToString();
                }
                else
                {
                    MessageBox.Show("Şifrenizi kontrol ediniz");
                }
            }
            else
            {
                MessageBox.Show("Böyle bir kullanıcı bulunmamaktadır");
            }
            conn.Close();

        }
        public void profil_duzenle(string mail, string sifre, string telefon, string adres)
        {




            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();

            try
            {
                conn.Open();
                komut.CommandText = "UPDATE personel SET mail=\"" + mail + "\",sifre=\"" + MD5Hash(sifre) + "\",telefon=\"" + telefon + "\",adres=\"" + adres + "\" WHERE id=" + kullanici_bilgileri.id;
                komut.Connection = conn;
                int rowsAffected = komut.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    kullanici_bilgileri.mail = mail;
                    kullanici_bilgileri.sifre = sifre;
                    kullanici_bilgileri.telefon = telefon;
                    kullanici_bilgileri.adres = adres;
                    MessageBox.Show("Değişikleriniz kaydedildi");

                }
                else
                {
                    MessageBox.Show("Bir hata oluştu");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorgu hatası uygulamayı yeniden başlatınız" + ex.Message);
            }

        }

        public void kitap_ekle(string kitap_adi, string yazar_adi, string yazar_soyadi, string yayinevi, string kitap_turu, int basim_yili)
        {
            if (kitap_adi.Length <= 0 || yazar_adi.Length <= 0 || yazar_soyadi.Length <= 0 || yayinevi.Length <= 0 || kitap_turu.Length <= 0)
            {
                MessageBox.Show("Lütfen boş alan bırakmayınız");
            }
            else if (basim_yili < 1500 || basim_yili > DateTime.Now.Year)
            {
                MessageBox.Show("Geçerli bir yıl giriniz");
            }
            else
            {

                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                kitap_adi = textInfo.ToTitleCase(kitap_adi.ToLower());
                yazar_adi = textInfo.ToTitleCase(yazar_adi.ToLower());
                yazar_soyadi = textInfo.ToTitleCase(yazar_soyadi.ToLower());
                yayinevi = textInfo.ToTitleCase(yayinevi.ToLower());
                kitap_turu = textInfo.ToTitleCase(kitap_turu.ToLower());

                conn = new MySqlConnection(baglanti.bag);
                komut = new MySqlCommand();
                try
                {
                    conn.Open();
                    komut.CommandText = "INSERT INTO kitaplar VALUES(null,'" + kitap_adi + "','" + yazar_adi + "','" + yazar_soyadi + "','" + yayinevi + "','" + kitap_turu + "','" + basim_yili + "','0','0')";
                    komut.Connection = conn;
                    int rowsAffected = komut.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {

                        conn.Close();

                        conn.Open();
                        string duyuru = "Okuyucularımızın dikkatine!!! < br > " + yazar_adi + " " + yazar_soyadi + " adlı yazarın," + kitap_adi + " adlı kitabı kütüphanemize gelmiştir.Tüm okuyucularımıza duyurulur.";
                        komut.CommandText = "INSERT INTO duyurular VALUES(null,'" + kitap_adi + "','" + duyuru + "')";
                        komut.Connection = conn;
                        komut.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Kitap kayıt işlemi başarılı");
                    }
                    else
                    {
                        MessageBox.Show("Bir hata oluştu");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sorgu hatası uygulamayı yeniden başlatınız" + ex.Message);
                }

            }

        }
        public void kitap_tablo()
        {

            conn = new MySqlConnection(baglanti.bag);
            conn.Open();
            liste = new MySqlDataAdapter("select * from kitaplar where kitapdurum=0", conn);
            tablo = new DataTable("veriler");
            liste.Fill(tablo);
            conn.Close();


        }
        public void kitap_tablo_donusturme(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(string) && e.Value.ToString() == "0")
            {
                e.Value = "Müsait";
            }
            else if (e.Value != null && e.Value.GetType() == typeof(string) && e.Value.ToString() == "1")
            {
                e.Value = "Müsait Değil";
            }
            else
            { }
        }

        public void kul_kayit(string ad, string soyad, string kadi, string mail, string telefon, string adres, string parola, string parola2)
        {
            int kadi_err = 0, mail_err = 0;
            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();

            //kullanıcı adı sorgusu
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "SELECT * FROM kullanicilar WHERE kullaniciadi='" + kadi + "'";
            oku = komut.ExecuteReader();

            if (oku.Read() == true)
            {
                kadi_err = 1;
            }
            conn.Close();
            //

            // mail adresi sorgu
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "SELECT * FROM kullanicilar WHERE mail='" + mail + "'";
            oku = komut.ExecuteReader();

            if (oku.Read() == true)
            {
                mail_err = 1;
            }
            conn.Close();
            //


            if (ad.Length <= 0 || soyad.Length <= 0 || kadi.Length <= 0 || mail.Length <= 0 || telefon.Length <= 0 || adres.Length <= 0 || parola.Length <= 0 || parola2.Length <= 0)
            {
                MessageBox.Show("Boş alan bırakmayınız lütfen");
            }
            else if (parola != parola2)
            {
                MessageBox.Show("Parolalar uyuşmuyor");
            }
            else if (!Regex.IsMatch(kadi, @"^[a-z\d_-]{5,20}$", RegexOptions.IgnoreCase))
            {
                MessageBox.Show("Girdiğiniz kullanıcı adı geçerli formatta değil.");
            }
            else if (kadi_err == 1)
            {
                MessageBox.Show("Bu kullanıcı adı daha önce kullanılmıştır");
            }
            else if (mail_err == 1)
            {
                MessageBox.Show("Bu e-posta adresi daha önce kullanılmıştır");
            }
            else if (mail.IndexOf("@") == -1 || mail.IndexOf(".com") == -1)
            {
                MessageBox.Show("Geçersiz mail adresi");
            }
            else
            {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                ad = textInfo.ToTitleCase(ad.ToLower());
                soyad = textInfo.ToTitleCase(soyad.ToLower());
                conn.Open();
                komut.Connection = conn;
                komut.CommandText = "INSERT INTO kullanicilar VALUES(null,'" + ad + "','" + soyad + "','" + kadi + "','" + mail + "','" + MD5Hash(parola) + "','" + telefon + "','" + adres + "',CURRENT_DATE,null,0)";

                int etki = komut.ExecuteNonQuery();
                if (etki > 0)
                {
                    MessageBox.Show("Kullanıcı kaydı başarı ile gerçekleştirildi");
                }
                else
                {
                    MessageBox.Show("Kayıt sırasında hata oluştu programı yeniden başlatınız");
                }
            }



        }

        public void kitap_sil(int id)
        {
            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "UPDATE kitaplar SET kitapdurum=1 WHERE id=" + id;
            int rowsAffected = komut.ExecuteNonQuery();
            if (rowsAffected > 0)
            {


                MessageBox.Show("Kitap kütüphane listesinden kaldırıldı");

            }
            else
            {
                MessageBox.Show("Bir hata oluştu");
            }
            conn.Close();

        }
        public void kul_doldur()
        {
            conn = new MySqlConnection(baglanti.bag);
            conn.Open();
            liste = new MySqlDataAdapter("select * from kullanicilar where kullanicidurum=0", conn);
            tablo = new DataTable("veriler");
            liste.Fill(tablo);
            conn.Close();

        }

        public void kul_sil(string kadi)
        {
            if (kadi.Length < 5)
            {
                MessageBox.Show("Geçerli bir kullanıcı adı giriniz");
            }
            else
            {
                conn = new MySqlConnection(baglanti.bag);
                komut = new MySqlCommand();
                conn.Open();
                komut.CommandText = "UPDATE kullanicilar SET kullanicidurum=1, sifre='"+MD5Hash("kütüphane")+"'WHERE kullaniciadi='" + kadi + "'";
                komut.Connection = conn;
                int rowsAffected = komut.ExecuteNonQuery();
                if (rowsAffected > 0)
                {

                    MessageBox.Show("Kullanıcı silme işlemi tamamlandı");

                }
                else
                {
                    MessageBox.Show("Kullanıcı adı bulunamadı");
                }
            }
        }

        public void kul_sifre(string kadi)
        {
            if (kadi.Length < 5)
            {
                MessageBox.Show("Geçerli bir kullanıcı adı giriniz");
            }
            else
            {
                Random kod = new Random();
                int skodu = kod.Next(10000000, 99999999);

                conn = new MySqlConnection(baglanti.bag);
                komut = new MySqlCommand();
                conn.Open();
                komut.CommandText = "UPDATE kullanicilar SET kod=" + skodu + " WHERE kullaniciadi='" + kadi + "'";
                komut.Connection = conn;
                int rowsAffected = komut.ExecuteNonQuery();
                if (rowsAffected > 0)
                {

                    MessageBox.Show("Kullanıcının şifre sıfırlama kodu:" + skodu);

                }
                else
                {
                    MessageBox.Show("Kullanıcı adı bulunamadı");
                }
            }
        }

        public void pers_doldur()
        {
            conn = new MySqlConnection(baglanti.bag);
            conn.Open();
            liste = new MySqlDataAdapter("select * from personel where personeldurum=0", conn);
            tablo = new DataTable("veriler");
            liste.Fill(tablo);
            conn.Close();

        }
        //otomatik pers kullanıcı adı belirleme
        string kadi;
        public string pers_kadi()
        {

            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();

            conn.Open();
            komut.CommandText = "SELECT COUNT(id) AS sayi FROM personel";
            komut.Connection = conn;
            oku = komut.ExecuteReader();
            if (oku.Read() == true)
            {
                string sayi = oku["sayi"].ToString();
                kadi = "memur" + sayi;
            }
            conn.Close();
            return kadi;
        }

        public void pers_kayit(string ad, string soyad, string kadi, string mail, string telefon, string tc, string gorev, string adres, string sifre)
        {




            if (ad.Length < 0 || soyad.Length < 0 || mail.Length < 0 || telefon.Length < 0 || tc.Length < 0 || adres.Length < 0)
            {
                MessageBox.Show("Boş alan bırakmayınız lütfen");
            }
            else if (tc.Length != 11)
            {
                MessageBox.Show("T.C. kimlik no geçersiz");
            }
            else if (mail.IndexOf("@") == -1 || mail.IndexOf(".com") == -1)
            {
                MessageBox.Show("Geçersiz mail adresi");
            }
            else
            {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                ad = textInfo.ToTitleCase(ad.ToLower());
                soyad = textInfo.ToTitleCase(soyad.ToLower());
                conn = new MySqlConnection(baglanti.bag);
                komut = new MySqlCommand();
                conn.Open();
                komut.CommandText = "INSERT INTO personel VALUES(null,'" + ad + "','" + soyad + "','" + kadi + "','" + mail + "','" + MD5Hash(sifre) + "','" + gorev + "','" + telefon + "','" + tc + "','" + adres + "',CURRENT_DATE)";
                komut.Connection = conn;
                int rowsAffected = komut.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Personel kaydı tamamlandı");
                }
                else
                {
                    MessageBox.Show("Bir hata oluştu programı yeniden başlatınız");
                }
                conn.Close();
            }

        }
        public void pers_sil(string kadi)
        {
            if (kadi.Length < 5)
            {
                MessageBox.Show("Geçerli bir kullanıcı adı giriniz");
            }
            else
            {
                conn = new MySqlConnection(baglanti.bag);
                komut = new MySqlCommand();
                conn.Open();
                komut.CommandText = "UPDATE personel SET personeldurum=1,sifre='"+MD5Hash("kütüphane")+"' WHERE kullaniciadi='" + kadi + "'";
                komut.Connection = conn;
                int rowsAffected = komut.ExecuteNonQuery();
                if (rowsAffected > 0)
                {

                    MessageBox.Show("Personel silme işlemi tamamlandı");

                }
                else
                {
                    MessageBox.Show("Kullanıcı adı bulunamadı");
                }
                conn.Close();
            }
        }

        int kul_id;

        public int kul_id_bul(string kadi)
        {

            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.CommandText = "SELECT id FROM kullanicilar where kullaniciadi='" + kadi + "'";
            komut.Connection = conn;
            oku = komut.ExecuteReader();
            if (oku.Read() == true)
            {
                kul_id = Convert.ToInt32(oku["id"]);
            }
            else
            {
                kul_id = -1;

            }
            conn.Close();
            return kul_id;
        }

        int kitap_sayi_kontrol = -1;

        public int kitap_say()
        {
            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "SELECT count(id) as kitapsayi FROM kitaplar";

            oku = komut.ExecuteReader();
            if (oku.Read() == true)
            {
                kitap_sayi_kontrol = Convert.ToInt32(oku["kitapsayi"]);
            }
            conn.Close();
            return kitap_sayi_kontrol;
        }

        int kul_kirada_kitap;
        public int eldeki_kitap_kontrol(int kul_id)
        {
            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "SELECT count(id) as kontrol FROM kayit WHERE kullaniciid=" + kul_id + " AND teslimdurumu=1";
            oku = komut.ExecuteReader();
            if (oku.Read() == true)
            {
                kul_kirada_kitap = Convert.ToInt32(oku["kontrol"]);
            }
            conn.Close();
            return kul_kirada_kitap;
        }

        int kitap_kira_durum = -1;

        public int kitap_kira_kontrol(int kitap_id)
        {
            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "SELECT kira FROM kitaplar WHERE id=" + kitap_id;
            oku = komut.ExecuteReader();
            if (oku.Read() == true)
            {
                kitap_kira_durum = Convert.ToInt32(oku["kira"]);
            }
            conn.Close();
            return kitap_kira_durum;
        }

        int tarih_kontrol_deger = -1;

        public int tarih_kontrol()
        {
            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "SELECT isnull(SUM(DATEDIFF(NOW(),teslimtarihi))) as durum FROM kayit WHERE kullaniciid=" + kul_id + " AND teslimdurumu=1 AND teslimtarihi<CURRENT_DATE";
            oku = komut.ExecuteReader();
           
            if (oku.Read() == true)
            {
                
                if (Convert.ToInt32(oku["durum"])!=1)
                {
                    
                    conn = new MySqlConnection(baglanti.bag);
                    komut = new MySqlCommand();
                    conn.Open();
                    komut.Connection = conn;
                    komut.CommandText = "SELECT SUM(DATEDIFF(NOW(),teslimtarihi)) as durum FROM kayit WHERE kullaniciid=" + kul_id + " AND teslimdurumu=1 AND teslimtarihi<CURRENT_DATE";
                    oku = komut.ExecuteReader();
                  
                    if(oku.Read() == true)
                    {
                        tarih_kontrol_deger = Convert.ToInt32(oku["durum"]);
                    }
                }
                else
                {
                    tarih_kontrol_deger = -1;
                }
                
            }
            return tarih_kontrol_deger;
        }

        public string hakkimizda, telefon, adres, fax;
        public void kutupbilgi_veri_cekme()
        {
            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "SELECT * FROM kutupbilgiler";
            oku=komut.ExecuteReader();
            if(oku.Read()==true)
            {
                hakkimizda = oku["hakkimizda"].ToString();
                telefon = oku["telefon"].ToString();
                fax= oku["fax"].ToString() ;
                adres = oku["adres"].ToString();
            }
            conn.Close();
        }

        int kitap_durum_deger;
        public int  kitap_durum_kontrol(int kitap_id)
        {
            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "SELECT kitapdurum FROM kitaplar WHERE id=" + kitap_id;
            oku = komut.ExecuteReader();
            if (oku.Read() == true)
            {
                kitap_durum_deger = Convert.ToInt32(oku["kitapdurum"]);
            }
            conn.Close();
            return kitap_durum_deger;

        }








        public void kitap_kirala(string kadi, int kitap_id)
        {
            kul_id_bul(kadi);
            kitap_say();
            eldeki_kitap_kontrol(kul_id);
            kitap_kira_kontrol(kitap_id);
            tarih_kontrol();
            kitap_durum_kontrol(kitap_id);
            

            if (kul_id == -1)
            {
                MessageBox.Show("Böyle bir kullanıcı bulunmamaktadır");
            }
            else if (kitap_sayi_kontrol < kitap_id)
            {
                MessageBox.Show("Böyle bir kitap kaydı bulunmamaktadır"+kitap_sayi_kontrol);
            }
            else if(kitap_durum_deger==1)
            {
                MessageBox.Show("Listeden kaldırılmış kitap");
            }
            else if (kul_kirada_kitap >= 3)
            {
                MessageBox.Show("Kullanıcı aynı anda 3'ten fazla kitap kiralayamaz");
            }
            else if (kitap_kira_durum == 1)
            {
                MessageBox.Show("Kitap kirada gözükmetedir");
            }
            else if (tarih_kontrol_deger >0)
            {
                MessageBox.Show("Kullanıcıda teslim tarihi geçmiş kitap bulunmaktadır\n Toplamda " + tarih_kontrol_deger * 10 + " TL cezası mevcuttur");
            }

            else
            {

                conn = new MySqlConnection(baglanti.bag);
                komut = new MySqlCommand();
                conn.Open();
                komut.Connection = conn;
                komut.CommandText = "INSERT INTO kayit VALUES(null," + kul_id + "," + kitap_id + "," + kullanici_bilgileri.id + ",CURRENT_DATE,DATE_ADD(CURDATE(), INTERVAL 15 DAY),1)";
                int rowsAffected = komut.ExecuteNonQuery();
                conn.Close();
                if (rowsAffected > 0)
                {
                    conn= new MySqlConnection(baglanti.bag);
                    komut = new MySqlCommand();
                    conn.Open();
                    komut.Connection = conn;
                    komut.CommandText = "UPDATE kitaplar SET kira=1 WHERE id=" + kitap_id;
                     rowsAffected = komut. ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Kitap kira işlemi tamamlandı");
                    }
                }

            }

        }

        public void kitap_teslim(string kadi,int kitap_id)
        {
            kul_id_bul(kadi);
            tarih_kontrol();
            if (kul_id == -1)
            {
                MessageBox.Show("Kullanıcı bulunamadı");
            }
            else
            {
                conn = new MySqlConnection(baglanti.bag);
                komut = new MySqlCommand();
                conn.Open();
                komut.Connection = conn;
                komut.CommandText = "SELECT id FROM kayit WHERE kitapid=" + kitap_id + " AND kullaniciid=" + kul_id + " AND teslimdurumu=1";
                object kontrol = komut.ExecuteScalar();
                if (kontrol != null)
                {


                    komut = new MySqlCommand();
                    komut.Connection = conn;
                    komut.CommandText = "UPDATE kayit set teslimdurumu=0, teslimtarihi=CURRENT_DATE WHERE kullaniciid=" + kul_id + " AND kitapid=" + kitap_id;
                    int rowsAffected = komut.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        komut.CommandText = "UPDATE kitaplar SET kira=0 WHERE id=" + kitap_id;
                        komut.ExecuteNonQuery();
                        if (tarih_kontrol_deger > 0)
                        {
                            MessageBox.Show("Kullanıcıda teslim tarihi geçmiş kitap bulunmaktadır\n Toplamda " + tarih_kontrol_deger * 10 + " TL cezası mevcuttur");
                        }
                        else
                        {
                            MessageBox.Show("Teslim alma işlemi tamamlandı");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Böyle bir kira kaydı bulunamadı");
                }
            }

        }

        public void kayit_doldur()
        {
            conn = new MySqlConnection(baglanti.bag);
            conn.Open();
            liste = new MySqlDataAdapter("SELECT kayit.id,kullanicilar.ad as kullaniciad,kullanicilar.soyad as kullanicisoyad,kullanicilar.kullaniciadi,kitaplar.id as kitapid,kitaplar.kitapadi,kayit.kayittarihi,kayit.teslimtarihi,kayit.teslimdurumu,personel.ad as personelad,personel.soyad as personelsoyad FROM kayit,kullanicilar,kitaplar,personel WHERE kayit.kullaniciid=kullanicilar.id AND kayit.kitapid=kitaplar.id AND kayit.calisanid=personel.id order by kayit.id asc", conn);
            tablo = new DataTable("veriler");
            liste.Fill(tablo);
            conn.Close();

        }
        
        public void kutupbilgi_guncelleme(string guncel_hakkimiz,string guncel_telefon, string guncel_fax, string guncel_adres)
        {
            conn=new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.Connection= conn;
            komut.CommandText = "UPDATE kutupbilgiler SET hakkimizda=\"" + guncel_hakkimiz + "\",telefon=\"" + guncel_telefon + "\",fax=\"" + guncel_fax + "\",adres=\"" + guncel_adres + "\"";
            int rowsAffected=komut.ExecuteNonQuery();
            if(rowsAffected > 0)
            {
                MessageBox.Show("Kütüphane bilgileri güncellenmiştir ");
            }
            else
            {
                MessageBox.Show("Bir sorun oluştu tekrar deneyiniz");
            }
            conn.Close();
        }

        public void duyuru(string baslik,string icerik)
        {
            conn=new MySqlConnection(baglanti.bag);
            komut =new MySqlCommand();
            conn.Open();
            komut.Connection= conn;
            komut.CommandText = "INSERT INTO duyurular VALUES(null,'" + baslik + "','" + icerik + "')";
            int rowsAffected=komut.ExecuteNonQuery();
            if(rowsAffected > 0)
            {
                MessageBox.Show("Duyuru yayınlandı.");
            }
            else
            {
                MessageBox.Show("Yayınlanırken bir hata oluştu");
            }
        }

        public void duyuru_doldur()
        {
            conn = new MySqlConnection(baglanti.bag);
            conn.Open();
            liste = new MySqlDataAdapter("select * from duyurular", conn);
            tablo = new DataTable("veriler");
            liste.Fill(tablo);
            conn.Close();
        }

        public void duyuru_sil(int id)
        {
            conn = new MySqlConnection(baglanti.bag);
            komut=new MySqlCommand();
            conn.Open();
            komut.Connection= conn;
            komut.CommandText = "DELETE FROM duyurular WHERE id=" + id;
            int rowsAffected =  komut.ExecuteNonQuery();
            if(rowsAffected > 0)
            {
                MessageBox.Show("Duyuru kaldırıldı");
            }
            else
            {
                MessageBox.Show("Duyuru kaldırılırken hata oluştu");
            }
        }

        public void dilekce_doldur()
        {
            conn = new MySqlConnection(baglanti.bag);
            conn.Open();
            liste = new MySqlDataAdapter("select * from dilekce", conn);
            tablo = new DataTable("veriler");
            liste.Fill(tablo);
            conn.Close();
        }

        public void dilekce_sil(int id)
        {
            conn = new MySqlConnection(baglanti.bag);
            komut = new MySqlCommand();
            conn.Open();
            komut.Connection = conn;
            komut.CommandText = "DELETE FROM dilekce WHERE id=" + id;
            int rowsAffected = komut.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                MessageBox.Show("Dilekçe silindi");
            }
            else
            {
                MessageBox.Show("Dilekçe silinirken hata oluştu");
            }
            conn.Close();
        }

        string MD5Hash(string text)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] dizi = Encoding.UTF8.GetBytes(text);
                dizi = md5.ComputeHash(dizi);
                StringBuilder sb = new StringBuilder();
                foreach (byte ba in dizi)
                {
                    sb.Append(ba.ToString("x2").ToLower());
                }
                return sb.ToString();
            } 
    }
}
