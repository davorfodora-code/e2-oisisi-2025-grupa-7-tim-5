using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

public class Visitor : ISerializable
{
    public enum Status_type {P, B}
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
    }

    public Visitor(string name, string lastName, string dateofbirth, Address address, string contactPhone, string email, string membershipNumber, string membershipYear, string status, List<Purchase> purchases, List<Book> wishlist)
    {
        Name = name;
        LastName = lastName;
        DateOfBirth = dateofbirth;
        Address = address;
        ContactPhone = contactPhone;
        Email = email;
        MembershipNumber = membershipNumber;
        MembershipYear = membershipYear;
        Status = status;
        Purchases = purchases;
        Wishlist = wishlist;
    }

    public Visitor(string name, string lastName, string dateofbirth, Address address, string contactPhone, string email, string membershipNumber, string membershipYear, string status)
    {
        Name = name;
        LastName = lastName;
        DateOfBirth = dateofbirth;
        Address = address;
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
        string wishlistString = string.Join(";",Wishlist.Select(book => $"{book.Isbn},{book.Name}"));
        string purchasesString = string.Join(";",Purchases.Select(purchase =>$"{purchase.Book.Isbn},{purchase.Book.Name},{purchase.DateOfPurchase:dd.MM.yyyy.}"));
        string[] Values =
        {
            Name,
            LastName,
            DateOfBirth,
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
        Name = values[0];
        LastName = values[1];
        DateOfBirth = values[2];
        ContactPhone = values[3];
        Email = values[4];
        MembershipNumber = values[5];
        MembershipYear = values[6];
        Status = values[7];
        AverageReview = double.Parse(values[8]);

        if (values.Length > 9 && !string.IsNullOrEmpty(values[9]))
        {
            var wishlistEntries = values[9].Split(';');
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

        if (values.Length > 10 && !string.IsNullOrEmpty(values[10]))
        {
            var purchaseEntries = values[10].Split(';');
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


    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Name: {Name}, ");
        sb.Append($"Last name: {LastName}, ");
        sb.Append($"Date of birth: {DateOfBirth}, ");
        sb.Append($"Contact phone: {ContactPhone}, ");
        sb.Append($"Email: {Email}, ");
        sb.Append($"Membership number: {MembershipNumber}, ");
        sb.Append($"Membership year: {MembershipYear}, ");
        sb.Append($"Status: {Status}, ");
        sb.Append($"Average review: {AverageReview}, ");
        return sb.ToString();
    }

    

}

