namespace SrpLab;

/// <summary>
/// Support ticket: SLA clocks, NLP-ish priority from free text, and public reply templates.
/// </summary>
public sealed class SupportTicket
{
    public string Id { get; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public DateTimeOffset OpenedAt { get; }
    public string Priority { get; private set; } = "P3";

    public SupportTicket(string id, string subject, string body, DateTimeOffset openedAt)
    {
        Id = id;
        Subject = subject;
        Body = body;
        OpenedAt = openedAt;
        RecalculatePriorityFromText();
    }

    public void AppendCustomerMessage(string text)
    {
        Body += "\n---\n" + text;
        RecalculatePriorityFromText();
    }

    public void RecalculatePriorityFromText()
    {
        // Keyword heuristics will churn with support playbooks; SLA math will not.
        var blob = (Subject + " " + Body).ToLowerInvariant();
        if (blob.Contains("down") || blob.Contains("outage") || blob.Contains("cannot login"))
            Priority = "P1";
        else if (blob.Contains("urgent") || blob.Contains("asap") || blob.Contains("blocked"))
            Priority = "P2";
        else
            Priority = "P3";
    }

    public DateTimeOffset SlaDeadline()
    {
        // Operational SLA policy — different stakeholders than reply wording.
        var hours = Priority switch
        {
            "P1" => 4,
            "P2" => 24,
            _ => 72
        };
        return OpenedAt.AddHours(hours);
    }

    public bool IsBreached(DateTimeOffset now) => now > SlaDeadline();

    public string DraftPublicReply(string agentName)
    {
        // Tone/templates owned by CX — not by SLA engineering.
        var apology = Priority == "P1" ? "We are treating this as a critical incident." : "Thanks for reaching out.";
        return $"Hi,\n{apology}\nTicket {Id} is with {agentName}. Next update before {SlaDeadline():u}.\n";
    }

    public string InternalEscalationBlurb()
    {
        return $"ESCALATE {Id} priority={Priority} breachAt={SlaDeadline():u} keywords-scanned=yes";
    }
}
