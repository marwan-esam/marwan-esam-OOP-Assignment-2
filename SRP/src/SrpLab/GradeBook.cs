using System;
using System.Collections.Generic;
using System.Linq;

namespace SrpLab;

public class GradeAggregator
{
    private readonly Dictionary<string, List<decimal>> _scores = new(StringComparer.OrdinalIgnoreCase);

    public void Record(string studentId, decimal score)
    {
        if (score is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(score));
        if (!_scores.TryGetValue(studentId, out var list))
        {
            list = new List<decimal>();
            _scores[studentId] = list;
        }
        list.Add(score);
    }

    public decimal Average(string studentId)
    {
        if (!_scores.TryGetValue(studentId, out var list) || list.Count == 0) return 0m;
        return Math.Round(list.Average(), 2);
    }

    public IReadOnlyCollection<string> StudentIds => _scores.Keys;
}

public class AcademicPolicy
{
    public string Letter(decimal average)
    {
        if (average >= 90) return "A";
        if (average >= 80) return "B";
        if (average >= 70) return "C";
        if (average >= 60) return "D";
        return "F";
    }

    public bool MeetsHonorRoll(decimal average)
    {
        return average >= 85 && Letter(average) is "A" or "B";
    }
}

public class TranscriptFormatter
{
    public string TranscriptPlain(string studentId, string fullName, decimal average, string letter, bool honorRoll)
    {
        return $"TRANSCRIPT\nStudent: {fullName} ({studentId})\nAverage: {average}\nLetter: {letter}\nHonor: {honorRoll}\n";
    }

    public string ExportCsv(GradeAggregator aggregator, AcademicPolicy policy)
    {
        var rows = new List<string> { "studentId,average,letter,honor" };
        foreach (var id in aggregator.StudentIds.OrderBy(x => x))
        {
            var avg = aggregator.Average(id);
            var letter = policy.Letter(avg);
            var honor = policy.MeetsHonorRoll(avg);
            rows.Add($"{id},{avg},{letter},{(honor ? 1 : 0)}");
        }
        return string.Join('\n', rows);
    }
}
