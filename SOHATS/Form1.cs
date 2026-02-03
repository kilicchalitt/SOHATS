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
    public partial class Form1 : Form
    {
        TextBox txtKullaniciAdi;
        TextBox txtSifre;
        Button btnGiris;
        Button btnTemizle;
        Button btnCikis;

        public Form1()
        {
            //InitializeComponent();
            InitUI();
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            if (txtKullaniciAdi.Text == "" || txtSifre.Text == "")
            {
                MessageBox.Show("Kullanıcı adı ve şifre zorunludur.");
                return;
            }

            using (SqlCommand komut = new SqlCommand("SELECT * FROM kullanici WHERE kullaniciAd=@kad AND sifre=@sifre", SQLHelper.Baglanti))
            {
                komut.Parameters.AddWithValue("@kad", txtKullaniciAdi.Text);
                komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
                
                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    this.Hide();
                    AnaForm ana = new AnaForm();
                    ana.Show();
                }
                else
                {
                    MessageBox.Show("Hatalı Kullanıcı Adı veya Şifre!");
                }
            }
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtKullaniciAdi.Clear();
            txtSifre.Clear();
            txtKullaniciAdi.Focus();
        }

        // --- MANUEL GUI ---
        private void InitUI()
        {
            // Form Ayarları
            this.ClientSize = new Size(320, 160); // Resimdeki gibi küçük ve kompakt
            this.Text = "SOHATS - Login";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow; // Sadece çarpı işareti olan ince çerçeve
            this.BackColor = SystemColors.Control; // Standart Windows Rengi (Mavilik Yok)
            this.ControlBox = true;
            this.MaximizeBox = false;

            // Fontlar
            Font lblFont = new Font("Microsoft Sans Serif", 9, FontStyle.Bold); // Resimdeki gibi biraz kalın
            Font txtFont = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);

            // Kullanıcı Adı
            Label lblKullanici = new Label();
            lblKullanici.Text = "Kullanıcı Adı";
            lblKullanici.Location = new Point(20, 30);
            lblKullanici.AutoSize = true;
            lblKullanici.Font = lblFont;
            this.Controls.Add(lblKullanici);

            txtKullaniciAdi = new TextBox();
            txtKullaniciAdi.Location = new Point(120, 27);
            txtKullaniciAdi.Width = 160;
            txtKullaniciAdi.Font = txtFont;
            this.Controls.Add(txtKullaniciAdi);

            // Şifre
            Label lblSifre = new Label();
            lblSifre.Text = "Şifre";
            lblSifre.Location = new Point(20, 65);
            lblSifre.AutoSize = true;
            lblSifre.Font = lblFont;
            this.Controls.Add(lblSifre);

            txtSifre = new TextBox();
            txtSifre.Location = new Point(120, 62);
            txtSifre.Width = 160;
            txtSifre.PasswordChar = '*';
            txtSifre.Font = txtFont;
            this.Controls.Add(txtSifre);

            // Butonlar
            int btnY = 100;
            
            btnGiris = new Button();
            btnGiris.Text = "Giriş";
            btnGiris.Location = new Point(20, btnY);
            btnGiris.Size = new Size(80, 35);
            btnGiris.UseVisualStyleBackColor = true; // Standart Görünüm (Renk Yok)
            btnGiris.Click += btnGiris_Click;
            this.Controls.Add(btnGiris);

            btnTemizle = new Button();
            btnTemizle.Text = "Temizle";
            btnTemizle.Location = new Point(115, btnY);
            btnTemizle.Size = new Size(80, 35);
            btnTemizle.UseVisualStyleBackColor = true;
            btnTemizle.Click += btnTemizle_Click;
            this.Controls.Add(btnTemizle);

            btnCikis = new Button();
            btnCikis.Text = "Çıkış";
            btnCikis.Location = new Point(210, btnY);
            btnCikis.Size = new Size(80, 35);
            btnCikis.UseVisualStyleBackColor = true;
            btnCikis.Click += btnCikis_Click;
            this.Controls.Add(btnCikis);
        }
    }
}
