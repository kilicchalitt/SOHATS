using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class DosyaBul : Form
    {
        public string SecilenDosyaNo { get; set; } = "";

        public DosyaBul()
        {
            InitializeComponent();
        }

        private void DosyaBul_Load(object sender, EventArgs e)
        {
            cmbKriter.SelectedIndex = 0; // Varsayılan: Hasta Adı Soyadı
        }

        private void cmbKriter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eğer "Hasta Adı Soyadı" seçiliyse "ve" kutusu ve 2. textbox aktif olsun
            if (cmbKriter.Text == "Hasta Adı Soyadı")
            {
                chkVe.Visible = true;
                chkVe.Checked = true;
                txtDeger2.Visible = true;
                txtDeger1.Width = 120; // Küçült
            }
            else
            {
                chkVe.Visible = false;
                txtDeger2.Visible = false;
                txtDeger1.Width = 270; // Genişlet
            }
        }

        private void btnBul_Click(object sender, EventArgs e)
        {
            string sql = "SELECT dosyaNo, ad, soyad, tcKimlikNo, kurumAdi, kurumSicilNo FROM hasta WHERE 1=1";
            
            string kriter = cmbKriter.Text;
            string val1 = txtDeger1.Text.Trim();
            string val2 = txtDeger2.Text.Trim();

            if (kriter == "Hasta Adı Soyadı")
            {
                if (val1 != "") sql += " AND ad LIKE @p1 + '%'";
                if (chkVe.Checked && val2 != "") sql += " AND soyad LIKE @p2 + '%'";
            }
            else if (kriter == "Kimlik No")
            {
                if (val1 != "") sql += " AND tcKimlikNo LIKE @p1 + '%'";
            }
            else if (kriter == "Kurum Sicil No")
            {
                if (val1 != "") sql += " AND kurumSicilNo LIKE @p1 + '%'";
            }
            else if (kriter == "Dosya No")
            {
                if (val1 != "") sql += " AND dosyaNo LIKE @p1 + '%'";
            }

            using (SqlDataAdapter da = new SqlDataAdapter(sql, SQLHelper.Baglanti))
            {
                if (val1 != "") da.SelectCommand.Parameters.AddWithValue("@p1", val1);
                if (val2 != "") da.SelectCommand.Parameters.AddWithValue("@p2", val2);
                
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SecilenDosyaNo = dataGridView1.Rows[e.RowIndex].Cells["dosyaNo"].Value.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        // --- GUI Oluşturma ---
        private Label lblKriter;
        private ComboBox cmbKriter;
        private TextBox txtDeger1, txtDeger2;
        private CheckBox chkVe;
        private Button btnBul;
        private DataGridView dataGridView1;

        private void InitializeComponent()
        {
            this.ClientSize = new Size(600, 400);
            this.Text = "< Dosya Bul >";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            // Üst Panel
            Panel pnlUst = new Panel(); pnlUst.Dock = DockStyle.Top; pnlUst.Height = 60; pnlUst.BackColor = SystemColors.Control;
            this.Controls.Add(pnlUst);

            lblKriter = new Label(); lblKriter.Text = "Arama Kriteri"; lblKriter.Location = new Point(10, 20); lblKriter.AutoSize = true; lblKriter.Font = new Font("Arial", 9, FontStyle.Bold);
            pnlUst.Controls.Add(lblKriter);

            cmbKriter = new ComboBox(); cmbKriter.Location = new Point(100, 17); cmbKriter.Width = 140; cmbKriter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKriter.Items.AddRange(new string[] { "Hasta Adı Soyadı", "Kimlik No", "Kurum Sicil No", "Dosya No" });
            cmbKriter.SelectedIndexChanged += cmbKriter_SelectedIndexChanged;
            pnlUst.Controls.Add(cmbKriter);

            txtDeger1 = new TextBox(); txtDeger1.Location = new Point(250, 17); txtDeger1.Width = 120;
            pnlUst.Controls.Add(txtDeger1);

            chkVe = new CheckBox(); chkVe.Text = "ve"; chkVe.Location = new Point(380, 19); chkVe.AutoSize = true;
            pnlUst.Controls.Add(chkVe);

            txtDeger2 = new TextBox(); txtDeger2.Location = new Point(420, 17); txtDeger2.Width = 100;
            pnlUst.Controls.Add(txtDeger2);

            btnBul = new Button(); btnBul.Text = "Bul"; btnBul.Location = new Point(530, 15); btnBul.Size = new Size(60, 25);
            btnBul.Click += btnBul_Click;
            pnlUst.Controls.Add(btnBul);

            // Grid
            dataGridView1 = new DataGridView();
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.BackgroundColor = Color.Gray; // Görseldeki gibi gri arka plan (içi değil, boşluk)
            dataGridView1.BackgroundColor = SystemColors.ControlDark;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            this.Controls.Add(dataGridView1);
        }
    }
}
