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
    public partial class PoliklinikTanitma : Form
    {
        int _poliklinikID = 0;
        
        // Form Elemanları (Buraya taşındı)
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPoliklinikAdi;
        private System.Windows.Forms.CheckBox chkGecerli;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAciklama;
        private System.Windows.Forms.Button btnKaydet;
        private System.Windows.Forms.Button btnSil;
        private System.Windows.Forms.Button btnCikis;
        
        public PoliklinikTanitma()
        {
            //InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtPoliklinikAdi = new System.Windows.Forms.TextBox();
            this.chkGecerli = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAciklama = new System.Windows.Forms.TextBox();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.btnSil = new System.Windows.Forms.Button();
            this.btnCikis = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.Text = "Poliklinik Adı:";
            // 
            // txtPoliklinikAdi
            // 
            this.txtPoliklinikAdi.Location = new System.Drawing.Point(100, 27);
            this.txtPoliklinikAdi.Name = "txtPoliklinikAdi";
            this.txtPoliklinikAdi.Size = new System.Drawing.Size(200, 20);
            this.txtPoliklinikAdi.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPoliklinikAdi_KeyDown);
            // 
            // chkGecerli
            // 
            this.chkGecerli.AutoSize = true;
            this.chkGecerli.Checked = true;
            this.chkGecerli.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGecerli.Location = new System.Drawing.Point(100, 60);
            this.chkGecerli.Name = "chkGecerli";
            this.chkGecerli.Size = new System.Drawing.Size(107, 17);
            this.chkGecerli.Text = "Geçerli / Geçersiz";
            this.chkGecerli.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.Text = "Açıklama:";
            // 
            // txtAciklama
            // 
            this.txtAciklama.Location = new System.Drawing.Point(23, 110);
            this.txtAciklama.Multiline = true;
            this.txtAciklama.Name = "txtAciklama";
            this.txtAciklama.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAciklama.Size = new System.Drawing.Size(277, 100);
            // 
            // btnKaydet
            // 
            this.btnKaydet.Location = new System.Drawing.Point(23, 220);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(75, 40);
            this.btnKaydet.Text = "Kaydet";
            this.btnKaydet.UseVisualStyleBackColor = true;
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            // 
            // btnSil
            // 
            this.btnSil.Enabled = false;
            this.btnSil.Location = new System.Drawing.Point(125, 220);
            this.btnSil.Name = "btnSil";
            this.btnSil.Size = new System.Drawing.Size(75, 40);
            this.btnSil.Text = "Sil";
            this.btnSil.UseVisualStyleBackColor = true;
            this.btnSil.Click += new System.EventHandler(this.btnSil_Click);
            // 
            // btnCikis
            // 
            this.btnCikis.Location = new System.Drawing.Point(225, 220);
            this.btnCikis.Name = "btnCikis";
            this.btnCikis.Size = new System.Drawing.Size(75, 40);
            this.btnCikis.Text = "Çıkış";
            this.btnCikis.UseVisualStyleBackColor = true;
            this.btnCikis.Click += new System.EventHandler(this.btnCikis_Click);
            // 
            // PoliklinikTanitma
            // 
            this.ClientSize = new System.Drawing.Size(330, 280);
            this.Controls.Add(this.btnCikis);
            this.Controls.Add(this.btnSil);
            this.Controls.Add(this.btnKaydet);
            this.Controls.Add(this.txtAciklama);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkGecerli);
            this.Controls.Add(this.txtPoliklinikAdi);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Poliklinik Tanıtma";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void txtPoliklinikAdi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string aranan = txtPoliklinikAdi.Text.Trim();
                using (SqlCommand komut = new SqlCommand("SELECT * FROM poliklinik WHERE poliklinikAd=@ad", SQLHelper.Baglanti))
                {
                    komut.Parameters.AddWithValue("@ad", aranan);
                    SqlDataReader dr = komut.ExecuteReader();

                    if (dr.Read())
                    {
                        _poliklinikID = (int)dr["poliklinikID"];
                        chkGecerli.Checked = (bool)dr["durum"];
                        txtAciklama.Text = dr["aciklama"].ToString(); // Açıklamayı doldur
                        btnKaydet.Text = "Güncelle";
                        btnSil.Enabled = true;
                    }
                    else
                    {
                        DialogResult cevap = MessageBox.Show("Böyle bir poliklinik bulunamadı. Eklemek ister misiniz?", "Kayıt Yok", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (cevap == DialogResult.Yes)
                        {
                            _poliklinikID = 0;
                            btnKaydet.Text = "Kaydet";
                            btnSil.Enabled = false;
                            chkGecerli.Checked = true;
                        }
                        else
                        {
                            Temizle();
                        }
                    }
                }
            }

        }
        void Temizle()
        {
            txtPoliklinikAdi.Clear();
            txtAciklama.Clear(); // Açıklamayı temizle
            chkGecerli.Checked = true;
            btnKaydet.Text = "Kaydet";
            btnSil.Enabled = false;
            _poliklinikID = 0; // Hafızayı sıfırla
            txtPoliklinikAdi.Focus();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            // 1. Ad kısmı boşsa işlem yapma, uyar
            if (txtPoliklinikAdi.Text.Trim() == "")
            {
                MessageBox.Show("Lütfen poliklinik adı giriniz.");
                return;
            }

            using (SqlCommand komut = new SqlCommand("", SQLHelper.Baglanti))
            {
                // 2. Eğer ID 0 ise YENİ KAYIT (INSERT), değilse GÜNCELLEME (UPDATE)
                if (_poliklinikID == 0)
                {
                    komut.CommandText = "INSERT INTO poliklinik (poliklinikAd, durum, aciklama) VALUES (@ad, @durum, @aciklama)";
                }
                else
                {
                    komut.CommandText = "UPDATE poliklinik SET poliklinikAd=@ad, durum=@durum, aciklama=@aciklama WHERE poliklinikID=@id";
                    komut.Parameters.AddWithValue("@id", _poliklinikID);
                }

                // 3. Parametreleri ekle ve işlemi yap
                komut.Parameters.AddWithValue("@ad", txtPoliklinikAdi.Text.Trim());
                komut.Parameters.AddWithValue("@durum", chkGecerli.Checked);
                komut.Parameters.AddWithValue("@aciklama", txtAciklama.Text); // Açıklamayı ekle

                komut.ExecuteNonQuery(); // Veritabanına gönder

                MessageBox.Show("İşlem başarıyla tamamlandı.");
                Temizle(); // Formu temizle
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (_poliklinikID > 0)
            {
                DialogResult cevap = MessageBox.Show("Bu polikliniği silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (cevap == DialogResult.Yes)
                {
                    using (SqlCommand komut = new SqlCommand("DELETE FROM poliklinik WHERE poliklinikID=@id", SQLHelper.Baglanti))
                    {
                        komut.Parameters.AddWithValue("@id", _poliklinikID);
                        komut.ExecuteNonQuery();
                        MessageBox.Show("Kayıt silindi.");
                        Temizle();
                    }
                }
            }
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
