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
    public partial class DoktorTanitma : Form
    {
        int _doktorKod = 0; // Seçili doktorun ID'si (0 ise yeni kayıt)

        public DoktorTanitma()
        {
            InitializeComponent();
        }

        private void DoktorTanitma_Load(object sender, EventArgs e)
        {
            PoliklinikYukle();
        }

        void PoliklinikYukle()
        {
            cmbPoliklinik.Items.Clear();
            using (SqlCommand komut = new SqlCommand("SELECT poliklinikAd FROM poliklinik WHERE durum='True'", SQLHelper.Baglanti))
            {
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    cmbPoliklinik.Items.Add(dr["poliklinikAd"].ToString());
                }
            }
        }

        private void txtAd_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter'a basınca isme göre arama yapalım (Basit arama)
            if (e.KeyCode == Keys.Enter)
            {
                string aranan = txtAd.Text.Trim();
                using (SqlCommand komut = new SqlCommand("SELECT * FROM doktor WHERE ad LIKE @ad + '%'", SQLHelper.Baglanti))
                {
                    komut.Parameters.AddWithValue("@ad", aranan);
                    SqlDataReader dr = komut.ExecuteReader();

                    if (dr.Read())
                    {
                        // Kayıt bulundu, formu doldur
                        _doktorKod = (int)dr["doktorKod"];
                        txtAd.Text = dr["ad"].ToString();
                        txtSoyad.Text = dr["soyad"].ToString();
                        txtTel.Text = dr["tel"].ToString();
                        
                        // Poliklinik ID'den Adı bulmamız lazım ama basitlik için direkt ID'yi tutmuyoruz, ismini bulup seçtirelim.
                        // Daha doğrusu veritabanında PoliklinikID tutuyoruz.
                        int poliID = (int)dr["poliklinikID"];
                        
                        // Bu ID'ye ait poliklinik adını bulmak için yeni bir sorgu gerekebilir veya 
                        // combo box itemlarında dönebiliriz. Basit yöntemle sadece ID'yi alıp adını bulalım.
                        dr.Close(); // İlk reader'ı kapat
                        
                        using (SqlCommand pKomut = new SqlCommand("SELECT poliklinikAd FROM poliklinik WHERE poliklinikID=@id", SQLHelper.Baglanti))
                        {
                            pKomut.Parameters.AddWithValue("@id", poliID);
                            object sonuc = pKomut.ExecuteScalar();
                            if(sonuc != null) cmbPoliklinik.Text = sonuc.ToString();
                        }

                        btnKaydet.Text = "Güncelle";
                        btnSil.Enabled = true;
                    }
                    else
                    {
                         MessageBox.Show("Doktor bulunamadı, yeni kayıt yapabilirsiniz.");
                         Temizle(sadeceID: true);
                    }
                }
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txtAd.Text == "" || txtSoyad.Text == "" || cmbPoliklinik.Text == "")
            {
                MessageBox.Show("Lütfen Ad, Soyad ve Poliklinik seçiniz.");
                return;
            }

            // Poliklinik ID'sini bul
            int secilenPoliID = 0;
            using (SqlCommand poliKomut = new SqlCommand("SELECT poliklinikID FROM poliklinik WHERE poliklinikAd=@ad", SQLHelper.Baglanti))
            {
                poliKomut.Parameters.AddWithValue("@ad", cmbPoliklinik.Text);
                object sonuc = poliKomut.ExecuteScalar();
                if (sonuc != null) secilenPoliID = Convert.ToInt32(sonuc);
            }

            using (SqlCommand komut = new SqlCommand("", SQLHelper.Baglanti))
            {
                if (_doktorKod == 0) // Yeni Kayıt
                {
                     komut.CommandText = "INSERT INTO doktor (ad, soyad, poliklinikID, tel) VALUES (@ad, @soyad, @pid, @tel)";
                }
                else // Güncelleme
                {
                     komut.CommandText = "UPDATE doktor SET ad=@ad, soyad=@soyad, poliklinikID=@pid, tel=@tel WHERE doktorKod=@kod";
                     komut.Parameters.AddWithValue("@kod", _doktorKod);
                }

                komut.Parameters.AddWithValue("@ad", txtAd.Text.Trim());
                komut.Parameters.AddWithValue("@soyad", txtSoyad.Text.Trim());
                komut.Parameters.AddWithValue("@pid", secilenPoliID);
                komut.Parameters.AddWithValue("@tel", txtTel.Text);
                
                komut.ExecuteNonQuery();
                MessageBox.Show("İşlem Başarılı");
                Temizle();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
             if (_doktorKod > 0)
            {
                if (MessageBox.Show("Silmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (SqlCommand komut = new SqlCommand("DELETE FROM doktor WHERE doktorKod=@kod", SQLHelper.Baglanti))
                    {
                        komut.Parameters.AddWithValue("@kod", _doktorKod);
                        komut.ExecuteNonQuery();
                        MessageBox.Show("Silindi.");
                        Temizle();
                    }
                }
            }
        }

        void Temizle(bool sadeceID = false)
        {
            if(!sadeceID) 
            {
                txtAd.Clear();
                txtSoyad.Clear();
                txtTel.Clear();
                cmbPoliklinik.SelectedIndex = -1;
            }
            
            if(!sadeceID) _doktorKod = 0;
            
            btnKaydet.Text = "Kaydet";
            btnSil.Enabled = false;
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Tasarımcı Kodu (Designer Code) - Normalde Designer.cs dosyasında olur ama
        /// tek dosyada birleştirdik. Form elemanlarını burada oluşturuyoruz.
        /// </summary>
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSoyad;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox txtTel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbPoliklinik;
        private System.Windows.Forms.Button btnKaydet;
        private System.Windows.Forms.Button btnSil;
        private System.Windows.Forms.Button btnCikis;

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtAd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSoyad = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTel = new System.Windows.Forms.MaskedTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbPoliklinik = new System.Windows.Forms.ComboBox();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.btnSil = new System.Windows.Forms.Button();
            this.btnCikis = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ad:";
            // 
            // txtAd
            // 
            this.txtAd.Location = new System.Drawing.Point(100, 27);
            this.txtAd.Name = "txtAd";
            this.txtAd.Size = new System.Drawing.Size(150, 20);
            this.txtAd.TabIndex = 1;
            this.txtAd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAd_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Soyad:";
            // 
            // txtSoyad
            // 
            this.txtSoyad.Location = new System.Drawing.Point(100, 57);
            this.txtSoyad.Name = "txtSoyad";
            this.txtSoyad.Size = new System.Drawing.Size(150, 20);
            this.txtSoyad.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Tel:";
            // 
            // txtTel
            // 
            this.txtTel.Location = new System.Drawing.Point(100, 87);
            this.txtTel.Mask = "(999) 000-0000";
            this.txtTel.Name = "txtTel";
            this.txtTel.Size = new System.Drawing.Size(150, 20);
            this.txtTel.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Poliklinik:";
            // 
            // cmbPoliklinik
            // 
            this.cmbPoliklinik.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPoliklinik.FormattingEnabled = true;
            this.cmbPoliklinik.Location = new System.Drawing.Point(100, 117);
            this.cmbPoliklinik.Name = "cmbPoliklinik";
            this.cmbPoliklinik.Size = new System.Drawing.Size(150, 21);
            this.cmbPoliklinik.TabIndex = 7;
            // 
            // btnKaydet
            // 
            this.btnKaydet.Location = new System.Drawing.Point(33, 160);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(75, 40);
            this.btnKaydet.TabIndex = 8;
            this.btnKaydet.Text = "Kaydet";
            this.btnKaydet.UseVisualStyleBackColor = true;
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            // 
            // btnSil
            // 
            this.btnSil.Enabled = false;
            this.btnSil.Location = new System.Drawing.Point(114, 160);
            this.btnSil.Name = "btnSil";
            this.btnSil.Size = new System.Drawing.Size(75, 40);
            this.btnSil.TabIndex = 9;
            this.btnSil.Text = "Sil";
            this.btnSil.UseVisualStyleBackColor = true;
            this.btnSil.Click += new System.EventHandler(this.btnSil_Click);
            // 
            // btnCikis
            // 
            this.btnCikis.Location = new System.Drawing.Point(195, 160);
            this.btnCikis.Name = "btnCikis";
            this.btnCikis.Size = new System.Drawing.Size(75, 40);
            this.btnCikis.TabIndex = 10;
            this.btnCikis.Text = "Çıkış";
            this.btnCikis.UseVisualStyleBackColor = true;
            this.btnCikis.Click += new System.EventHandler(this.btnCikis_Click);
            // 
            // DoktorTanitma
            // 
            this.ClientSize = new System.Drawing.Size(300, 230);
            this.Controls.Add(this.btnCikis);
            this.Controls.Add(this.btnSil);
            this.Controls.Add(this.btnKaydet);
            this.Controls.Add(this.cmbPoliklinik);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSoyad);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAd);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DoktorTanitma";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Doktor Tanıtma";
            this.Load += new System.EventHandler(this.DoktorTanitma_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
