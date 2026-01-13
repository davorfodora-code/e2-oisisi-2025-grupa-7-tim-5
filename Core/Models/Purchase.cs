using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

public class Purchase : ISerializable
{
    public Visitor Visitor { get; set; }
    public Book Book { get; set; }
    public DateOnly DateOfPurchase { get; set; }
    public double Review { get; set; }
    public string Comment { get; set; }
    public Purchase()
    {
    }

    public Purchase(Visitor visitor, Book book, DateOnly dateofpurchase, double review, string comment)
    {
        Visitor = visitor;
        Book = book;
        DateOfPurchase = dateofpurchase;
        Review = review;
        Comment = comment;
    }

    public void FromCSV(string[] values)
    {
        Visitor = new Visitor();
        Visitor.MembershipNumber = values[0];
        Book = new Book();
        Book.Isbn = values[1];
        DateOfPurchase = DateOnly.Parse(values[2]);
        Review = double.Parse(values[3]);
        Comment = values[4];
    }

    public string[] ToCSV()
    {
        string[] values =
        {
            Visitor.MembershipNumber.ToString(),
            Book.Isbn.ToString(),
            DateOfPurchase.ToString("dd.MM.yyyy."),
            Review.ToString(),
            Comment
        };
        return values;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Date of purchase: {DateOfPurchase}, ");
        sb.Append($"Review: {Review}, ");
        sb.Append($"Comment: {Comment}, ");

        return sb.ToString();
    }
}

