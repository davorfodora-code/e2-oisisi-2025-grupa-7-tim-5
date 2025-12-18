using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

public class Visitor : ISerializable
{
    public enum Status_type {P, B}
    public string Name { get; set; }
    public string Last_name { get; set; }
    public string Date_of_birth { get; set; }
    public Address Address { get; set; }
    public string Contact_phone { get; set; }
    public string Email { get; set; }
    public string Membership_number { get; set; }
    public string Membership_year { get; set; }
    public string Status { get; set; }
    public double Average_review { get; set; }
    public List<Purchase> Purchases { get; set; }
    public List<Book> Wishlist { get; set; }

    public Visitor()
    {


        Purchases = new List<Purchase>();
        Wishlist = new List<Book>();
    }

    public Visitor(string name, string last_name, string date_of_birth, Address address, string contact_phone, string email, string membership_number, string membership_year, string status, double average_review, List<Purchase> purchases, List<Book> wishlist)
    {
        Name = name;
        Last_name = last_name;
        Date_of_birth = date_of_birth;
        Address = address;
        Contact_phone = contact_phone;
        Email = email;
        Membership_number = membership_number;
        Membership_year = membership_year;
        Status = status;
        Average_review = average_review;
        Purchases = purchases;
        Wishlist = wishlist;
    }

    public string[] ToCSV()
    {
        string[] Values =
        {
            Name,
            Last_name,
            Date_of_birth,
            Contact_phone,
            Email,
            Membership_number,
            Membership_year,
            Status,
            Average_review.ToString()
        };
        return Values;
    }

    public void FromCSV(string[] values)
    {
        Name = values[0];
        Last_name = values[1];
        Date_of_birth = values[2];
        //Address = new Address {  }
        Contact_phone = values[3];
        Email = values[4];
        Membership_number = values[5];
        Membership_year = values[6];
        Status = values[7];
        Average_review = double.Parse(values[8]);
        //Purchases = new Purchases {  }
        //Wishlist = new Wishlist {  }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Name: {Name}, ");
        sb.Append($"Last name: {Last_name}, ");
        sb.Append($"Date of birth: {Date_of_birth}, ");
        sb.Append($"Contact phone: {Contact_phone}, ");
        sb.Append($"Email: {Email}, ");
        sb.Append($"Membership number: {Membership_number}, ");
        sb.Append($"Membership year: {Membership_year}, ");
        sb.Append($"Status: {Status}, ");
        sb.Append($"Average review: {Average_review}, ");
        return sb.ToString();
    }

    

}

