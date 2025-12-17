using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Purchase : ISerializable
{
    public Visitor Visitor { get; set; }
    public Book Book { get; set; }
    public string Date_of_purchase { get; set; }
    public int Review { get; set; }
    public string Comment { get; set; }
    public Purchase()
    {
    }
    public Purchase(Visitor visitor, Book book, string comment, string date_of_purchase, int review)
    {
        this.Visitor = visitor;
        this.Book = book;
        this.Comment = comment;
        this.Review = review;
        this.Date_of_purchase = date_of_purchase;
    }

       

    public void FromCSV(string[] values)
    {
        throw new NotImplementedException();
    }

    public string[] ToCSV()
    {
        throw new NotImplementedException();
    }
}

