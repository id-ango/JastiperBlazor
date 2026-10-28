namespace JastiperBlazor.Models;

public record DPRecord(decimal Amount, DateTime Date);

public record FullPaymentRecord(decimal Amount, decimal Shipping, DateTime Date);
