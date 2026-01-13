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

    public string[] ToCSV()
    {
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
            AverageReview.ToString()
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

