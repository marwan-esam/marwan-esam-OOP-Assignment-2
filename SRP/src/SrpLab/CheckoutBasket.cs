namespace SrpLab;

/// <summary>
/// Online checkout basket: totals, coupon linguistics, gift-wrap copy, and a faux payment auth code.
/// </summary>
public sealed class CheckoutBasket
{
    private readonly List<(string Sku, decimal Price, int Qty)> _lines = new();
    private string? _couponRaw;
    private bool _giftWrap;

    public void AddLine(string sku, decimal price, int qty)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        _lines.Add((sku, price, qty));
    }

    public void ApplyCouponText(string? couponText) => _couponRaw = couponText;
    public void EnableGiftWrap() => _giftWrap = true;

    public decimal SubTotal() => _lines.Sum(l => l.Price * l.Qty);

    public decimal DiscountAmount()
    {
        // Parsing marketing strings is a different reason to change than pricing math.
        if (string.IsNullOrWhiteSpace(_couponRaw)) return 0m;
        var t = _couponRaw.Trim().ToUpperInvariant();
        if (t.StartsWith("SAVE") && int.TryParse(t[4..], out var pct) && pct is > 0 and <= 50)
            return Math.Round(SubTotal() * pct / 100m, 2);
        if (t.Contains("FREESHIP")) return 0m; // ship is elsewhere — still parsed here
        if (t == "WELCOME10") return Math.Min(10m, SubTotal());
        return 0m;
    }

    public decimal GrandTotal()
    {
        var total = SubTotal() - DiscountAmount();
        if (_giftWrap) total += 4.99m; // packaging fee policy ≠ cart math
        return Math.Max(0m, total);
    }

    public string GiftMessageCard(string fromName)
    {
        // Customer-facing copy will change with marketing, not with totals.
        var items = string.Join(", ", _lines.Select(l => l.Sku));
        return $"Dear friend,\nA gift from {fromName} awaits ({items}).\nTotal surprise value: {GrandTotal():C}\n";
    }

    public string AuthorizePaymentStub(string cardLast4)
    {
        // Pretends to talk to a gateway — auth scheme changes independently of cart rules.
        var payload = $"{GrandTotal():0.00}|{cardLast4}|{_lines.Count}";
        var hash = payload.GetHashCode();
        return $"AUTH-{Math.Abs(hash):X8}";
    }
}
