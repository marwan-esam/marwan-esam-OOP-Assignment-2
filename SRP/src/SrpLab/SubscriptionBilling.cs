using System;

namespace SrpLab;

public class SubscriptionProration
{
    public decimal MonthlyPrice { get; }
    public DateOnly PeriodStart { get; }
    public DateOnly PeriodEnd { get; }

    public SubscriptionProration(decimal monthlyPrice, DateOnly periodStart, DateOnly periodEnd)
    {
        MonthlyPrice = monthlyPrice;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }

    public decimal Prorate(DateOnly activeFrom)
    {
        if (activeFrom <= PeriodStart) return MonthlyPrice;
        if (activeFrom >= PeriodEnd) return 0m;
        var totalDays = PeriodEnd.DayNumber - PeriodStart.DayNumber;
        if (totalDays <= 0) return MonthlyPrice;
        var used = PeriodEnd.DayNumber - activeFrom.DayNumber;
        return Math.Round(MonthlyPrice * used / totalDays, 2);
    }
}

public static class InvoiceNumberGenerator
{
    private static int _invoiceSeq = 1000;

    public static string NextInvoiceNumber(DateOnly periodStart)
    {
        var n = ++_invoiceSeq;
        return $"INV-{periodStart:yyyyMM}-{n:D5}";
    }
}

public class PaymentStatusTracker
{
    public int FailedPayments { get; private set; }
    public void RegisterFailedPayment() => FailedPayments++;
}

public class DunningEmailFormatter
{
    public string DunningEmail(string customerName, DateOnly asOf, int failedPayments, decimal amount, string invoiceNumber)
    {
        var severity = failedPayments switch
        {
            <= 1 => "friendly reminder",
            2 => "second notice",
            _ => "final notice before suspension"
        };
        return $"Subject: {severity} {invoiceNumber}\nHi {customerName},\nBalance {amount:C} as of {asOf:o} ({failedPayments} failures).\n";
    }
}

public class LedgerJournalExporter
{
    public string LedgerJournalLine(string customerId, string invoiceNumber, decimal amount)
    {
        return $"{customerId},{invoiceNumber},{amount:0.00},AR-SUB";
    }
}
