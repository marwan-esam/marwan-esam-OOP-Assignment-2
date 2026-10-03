using System;
using System.Collections.Generic;
using LibrarySystem;

class Program
{
    static void Main()
    {
        // 1. Try to create plain Person, Member, Staff, or LibraryItem (must NOT compile)
        // Person p = new Person("1", "Ali", "123");
        // Member m = new Member("2", "Omar", "123", 3, 0);
        // Staff s = new Staff("3", "Sara", "123", DateOnly.FromDateTime(DateTime.Now), 1000, 0);
        // LibraryItem item = new LibraryItem("A1", "Title", 10, 1m, 1m);
        // StudentMember sm = new StudentMember("4", "Nora", "123");
        // sm.FullName = "New Name"; // (must NOT compile)
        // sm.Loans.Add(new Loan(...)); // (must NOT compile)
        // sm.IsOnLoan = true; // (must NOT compile from outside assembly)

        Console.WriteLine("=== Business Rules Tests ===");

        // Borrow a withdrawn item
        var book1 = new Book("B1", "C# Basics", 1m);
        book1.Withdraw();
        var student1 = new StudentMember("S1", "Ali", "555-0100");
        try
        {
            student1.Borrow(book1, new DateOnly(2026, 1, 1));
        }
        catch (Exception ex) { Console.WriteLine($"Withdrawn Test: Rejected ({ex.Message})"); }

        // Borrow an item already on loan
        var book2 = new Book("B2", "Design Patterns", 1m);
        student1.Borrow(book2, new DateOnly(2026, 1, 1));
        var student2 = new StudentMember("S2", "Sara", "555-0200");
        try
        {
            student2.Borrow(book2, new DateOnly(2026, 1, 2));
        }
        catch (Exception ex) { Console.WriteLine($"Already Loaned Test: Rejected ({ex.Message})"); }

        // Make a Student member borrow a 4th item
        var book3 = new Book("B3", "Clean Code", 1m);
        var book4 = new Book("B4", "Refactoring", 1m);
        var book5 = new Book("B5", "Domain Driven Design", 1m);
        student1.Borrow(book3, new DateOnly(2026, 1, 1));
        student1.Borrow(book4, new DateOnly(2026, 1, 1));
        try
        {
            student1.Borrow(book5, new DateOnly(2026, 1, 1));
        }
        catch (Exception ex) { Console.WriteLine($"Loan Limit Test: Rejected ({ex.Message})"); }

        Console.WriteLine("\n=== Staff Pay ===");
        var staffList = new List<Staff>
        {
            new Librarian("L1", "John", "111", new DateOnly(2020, 1, 1), 3000m),
            new HeadLibrarian("H1", "Mary", "222", new DateOnly(2015, 1, 1), 5000m),
            new Shelver("SH1", "Tom", "333", new DateOnly(2022, 1, 1), 2000m, "Fiction")
        };
        foreach (var s in staffList)
        {
            Console.WriteLine($"{s.GetType().Name} ({s.FullName}): Pay = {s.MonthlyPay:C}");
        }

        Console.WriteLine("\n=== Items ===");
        var itemsList = new List<LibraryItem>
        {
            new Book("BK1", "The Hobbit", 1.00m),
            new DVD("D1", "Inception", 1.00m),
            new Magazine("M1", "Time", 1.00m)
        };
        foreach (var i in itemsList)
        {
            Console.WriteLine($"{i.GetType().Name}: Loan Period = {i.LoanPeriodDays} days, Daily Late Fee = {i.DailyLateFee:C}");
        }

        Console.WriteLine("\n=== Premium Member Returns Late ===");
        var premium = new PremiumMember("P1", "Bruce", "999", 2.00m); // $2 discount
        var dvd = new DVD("D2", "The Matrix", 1.00m); // Base fee 1.00 * multiplier 2.0 = 2.00/day
        var loan = premium.Borrow(dvd, new DateOnly(2026, 1, 1)); // Due: 2026-01-08
        loan.ReturnItem(new DateOnly(2026, 1, 13)); // 5 days late
        Console.WriteLine($"Due Date: {loan.DueDate}");
        Console.WriteLine($"Late Fee: {loan.LateFee:C} (5 days * 2.00 - 2.00 discount)");
        Console.WriteLine($"Reading Points: {premium.ReadingPoints}");

        Console.WriteLine("\n=== Invalid State Changes ===");
        try
        {
            loan.ReturnItem(new DateOnly(2026, 1, 14));
        }
        catch (Exception ex) { Console.WriteLine($"Return Twice Test: Rejected ({ex.Message})"); }

        try
        {
            loan.MarkAsLost();
        }
        catch (Exception ex) { Console.WriteLine($"Mark Returned as Lost Test: Rejected ({ex.Message})"); }
    }
}
