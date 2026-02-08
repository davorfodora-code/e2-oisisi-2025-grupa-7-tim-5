using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public class Publisher : ISerializable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Author Manager { get; set; }
    public List<Author> Authors { get; set; }
    public List<Book> Books { get; set; }
    public Publisher()
    {
        Books = new List<Book>();
    }

    public Publisher(string name, Author manager, List<Author> authors, List<Book> books)
    {
        Name = name;
        Manager = manager;
        Authors = authors;
        Books = books;
    }

    public Publisher(int id, string name, Author manager, List<Author> authors, List<Book> books)
    {
        Id = id;
        Name = name;
        Manager = manager;
        Authors = authors;
        Books = books;
    }

    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];

        if (values.Length > 2 && !string.IsNullOrEmpty(values[2]))
        {
            var AuthorsToAdd = values[2].Split(';');
            Authors = new List<Author>();
            foreach (var author in AuthorsToAdd)
            {
                var parts = author.Split(',');
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

        if (values.Length > 3 && !string.IsNullOrEmpty(values[3]))
        {
            var bookEntries = values[3].Split(';');
            Books = new List<Book>();
            foreach (var entry in bookEntries)
            {
                var parts = entry.Split(',');
                if (parts.Length == 2)
                {
                    Books.Add(new Book
                    {
                        Isbn = parts[0],
                        Name = parts[1]
                    });
                }
            }
        }
    }

    public string[] ToCSV()
    {
        string authorsString = string.Join(";",Authors.Select(a => $"{a.IdCard},{a.Name}"));

        string booksString = string.Join(";",Books.Select(b => $"{b.Isbn},{b.Name}"));

        return new string[]
        {
            Id.ToString(),
            Name,
            authorsString,
            booksString
        };
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Id: {Id}, ");
        sb.Append($"Name: {Name}, ");
        sb.Append($"Authors: {Authors.Count}, ");
        sb.Append($"Books: {Books.Count}");

        return sb.ToString();
    }

}
