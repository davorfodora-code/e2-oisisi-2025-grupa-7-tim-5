using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Book : ISerializable
{
	public string Isbn { get; set; }
	public string Name { get; set; }
	public string Genre { get; set; }
	public int PublicationYear { get; set; }
	public double Price { get; set; }
	public int NumberOfPages { get; set; }
	public List<Author> Authors { get; set; }
	public Publisher Publisher { get; set; }
	public List<Visitor> VisitorBoughtBook { get; set; }
	public List<Visitor> VisitorWishList { get; set; }

	public Book()
	{

        VisitorBoughtBook = new List<Visitor>();
        VisitorWishList = new List<Visitor>();
	}

    public Book(string isbn, string name, string genre, int publicationYear, double price, int numberOfPages, List<Author> authors, Publisher publisher, List<Visitor> visitorBoughtBook, List<Visitor> visitorWishList)
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
		Genre = values[2];
		PublicationYear = int.Parse(values[3]);
		Price = double.Parse(values[4]);
		Publisher = new Publisher();
		Publisher.Id = values[5];
		Publisher.Name = values[6];
    }

    public string[] ToCSV()
    {
        string[] values =
        {
           Isbn, Name, Genre, 
            PublicationYear.ToString(),
		   Price.ToString(), Publisher.Id.ToString(), Publisher.Name.ToString()
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
