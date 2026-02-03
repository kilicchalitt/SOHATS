using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SOHATS
{
    class SQLHelper
    {
        // Veritabanı dosyasının yolunu dinamik olarak (proje klasöründe) bulur.
        // Böylece projeyi başka bilgisayara taşıdığında hata vermez.
        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\SOHATS.mdf;Integrated Security=True";
        //private static string connectionString = @"Server=.\SQLEXPRESS;Database=HastaTakip;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection Baglanti
        {
            get
            {
                SqlConnection conn = new SqlConnection(connectionString);
                // Eğer bağlantı kapalıysa açar, açıksa dokunmaz.
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                return conn;
            }
        }
    }
}