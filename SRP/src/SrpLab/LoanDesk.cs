using System;
using System.Collections.Generic;

namespace SrpLab;

public class LoanApplication
{
    public decimal RequestedAmount { get; }
    public int CreditScore { get; }
    public int EmploymentMonths { get; }
    public bool HasCollateral { get; }

    public LoanApplication(decimal requestedAmount, int creditScore, int employmentMonths, bool hasCollateral)
    {
        RequestedAmount = requestedAmount;
        CreditScore = creditScore;
        EmploymentMonths = employmentMonths;
        HasCollateral = hasCollateral;
    }
}

public class RiskAssessor
{
    public decimal RiskScore(LoanApplication app)
    {
        decimal score = 100m;
        score -= Math.Max(0, 700 - app.CreditScore) * 0.15m;
        if (app.EmploymentMonths < 6) score -= 20m;
        if (app.RequestedAmount > 50_000m && !app.HasCollateral) score -= 25m;
        if (app.RequestedAmount > 150_000m) score -= 10m;
        return Math.Clamp(score, 0m, 100m);
    }

    public bool IsEligible(decimal riskScore, int creditScore) => riskScore >= 55m && creditScore >= 580;
}

public class ComplianceChecklist
{
    public IReadOnlyList<string> RequiredDocuments(LoanApplication app, bool isEligible)
    {
        var docs = new List<string> { "National ID", "Proof of income (3 months)" };
        if (app.RequestedAmount > 40_000m) docs.Add("Bank statements (6 months)");
        if (app.HasCollateral) docs.Add("Collateral ownership deed");
        if (app.EmploymentMonths < 12) docs.Add("Employer letter");
        if (!isEligible) docs.Add("Manual underwriter referral form");
        return docs;
    }
}

public class DecisionLetterFormatter
{
    public string DecisionLetter(string applicantName, decimal requestedAmount, decimal riskScore, bool isEligible, IReadOnlyList<string> docs)
    {
        if (isEligible)
        {
            return $"Dear {applicantName},\nYour request for {requestedAmount:C} is pre-approved (risk {riskScore:0}).\n" +
                   $"Please upload: {string.Join("; ", docs)}.\n";
        }

        return $"Dear {applicantName},\nWe are unable to approve {requestedAmount:C} at this time.\n" +
               $"Reference risk={riskScore:0}. You may reapply after improving documentation.\n";
    }
}

public class UnderwriterCsvExporter
{
    public string UnderwriterCsvRow(string applicationId, LoanApplication app, decimal riskScore, bool isEligible)
    {
        return $"{applicationId},{app.CreditScore},{app.EmploymentMonths},{(app.HasCollateral ? 1 : 0)},{riskScore:0.00},{(isEligible ? "Y" : "N")}";
    }
}
