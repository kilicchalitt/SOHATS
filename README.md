SOHATS - Sağlık Ocağı Otomasyon Sistemi
Bu proje, bir sağlık kuruluşunun günlük operasyonel süreçlerini (hasta kayıt, poliklinik sevkleri ve finansal takip) dijital ortamda yönetmek amacıyla geliştirilmiş kapsamlı bir masaüstü otomasyon sistemidir.

🛠 Teknik Yetkinlikler ve Araçlar
Proje geliştirme sürecinde aşağıdaki teknolojiler ve yöntemler tercih edilmiştir:

Programlama Dili: C#

Arayüz Teknolojisi: Windows Forms (WinForms)

Veritabanı Yönetimi: Microsoft SQL Server LocalDB (.mdf)

Veri Erişim Katmanı: ADO.NET (Sorgu bazlı veri yönetimi için)

Mimari Yaklaşım: Nesne Yönelimli Programlama (OOP) prensipleri

🚀 Öne Çıkan Fonksiyonel Özellikler
Güvenli Kimlik Doğrulama: Yetkilendirilmiş kullanıcı girişi modülü.

Dinamik Hasta Yönetimi: TC Kimlik numarası üzerinden hasta kayıt, dosya açma ve geçmiş bilgileri görüntüleme.

Poliklinik ve İşlem Takibi: Hastaların ilgili birimlere sevki ve yapılan tıbbi işlemlerin gerçek zamanlı takibi.

Otomatik Maliyet Hesaplama: Miktar ve birim fiyat üzerinden toplam tutarın otomatik hesaplanması ve listelenmesi.

Taburcu ve Arşivleme: Tedavi süreci tamamlanan hastaların dosyalarının kapatılması ve veritabanı üzerinde arşivlenmesi.

🧠 Teknik Detaylar ve Mimari Kararlar
Bir bilgisayar mühendisliği öğrencisi olarak, projenin sadece çalışmasına değil, veri yönetimi süreçlerine de odaklanılmıştır:

ADO.NET Kullanımı: Hazır ORM araçları yerine doğrudan ADO.NET tercih edilerek, veritabanı üzerindeki SQL hakimiyeti ve performans yönetimi el ile sağlanmıştır.

Taşınabilir Veritabanı: Proje, SQL Server kurulumu gerektirmeyen LocalDB mimarisiyle tasarlanmıştır; bu sayede projenin farklı ortamlarda kurulum gerektirmeden çalışması mümkün kılınmıştır.

Hata Yönetimi: Veritabanı işlemleri sırasında oluşabilecek bağlantı hataları ve veri tutarsızlıkları için kapsamlı "Try-Catch" blokları ve kontrol mekanizmaları uygulanmıştır.
