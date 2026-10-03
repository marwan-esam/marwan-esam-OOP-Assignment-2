using System;
using System.Collections.Generic;
using System.Linq;

namespace SrpLab;

public class CheckoutBasket
{
    private readonly List<(string Sku, decimal Price, int Qty)> _lines = new();
    public void AddLine(string sku, decimal price, int qty)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        _lines.Add((sku, price, qty));
    }
    public IReadOnlyList<(string Sku, decimal Price, int Qty)> Lines => _lines;
    public decimal SubTotal() => _lines.Sum(l => l.Price * l.Qty);
}

public class CouponParser
{
    public decimal DiscountAmount(string? couponRaw, decimal subTotal)
    {
        if (string.IsNullOrWhiteSpace(couponRaw)) return 0m;
        var t = couponRaw.Trim().ToUpperInvariant();
        if (t.StartsWith("SAVE") && int.TryParse(t[4..], out var pct) && pct is > 0 and <= 50)
            return Math.Round(subTotal * pct / 100m, 2);
        if (t.Contains("FREESHIP")) return 0m;
        if (t == "WELCOME10") return Math.Min(10m, subTotal);
        return 0m;
    }
}

public class GiftWrapPolicy
{
    public decimal CalculateGiftWrapFee(bool giftWrapEnabled) => giftWrapEnabled ? 4.99m : 0m;
}

public class GiftMessageFormatter
{
    public string GiftMessageCard(string fromName, CheckoutBasket basket, decimal grandTotal)
    {
        var items = string.Join(", ", basket.Lines.Select(l => l.Sku));
        return $"Dear friend,\nA gift from {fromName} awaits ({items}).\nTotal surprise value: {grandTotal:C}\n";
    }
}

public class PaymentGatewayStub
{
    public string AuthorizePaymentStub(decimal grandTotal, string cardLast4, int lineCount)
    {
        var payload = $"{grandTotal:0.00}|{cardLast4}|{lineCount}";
        var hash = payload.GetHashCode();
        return $"AUTH-{Math.Abs(hash):X8}";
    }
}
