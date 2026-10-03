using System;
using System.Collections.Generic;
using System.Linq;

namespace SrpLab;

public class BedTracker
{
    private readonly Dictionary<int, string> _bedPatient = new();
    
    public void AssignBed(int bed, string patientId)
    {
        if (bed <= 0) throw new ArgumentOutOfRangeException(nameof(bed));
        if (string.IsNullOrWhiteSpace(patientId)) throw new ArgumentException("patient required");

        _bedPatient[bed] = patientId.Trim().ToUpperInvariant();
    }
    
    public string GetPatient(int bed) => _bedPatient.TryGetValue(bed, out var p) ? p : null;
    public IReadOnlyCollection<int> Beds => _bedPatient.Keys;
}

public class ClinicalAcuityPolicy
{
    public int ScoreAcuity(int heartRate, int spo2)
    {
        var score = 0;
        if (heartRate > 120 || heartRate < 45) score += 4;
        else if (heartRate > 100) score += 2;
        if (spo2 < 90) score += 5;
        else if (spo2 < 94) score += 2;
        return Math.Min(score, 10);
    }
}

public class PagerEscalationPolicy
{
    private readonly List<string> _pagerLog = new();

    public void EvaluateAcuity(int bed, int acuity)
    {
        if (acuity >= 8)
            _pagerLog.Add($"CODE-YELLOW bed={bed} at {DateTime.UtcNow:HH:mm}");
    }

    public IReadOnlyList<string> DrainPagerLog()
    {
        var copy = _pagerLog.ToList();
        _pagerLog.Clear();
        return copy;
    }
}

public class HandoffNoteFormatter
{
    public string BuildHandoffNote(int bed, string patient, int acuity)
    {
        if (patient == null)
            return $"Bed {bed}: empty";

        var tone = acuity >= 8 ? "ESCALATE" : acuity >= 4 ? "WATCH" : "STABLE";
        return $"[HANDOFF {DateTime.UtcNow:yyyy-MM-dd}] Bed {bed} · {patient} · acuity={acuity} · {tone}";
    }
}

public class CensusCsvExporter
{
    public string ExportCensusCsv(BedTracker tracker, Dictionary<int, int> vitalsScores)
    {
        var lines = new List<string> { "bed,patient,acuity" };
        foreach (var bed in tracker.Beds.OrderBy(x => x))
            lines.Add($"{bed},{tracker.GetPatient(bed)},{vitalsScores.GetValueOrDefault(bed, 0)}");
        return string.Join('\n', lines);
    }
}
