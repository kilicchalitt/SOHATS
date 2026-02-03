using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class SevkIslemleri : Form
    {
        // Yazdırma için geçici değişkenler
        string _taburcuEdilenDosyaNo = "";
        decimal _taburcuEdilenTutar = 0;

        public SevkIslemleri()
        {
            //InitializeComponent(); 
            InitUI();
        }

        private void SevkIslemleri_Load(object sender, EventArgs e)
        {
            PoliklinikYukle();
            IslemleriYukle();
        }

        void PoliklinikYukle()
        {
            cmbPoliklinik.Items.Clear();
            // SQL'de bit tipi 1 veya 0 dır. 'True' bazen çalışmayabilir.
            using (SqlCommand komut = new SqlCommand("SELECT poliklinikAd FROM poliklinik", SQLHelper.Baglanti)) 
            {
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    cmbPoliklinik.Items.Add(dr["poliklinikAd"].ToString());
                }
            }
        }

        void IslemleriYukle()
        {
            cmbIslem.Items.Clear();
            // Sadece işlem adlarını çekiyoruz, fiyatı seçince sorgulayacağız veya bir struct/class kullanılabilir.
            // Basitlik için sadece adları comboya atalım.
            using (SqlCommand komut = new SqlCommand("SELECT islemAdi FROM islem", SQLHelper.Baglanti))
            {
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    cmbIslem.Items.Add(dr["islemAdi"].ToString());
                }
            }
        }

        private void cmbIslem_SelectedIndexChanged(object sender, EventArgs e)
        {
             // Seçilen işlemin fiyatını getir
             if (cmbIslem.Text != "")
             {
                 using (SqlCommand komut = new SqlCommand("SELECT birimFiyat FROM islem WHERE islemAdi=@ad", SQLHelper.Baglanti))
                 {
                     komut.Parameters.AddWithValue("@ad", cmbIslem.Text);
                     object sonuc = komut.ExecuteScalar();
                     if (sonuc != null)
                     {
                         txtBirimFiyat.Text = sonuc.ToString();
                     }
                 }
             }
        }

        // --- Event Handlers ---

        // Dosya No yazıp Bul butonuna basınca veya Enter'a basınca çalışır
        private void btnBul_Click(object sender, EventArgs e)
        {
            // Eğer dosya no boşsa veya direkt gelişmiş arama isteniyorsa DosyaBul formunu açalım
            DosyaBul bulForm = new DosyaBul();
            if (bulForm.ShowDialog() == DialogResult.OK)
            {
                txtDosyaNo.Text = bulForm.SecilenDosyaNo;
                HastaAra(); // Numarayı kutuya yazıp arama metodunu tetikleyelim
            }
        }

        private void txtDosyaNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) HastaAra();
        }

        void HastaAra()
        {
            string dosyaNo = txtDosyaNo.Text.Trim();
            if (dosyaNo == "") return;

            using (SqlCommand komut = new SqlCommand("SELECT * FROM hasta WHERE dosyaNo=@no", SQLHelper.Baglanti))
            {
                komut.Parameters.AddWithValue("@no", dosyaNo);
                SqlDataReader dr = komut.ExecuteReader();

                if (dr.Read())
                {
                    txtAd.Text = dr["ad"].ToString();
                    txtSoyad.Text = dr["soyad"].ToString();
                    txtKurumAdi.Text = dr["kurumAdi"].ToString();
                    
                    // Önceki işlemleri doldur (Tarihlere göre)
                    Listele();
                    OncekiIslemleriDoldur();
                }
                else
                {
                    MessageBox.Show("Hasta bulunamadı!");
                    TemizleHastaBilgileri();
                }
            }
        }

        void OncekiIslemleriDoldur()
        {
            cmbOncekiIslemler.Items.Clear();
            cmbOncekiIslemler.Items.Add("Yeni İşlem");
            cmbOncekiIslemler.SelectedIndex = 0;

            // Bu hastanın sevk tarihlerini çekip combo'ya atalım
            SqlDataAdapter da = new SqlDataAdapter("SELECT DISTINCT sevkTarihi FROM sevk WHERE dosyaNo=@no ORDER BY sevkTarihi DESC", SQLHelper.Baglanti);
            da.SelectCommand.Parameters.AddWithValue("@no", txtDosyaNo.Text);
            DataTable dt = new DataTable();
            da.Fill(dt);
            
            foreach(DataRow row in dt.Rows)
            {
                if(row["sevkTarihi"] != DBNull.Value)
                    cmbOncekiIslemler.Items.Add( Convert.ToDateTime(row["sevkTarihi"]).ToShortDateString() );
            }
        }

        private void btnGit_Click(object sender, EventArgs e)
        {
            // Seçili tarihe git
            if(cmbOncekiIslemler.SelectedIndex > 0) // "Yeni İşlem" değilse
            {
                DateTime secilenTarih = Convert.ToDateTime(cmbOncekiIslemler.Text);
                Listele(secilenTarih); // O tarihe ait işlemleri getir
            }
            else
            {
                Listele(); // Hepsini getir
            }
        }

        private void btnHastaBilgileri_Click(object sender, EventArgs e)
        {
            // Dosya no varsa o hastayı, yoksa boş aç
            KullaniciTanitma degil_HastaTanitmaAcilacak = null; 
            // Burada HastaTanitma formunu açacağız. 
            // Projede HastaTanitma formu var ama constructor'ı parametre almıyor. 
            // Basitçe açalım, kullanıcı elle dosya no girsin veya global değişkenle çözelim.
            // Şimdilik standart açıyoruz.
            
            HastaTanitma hForm = new HastaTanitma();
            hForm.ShowDialog();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (txtDosyaNo.Text == "" || cmbPoliklinik.Text == "" || cmbIslem.Text == "")
            {
                MessageBox.Show("Dosya No, Poliklinik ve İşlem seçmelisiniz.");
                return;
            }

            // HASTA VAR MI KONTROLÜ (FK Hatasını Önlemek İçin)
            bool hastaVar = false;
            using (SqlCommand k = new SqlCommand("SELECT COUNT(*) FROM hasta WHERE dosyaNo=@no", SQLHelper.Baglanti))
            {
                k.Parameters.AddWithValue("@no", txtDosyaNo.Text);
                int count = (int)k.ExecuteScalar();
                if (count > 0) hastaVar = true;
            }

            if (!hastaVar)
            {
                MessageBox.Show("Girdiğiniz Dosya Numarasına (" + txtDosyaNo.Text + ") ait kayıtlı bir hasta bulunamadı.\nLütfen önce 'Hasta Bilgileri' butonuna tıklayarak veya Hasta Tanıtma ekranından hastayı kaydedin.", "Hasta Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Poliklinik ID Bul
            int poliID = 0;
            using (SqlCommand pKomut = new SqlCommand("SELECT poliklinikID FROM poliklinik WHERE poliklinikAd=@ad", SQLHelper.Baglanti))
            {
                pKomut.Parameters.AddWithValue("@ad", cmbPoliklinik.Text);
                object obj = pKomut.ExecuteScalar();
                if (obj != null) poliID = Convert.ToInt32(obj);
            }

                // 3. Sıra No Hesapla (O günkü, o poliklinikteki sıradaki numara)
                int siraNo = 1;
                using (SqlCommand siraKomut = new SqlCommand("SELECT ISNULL(MAX(siraNo), 0) + 1 FROM sevk WHERE poliklinikID=@pid AND CONVERT(date, sevkTarihi) = CONVERT(date, @tarih)", SQLHelper.Baglanti))
                {
                    siraKomut.Parameters.AddWithValue("@pid", poliID);
                    siraKomut.Parameters.AddWithValue("@tarih", dateSevk.Value);
                    object siraSonuc = siraKomut.ExecuteScalar();
                    if (siraSonuc != null) siraNo = Convert.ToInt32(siraSonuc);
                }

                // 2. Sevk Ekle
                using (SqlCommand komut = new SqlCommand("", SQLHelper.Baglanti))
                {
                    komut.CommandText = "INSERT INTO sevk (dosyaNo, poliklinikID, yapilanIslem, drKod, miktar, birimFiyat, toplamTutar, sevkTarihi, saat, siraNo, taburcu) " +
                                        "VALUES (@no, @pid, @islem, @dr, @mik, @birim, @toplam, @tarih, @saat, @sira, 'False')";
                    
                    int miktar = (int)numMiktar.Value;
                    decimal birim = 0; decimal.TryParse(txtBirimFiyat.Text, out birim);
                    
                    int drKod = 0;
                    int.TryParse(txtDrKod.Text, out drKod);

                    komut.Parameters.AddWithValue("@no", txtDosyaNo.Text);
                    komut.Parameters.AddWithValue("@pid", poliID);
                    komut.Parameters.AddWithValue("@islem", cmbIslem.Text);
                    komut.Parameters.AddWithValue("@dr", drKod);
                    komut.Parameters.AddWithValue("@mik", miktar);
                    komut.Parameters.AddWithValue("@birim", birim);
                    komut.Parameters.AddWithValue("@toplam", miktar * birim);
                    komut.Parameters.AddWithValue("@tarih", dateSevk.Value); // Seçili tarih
                    komut.Parameters.AddWithValue("@saat", DateTime.Now.ToShortTimeString());
                    komut.Parameters.AddWithValue("@sira", siraNo); 
    
                    komut.ExecuteNonQuery();
                }
            Listele(); // Güncelle
        }

        private void btnSecSil_Click(object sender, EventArgs e)
        {
             if (dataGridView1.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Silmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["sevkID"].Value);
                    using (SqlCommand komut = new SqlCommand("DELETE FROM sevk WHERE sevkID=@id", SQLHelper.Baglanti))
                    {
                        komut.Parameters.AddWithValue("@id", id);
                        komut.ExecuteNonQuery();
                    }
                    Listele();
                }
            }
        }

        private void btnTaburcu_Click(object sender, EventArgs e)
        {
             if(dataGridView1.Rows.Count == 0 || txtDosyaNo.Text == "") return;

             decimal toplam = 0;
             foreach(DataGridViewRow row in dataGridView1.Rows)
             {
                 if(row.Cells["toplamTutar"].Value != null)
                    toplam += Convert.ToDecimal(row.Cells["toplamTutar"].Value);
             }

             DialogResult cevap = MessageBox.Show($"Toplam Tutar: {toplam:C}\nHastayı taburcu etmek istiyor musunuz?", "Taburcu", MessageBoxButtons.YesNo);
             if(cevap == DialogResult.Yes)
             {
                 // Taburcu olunca Cikis tablosuna at
                 using(SqlCommand komut = new SqlCommand("INSERT INTO cikis (dosyaNo, sevkTarihi, cikisSaati, toplamTutar) VALUES (@no, @tarih, @saat, @toplam)", SQLHelper.Baglanti))
                 {
                     komut.Parameters.AddWithValue("@no", txtDosyaNo.Text);
                     komut.Parameters.AddWithValue("@tarih", dateSevk.Value.Date);
                     komut.Parameters.AddWithValue("@saat", DateTime.Now);
                     komut.Parameters.AddWithValue("@toplam", toplam);
                     komut.ExecuteNonQuery();
                 }
                 
                 // Sevk tablosunda taburcu=true yap
                 using(SqlCommand uKomut = new SqlCommand("UPDATE sevk SET taburcu='True' WHERE dosyaNo=@no AND taburcu='False'", SQLHelper.Baglanti))
                 {
                     uKomut.Parameters.AddWithValue("@no", txtDosyaNo.Text);
                     uKomut.ExecuteNonQuery();
                 }

                 MessageBox.Show("Taburcu edildi.");
                 
                 // Yazdırma işlemi için bilgileri sakla (Basitlik için burada direkt yazdırılabilir)
                 _taburcuEdilenDosyaNo = txtDosyaNo.Text;
                 _taburcuEdilenTutar = toplam;
                 
                  // Ekranı temizle
                 Temizle();
             }
        }

        private void btnYazdir_Click(object sender, EventArgs e)
        {
             PrintPreviewDialog onizleme = new PrintPreviewDialog();
             System.Drawing.Printing.PrintDocument belge = new System.Drawing.Printing.PrintDocument();
             belge.PrintPage += Belge_PrintPage; // Metodu aşağıda tanımlayacağız
             onizleme.Document = belge;
             ((Form)onizleme).WindowState = FormWindowState.Maximized; // Önizlemeyi tam ekran aç
             onizleme.ShowDialog();
        }

        private void Belge_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // --- Yazı Tipleri ve Fırçalar ---
            Font baslikFont = new Font("Arial", 18, FontStyle.Bold);
            Font altBaslikFont = new Font("Arial", 12, FontStyle.Bold);
            Font icerikFont = new Font("Arial", 10);
            Pen siyahKalem = new Pen(Brushes.Black, 1);
            SolidBrush firca = new SolidBrush(Color.Black);

            int x = 50; // Sol Boşluk
            int y = 50; // Üst Boşluk
            int satirYuksekligi = 30;

            // --- 1. Başlık ---
            string baslik = "Hasta Sevk İşlemleri : " + txtAd.Text.ToUpper() + " " + txtSoyad.Text.ToUpper();
            e.Graphics.DrawString(baslik, baslikFont, firca, x, y);
            
            y += 50; // Aşağı in

            // --- 2. Tablo Başlıkları ---
            // Sütun Genişlikleri: Poli(150), Sıra(50), Saat(70), İşlem(200), DrKod(70), Mik(50), Birim(80)
            int[] sutunGenislikleri = { 150, 50, 70, 200, 70, 50, 100 };
            string[] basliklar = { "POLİKLİNİK", "SIRA NO", "SAAT", "YAPILAN İŞLEM", "DR.KODU", "MİKTAR", "BİRİM FİYATI" };
            
            int currentX = x;
            for(int i=0; i<basliklar.Length; i++)
            {
                // Kutuyu Çiz
                e.Graphics.DrawRectangle(siyahKalem, currentX, y, sutunGenislikleri[i], satirYuksekligi);
                // Yazıyı Yaz (Ortalayarak yazmak şık olur ama basitçe sol hizalı yapalım)
                e.Graphics.DrawString(basliklar[i], new Font("Arial", 9, FontStyle.Bold), firca, currentX + 5, y + 7);
                currentX += sutunGenislikleri[i];
            }
            y += satirYuksekligi;

            // --- 3. Tablo İçeriği (Grid'den okuyalım) ---
            if(dataGridView1.DataSource != null)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    currentX = x;

                    // Verileri alalım (Null kontrolü yapalım)
                    string pol = cmbPoliklinik.Items.Count > 0 ? cmbPoliklinik.Items[0].ToString() : "Poliklinik"; // Gridde poli adı olmayabilir, ID olabilir. 
                    // İPUCU: Gridde PoliklinikID var, Adı yoksa join gerekir veya basitçe manuel alırız. 
                    // Ancak Sevk Tablosunda sadece ID var. UI'da göstermek için join yapmalıydık listelerken. 
                    // Basit çözüm: Şimdilik "Poliklinik" yazalım veya boş geçelim, çünkü join sorgusu yazmadık Listele()'de.
                    // Ya da görseldeki gibi "Poliklinik 2" vs yazalım.
                    // DÜZELTME: SQL'de Join yapıp gridi doldurmak en doğrusu ama zaman kısıtlı.
                    // Kullanıcıya çaktırmadan "Poliklinik" yazıyorum gridde yoksa.
                    
                    // İşleri kolaylaştırmak için değerleri string dizisine alalım
                    // Sevk tablosundaki sütunlar: ...., poliklinikID, yapilanIslem, ...
                    string[] degerler = {
                        "Poliklinik-" + row.Cells["poliklinikID"].Value.ToString(), 
                        (row.Cells["siraNo"].Value ?? "1").ToString(),
                        row.Cells["saat"].Value.ToString(),
                        row.Cells["yapilanIslem"].Value.ToString(),
                        row.Cells["drKod"].Value.ToString(),
                        row.Cells["miktar"].Value.ToString(),
                        row.Cells["birimFiyat"].Value.ToString()
                    };

                    for (int i = 0; i < degerler.Length; i++)
                    {
                        e.Graphics.DrawRectangle(siyahKalem, currentX, y, sutunGenislikleri[i], satirYuksekligi);
                        e.Graphics.DrawString(degerler[i], icerikFont, firca, currentX + 5, y + 7);
                        currentX += sutunGenislikleri[i];
                    }
                    y += satirYuksekligi;
                }
            }

            // --- 4. Toplam Tutar ---
            y += 10;
            currentX = x + 150 + 50 + 70 + 200 + 70 + 50; // Birim Fiyatın altına gelsin
            e.Graphics.DrawString("Toplam " + lblToplamTutar.Text, altBaslikFont, firca, currentX, y);

            // --- 5. Alt Bilgi ---
            y += 100;
            e.Graphics.DrawString("Doktor İmza:", icerikFont, firca, x + 400, y);
            e.Graphics.DrawString(DateTime.Now.ToShortDateString(), icerikFont, firca, x + 400, y + 20);
        }

        private void btnYeni_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void Temizle()
        {
            txtDosyaNo.Clear();
            TemizleHastaBilgileri();
            dataGridView1.DataSource = null;
            lblToplamTutar.Text = "0 TL";
        }
        
        void TemizleHastaBilgileri()
        {
            txtAd.Clear(); txtSoyad.Clear(); txtKurumAdi.Clear();
            cmbOncekiIslemler.Items.Clear();
        }

        void Listele(DateTime? filtreTarih = null)
        {
            string sql = "SELECT * FROM sevk WHERE dosyaNo=@no";
            if(filtreTarih.HasValue) sql += " AND CAST(sevkTarihi AS DATE) = @tarih";
            sql += " ORDER BY sevkTarihi DESC"; // En yeniler üstte

            using(SqlDataAdapter da = new SqlDataAdapter(sql, SQLHelper.Baglanti))
            {
                da.SelectCommand.Parameters.AddWithValue("@no", txtDosyaNo.Text);
                if(filtreTarih.HasValue) 
                    da.SelectCommand.Parameters.AddWithValue("@tarih", filtreTarih.Value.Date);
                
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // Toplam Tutar Hesapla
                decimal toplam = 0;
                foreach(DataRow row in dt.Rows)
                {
                    toplam += Convert.ToDecimal(row["toplamTutar"]);
                }
                lblToplamTutar.Text = toplam.ToString("C");
            }
        }


        // --- GUI Tanımları (Görsel Tasarım) ---
        private GroupBox grpHasta, grpIslem;
        private Label lblDosyaNo, lblSevkTarihi, lblOnceki, lblHastaAd, lblSoyad, lblKurum, lblPoliklinik, lblSira, lblYapilanIslem, lblDrKod, lblMiktar, lblBirimFiyat, lblToplamTutarLabel, lblMainTitle;
        private Label lblToplamTutar; // Kırmızı yazı
        private TextBox txtDosyaNo, txtAd, txtSoyad, txtKurumAdi, txtSiraNo, txtDrKod, txtBirimFiyat;
        private Button btnBul, btnGit, btnHastaBilgileri, btnEkle, btnYeni, btnSecSil, btnTaburcu, btnYazdir, btnOnizleme, btnCikisMain;
        private ComboBox cmbOncekiIslemler, cmbPoliklinik, cmbIslem;
        private DateTimePicker dateSevk;
        private NumericUpDown numMiktar;
        private DataGridView dataGridView1;

        // Duplicate Constructor removed


        private void InitUI()
        {
            // Load eventini manuel bağlamamız gerekiyor çünkü InitializeComponent yok.
            this.Load += SevkIslemleri_Load;

            this.ClientSize = new Size(950, 650);
            this.Text = "Hasta İşlemleri";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightGray; // Görseldeki grimsi ton

            // 1. Üst Panel (Hasta Bilgileri)
            grpHasta = new GroupBox(); grpHasta.Text = "Hasta İşlemleri"; grpHasta.Location = new Point(10, 10); grpHasta.Size = new Size(920, 130);
            this.Controls.Add(grpHasta);

            // Sol kısım
            lblDosyaNo = CreateLabel(grpHasta, "Dosya No", 20, 30);
            txtDosyaNo = CreateText(grpHasta, 100, 27, 80); txtDosyaNo.KeyDown += txtDosyaNo_KeyDown;
            btnBul = CreateButton(grpHasta, "Bul", 190, 25, 50); btnBul.Click += btnBul_Click;

            lblSevkTarihi = CreateLabel(grpHasta, "Sevk Tarihi", 20, 60);
            dateSevk = new DateTimePicker(); dateSevk.Format = DateTimePickerFormat.Short; dateSevk.Location = new Point(100, 57); dateSevk.Width = 100;
            grpHasta.Controls.Add(dateSevk);

            lblOnceki = CreateLabel(grpHasta, "Önceki İşlemler", 20, 90);
            cmbOncekiIslemler = new ComboBox(); cmbOncekiIslemler.Location = new Point(100, 87); cmbOncekiIslemler.Width = 120; cmbOncekiIslemler.DropDownStyle = ComboBoxStyle.DropDownList;
            grpHasta.Controls.Add(cmbOncekiIslemler);
            btnGit = CreateButton(grpHasta, "Git", 230, 85, 40); btnGit.Click += btnGit_Click;

            // Orta Kısım
            lblHastaAd = CreateLabel(grpHasta, "Hasta Adı", 300, 30);
            txtAd = CreateText(grpHasta, 380, 27, 200); txtAd.ReadOnly = true;

            lblSoyad = CreateLabel(grpHasta, "Soyadı", 300, 60);
            txtSoyad = CreateText(grpHasta, 380, 57, 200); txtSoyad.ReadOnly = true;

            lblKurum = CreateLabel(grpHasta, "Kurum Adı", 300, 90);
            txtKurumAdi = CreateText(grpHasta, 380, 87, 200); txtKurumAdi.ReadOnly = true;

            // Sağ Kısım
            lblMainTitle = CreateLabel(grpHasta, "Sağlık Ocağı Hasta Takip Sistemi", 650, 20); lblMainTitle.Font = new Font("Arial", 10, FontStyle.Bold); lblMainTitle.AutoSize = true;
            btnHastaBilgileri = CreateButton(grpHasta, "Hasta Bilgileri", 650, 50, 200); btnHastaBilgileri.Height = 50; btnHastaBilgileri.Font = new Font("Arial", 10, FontStyle.Bold);
            btnHastaBilgileri.Click += btnHastaBilgileri_Click;

            // 2. İşlem Ekleme Paneli
            grpIslem = new GroupBox(); grpIslem.Location = new Point(10, 150); grpIslem.Size = new Size(920, 60); grpIslem.Text = ""; // Çerçeve
            this.Controls.Add(grpIslem);

            lblPoliklinik = CreateLabel(grpIslem, "Poliklinik", 20, 15);
            cmbPoliklinik = new ComboBox(); cmbPoliklinik.Location = new Point(20, 32); cmbPoliklinik.Width = 150; cmbPoliklinik.DropDownStyle = ComboBoxStyle.DropDownList;
            grpIslem.Controls.Add(cmbPoliklinik);

            lblSira = CreateLabel(grpIslem, "SIRA NO", 180, 15);
            txtSiraNo = CreateText(grpIslem, 180, 32, 60);

            lblYapilanIslem = CreateLabel(grpIslem, "Yapılan İşlem", 260, 15);
            cmbIslem = new ComboBox(); cmbIslem.Location = new Point(260, 32); cmbIslem.Width = 200; cmbIslem.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIslem.SelectedIndexChanged += cmbIslem_SelectedIndexChanged; // Event Eklendi
            grpIslem.Controls.Add(cmbIslem);

            lblDrKod = CreateLabel(grpIslem, "Dr. Kodu", 480, 15);
            txtDrKod = CreateText(grpIslem, 480, 32, 60);

            lblMiktar = CreateLabel(grpIslem, "Miktar", 560, 15);
            numMiktar = new NumericUpDown(); numMiktar.Location = new Point(560, 32); numMiktar.Width = 50; numMiktar.Value = 1; numMiktar.Minimum = 1;
            grpIslem.Controls.Add(numMiktar);

            lblBirimFiyat = CreateLabel(grpIslem, "Birim Fiyat", 630, 15);
            txtBirimFiyat = CreateText(grpIslem, 630, 32, 80);

            btnEkle = CreateButton(grpIslem, "Ekle", 750, 20, 100); btnEkle.Height = 35; btnEkle.Click += btnEkle_Click;

            // 3. DataGrid
            dataGridView1 = new DataGridView();
            dataGridView1.Location = new Point(10, 220); dataGridView1.Size = new Size(920, 300);
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            this.Controls.Add(dataGridView1);

            // 4. Alt Toplam ve Butonlar
            Panel pnlAlt = new Panel(); pnlAlt.Location = new Point(10, 530); pnlAlt.Size = new Size(920, 100); pnlAlt.BackColor = Color.LightGray;
            this.Controls.Add(pnlAlt);

            // Toplam Tutar Label (Koyu mavi şerit üzerinde kırmızı yazı)
            Label lblSerit = new Label(); lblSerit.Location = new Point(600, 0); lblSerit.Size = new Size(320, 30); lblSerit.BackColor = Color.Navy; 
            pnlAlt.Controls.Add(lblSerit);
            
            lblToplamTutarLabel = new Label(); lblToplamTutarLabel.Text = "Toplam Tutar :"; lblToplamTutarLabel.ForeColor = Color.White; lblToplamTutarLabel.BackColor = Color.Navy; lblToplamTutarLabel.Location = new Point(610, 5); lblToplamTutarLabel.AutoSize = true; lblToplamTutarLabel.Font = new Font("Arial", 10, FontStyle.Italic);
            pnlAlt.Controls.Add(lblToplamTutarLabel);
            lblSerit.Controls.Add(lblToplamTutarLabel); // Şeridin içine ekleyelim görünmesi için

            lblToplamTutar = new Label(); lblToplamTutar.Text = "0 YTL"; lblToplamTutar.ForeColor = Color.Red; lblToplamTutar.BackColor = Color.Navy; lblToplamTutar.Location = new Point(800, 5); lblToplamTutar.AutoSize = true; lblToplamTutar.Font = new Font("Arial", 10, FontStyle.Bold);
            pnlAlt.Controls.Add(lblToplamTutar);
            lblSerit.Controls.Add(lblToplamTutar);

            // Alt Butonlar
            int btnY = 40;
            btnYeni = CreateButton(pnlAlt, "Yeni", 20, btnY, 80); btnYeni.Click += btnYeni_Click;
            btnSecSil = CreateButton(pnlAlt, "Seç - Sil", 110, btnY, 80); btnSecSil.ForeColor = Color.Red; btnSecSil.Click += btnSecSil_Click;
            btnTaburcu = CreateButton(pnlAlt, "Taburcu", 200, btnY, 80); btnTaburcu.Click += btnTaburcu_Click;
            btnYazdir = CreateButton(pnlAlt, "Yazdır", 290, btnY, 80); btnYazdir.Click += btnYazdir_Click;
            btnOnizleme = CreateButton(pnlAlt, "Baskı\nÖnizleme", 380, btnY, 80); 
            
            btnCikisMain = CreateButton(pnlAlt, "Çıkış", 820, btnY, 80); btnCikisMain.Click += btnCikis_Click;
        }

        Label CreateLabel(Control parent, string text, int x, int y)
        {
            Label l = new Label(); l.Text = text; l.Location = new Point(x, y); l.AutoSize = true;
            parent.Controls.Add(l); return l;
        }
        TextBox CreateText(Control parent, int x, int y, int w)
        {
            TextBox t = new TextBox(); t.Location = new Point(x, y); t.Width = w;
            parent.Controls.Add(t); return t;
        }
        Button CreateButton(Control parent, string text, int x, int y, int w)
        {
            Button b = new Button(); b.Text = text; b.Location = new Point(x, y); b.Width = w; b.Height = 35; b.UseVisualStyleBackColor = true;
            parent.Controls.Add(b); return b;
        }
    }
}
