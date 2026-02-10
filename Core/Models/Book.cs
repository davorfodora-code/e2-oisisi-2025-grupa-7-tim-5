using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
public enum Genre
{
    Roman,
    Poezija,
    Esej,
    Pripovetka,
    Drama,
    Biografija,
    NaučnaFantastika,
    Triler,
    Horor,
    Istorija,
    Autobiografija,
    Novela,
    Misterija
}
public class Book : ISerializable
{
	public string Isbn { get; set; }
	public string Name { get; set; }
	public Genre Genre { get; set; }
	public string PublicationYear { get; set; }
	public double Price { get; set; }
	public int NumberOfPages { get; set; }
	public List<Author> Authors { get; set; }
	public Publisher Publisher { get; set; }
	public List<Visitor> VisitorBoughtBook { get; set; }
	public List<Visitor> VisitorWishList { get; set; }

    public Book()
    {
        Authors = new List<Author>();
        Publisher = new Publisher();
        VisitorBoughtBook = new List<Visitor>();
        VisitorWishList = new List<Visitor>();
        Isbn = "";
        Name = "";
        PublicationYear = "";
    }

    public Book(string isbn, string name, Genre genre, string publicationYear, double price, int numberOfPages, List<Author> authors, Publisher publisher, List<Visitor> visitorBoughtBook, List<Visitor> visitorWishList)
    {
        Isbn = isbn;
        Name = name;
        Genre = genre;
        PublicationYear = publicationYear;
        Price = price;
        NumberOfPages = numberOfPages;
        Authors = authors;
        Publisher = publisher;
        VisitorBoughtBook = visitorBoughtBook;
        VisitorWishList = visitorWishList;
    }

    public void FromCSV(string[] values)
    {
        Isbn = values[0];
        Name = values[1];

        string genreValue = values[2];
        if (genreValue == "zanr")
        {
            Genre = Genre.Roman; 
        }
        else if (Enum.TryParse<Genre>(genreValue, true, out Genre parsedGenre))
        {
            Genre = parsedGenre;
        }
        else
        {
            Genre = Genre.Roman;
            Console.WriteLine($"Upozorenje: Nevažeći žanr '{genreValue}'. Postavljeno na {Genre.Roman}");
        }

        PublicationYear = values[3];
        Price = double.Parse(values[4]);
        Publisher = new Publisher();
        Publisher.Id = int.Parse(values[5]);
        Publisher.Name = values[6];
    }

    public string[] ToCSV()
    {
        string[] values =
        {
           Isbn, Name, Genre.ToString(), 
           PublicationYear,Price.ToString(), Publisher.Id.ToString(), Publisher.Name.ToString()
        };
        return values;
    }

    public override string ToString()
{
    StringBuilder sb = new StringBuilder();

    sb.Append("ISBN: " + Isbn + ", ");
    sb.Append("NAZIV: " + Name + ", ");
    sb.Append("ŽANR: " + Genre + ", ");
    sb.Append("GODINA: " + PublicationYear + ", ");
    sb.Append("CENA: " + Price + ", ");
    sb.Append("BROJ STRANA: " + NumberOfPages + ", ");


    

    return sb.ToString();
}

   
}
