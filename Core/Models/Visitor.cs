using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

public class Visitor : ISerializable
{
    public enum Status_type { R, V }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string DateOfBirth { get; set; }
    public Address Address { get; set; }
    public string ContactPhone { get; set; }
    public string Email { get; set; }
    public string MembershipNumber { get; set; }
    public string MembershipYear { get; set; }
    public string Status { get; set; }
    public double AverageReview { get; set; }
    public List<Purchase> Purchases { get; set; }
    public List<Book> Wishlist { get; set; }

    public Visitor()
    {
        Purchases = new List<Purchase>();
        Wishlist = new List<Book>();
        Address = new Address();
    }

    public Visitor(string name, string lastName, string dateofbirth, Address address, string contactPhone, string email, string membershipNumber, string membershipYear, string status, List<Purchase> purchases, List<Book> wishlist)
    {
        Name = name;
        LastName = lastName;
        DateOfBirth = dateofbirth;
        Address = address ?? new Address();
        ContactPhone = contactPhone;
        Email = email;
        MembershipNumber = membershipNumber;
        MembershipYear = membershipYear;
        Status = status;
        Purchases = purchases ?? new List<Purchase>();
        Wishlist = wishlist ?? new List<Book>();
    }

    public Visitor(string name, string lastName, string dateofbirth, Address address, string contactPhone, string email, string membershipNumber, string membershipYear, string status)
    {
        Name = name;
        LastName = lastName;
        DateOfBirth = dateofbirth;
        Address = address ?? new Address();
        ContactPhone = contactPhone;
        Email = email;
        MembershipNumber = membershipNumber;
        MembershipYear = membershipYear;
        Status = status;
        Purchases = new List<Purchase>();
        Wishlist = new List<Book>();
    }

    public string[] ToCSV()
    {
        string wishlistString = string.Join(";", Wishlist.Select(book => $"{book.Isbn},{book.Name}"));
        string purchasesString = string.Join(";", Purchases.Select(purchase => $"{purchase.Book.Isbn},{purchase.Book.Name},{purchase.DateOfPurchase:dd.MM.yyyy.}"));

        // VAŽNO: Serijalizuj svaki deo adrese kao posebno polje
        string[] Values =
        {
            Name,
            LastName,
            DateOfBirth,
            Address?.Street ?? "",      // Street
            Address?.Number ?? "",      // Number
            Address?.City ?? "",        // City
            Address?.State ?? "",       // State
            Address?.Country ?? "",     // Country
            ContactPhone,
            Email,
            MembershipNumber,
            MembershipYear,
            Status,
            AverageReview.ToString(),
            wishlistString,
            purchasesString
        };
        return Values;
    }

    public void FromCSV(string[] values)
    {
        if (values.Length < 15)
        {
            Console.WriteLine($"ERROR: Expected at least 15 values, got {values.Length}");
            return;
        }

        Name = values[0];
        LastName = values[1];
        DateOfBirth = values[2];

        // Čitaj adresu iz pojedinačnih polja
        Address = new Address
        {
            Street = values[3] ?? "",
            Number = values[4] ?? "",
            City = values[5] ?? "",
            State = values[6] ?? "",
            Country = values[7] ?? ""
        };

        ContactPhone = values[8];
        Email = values[9];
        MembershipNumber = values[10];
        MembershipYear = values[11];
        Status = values[12];

        // Safe parsing za AverageReview
        if (!string.IsNullOrWhiteSpace(values[13]) && double.TryParse(values[13], out double avgReview))
        {
            AverageReview = avgReview;
        }
        else
        {
            AverageReview = 0.0;
        }

        // Wishlist
        if (values.Length > 14 && !string.IsNullOrWhiteSpace(values[14]))
        {
            var wishlistEntries = values[14].Split(';');
            Wishlist = new List<Book>();
            foreach (var entry in wishlistEntries)
            {
                var parts = entry.Split(',');
                if (parts.Length == 2)
                {
                    Wishlist.Add(new Book
                    {
                        Isbn = parts[0],
                        Name = parts[1]
                    });
                }
            }
        }
        else
        {
            Wishlist = new List<Book>();
        }

        // Purchases
        if (values.Length > 15 && !string.IsNullOrWhiteSpace(values[15]))
        {
            var purchaseEntries = values[15].Split(';');
            Purchases = new List<Purchase>();
            foreach (var entry in purchaseEntries)
            {
                var parts = entry.Split(',');
                if (parts.Length == 3)
                {
                    Purchases.Add(new Purchase
                    {
                        Book = new Book
                        {
                            Isbn = parts[0],
                            Name = parts[1]
                        },
                        DateOfPurchase = DateOnly.Parse(parts[2])
                    });
                }
            }
        }
        else
        {
            Purchases = new List<Purchase>();
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Name: {Name}, ");
        sb.Append($"Last name: {LastName}, ");
        sb.Append($"Date of birth: {DateOfBirth}, ");

        if (Address != null)
        {
            sb.Append($"Address: {Address.Street} {Address.Number}, {Address.City}, ");
        }
        else
        {
            sb.Append($"Address: N/A, ");
        }

        sb.Append($"Contact phone: {ContactPhone}, ");
        sb.Append($"Email: {Email}, ");
        sb.Append($"Membership number: {MembershipNumber}, ");
        sb.Append($"Membership year: {MembershipYear}, ");
        sb.Append($"Status: {Status}, ");
        sb.Append($"Average review: {AverageReview}");
        return sb.ToString();
    }
}