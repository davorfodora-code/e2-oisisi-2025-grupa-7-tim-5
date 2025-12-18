using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

public class Purchase : ISerializable
{
    public Visitor Visitor { get; set; }
    public Book Book { get; set; }
    public string Date_of_purchase { get; set; }
    public double Review { get; set; }
    public string Comment { get; set; }
    public Purchase()
    {
    }

    public Purchase(Visitor visitor, Book book, string date_of_purchase, double review, string comment)
    {
        Visitor = visitor;
        Book = book;
        Date_of_purchase = date_of_purchase;
        Review = review;
        Comment = comment;
    }

    public void FromCSV(string[] values)
    {
        //Visitor
        //Book
        Date_of_purchase = values[0];
        Review = double.Parse(values[1]);
        Comment = values[2];
    }

    public string[] ToCSV()
    {
        string[] values =
        {
            Date_of_purchase,
            Review.ToString(),
            Comment
        };
        return values;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Date of purchase: {Date_of_purchase}, ");
        sb.Append($"Review: {Review}, ");
        sb.Append($"Comment: {Comment}, ");

        return sb.ToString();
    }
}

