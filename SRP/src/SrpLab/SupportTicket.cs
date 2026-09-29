using System;

namespace SrpLab;

public class TicketData
{
    public string Id { get; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public DateTimeOffset OpenedAt { get; }

    public TicketData(string id, string subject, string body, DateTimeOffset openedAt)
    {
        Id = id;
        Subject = subject;
        Body = body;
        OpenedAt = openedAt;
    }

    public void AppendCustomerMessage(string text)
    {
        Body += "\n---\n" + text;
    }
}

public class PriorityScanner
{
    public string RecalculatePriorityFromText(TicketData ticket)
    {
        var blob = (ticket.Subject + " " + ticket.Body).ToLowerInvariant();
        if (blob.Contains("down") || blob.Contains("outage") || blob.Contains("cannot login"))
            return "P1";
        if (blob.Contains("urgent") || blob.Contains("asap") || blob.Contains("blocked"))
            return "P2";
        return "P3";
    }
}

public class SlaPolicy
{
    public DateTimeOffset SlaDeadline(DateTimeOffset openedAt, string priority)
    {
        var hours = priority switch
        {
            "P1" => 4,
            "P2" => 24,
            _ => 72
        };
        return openedAt.AddHours(hours);
    }

    public bool IsBreached(DateTimeOffset deadline, DateTimeOffset now) => now > deadline;
}

public class PublicReplyFormatter
{
    public string DraftPublicReply(string agentName, string ticketId, string priority, DateTimeOffset deadline)
    {
        var apology = priority == "P1" ? "We are treating this as a critical incident." : "Thanks for reaching out.";
        return $"Hi,\n{apology}\nTicket {ticketId} is with {agentName}. Next update before {deadline:u}.\n";
    }
}

public class InternalEscalationFormatter
{
    public string InternalEscalationBlurb(string ticketId, string priority, DateTimeOffset deadline)
    {
        return $"ESCALATE {ticketId} priority={priority} breachAt={deadline:u} keywords-scanned=yes";
    }
}
