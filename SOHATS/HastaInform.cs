using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class HastaInform : Form
    {
        string _dosyaNo = "";

        public HastaInform()
        {
            InitializeComponent();
        }

        private void HastaTanitma_Load(object sender, EventArgs e)
        {
            // Form açýldýðýnda boþ gelsin veya parametre alabilir
        }

        private void txtDosyaNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string aranan = txtDosyaNo.Text.Trim();
                using (SqlCommand komut = new SqlCommand("SELECT * FROM hasta WHERE dosyaNo=@no", SQLHelper.Baglanti))
                {
                    komut.Parameters.AddWithValue("@no", aranan);
                    SqlDataReader dr = komut.ExecuteReader();

                    if (dr.Read())
                    {
                        // Varolan Kayýt
                        _dosyaNo = dr["dosyaNo"].ToString();
                        
                        txtTC.Text = dr["tcKimlikNo"].ToString();
                        txtAd.Text = dr["ad"].ToString();
                        txtSoyad.Text = dr["soyad"].ToString();
                        txtDogumYeri.Text = dr["dogumYeri"].ToString();
                        txtBaba.Text = dr["babaAdi"].ToString();
                        txtAnne.Text = dr["anneAdi"].ToString();
                        txtAdres.Text = dr["adres"].ToString();
                        txtTel.Text = dr["tel"].ToString();
                        txtKurumSicil.Text = dr["kurumSicilNo"].ToString();
                        txtKurumAdi.Text = dr["kurumAdi"].ToString();
                        txtYakinTel.Text = dr["yakinTel"].ToString();
                        txtYakinKurumSicil.Text = dr["yakinKurumSicilNo"].ToString();
                        txtYakinKurumAdi.Text = dr["yakinKurumAdi"].ToString();

                        if(dr["dogumTarihi"] != DBNull.Value)
                            dateDogumTarihi.Value = (DateTime)dr["dogumTarihi"];
                        
                        cmbCinsiyet.Text = dr["cinsiyet"].ToString();
                        cmbMedeni.Text = dr["medeniHal"].ToString();
                        cmbKan.Text = dr["kanGrubu"].ToString();

                        lblDurum.Text = "< KAYIT BULUNDU >";
                        btnKaydet.Text = "Güncelle";
                    }
                    else
                    {
                        // Yeni Kayýt için Hazýrlýk
                        DialogResult cvp = MessageBox.Show("Kayýt bulunamadý. Yeni kayýt yapýlsýn mý?", "Yeni Kayýt", MessageBoxButtons.YesNo);
                        if(cvp == DialogResult.Yes)
                        {
                            Temizle(false); // Dosya No kalsýn
                            lblDurum.Text = "< YENÝ KAYIT >";
                             btnKaydet.Text = "Kaydet";
                        }
                    }
                }
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txtDosyaNo.Text == "" || txtAd.Text == "") return;

            using (SqlCommand komut = new SqlCommand("", SQLHelper.Baglanti))
            {
               
                string sql = "";
                // Dosya No veritabanýnda var mý tekrar kontrol edelim, _dosyaNo deðiþkeninden ziyade
                // Fakat basitlik adýna _dosyaNo doluysa UPDATE diyelim.
                // Ya da "Kaydet" butonu metninden anlayabiliriz veya simple logic:
                
                // NOT: Gerçek projede DosyaNo PRIMARY KEY olduðu için, önce var mý diye bakmak lazým. 
                // Biz burada daha önce Enter ile arattýysak _dosyaNo dolmuþtur.
                
                if (_dosyaNo == "") // YENÝ
                {
                     sql = "INSERT INTO hasta (dosyaNo, tcKimlikNo, ad, soyad, dogumYeri, dogumTarihi, babaAdi, anneAdi, adres, tel, kurumSicilNo, kurumAdi, yakinTel, yakinKurumSicilNo, yakinKurumAdi, cinsiyet, medeniHal, kanGrubu) " +
                            "VALUES (@no, @tc, @ad, @soyad, @dyeri, @dtarih, @baba, @anne, @adres, @tel, @ksicil, @kadi, @ytel, @yksicil, @ykadi, @cins, @medeni, @kan)";
                }
                else // GÜNCELLEME
                {
                     sql = "UPDATE hasta SET tcKimlikNo=@tc, ad=@ad, soyad=@soyad, dogumYeri=@dyeri, dogumTarihi=@dtarih, babaAdi=@baba, anneAdi=@anne, adres=@adres, tel=@tel, kurumSicilNo=@ksicil, kurumAdi=@kadi, " +
                           "yakinTel=@ytel, yakinKurumSicilNo=@yksicil, yakinKurumAdi=@ykadi, cinsiyet=@cins, medeniHal=@medeni, kanGrubu=@kan WHERE dosyaNo=@no";
                }
                
                komut.CommandText = sql;
                komut.Parameters.AddWithValue("@no", txtDosyaNo.Text);
                komut.Parameters.AddWithValue("@tc", txtTC.Text);
                komut.Parameters.AddWithValue("@ad", txtAd.Text);
                komut.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                komut.Parameters.AddWithValue("@dyeri", txtDogumYeri.Text);
                komut.Parameters.AddWithValue("@dtarih", dateDogumTarihi.Value);
                komut.Parameters.AddWithValue("@baba", txtBaba.Text);
                komut.Parameters.AddWithValue("@anne", txtAnne.Text);
                komut.Parameters.AddWithValue("@adres", txtAdres.Text);
                komut.Parameters.AddWithValue("@tel", txtTel.Text);
                komut.Parameters.AddWithValue("@ksicil", txtKurumSicil.Text);
                komut.Parameters.AddWithValue("@kadi", txtKurumAdi.Text);
                komut.Parameters.AddWithValue("@ytel", txtYakinTel.Text);
                komut.Parameters.AddWithValue("@yksicil", txtYakinKurumSicil.Text);
                komut.Parameters.AddWithValue("@ykadi", txtYakinKurumAdi.Text);
                komut.Parameters.AddWithValue("@cins", cmbCinsiyet.Text);
                komut.Parameters.AddWithValue("@medeni", cmbMedeni.Text);
                komut.Parameters.AddWithValue("@kan", cmbKan.Text);

                komut.ExecuteNonQuery();
                lblDurum.Text = "< ÝÞLEM TAMAMLANDI >";
                MessageBox.Show("Kaydedildi.");
                _dosyaNo = txtDosyaNo.Text; // Artýk kayýtlý
                btnKaydet.Text = "Güncelle";
            }
        }

        private void btnYeni_Click(object sender, EventArgs e)
        {
            Temizle(true);
            lblDurum.Text = "< YENÝ KAYIT MODU >";
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void Temizle(bool dosyaNoSil)
        {
            if(dosyaNoSil) txtDosyaNo.Clear();
            txtTC.Clear(); txtAd.Clear(); txtSoyad.Clear(); txtDogumYeri.Clear();
            txtBaba.Clear(); txtAnne.Clear(); txtAdres.Clear();
            txtTel.Clear(); txtKurumSicil.Clear(); txtKurumAdi.Clear();
            txtYakinTel.Clear(); txtYakinKurumSicil.Clear(); txtYakinKurumAdi.Clear();
            cmbCinsiyet.SelectedIndex = -1; cmbMedeni.SelectedIndex = -1; cmbKan.SelectedIndex = -1;
            
            _dosyaNo = "";
            btnKaydet.Text = "Kaydet";
        }

        // --- GUI ---
        TextBox txtDosyaNo, txtTC, txtAd, txtSoyad, txtDogumYeri, txtBaba, txtAnne, txtAdres;
        TextBox txtTel, txtKurumSicil, txtKurumAdi;
        TextBox txtYakinTel, txtYakinKurumSicil, txtYakinKurumAdi;
        ComboBox cmbCinsiyet, cmbKan, cmbMedeni;
        DateTimePicker dateDogumTarihi;
        Button btnYeni, btnKaydet, btnCikis;
        Label lblDurum; // Alt ortadaki durum mesajý

        private void InitializeComponent()
        {
            this.ClientSize = new Size(680, 500);
            this.Text = "Hasta Bilgileri";
            this.StartPosition = FormStartPosition.CenterParent; // Parent (Sevk) içinde açýlýrsa
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow; // Görseldeki gibi ince çerçeve
            
            // Üst Kýsým
            Label l1 = CreateLabel("Dosya No", 20, 20);
            txtDosyaNo = CreateText(100, 17, 100); txtDosyaNo.KeyDown += txtDosyaNo_KeyDown;

            Label l2 = CreateLabel("T.C. Kimlik Numarasý", 250, 20);
            txtTC = CreateText(380, 17, 150);

            Label l3 = CreateLabel("Ad", 80, 50);
            txtAd = CreateText(100, 47, 150);

            Label l4 = CreateLabel("Soyadý", 300, 50);
            txtSoyad = CreateText(380, 47, 150);

            // Orta Sol
            Label l5 = CreateLabel("Doðum Yeri", 20, 90); txtDogumYeri = CreateText(100, 87, 150);
            Label l6 = CreateLabel("Baba Adý", 20, 120); txtBaba = CreateText(100, 117, 150);
            Label l7 = CreateLabel("Anne Adý", 20, 150); txtAnne = CreateText(100, 147, 150);
            
            Label lAddr = CreateLabel("Adres", 20, 180); 
            txtAdres = CreateText(100, 180, 430); txtAdres.Multiline = true; txtAdres.Height = 80;

            // Orta Sað
            Label l8 = CreateLabel("Doðum Tarihi", 300, 90); dateDogumTarihi = new DateTimePicker(); dateDogumTarihi.Location=new Point(380,87); dateDogumTarihi.Format=DateTimePickerFormat.Short; dateDogumTarihi.Width=110; this.Controls.Add(dateDogumTarihi);
            
            Label l9 = CreateLabel("Cinsiyeti", 310, 120); cmbCinsiyet = CreateCombo(380, 117, new string[]{"BAY","BAYAN"});
            Label l10 = CreateLabel("Medeni Hali", 470, 120); cmbMedeni = CreateCombo(540, 117, new string[]{"EVLÝ","BEKAR"});
            Label l11 = CreateLabel("Kan Grubu", 300, 150); cmbKan = CreateCombo(380, 147, new string[]{"0 Rh+","0 Rh-","A Rh+","A Rh-"}); // vb.

            // Alt Sol (Kendi Bilgileri)
            int yAlt = 280;
            Label l12 = CreateLabel("Telefon No", 20, yAlt); txtTel = CreateText(100, yAlt-3, 150);
            Label l13 = CreateLabel("Kurum Sicil No", 20, yAlt+30); txtKurumSicil = CreateText(100, yAlt+27, 150);
            Label l14 = CreateLabel("Kurum Adý", 20, yAlt+60); txtKurumAdi = CreateText(100, yAlt+57, 150);

            // Alt Sað (Yakýnýnýn Bilgileri)
            Label l15 = CreateLabel("Yakýnýn Telefonu", 300, yAlt); txtYakinTel = CreateText(400, yAlt-3, 150);
            Label l16 = CreateLabel("Kurum Sicil No", 300, yAlt+30); txtYakinKurumSicil = CreateText(400, yAlt+27, 150);
            Label l17 = CreateLabel("Kurum Adý", 300, yAlt+60); txtYakinKurumAdi = CreateText(400, yAlt+57, 150);

            // Butonlar
            int yBtn = 400;
            btnYeni = CreateButton("Yeni", 50, yBtn); btnYeni.Click += btnYeni_Click;
            btnKaydet = CreateButton("Kaydet", 150, yBtn); btnKaydet.Click += btnKaydet_Click;
            
            lblDurum = new Label(); lblDurum.Text = "< ÝÞLEM TAMAMLANDI >"; lblDurum.Location = new Point(250, yBtn+10); lblDurum.AutoSize = true; lblDurum.Font=new Font("Arial", 9, FontStyle.Bold);
            this.Controls.Add(lblDurum);

            btnCikis = CreateButton("Çýkýþ", 500, yBtn); btnCikis.Click += btnCikis_Click;
        }

        Label CreateLabel(string t, int x, int y) { Label l = new Label(); l.Text=t; l.Location=new Point(x,y); l.AutoSize=true; this.Controls.Add(l); return l; }
        TextBox CreateText(int x, int y, int w) { TextBox t = new TextBox(); t.Location=new Point(x,y); t.Width=w; this.Controls.Add(t); return t; }
        ComboBox CreateCombo(int x, int y, string[] i) { ComboBox c = new ComboBox(); c.Location=new Point(x,y); c.Width=80; c.Items.AddRange(i); this.Controls.Add(c); return c; }
        Button CreateButton(string t, int x, int y) { Button b = new Button(); b.Text=t; b.Location=new Point(x,y); b.Size=new Size(90,40); this.Controls.Add(b); return b; }
    }
}
