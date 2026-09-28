namespace SrpLab;

/// <summary>
/// Hospital ward board: tracks beds, computes acuity scores, drafts nurse handoff notes,
/// and decides which pager code to fire. Looks like "one ward concern" — it is not.
/// </summary>
public sealed class WardBoard
{
    private readonly Dictionary<int, string> _bedPatient = new();
    private readonly Dictionary<int, int> _vitalsScore = new();
    private readonly List<string> _pagerLog = new();

    public void AssignBed(int bed, string patientId, int heartRate, int spo2)
    {
        if (bed <= 0) throw new ArgumentOutOfRangeException(nameof(bed));
        if (string.IsNullOrWhiteSpace(patientId)) throw new ArgumentException("patient required");

        _bedPatient[bed] = patientId.Trim().ToUpperInvariant();
        _vitalsScore[bed] = ScoreAcuity(heartRate, spo2);

        if (_vitalsScore[bed] >= 8)
            _pagerLog.Add($"CODE-YELLOW bed={bed} at {DateTime.UtcNow:HH:mm}");
    }

    public int ScoreAcuity(int heartRate, int spo2)
    {
        // Clinical scoring mixed with arbitrary pager thresholds (policy will churn separately).
        var score = 0;
        if (heartRate > 120 || heartRate < 45) score += 4;
        else if (heartRate > 100) score += 2;
        if (spo2 < 90) score += 5;
        else if (spo2 < 94) score += 2;
        return Math.Min(score, 10);
    }

    public string BuildHandoffNote(int bed)
    {
        if (!_bedPatient.TryGetValue(bed, out var patient))
            return $"Bed {bed}: empty";

        var acuity = _vitalsScore[bed];
        var tone = acuity >= 8 ? "ESCALATE" : acuity >= 4 ? "WATCH" : "STABLE";
        // Presentation / narrative format will change without clinical rules changing.
        return $"[HANDOFF {DateTime.UtcNow:yyyy-MM-dd}] Bed {bed} · {patient} · acuity={acuity} · {tone}";
    }

    public IReadOnlyList<string> DrainPagerLog()
    {
        var copy = _pagerLog.ToList();
        _pagerLog.Clear();
        return copy;
    }

    public string ExportCensusCsv()
    {
        // Persistence/export shape mixed into the same type as acuity + paging.
        var lines = new List<string> { "bed,patient,acuity" };
        foreach (var bed in _bedPatient.Keys.OrderBy(x => x))
            lines.Add($"{bed},{_bedPatient[bed]},{_vitalsScore[bed]}");
        return string.Join('\n', lines);
    }
}
