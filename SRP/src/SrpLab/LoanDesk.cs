namespace SrpLab;

/// <summary>
/// Loan desk: eligibility arithmetic, required-document lists, and rejection letter prose.
/// </summary>
public sealed class LoanDesk
{
    public decimal RequestedAmount { get; }
    public int CreditScore { get; }
    public int EmploymentMonths { get; }
    public bool HasCollateral { get; }

    public LoanDesk(decimal requestedAmount, int creditScore, int employmentMonths, bool hasCollateral)
    {
        RequestedAmount = requestedAmount;
        CreditScore = creditScore;
        EmploymentMonths = employmentMonths;
        HasCollateral = hasCollateral;
    }

    public decimal RiskScore()
    {
        // Risk model will change with risk committee — not with letter templates.
        decimal score = 100m;
        score -= Math.Max(0, 700 - CreditScore) * 0.15m;
        if (EmploymentMonths < 6) score -= 20m;
        if (RequestedAmount > 50_000m && !HasCollateral) score -= 25m;
        if (RequestedAmount > 150_000m) score -= 10m;
        return Math.Clamp(score, 0m, 100m);
    }

    public bool IsEligible() => RiskScore() >= 55m && CreditScore >= 580;

    public IReadOnlyList<string> RequiredDocuments()
    {
        // Compliance checklist changes with regulation, independently of risk formula.
        var docs = new List<string> { "National ID", "Proof of income (3 months)" };
        if (RequestedAmount > 40_000m) docs.Add("Bank statements (6 months)");
        if (HasCollateral) docs.Add("Collateral ownership deed");
        if (EmploymentMonths < 12) docs.Add("Employer letter");
        if (!IsEligible()) docs.Add("Manual underwriter referral form");
        return docs;
    }

    public string DecisionLetter(string applicantName)
    {
        // Legal/comms wording ≠ underwriting math.
        if (IsEligible())
        {
            return $"Dear {applicantName},\nYour request for {RequestedAmount:C} is pre-approved (risk {RiskScore():0}).\n" +
                   $"Please upload: {string.Join("; ", RequiredDocuments())}.\n";
        }

        return $"Dear {applicantName},\nWe are unable to approve {RequestedAmount:C} at this time.\n" +
               $"Reference risk={RiskScore():0}. You may reapply after improving documentation.\n";
    }

    public string UnderwriterCsvRow(string applicationId)
    {
        // Analytics export schema is yet another reason to change.
        return $"{applicationId},{CreditScore},{EmploymentMonths},{(HasCollateral ? 1 : 0)},{RiskScore():0.00},{(IsEligible() ? "Y" : "N")}";
    }
}
