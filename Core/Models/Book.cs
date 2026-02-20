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
        if (Enum.TryParse<Genre>(genreValue, true, out Genre parsedGenre))
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

        if (values.Length > 5)
        {
            NumberOfPages = int.Parse(values[5]);
        }

        Authors = new List<Author>();
        if (values.Length > 6 && !string.IsNullOrEmpty(values[6]))
        {
            var authorEntries = values[6].Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var entry in authorEntries)
            {
                var parts = entry.Split(',');
                if (parts.Length == 2)
                {
                    Authors.Add(new Author
                    {
                        IdCard = parts[0],
                        Name = parts[1]
                    });
                }
            }
        }

        Publisher = new Publisher();
        if (values.Length > 8)
        {
            Publisher.Id = int.Parse(values[7]);
            Publisher.Name = values[8];
        }
    }

    public string[] ToCSV()
    {
        string authorsString = Authors != null && Authors.Any()
            ? string.Join(";", Authors.Select(a => $"{a.IdCard},{a.Name}"))
            : "";

        string[] values =
        {
            Isbn ?? "",
            Name ?? "",
            Genre.ToString(),
            PublicationYear ?? "",
            Price.ToString(),
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

        if (Authors != null && Authors.Any())
        {
            string imenaAutora = string.Join(", ", Authors.Select(a => a.Name + " " + a.Lastname));
            sb.Append("AUTORI: " + imenaAutora);
        }
        else
        {
            sb.Append("AUTORI: Nema autora");
        }

        return sb.ToString();
    }
}