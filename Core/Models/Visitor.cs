using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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


    public string[] ToCSV()
    {
        throw new NotImplementedException();
    }

    public void FromCSV(string[] values)
    {
        throw new NotImplementedException();
    }
}

