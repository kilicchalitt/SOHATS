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
    public partial class KullaniciGirisFormu : Form
    {
        public KullaniciGirisFormu()
        {
            InitializeComponent();
        }

        private void KullaniciGirisFormu_Load(object sender, EventArgs e)
        {
            KullanicilariDoldur();
        }

        void KullanicilariDoldur()
        {
            cmbKullanicilar.Items.Clear();
            cmbKullanicilar.Text = "";
            
            // ComboBox'a kullanıcı kodlarını yükleyelim (İlk görseldeki gibi)
            using (SqlCommand komut = new SqlCommand("SELECT kullaniciKodu FROM kullanici", SQLHelper.Baglanti))
            {
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    cmbKullanicilar.Items.Add(dr["kullaniciKodu"].ToString());
                }
            }
        }

        private void cmbKullanicilar_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kullanıcı seçildiği an detay formunu açalım
            if (cmbKullanicilar.Text != "")
            {
                int secilenID = Convert.ToInt32(cmbKullanicilar.Text);
                KullaniciTanitma detayForm = new KullaniciTanitma();
                detayForm.GelenKullaniciID = secilenID; // ID'yi gönderiyoruz
                detayForm.ShowDialog(); // Formu aç
                
                // Form kapanınca (geri dönünce) tekrar listeyi doldur (belki silinmiştir)
                KullanicilariDoldur();
            }
        }

        private void btnYeni_Click(object sender, EventArgs e)
        {
            // Yeni kullanıcı için ID'siz açıyoruz
            KullaniciTanitma detayForm = new KullaniciTanitma();
            detayForm.GelenKullaniciID = 0; 
            detayForm.ShowDialog();
            KullanicilariDoldur();
        }

        // --- Tasarım Kodları ---
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbKullanicilar;
        private System.Windows.Forms.Button btnYeni;

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cmbKullanicilar = new System.Windows.Forms.ComboBox();
            this.btnYeni = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(30, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kullanıcı Kodu:";
            // 
            // cmbKullanicilar
            // 
            this.cmbKullanicilar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKullanicilar.FormattingEnabled = true;
            this.cmbKullanicilar.Location = new System.Drawing.Point(160, 38);
            this.cmbKullanicilar.Name = "cmbKullanicilar";
            this.cmbKullanicilar.Size = new System.Drawing.Size(150, 24);
            this.cmbKullanicilar.TabIndex = 1;
            this.cmbKullanicilar.SelectedIndexChanged += new System.EventHandler(this.cmbKullanicilar_SelectedIndexChanged);
            // 
            // btnYeni
            // 
            this.btnYeni.BackColor = System.Drawing.Color.PaleGreen;
            this.btnYeni.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnYeni.Location = new System.Drawing.Point(80, 80);
            this.btnYeni.Name = "btnYeni";
            this.btnYeni.Size = new System.Drawing.Size(200, 40);
            this.btnYeni.TabIndex = 2;
            this.btnYeni.Text = "Yeni Kullanıcı Ekle";
            this.btnYeni.UseVisualStyleBackColor = false;
            this.btnYeni.Click += new System.EventHandler(this.btnYeni_Click);
            // 
            // KullaniciGirisFormu
            // 
            this.ClientSize = new System.Drawing.Size(350, 150);
            this.Controls.Add(this.btnYeni);
            this.Controls.Add(this.cmbKullanicilar);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "KullaniciGirisFormu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SOHATS - Kullanıcı Tanıtma";
            this.Load += new System.EventHandler(this.KullaniciGirisFormu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
