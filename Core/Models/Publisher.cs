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


    public void FromCSV(string[] values)
    {
        Id = int.Parse(values[0]);
        Name = values[1];
    }

    public string[] ToCSV()
    {
        string[] values =
        {
            Id.ToString(),
            Name
            //Manager
            //Authors
            //Books
        };
        return values;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Id: {Id}, ");
        sb.Append($"Name: {Name}, ");

        return sb.ToString();
    }

}
