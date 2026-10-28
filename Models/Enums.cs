namespace JastiperBlazor.Models;

public enum OrderStatus
{
    PendingDP,              // Baru masuk, belum DP
    DPReceived,             // Sudah bayar DP
    OrderedToSupplier,      // Sudah dipesan ke supplier (sudah bayar ke supplier)
    InTransit,              // Dalam perjalanan dari supplier
    Arrived,                // Barang sudah sampai di toko
    AwaitingFullPayment,    // Sudah dihubungi untuk bayar full + ongkir
    FullPaymentReceived,    // Sudah bayar full
    Shipped,                // Sudah dikirim ke customer
    Completed,              // Selesai
    Cancelled,
    PartiallyArrived        // Sebagian barang sudah sampai di toko
}

public enum PaymentType
{
    DownPayment,
    FullPayment,
    Shipping
}
