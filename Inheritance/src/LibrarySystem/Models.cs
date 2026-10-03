using System;
using System.Collections.Generic;
using System.Linq;

namespace LibrarySystem;

public enum LoanStatus { Borrowed, Returned, Lost }

public abstract class Person
{
    public string PersonId { get; }
    public string FullName { get; }
    public string Phone { get; }

    protected Person(string personId, string fullName, string phone)
    {
        if (string.IsNullOrWhiteSpace(personId) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("ID, Name, and Phone must not be empty");

        PersonId = personId;
        FullName = fullName;
        Phone = phone;
    }
}

public abstract class Member : Person
{
    private readonly List<Loan> _loans = new();
    public IReadOnlyList<Loan> Loans => _loans.AsReadOnly();

    protected int MaxLoans { get; }
    public decimal Discount { get; }

    protected Member(string personId, string fullName, string phone, int maxLoans, decimal discount) 
        : base(personId, fullName, phone)
    {
        MaxLoans = maxLoans;
        Discount = discount;
    }

    public Loan Borrow(LibraryItem item, DateOnly borrowDate)
    {
        if (item.IsWithdrawn) throw new InvalidOperationException("Cannot borrow a withdrawn item.");
        if (item.IsOnLoan) throw new InvalidOperationException("Item is already on loan.");
        if (_loans.Count(l => l.Status == LoanStatus.Borrowed) >= MaxLoans)
            throw new InvalidOperationException($"Cannot exceed {MaxLoans} active loans.");

        var loan = new Loan(Guid.NewGuid().ToString(), borrowDate, this, item);
        item.IsOnLoan = true;
        _loans.Add(loan);
        return loan;
    }
}

public class StudentMember : Member
{
    public StudentMember(string personId, string fullName, string phone)
        : base(personId, fullName, phone, 3, 0m) { }
}

public class PremiumMember : Member
{
    public PremiumMember(string personId, string fullName, string phone, decimal discount)
        : base(personId, fullName, phone, 10, discount) { }

    public int ReadingPoints => Loans.Count(l => l.Status == LoanStatus.Returned) * 5;
}

public abstract class Staff : Person
{
    public DateOnly HireDate { get; }
    public decimal MonthlySalary { get; private set; }
    protected decimal ResponsibilityAllowance { get; }

    protected Staff(string personId, string fullName, string phone, DateOnly hireDate, decimal startingSalary, decimal allowance)
        : base(personId, fullName, phone)
    {
        if (startingSalary <= 0) throw new ArgumentException("Salary must be positive.");
        HireDate = hireDate;
        MonthlySalary = startingSalary;
        ResponsibilityAllowance = allowance;
    }

    public decimal MonthlyPay => MonthlySalary + ResponsibilityAllowance;

    public void GiveRaise(decimal percentage)
    {
        if (percentage <= 0) throw new ArgumentException("Raise must be positive.");
        MonthlySalary += MonthlySalary * (percentage / 100m);
    }
}

public class Librarian : Staff
{
    public Librarian(string personId, string fullName, string phone, DateOnly hireDate, decimal startingSalary)
        : base(personId, fullName, phone, hireDate, startingSalary, 0m) { }
}

public class Shelver : Staff
{
    public string Section { get; private set; }

    public Shelver(string personId, string fullName, string phone, DateOnly hireDate, decimal startingSalary, string section)
        : base(personId, fullName, phone, hireDate, startingSalary, 0m)
    {
        Section = section;
    }

    public void Reassign(string newSection)
    {
        if (string.IsNullOrWhiteSpace(newSection)) throw new ArgumentException();
        Section = newSection;
    }
}

public class HeadLibrarian : Staff
{
    public HeadLibrarian(string personId, string fullName, string phone, DateOnly hireDate, decimal startingSalary)
        : base(personId, fullName, phone, hireDate, startingSalary, 400m) { }
}

public abstract class LibraryItem
{
    public string CatalogNumber { get; }
    public string Title { get; }
    public int LoanPeriodDays { get; }
    public decimal BaseLateFee { get; private set; }
    protected decimal LateFeeMultiplier { get; }
    
    public bool IsWithdrawn { get; private set; }
    internal bool IsOnLoan { get; set; }

    public decimal DailyLateFee => BaseLateFee * LateFeeMultiplier;

    protected LibraryItem(string catalogNumber, string title, int loanPeriodDays, decimal baseLateFee, decimal multiplier)
    {
        CatalogNumber = catalogNumber;
        Title = title;
        LoanPeriodDays = loanPeriodDays;
        BaseLateFee = baseLateFee;
        LateFeeMultiplier = multiplier;
    }

    public void AdjustBaseFee(decimal newFee)
    {
        if (newFee <= 0) throw new ArgumentException("Fee must be positive.");
        BaseLateFee = newFee;
    }

    public void Withdraw() => IsWithdrawn = true;
    public void Restore() => IsWithdrawn = false;
}

public class Book : LibraryItem
{
    public Book(string catalogNumber, string title, decimal baseLateFee)
        : base(catalogNumber, title, 21, baseLateFee, 1m) { }
}

public class DVD : LibraryItem
{
    public DVD(string catalogNumber, string title, decimal baseLateFee)
        : base(catalogNumber, title, 7, baseLateFee, 2m) { }
}

public class Magazine : LibraryItem
{
    public Magazine(string catalogNumber, string title, decimal baseLateFee)
        : base(catalogNumber, title, 3, baseLateFee, 0.5m) { }
}

public class Loan
{
    public string LoanId { get; }
    public DateOnly BorrowDate { get; }
    public Member Member { get; }
    public LibraryItem Item { get; }
    
    public DateOnly DueDate => BorrowDate.AddDays(Item.LoanPeriodDays);
    public LoanStatus Status { get; private set; }
    public DateOnly? ReturnDate { get; private set; }

    public Loan(string loanId, DateOnly borrowDate, Member member, LibraryItem item)
    {
        if (member == null || item == null) throw new ArgumentNullException();
        LoanId = loanId;
        BorrowDate = borrowDate;
        Member = member;
        Item = item;
        Status = LoanStatus.Borrowed;
    }

    public decimal LateFee
    {
        get
        {
            if (Status != LoanStatus.Returned || !ReturnDate.HasValue) return 0m;
            int lateDays = ReturnDate.Value.DayNumber - DueDate.DayNumber;
            if (lateDays <= 0) return 0m;
            var fee = (lateDays * Item.DailyLateFee) - Member.Discount;
            return Math.Max(0m, fee);
        }
    }

    public void ReturnItem(DateOnly returnDate)
    {
        if (Status != LoanStatus.Borrowed) throw new InvalidOperationException("Loan is not currently borrowed.");
        if (returnDate < BorrowDate) throw new ArgumentException("Return date cannot be before borrow date.");
        
        Status = LoanStatus.Returned;
        ReturnDate = returnDate;
        Item.IsOnLoan = false;
    }

    public void MarkAsLost()
    {
        if (Status != LoanStatus.Borrowed) throw new InvalidOperationException("Only borrowed items can be marked lost.");
        Status = LoanStatus.Lost;
    }
}
