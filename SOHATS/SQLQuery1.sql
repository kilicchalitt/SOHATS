-- 1. Kullanıcı Tablosu (Giriş yapacak kişiler)
CREATE TABLE kullanici (
    kullaniciKodu INT IDENTITY(1,1) PRIMARY KEY,
    kullaniciAd VARCHAR(20) NOT NULL,
    sifre VARCHAR(20) NOT NULL,
    yetki CHAR(1) DEFAULT '0', -- 1: Yönetici, 0: Personel
    ad VARCHAR(50),
    soyad VARCHAR(50),
    evTel VARCHAR(15),
    cepTel VARCHAR(15),
    adres VARCHAR(255),
    iseBaslama DATETIME,
    maas DECIMAL(18,2),
    -- Yeni Eklenenler
    tcKimlikNo VARCHAR(11),
    dogumYeri VARCHAR(50),
    babaAdi VARCHAR(20),
    anneAdi VARCHAR(20),
    dogumTarihi DATETIME,
    cinsiyet CHAR(5),
    medeniHal CHAR(10),
    kanGrubu CHAR(10),
    unvan VARCHAR(20)
);

-- İlk giriş için admin kullanıcısını hemen ekleyelim
INSERT INTO kullanici (kullaniciAd, sifre, ad, soyad, yetki, unvan, dogumYeri, medeniHal) 
VALUES ('admin', '1234', 'Yönetici', 'Personel', '1', 'Yönetici', 'ANKARA', 'BEKAR');

INSERT INTO kullanici (kullaniciAd, sifre, ad, soyad, yetki, unvan, dogumYeri, medeniHal)
VALUES ('dr1', '123', 'Ahmet', 'Yılmaz', '0', 'Doktor', 'İSTANBUL', 'EVLİ');

INSERT INTO kullanici (kullaniciAd, sifre, ad, soyad, yetki, unvan, dogumYeri, medeniHal)
VALUES ('hem1', '123', 'Ayşe', 'Demir', '0', 'Hemşire', 'İZMİR', 'BEKAR');

INSERT INTO kullanici (kullaniciAd, sifre, ad, soyad, yetki, unvan, dogumYeri, medeniHal)
VALUES ('per1', '123', 'Mehmet', 'Öztürk', '0', 'Personel', 'BURSA', 'EVLİ');

-- 2. Poliklinik Tablosu
CREATE TABLE poliklinik (
    poliklinikID INT IDENTITY(1,1) PRIMARY KEY,
    poliklinikAd VARCHAR(50) NOT NULL,
    durum BIT DEFAULT 1, -- Geçerli/Geçersiz durumu
    aciklama VARCHAR(255)
);

-- İlk test için örnek poliklinikleri ekleyelim
INSERT INTO poliklinik (poliklinikAd, aciklama) VALUES 
('Dahiliye', 'Dahiliye Polikliniği'),
('KBB', 'Kulak Burun Boğaz Polikliniği'),
('Göz', 'Göz Hastalıkları Polikliniği'),
('Ortopedi', 'Ortopedi ve Travmatoloji'),
('Çocuk', 'Çocuk Sağlığı ve Hastalıkları');

-- 3. Hasta Tablosu (Ana kimlik bilgileri)
CREATE TABLE hasta (
    dosyaNo CHAR(10) PRIMARY KEY, -- TC gibi sabit numara
    tcKimlikNo VARCHAR(11),
    ad VARCHAR(20),
    soyad VARCHAR(20),
    dogumYeri VARCHAR(50),
    dogumTarihi DATETIME,
    babaAdi VARCHAR(20),
    anneAdi VARCHAR(20),
    cinsiyet CHAR(5), 
    tel VARCHAR(15),
    kurumAdi VARCHAR(50),
    kurumSicilNo VARCHAR(20),
    yakinTel VARCHAR(15),
    yakinKurumSicilNo VARCHAR(20), -- Görselde sadece "Kurum Sicil No" vardı ama bir de Yakının verisi olabilir, görselde Yakının Kurum Sicil vs yok gibi ama 'Yakının Telefonu' var. Biz garanti olsun diye yakınTel ekledik. Görselde Kurum Sicil No ve Kurum Adı tek yerde.
    -- Tekrar görsele bakıyorum: Sol altta Tel, Kurum Sicil, Kurum Adı var. Sağ Altta Yakının Tel, Kurum Sicil, Kurum Adı var.
    -- Demek ki hem hasta hem yakını için ayrı alanlar lazım.
    yakinKurumAdi VARCHAR(50),
    kanGrubu CHAR(10),
    medeniHal CHAR(10),
    adres VARCHAR(255)
);

-- 4. Sevk Tablosu (HASTA HAREKETLERİ - En Kritik Tablo)
CREATE TABLE sevk (
    sevkID INT IDENTITY(1,1) PRIMARY KEY,
    dosyaNo CHAR(10) NOT NULL,
    poliklinikID INT NOT NULL, 
    sevkTarihi DATETIME DEFAULT GETDATE(), -- İşlem saati otomatik gelir
    saat CHAR(5), 
    siraNo INT, 
    yapilanIslem VARCHAR(255),
    drKod INT, 
    miktar INT DEFAULT 1,
    birimFiyat DECIMAL(18,2) DEFAULT 0,
    toplamTutar DECIMAL(18,2) DEFAULT 0,
    taburcu BIT DEFAULT 0, -- Taburcu oldu mu?
    
    -- İlişkileri kuralım (Veri tutarlılığı için)
    CONSTRAINT FK_Sevk_Hasta FOREIGN KEY (dosyaNo) REFERENCES hasta(dosyaNo),
    CONSTRAINT FK_Sevk_Poliklinik FOREIGN KEY (poliklinikID) REFERENCES poliklinik(poliklinikID)
);

-- 5. Çıkış Tablosu (Raporlama ve Muhasebeleşme için)
CREATE TABLE cikis (
    cikisID INT IDENTITY(1,1) PRIMARY KEY,
    dosyaNo CHAR(10),
    sevkTarihi DATETIME,
    cikisSaati DATETIME,
    toplamTutar DECIMAL(18,2)
);

-- 6. Doktor Tablosu (Referans Tanımı)
CREATE TABLE doktor (
    doktorKod INT IDENTITY(1,1) PRIMARY KEY,
    ad VARCHAR(30),
    soyad VARCHAR(30),
    poliklinikID INT,
    tel VARCHAR(15)
);

-- 7. İşlem Tablosu (Fiyat Listesi)
CREATE TABLE islem (
    islemID INT IDENTITY(1,1) PRIMARY KEY,
    islemAdi VARCHAR(50),
    birimFiyat DECIMAL(18,2)
);

INSERT INTO islem (islemAdi, birimFiyat) VALUES 
('Muayene', 50.00),
('Tahlil', 20.00),
('Röntgen', 30.00),
('Reçete', 10.00),
('Serum', 40.00),
('EKG', 25.00),
('Ultrason', 100.00);