using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOHATS
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Veritabanı Eksiklerini Otomatik Tamamla
            VeritabaniGuncelle();

            Application.Run(new Login());
        }

        static void VeritabaniGuncelle()
        {
            try
            {
                // Versiyon Kontrolü (Kullanıcının kodu güncelleyip güncellemediğini anlamak için)
                // MessageBox.Show("Veritabanı Onarımı Başlatılıyor... (v3.0)", "Sistem Kontrolü"); 

                using (System.Data.SqlClient.SqlConnection baglanti = SQLHelper.Baglanti)
                {
                    using (System.Data.SqlClient.SqlCommand komut = new System.Data.SqlClient.SqlCommand())
                    {
                        komut.Connection = baglanti;

                        
                        // --- TABLO OLUŞTURMA (DDL) ---

                        // 1. KULLANICI TABLOSU
                        string sqlKullanici = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'kullanici') " +
                                              "BEGIN " +
                                              "CREATE TABLE kullanici (kullaniciKodu INT IDENTITY(1,1) PRIMARY KEY, kullaniciAd VARCHAR(20) NOT NULL, sifre VARCHAR(20) NOT NULL, yetki CHAR(1) DEFAULT '0', ad VARCHAR(50), soyad VARCHAR(50), evTel VARCHAR(15), cepTel VARCHAR(15), adres VARCHAR(255), iseBaslama DATETIME, maas DECIMAL(18,2), tcKimlikNo VARCHAR(11), dogumYeri VARCHAR(50), babaAdi VARCHAR(20), anneAdi VARCHAR(20), dogumTarihi DATETIME, cinsiyet CHAR(5), medeniHal CHAR(10), kanGrubu CHAR(10), unvan VARCHAR(20)) " +
                                              "END";
                        komut.CommandText = sqlKullanici;
                        komut.ExecuteNonQuery();

                        // KULLANICI TABLOSU EKSİK SÜTUN KONTROLÜ
                        string[] kullaniciSutunlar = new string[] { 
                            "tcKimlikNo VARCHAR(11)", "dogumYeri VARCHAR(50)", "babaAdi VARCHAR(20)", "anneAdi VARCHAR(20)", 
                            "dogumTarihi DATETIME", "cinsiyet CHAR(5)", "medeniHal CHAR(10)", "kanGrubu CHAR(10)", "unvan VARCHAR(20)" 
                        };
                        foreach (string sutun in kullaniciSutunlar)
                        {
                            string ad = sutun.Split(' ')[0];
                            // sys.columns kullanımı daha güvenilirdir
                            komut.CommandText = $"IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'{ad}' AND Object_ID = Object_ID(N'kullanici')) BEGIN ALTER TABLE kullanici ADD {sutun} END";
                            komut.ExecuteNonQuery();
                        }

                        // 2. POLİKLİNİK TABLOSU
                        string sqlPoliklinik = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'poliklinik') " +
                                               "BEGIN " +
                                               "CREATE TABLE poliklinik (poliklinikID INT IDENTITY(1,1) PRIMARY KEY, poliklinikAd VARCHAR(50) NOT NULL, durum BIT DEFAULT 1, aciklama VARCHAR(255)) " +
                                               "END";
                        komut.CommandText = sqlPoliklinik;
                        komut.ExecuteNonQuery();

                        // 3. HASTA TABLOSU
                        string sqlHasta = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'hasta') " +
                                          "BEGIN " +
                                          "CREATE TABLE hasta (dosyaNo CHAR(10) PRIMARY KEY, tcKimlikNo VARCHAR(11), ad VARCHAR(20), soyad VARCHAR(20), dogumYeri VARCHAR(50), dogumTarihi DATETIME, babaAdi VARCHAR(20), anneAdi VARCHAR(20), cinsiyet CHAR(5), tel VARCHAR(15), kurumAdi VARCHAR(50), kurumSicilNo VARCHAR(20), yakinTel VARCHAR(15), yakinKurumSicilNo VARCHAR(20), yakinKurumAdi VARCHAR(50), kanGrubu CHAR(10), medeniHal CHAR(10), adres VARCHAR(255)) " +
                                          "END";
                        komut.CommandText = sqlHasta;
                        komut.ExecuteNonQuery();

                        // HASTA TABLOSU EKSİK SÜTUN KONTROLÜ
                        string[] hastaSutunlar = new string[] { "adres VARCHAR(255)", "kurumSicilNo VARCHAR(20)", "yakinTel VARCHAR(15)", "yakinKurumSicilNo VARCHAR(20)", "yakinKurumAdi VARCHAR(50)", "medeniHal CHAR(10)", "kanGrubu CHAR(10)" };
                        foreach (string sutun in hastaSutunlar)
                        {
                            string ad = sutun.Split(' ')[0];
                            komut.CommandText = $"IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'{ad}' AND Object_ID = Object_ID(N'hasta')) BEGIN ALTER TABLE hasta ADD {sutun} END";
                            komut.ExecuteNonQuery();
                        }

                        // 4. SEVK TABLOSU
                        string sqlSevk = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'sevk') " +
                                         "BEGIN " +
                                         "CREATE TABLE sevk (sevkID INT IDENTITY(1,1) PRIMARY KEY, dosyaNo CHAR(10), poliklinikID INT, sevkTarihi DATETIME DEFAULT GETDATE(), saat CHAR(5), siraNo INT, yapilanIslem VARCHAR(255), drKod INT, miktar INT DEFAULT 1, birimFiyat DECIMAL(18,2) DEFAULT 0, toplamTutar DECIMAL(18,2) DEFAULT 0, taburcu BIT DEFAULT 0) " +
                                         "END";
                        komut.CommandText = sqlSevk;
                        komut.ExecuteNonQuery();

                        komut.CommandText = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'siraNo' AND Object_ID = Object_ID(N'sevk')) BEGIN ALTER TABLE sevk ADD siraNo INT DEFAULT 1 END";
                        komut.ExecuteNonQuery();

                        // 5. ÇIKIŞ TABLOSU
                        string sqlCikis = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cikis') " +
                                          "BEGIN " +
                                          "CREATE TABLE cikis (cikisID INT IDENTITY(1,1) PRIMARY KEY, dosyaNo CHAR(10), sevkTarihi DATETIME, cikisSaati DATETIME, toplamTutar DECIMAL(18,2)) " +
                                          "END";
                        komut.CommandText = sqlCikis;
                        komut.ExecuteNonQuery();

                        // 6. DOKTOR TABLOSU
                        string sqlDoktor = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'doktor') " +
                                           "BEGIN " +
                                           "CREATE TABLE doktor (doktorKod INT IDENTITY(1,1) PRIMARY KEY, ad VARCHAR(30), soyad VARCHAR(30), poliklinikID INT, tel VARCHAR(15)) " +
                                           "END";
                        komut.CommandText = sqlDoktor;
                        komut.ExecuteNonQuery();

                        // 7. İŞLEM TABLOSU
                        string sqlIslem = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'islem') " +
                                          "BEGIN " +
                                          "CREATE TABLE islem (islemID INT IDENTITY(1,1) PRIMARY KEY, islemAdi VARCHAR(50), birimFiyat DECIMAL(18,2)) " +
                                          "END";
                        komut.CommandText = sqlIslem;
                        komut.ExecuteNonQuery();


                        // --- VARSAYILAN VERİLERİN EKLENMESİ (DML) - GÜÇLENDİRİLMİŞ ---
                        
                        // 1. Poliklinikleri Garantiye Al
                        string[] poliklinikler = { "Dahiliye", "KBB", "Göz", "Ortopedi", "Çocuk", "Acil" };
                        foreach (string poli in poliklinikler)
                        {
                             komut.CommandText = $"IF NOT EXISTS (SELECT * FROM poliklinik WHERE poliklinikAd = '{poli}') " +
                                                 $"BEGIN INSERT INTO poliklinik (poliklinikAd, durum, aciklama) VALUES ('{poli}', 1, 'Varsayılan Poliklinik') END";
                             komut.ExecuteNonQuery();
                        }
                        // Varsa Durumlarını Aktif Yap (Kullanıcı yanlışlıkla pasife almış olabilir veya null olabilir)
                        komut.CommandText = "UPDATE poliklinik SET durum = 1 WHERE durum IS NULL OR durum = 0";
                        komut.ExecuteNonQuery();

                        // 2. İşlemleri Garantiye Al
                        // Birim fiyatlarıyla birlikte
                        var islemler = new Dictionary<string, string> {
                            {"Muayene", "50.00"}, {"Tahlil", "20.00"}, {"Röntgen", "30.00"}, 
                            {"EKG", "25.00"}, {"Serum", "40.00"}, {"Ultrason", "100.00"}
                        };

                        foreach (var islem in islemler)
                        {
                             komut.CommandText = $"IF NOT EXISTS (SELECT * FROM islem WHERE islemAdi = '{islem.Key}') " +
                                                 $"BEGIN INSERT INTO islem (islemAdi, birimFiyat) VALUES ('{islem.Key}', {islem.Value}) END";
                             komut.ExecuteNonQuery();
                        }

                        // 3. Admin Kullanıcıyı Garantiye Al
                        komut.CommandText = "IF NOT EXISTS (SELECT * FROM kullanici WHERE kullaniciAd='admin') " +
                                            "BEGIN INSERT INTO kullanici (kullaniciAd, sifre, ad, soyad, yetki, unvan) VALUES ('admin', '1234', 'Admin', 'User', '1', 'Yonetici') END";
                        komut.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı yapılandırması sırasında hata: " + ex.Message);
            }
        }
    }
}
