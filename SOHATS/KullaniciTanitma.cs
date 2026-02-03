using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class KullaniciTanitma : Form
    {
        // Bu forma dışarıdan (KullaniciGirisFormu'ndan) ID gelecek.
        // 0 gelirse Yeni Kayıt, >0 gelirse Güncelleme demektir.
        public int GelenKullaniciID { get; set; } = 0;

        public KullaniciTanitma()
        {
            InitUI();
        }


        
        // ... (Diğer kodlar değişmeden kalır, sadece son metodu değiştiriyoruz)



        private void KullaniciTanitma_Load(object sender, EventArgs e)
        {
            // Form yüklenince ID kontrolü yapalım
            if (GelenKullaniciID > 0)
            {
                BilgileriGetir(GelenKullaniciID);
                txtKullaniciKodu.Text = GelenKullaniciID.ToString();
                btnKaydet.Text = "Güncelle";
                btnSil.Enabled = true;
            }
            else
            {
                Temizle(); // Yeni kayıt modu
            }
        }

        void BilgileriGetir(int id)
        {
            using (SqlCommand komut = new SqlCommand("SELECT * FROM kullanici WHERE kullaniciKodu=@id", SQLHelper.Baglanti))
            {
                komut.Parameters.AddWithValue("@id", id);
                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    txtKullaniciAdi.Text = dr["kullaniciAd"].ToString();
                    txtSifre.Text = dr["sifre"].ToString();
                    txtAd.Text = dr["ad"].ToString();
                    txtSoyad.Text = dr["soyad"].ToString();
                    txtTel.Text = dr["evTel"].ToString(); // "Telefon No"
                    txtGSM.Text = dr["cepTel"].ToString(); // "GSM"
                    txtAdres.Text = dr["adres"].ToString();

                    // Diğer Alanlar
                    txtTC.Text = dr["tcKimlikNo"].ToString();
                    txtDogumYeri.Text = dr["dogumYeri"].ToString();
                    txtBaba.Text = dr["babaAdi"].ToString();
                    txtAnne.Text = dr["anneAdi"].ToString();
                    txtMaas.Text = dr["maas"].ToString();
                    cmbUnvan.Text = dr["unvan"].ToString();
                    cmbCinsiyet.Text = dr["cinsiyet"].ToString();
                    cmbMedeni.Text = dr["medeniHal"].ToString();
                    cmbKan.Text = dr["kanGrubu"].ToString();

                    if(dr["iseBaslama"] != DBNull.Value)
                        dateIseBaslama.Value = (DateTime)dr["iseBaslama"];
                    
                    if(dr["dogumTarihi"] != DBNull.Value)
                        dateDogumTarihi.Value = (DateTime)dr["dogumTarihi"];

                    chkYetki.Checked = dr["yetki"].ToString() == "1";
                }
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            // Esnek kayıt: Sadece Kullanıcı Adı ve Şifre zorunlu
            if (txtKullaniciAdi.Text.Trim() == "" || txtSifre.Text.Trim() == "")
            {
                MessageBox.Show("Sisteme giriş için Kullanıcı Adı ve Şifre zorunludur. Diğer alanları boş bırakabilirsiniz.");
                return;
            }

            using (SqlCommand komut = new SqlCommand("", SQLHelper.Baglanti))
            {
                if (GelenKullaniciID == 0) // INSERT
                {
                    komut.CommandText = "INSERT INTO kullanici (kullaniciAd, sifre, ad, soyad, evTel, cepTel, adres, tcKimlikNo, dogumYeri, babaAdi, anneAdi, dogumTarihi, cinsiyet, medeniHal, kanGrubu, unvan, iseBaslama, maas, yetki) " +
                                        "VALUES (@kad, @sifre, @ad, @soyad, @ev, @cep, @adres, @tc, @dyeri, @baba, @anne, @dtarih, @cins, @medeni, @kan, @unvan, @ibaslama, @maas, @yetki)";
                }
                else // UPDATE
                {
                    komut.CommandText = "UPDATE kullanici SET kullaniciAd=@kad, sifre=@sifre, ad=@ad, soyad=@soyad, evTel=@ev, cepTel=@cep, adres=@adres, " +
                                        "tcKimlikNo=@tc, dogumYeri=@dyeri, babaAdi=@baba, anneAdi=@anne, dogumTarihi=@dtarih, cinsiyet=@cins, medeniHal=@medeni, kanGrubu=@kan, unvan=@unvan, iseBaslama=@ibaslama, maas=@maas, yetki=@yetki " +
                                        "WHERE kullaniciKodu=@id";
                    komut.Parameters.AddWithValue("@id", GelenKullaniciID);
                }

                // Parametreler
                komut.Parameters.AddWithValue("@kad", txtKullaniciAdi.Text);
                komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
                komut.Parameters.AddWithValue("@ad", txtAd.Text);
                komut.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                komut.Parameters.AddWithValue("@ev", txtTel.Text);
                komut.Parameters.AddWithValue("@cep", txtGSM.Text);
                komut.Parameters.AddWithValue("@adres", txtAdres.Text);
                komut.Parameters.AddWithValue("@tc", txtTC.Text);
                komut.Parameters.AddWithValue("@dyeri", txtDogumYeri.Text);
                komut.Parameters.AddWithValue("@baba", txtBaba.Text);
                komut.Parameters.AddWithValue("@anne", txtAnne.Text);
                komut.Parameters.AddWithValue("@dtarih", dateDogumTarihi.Value);
                komut.Parameters.AddWithValue("@cins", cmbCinsiyet.Text);
                komut.Parameters.AddWithValue("@medeni", cmbMedeni.Text);
                komut.Parameters.AddWithValue("@kan", cmbKan.Text);
                komut.Parameters.AddWithValue("@unvan", cmbUnvan.Text);
                komut.Parameters.AddWithValue("@ibaslama", dateIseBaslama.Value);
                komut.Parameters.AddWithValue("@yetki", chkYetki.Checked ? "1" : "0");

                decimal maas = 0;
                decimal.TryParse(txtMaas.Text, out maas);
                komut.Parameters.AddWithValue("@maas", maas);

                komut.ExecuteNonQuery();
                MessageBox.Show("İşlem Başarılı.");

                if (GelenKullaniciID == 0) // Yeni kayıtsa temizle ki yenisini girebilsin
                    Temizle();
                else
                    this.Close(); // Güncelleme ise kapat
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (GelenKullaniciID > 0)
            {
                if (MessageBox.Show("Bu kullanıcıyı silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    using (SqlCommand komut = new SqlCommand("DELETE FROM kullanici WHERE kullaniciKodu=@id", SQLHelper.Baglanti))
                    {
                        komut.Parameters.AddWithValue("@id", GelenKullaniciID);
                        komut.ExecuteNonQuery();
                        MessageBox.Show("Silindi.");
                        this.Close();
                    }
                }
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void Temizle()
        {
            // Kullanıcı Kodu hariç hepsini temizle, aslında kod da boş olabilir yeni kayıt için
            txtKullaniciKodu.Clear(); 
            txtTC.Clear();
            txtAd.Clear();
            txtSoyad.Clear();
            txtDogumYeri.Clear();
            txtBaba.Clear();
            txtAnne.Clear();
            txtTel.Clear();
            txtGSM.Clear();
            txtAdres.Clear();
            txtKullaniciAdi.Clear();
            txtSifre.Clear();
            txtMaas.Clear();
            
            cmbUnvan.SelectedIndex = -1;
            cmbCinsiyet.SelectedIndex = -1;
            cmbMedeni.SelectedIndex = -1;
            cmbKan.SelectedIndex = -1;
            
            chkYetki.Checked = false;
            dateIseBaslama.Value = DateTime.Now;
            dateDogumTarihi.Value = DateTime.Now;

            GelenKullaniciID = 0;
            btnKaydet.Text = "Kaydet";
            btnSil.Enabled = false;
        }

        // --- Designer Kodları ---
        // Sadece temel container ve controllari olusturuyorum, görsele benzer bir layout.
        private Label lblKodu, lblTC, lblAd, lblSoyad, lblDogumYeri, lblBaba, lblAnne, lblTel, lblGSM, lblAdres, lblUnvan, lblMaas, lblIseBaslama, lblDogumT, lblCinsiyet, lblMedeni, lblKan, lblKAdi, lblSifre;
        private TextBox txtKullaniciKodu, txtTC, txtAd, txtSoyad, txtDogumYeri, txtBaba, txtAnne, txtTel, txtGSM, txtAdres, txtMaas, txtKullaniciAdi, txtSifre;
        private ComboBox cmbUnvan, cmbCinsiyet, cmbMedeni, cmbKan;
        private DateTimePicker dateIseBaslama, dateDogumTarihi;
        private CheckBox chkYetki;
        private Button btnTemizle, btnKaydet, btnSil, btnCikis;
        private GroupBox grpKisisel, grpHesap;

        private void InitUI()
        {
            this.ClientSize = new System.Drawing.Size(700, 500);
            this.Text = "Kullanıcı Tanıtma";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Sol Taraf (Kişisel Bilgiler)
            lblKodu = CreateLabel("Kullanıcı Kodu:", 20, 20);
            txtKullaniciKodu = CreateTextBox(120, 17, 120); txtKullaniciKodu.Enabled = false; // Otomatik artıyor genelde veya ID sadece gösterilir

            lblTC = CreateLabel("T.C. Kimlik No:", 20, 50);
            txtTC = CreateTextBox(120, 47, 120);

            lblDogumYeri = CreateLabel("Doğum Yeri:", 20, 80);
            txtDogumYeri = CreateTextBox(120, 77, 120);

            lblBaba = CreateLabel("Baba Adı:", 20, 110);
            txtBaba = CreateTextBox(120, 107, 120);

            lblAnne = CreateLabel("Anne Adı:", 20, 140);
            txtAnne = CreateTextBox(120, 137, 120);

            lblTel = CreateLabel("Telefon No:", 20, 170);
            txtTel = CreateTextBox(120, 167, 120);

            lblGSM = CreateLabel("GSM:", 20, 200);
            txtGSM = CreateTextBox(120, 197, 120);

            chkYetki = new CheckBox(); chkYetki.Text = "Yetkili Kullanıcı"; chkYetki.Location = new Point(120, 230); chkYetki.AutoSize = true;
            this.Controls.Add(chkYetki);

            lblAdres = CreateLabel("Adres:", 20, 260);
            txtAdres = CreateTextBox(120, 260, 500);
            txtAdres.Multiline = true; txtAdres.Height = 60;

            // Sağ Taraf
            int sagX = 350; int sagY = 20;
            lblUnvan = CreateLabel("Unvan:", sagX, sagY);
            cmbUnvan = CreateCombo(sagX + 100, sagY - 3, new string[] { "Doktor", "Hemşire", "Sağlık Personeli", "Sekreter" });
            
            lblAd = CreateLabel("Ad:", sagX, sagY + 30);
            txtAd = CreateTextBox(sagX + 100, sagY + 27, 120);

            lblSoyad = CreateLabel("Soyad:", sagX, sagY + 60);
            txtSoyad = CreateTextBox(sagX + 100, sagY + 57, 120);

            lblMaas = CreateLabel("Maaş:", sagX, sagY + 90);
            txtMaas = CreateTextBox(sagX + 100, sagY + 87, 120);

            lblIseBaslama = CreateLabel("İşe Başlama:", sagX, sagY + 120);
            dateIseBaslama = CreateDate(sagX + 100, sagY + 117);

            lblDogumT = CreateLabel("Doğum Tarihi:", sagX, sagY + 150);
            dateDogumTarihi = CreateDate(sagX + 100, sagY + 147);

            lblCinsiyet = CreateLabel("Cinsiyet:", sagX, sagY + 180);
            cmbCinsiyet = CreateCombo(sagX + 100, sagY + 177, new string[] { "Erkek", "Kadın" });
            cmbCinsiyet.Width = 60;

            lblMedeni = CreateLabel("Medeni Hal:", sagX + 170, sagY + 180);
            cmbMedeni = CreateCombo(sagX + 240, sagY + 177, new string[] { "Bekar", "Evli" });
            cmbMedeni.Width = 60;

            lblKan = CreateLabel("Kan Grubu:", sagX, sagY + 210);
            cmbKan = CreateCombo(sagX + 100, sagY + 207, new string[] { "A Rh+", "A Rh-", "B Rh+", "B Rh-", "0 Rh+", "0 Rh-", "AB Rh+", "AB Rh-" });

            // Alt Kısım (Kullanıcı Adı Şifre ve Butonlar)
            int altY = 340;
            lblKAdi = CreateLabel("Kullanıcı Adı:", 20, altY);
            txtKullaniciAdi = CreateTextBox(120, altY - 3, 120);

            lblSifre = CreateLabel("Şifre:", 350, altY);
            txtSifre = CreateTextBox(450, altY - 3, 120);

            // Butonlar
            int btnY = 380;
            btnTemizle = CreateButton("Temizle", 20, btnY, btnTemizle_Click);
            btnKaydet = CreateButton("Kaydet/Güncelle", 120, btnY, btnKaydet_Click); btnKaydet.Width = 120;
            btnSil = CreateButton("Sil", 260, btnY, btnSil_Click);
            btnCikis = CreateButton("Çıkış", 550, btnY, btnCikis_Click);
        }

        // Helper Methods for UI Creation
        Label CreateLabel(string text, int x, int y)
        {
            Label l = new Label(); l.Text = text; l.Location = new Point(x, y); l.AutoSize = true;
            this.Controls.Add(l); return l;
        }
        TextBox CreateTextBox(int x, int y, int w)
        {
            TextBox t = new TextBox(); t.Location = new Point(x, y); t.Width = w;
            this.Controls.Add(t); return t;
        }
        ComboBox CreateCombo(int x, int y, string[] items)
        {
            ComboBox c = new ComboBox(); c.Location = new Point(x, y); c.Width = 120;
            c.Items.AddRange(items);
            this.Controls.Add(c); return c;
        }
        DateTimePicker CreateDate(int x, int y)
        {
            DateTimePicker d = new DateTimePicker(); d.Location = new Point(x, y); d.Format = DateTimePickerFormat.Short; d.Width = 120;
            this.Controls.Add(d); return d;
        }
        Button CreateButton(string text, int x, int y, EventHandler click)
        {
            Button b = new Button(); b.Text = text; b.Location = new Point(x, y); b.Size = new Size(80, 40); b.Click += click;
            this.Controls.Add(b); return b;
        }
    }
}
