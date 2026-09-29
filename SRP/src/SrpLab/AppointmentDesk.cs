using System;
using System.Collections.Generic;

namespace SrpLab;

public sealed class ClinicSchedulePolicy
{
    public TimeOnly Open {get;}
    public TimeOnly Close {get;}
    public int SlotMinutes {get;}

    public ClinicSchedulePolicy(TimeOnly open, TimeOnly close, int slotMinutes)
    {
        Open = open;
        Close = close;
        SlotMinutes = slotMinutes;
    }

    public bool IsWithinBusinessHours(DateTimeOffset when)
    {
        if(when.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday) return false;
        var t = TimeOnly.FromDateTime(when.DateTime);
        return t >= Open && t.AddMinutes(SlotMinutes) <= Close;
    }
}

public sealed class AppointmentScheduler {
    private readonly HashSet<DateTimeOffset> _booked = new();
    private ClinicSchedulePolicy ClinicPolicy {get;}

    public AppointmentScheduler(ClinicSchedulePolicy c)
    {
        ClinicPolicy = c;
    }

    public DateTimeOffset? FindNextSlot(DateTimeOffset from, int searchHours)
    {
        var cursor = Align(from);
        var end = from.AddHours(searchHours);
        while (cursor < end)
        {
            if (ClinicPolicy.IsWithinBusinessHours(cursor) && !_booked.Contains(cursor))
                return cursor;
            cursor = cursor.AddMinutes(ClinicPolicy.SlotMinutes);
        }
        return null;
    }

    public bool TryBook(DateTimeOffset slot)
    {
        if (!ClinicPolicy.IsWithinBusinessHours(slot) || _booked.Contains(slot)) return false;
        _booked.Add(slot);
        return true;
    }

    private DateTimeOffset Align(DateTimeOffset from)
    {
        var minutes = from.Minute - (from.Minute % ClinicPolicy.SlotMinutes);
        return new DateTimeOffset(from.Year, from.Month, from.Day, from.Hour, minutes, 0, from.Offset);
    }

}

public static class IcsExportFormatter
{
    public static string ToIcs(DateTimeOffset slot, int slotMinutes, string patientName, string clinician)
    {
        var uid = Guid.NewGuid();
        var end = slot.AddMinutes(slotMinutes);
        return "BEGIN:VCALENDAR\nVERSION:2.0\nBEGIN:VEVENT\n" +
               $"UID:{uid}\nDTSTART:{slot:yyyyMMdd'T'HHmmss'Z'}\nDTEND:{end:yyyyMMdd'T'HHmmss'Z'}\n" +
               $"SUMMARY:Visit {patientName} / {clinician}\nEND:VEVENT\nEND:VCALENDAR\n";
    }
}

public static class SmsReminderFormatter
{
    public static string SmsReminder(DateTimeOffset slot, string clinicPhone)
    {
        return $"Reminder: appointment {slot:MMM dd HH:mm}. Call {clinicPhone} to reschedule.";
    }
}
