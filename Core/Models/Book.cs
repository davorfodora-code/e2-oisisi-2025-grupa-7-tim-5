using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;

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

    public Book(string isbn, string name, Genre genre, string publicationYear,
                double price, int numberOfPages, List<Author> authors,
                Publisher publisher, List<Visitor> visitorBoughtBook, List<Visitor> visitorWishList)
    {
        Isbn = isbn;
        Name = name;
        Genre = genre;
        PublicationYear = publicationYear;
        Price = price;
        NumberOfPages = numberOfPages;
        Authors = authors ?? new List<Author>();
        Publisher = publisher ?? new Publisher();
        VisitorBoughtBook = visitorBoughtBook ?? new List<Visitor>();
        VisitorWishList = visitorWishList ?? new List<Visitor>();
    }

    public Book(string isbn, string name, Genre genre, string publicationYear,
                double price, int numberOfPages, List<Author> authors, Publisher publisher)
        : this(isbn, name, genre, publicationYear, price, numberOfPages, authors, publisher,
               new List<Visitor>(), new List<Visitor>())
    {
    }

    public void FromCSV(string[] values)
    {
        if (values.Length < 7) return;

        Isbn = values[0];
        Name = values[1];

        if (Enum.TryParse<Genre>(values[2], true, out Genre parsedGenre))
            Genre = parsedGenre;
        else
            Genre = Genre.Roman;

        PublicationYear = values[3];
        Price = double.Parse(values[4]);
        NumberOfPages = int.Parse(values[5]);

        Authors = new List<Author>();
        if (values.Length > 6 && !string.IsNullOrEmpty(values[6]))
        {
            var authorEntries = values[6].Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var entry in authorEntries)
            {
                var parts = entry.Split(',');
                if (parts.Length >= 3)
                {
                    Authors.Add(new Author
                    {
                        IdCard = parts[0],
                        Name = parts[1],
                        Lastname = parts[2]
                    });
                }
            }
        }

        Publisher = new Publisher();
        if (values.Length > 8)
        {
            if (int.TryParse(values[7], out int id))
                Publisher.Id = id;
            Publisher.Name = values[8];
        }
    }

    public string[] ToCSV()
    {
        string authorsString = Authors != null && Authors.Any()
            ? string.Join(";", Authors.Select(a => $"{a.IdCard},{a.Name},{a.Lastname}"))
            : "";

        string[] values =
        {
            Isbn ?? "",
            Name ?? "",
            Genre.ToString(),
            PublicationYear ?? "",
            Price.ToString("0.##"),
            NumberOfPages.ToString(),
            authorsString,
            Publisher?.Id.ToString() ?? "0",
            Publisher?.Name ?? ""
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
        sb.Append("AUTORI: " + (Authors != null && Authors.Any()
            ? string.Join(", ", Authors.Select(a => a.Name + " " + a.Lastname))
            : "Nema autora"));
        return sb.ToString();
    }
}