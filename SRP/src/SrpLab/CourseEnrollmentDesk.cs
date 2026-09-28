namespace SrpLab;

/// <summary>
/// Course enrollment: capacity, waitlist math, welcome-packet markdown, and invoice lines.
/// </summary>
public sealed class CourseEnrollmentDesk
{
    private readonly HashSet<string> _seated = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _waitlist = new();
    public int Capacity { get; }
    public decimal Tuition { get; }
    public string CourseCode { get; }

    public CourseEnrollmentDesk(string courseCode, int capacity, decimal tuition)
    {
        CourseCode = courseCode;
        Capacity = capacity;
        Tuition = tuition;
    }

    public string Register(string studentEmail)
    {
        if (string.IsNullOrWhiteSpace(studentEmail)) throw new ArgumentException("email");
        var email = studentEmail.Trim();

        if (_seated.Contains(email) || _waitlist.Contains(email))
            return "ALREADY_REGISTERED";

        if (_seated.Count < Capacity)
        {
            _seated.Add(email);
            return "SEATED";
        }

        _waitlist.Add(email);
        return $"WAITLIST:{_waitlist.Count}";
    }

    public int WaitlistPosition(string studentEmail)
    {
        var idx = _waitlist.FindIndex(x => x.Equals(studentEmail, StringComparison.OrdinalIgnoreCase));
        return idx < 0 ? -1 : idx + 1;
    }

    public string WelcomePacketMarkdown(string studentEmail, string studentName)
    {
        // Content design changes with academy marketing — not with seat algorithms.
        var status = _seated.Contains(studentEmail) ? "confirmed seat" : $"waitlist #{WaitlistPosition(studentEmail)}";
        return $"# Welcome to {CourseCode}\nHi {studentName},\nYour status: **{status}**.\n" +
               $"Bring a laptop. Discord onboarding link: https://example.invalid/{CourseCode.ToLowerInvariant()}\n";
    }

    public string TuitionInvoiceLine(string studentEmail)
    {
        // Finance formatting / tax later — separate from enrollment capacity.
        if (!_seated.Contains(studentEmail)) return $"{CourseCode},WAITLIST,0.00";
        var vat = Math.Round(Tuition * 0.14m, 2);
        return $"{CourseCode},TUITION,{Tuition:0.00},VAT,{vat:0.00},TOTAL,{(Tuition + vat):0.00}";
    }

    public void PromoteFromWaitlist(int seats)
    {
        // Operational promotion policy mixed with messaging responsibilities above.
        while (seats > 0 && _waitlist.Count > 0 && _seated.Count < Capacity)
        {
            var next = _waitlist[0];
            _waitlist.RemoveAt(0);
            _seated.Add(next);
            seats--;
        }
    }
}
