using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOHATS
{
    public partial class AnaForm : Form
    {
        public AnaForm()
        {
            InitializeComponent();
        }

        private void poliklinikTanıtmaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PoliklinikTanitma pForm = new PoliklinikTanitma();
            pForm.MdiParent = this; // Bu formun içinde açılsın
            pForm.Show();
        }

        private void referanslarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Bu event boş kalabilir veya silinebilir, çünkü alt menüler iş yapacak
        }

        private void doktorTanıtmaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoktorTanitma dForm = new DoktorTanitma();
            dForm.MdiParent = this;
            dForm.Show();
        }

        private void raporlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
             RaporForm rForm = new RaporForm();
             rForm.MdiParent = this;
             rForm.Show();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart(); // Uygulamayı yeniden başlatarak login ekranına döner
        }

        private void kullanıcıTanıtmaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KullaniciGirisFormu kGiris = new KullaniciGirisFormu();
            kGiris.ShowDialog(); 
        }

        private void hastaİşlemleriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HastaTanitma hForm = new HastaTanitma();
            hForm.MdiParent = this;
            hForm.Show();
        }

        private void sevkİşlemleriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SevkIslemleri sevkForm = new SevkIslemleri();
            sevkForm.MdiParent = this;
            sevkForm.Show();
        }
    }
}
