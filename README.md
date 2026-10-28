# Jastiper Blazor App

Sistem manajemen jastiper (personal shopper) berbasis **Blazor Server** + **MudBlazor** + **SQLite**.

## Fitur Lengkap

- **Dashboard**: Ringkasan status order
- **Customers**: CRUD + riwayat pembelian lengkap
- **Orders**: 
  - Buat order dengan multi-item
  - Workflow status lengkap: PendingDP → DPReceived → OrderedToSupplier → Arrived → AwaitingFullPayment → FullPaymentReceived → Shipped → Completed
  - Catat DP dan Full Payment + Ongkir
- **Supplier Batches**:
  - Buat batch pemesanan ke Korea/China
  - Assign multiple order yang sudah DP ke satu batch
  - Catat pembayaran ke supplier
- **Laporan**:
  - Item terlaris (qty & revenue)
  - Customer terbaik (untuk promosi)
- Database lokal SQLite (`jastiper.db`) — mudah di-backup
- Seed data contoh saat pertama kali dijalankan

## Cara Menjalankan

1. Install **.NET 10 SDK**
2. Buka terminal di folder project
3. Jalankan:
   ```bash
   dotnet restore
   dotnet run
   ```
4. Buka browser ke alamat yang muncul (biasanya `https://localhost:5xxx`)

## Alur Kerja yang Didukung

1. Terima order via WA → input ke aplikasi (nama, WA, item, harga jual, tanggal)
2. Setelah customer bayar DP → catat DP
3. Kumpulkan beberapa order → buat Supplier Batch → assign order ke batch → tandai sudah dipesan & bayar ke supplier
4. Barang datang → tandai Arrived
5. Kontak customer untuk ongkir + full payment → catat pembayaran
6. Kirim barang → tandai Shipped / Completed
7. Lihat laporan untuk promosi customer setia & analisa produk laris

## Pengembangan Selanjutnya (jika dibutuhkan)

- Login multi-user
- Export Excel
- Template pesan WhatsApp
- Upload bukti transfer
- Notifikasi

Aplikasi ini sudah siap digunakan untuk menggantikan Excel.
