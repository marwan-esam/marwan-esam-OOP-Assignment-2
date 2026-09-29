using System;
using System.Collections.Generic;

namespace SrpLab;

public class CourseEnrollmentDesk
{
    private readonly HashSet<string> _seated = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _waitlist = new();
    public int Capacity { get; }
    public string CourseCode { get; }

    public CourseEnrollmentDesk(string courseCode, int capacity)
    {
        CourseCode = courseCode;
        Capacity = capacity;
    }
    
    public IReadOnlySet<string> Seated => _seated;
    public int WaitlistCount => _waitlist.Count;

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

    public void PromoteFromWaitlist(int seats)
    {
        while (seats > 0 && _waitlist.Count > 0 && _seated.Count < Capacity)
        {
            var next = _waitlist[0];
            _waitlist.RemoveAt(0);
            _seated.Add(next);
            seats--;
        }
    }
}

public class WelcomePacketFormatter
{
    public string WelcomePacketMarkdown(string courseCode, string studentEmail, string studentName, CourseEnrollmentDesk desk)
    {
        var isSeated = desk.Seated.Contains(studentEmail);
        var status = isSeated ? "confirmed seat" : $"waitlist #{desk.WaitlistPosition(studentEmail)}";
        return $"# Welcome to {courseCode}\nHi {studentName},\nYour status: **{status}**.\n" +
               $"Bring a laptop. Discord onboarding link: https://example.invalid/{courseCode.ToLowerInvariant()}\n";
    }
}

public class TuitionInvoice
{
    public decimal Tuition { get; }

    public TuitionInvoice(decimal tuition)
    {
        Tuition = tuition;
    }

    public string TuitionInvoiceLine(string courseCode, string studentEmail, CourseEnrollmentDesk desk)
    {
        if (!desk.Seated.Contains(studentEmail)) return $"{courseCode},WAITLIST,0.00";
        var vat = Math.Round(Tuition * 0.14m, 2);
        return $"{courseCode},TUITION,{Tuition:0.00},VAT,{vat:0.00},TOTAL,{(Tuition + vat):0.00}";
    }
}
