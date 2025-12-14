using System;
using System.Runtime.Serialization;

public class Visitor : ISerializable
{
    public enum Status_type {P, B}


	public Visitor()
	{

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
    public list<Purchase> Purchases { get; set; }
    public list<Book> Wishlist { get; set; }


}
}
