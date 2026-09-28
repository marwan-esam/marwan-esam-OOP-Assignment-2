namespace SrpLab;

/// <summary>
/// Subscription billing: proration math, invoice number minting, and dunning email bodies.
/// </summary>
public sealed class SubscriptionBilling
{
    private static int _invoiceSeq = 1000;

    public string CustomerId { get; }
    public decimal MonthlyPrice { get; }
    public DateOnly PeriodStart { get; }
    public DateOnly PeriodEnd { get; }
    public int FailedPayments { get; private set; }

    public SubscriptionBilling(string customerId, decimal monthlyPrice, DateOnly periodStart, DateOnly periodEnd)
    {
        CustomerId = customerId;
        MonthlyPrice = monthlyPrice;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }

    public decimal Prorate(DateOnly activeFrom)
    {
        // Finance calendar rules change independently of email copy.
        if (activeFrom <= PeriodStart) return MonthlyPrice;
        if (activeFrom >= PeriodEnd) return 0m;
        var totalDays = PeriodEnd.DayNumber - PeriodStart.DayNumber;
        if (totalDays <= 0) return MonthlyPrice;
        var used = PeriodEnd.DayNumber - activeFrom.DayNumber;
        return Math.Round(MonthlyPrice * used / totalDays, 2);
    }

    public string NextInvoiceNumber()
    {
        // Numbering scheme / fiscal prefixes — ops concern, not pricing.
        var n = ++_invoiceSeq;
        return $"INV-{PeriodStart:yyyyMM}-{n:D5}";
    }

    public void RegisterFailedPayment() => FailedPayments++;

    public string DunningEmail(string customerName, DateOnly asOf)
    {
        // Collections tone & legal boilerplate ≠ proration formula.
        var amount = Prorate(PeriodStart);
        var invoice = NextInvoiceNumber(); // side-effect while composing mail — nasty on purpose
        var severity = FailedPayments switch
        {
            <= 1 => "friendly reminder",
            2 => "second notice",
            _ => "final notice before suspension"
        };
        return $"Subject: {severity} {invoice}\nHi {customerName},\nBalance {amount:C} as of {asOf:o} ({FailedPayments} failures).\n";
    }

    public string LedgerJournalLine(DateOnly activeFrom)
    {
        // Accounting export format is another axis of change.
        return $"{CustomerId},{NextInvoiceNumber()},{Prorate(activeFrom):0.00},AR-SUB";
    }
}
