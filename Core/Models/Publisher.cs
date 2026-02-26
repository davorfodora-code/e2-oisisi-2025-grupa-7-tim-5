using System;
using System.Collections.Generic;
using System.Linq;
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
        Name = "";
        Manager = new Author();
        Authors = new List<Author>();
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
            var managerParts = values[2].Split(',');
            if (managerParts.Length == 5)
            {
                Manager = new Author
                {
                    IdCard = managerParts[0],
                    Name = managerParts[1],
                    Lastname = managerParts[2],
                    ExperienceLength = double.Parse(managerParts[3]),
                    Email = managerParts[4]
                };
            }
        }

        if (values.Length > 3 && !string.IsNullOrEmpty(values[3]))
        {
            var authorEntries = values[3].Split(';', StringSplitOptions.RemoveEmptyEntries);
            Authors = new List<Author>();
            foreach (var entry in authorEntries)
            {
                var parts = entry.Split(',');
                if (parts.Length == 5)
                {
                    Authors.Add(new Author
                    {
                        IdCard = parts[0],
                        Name = parts[1],
                        Lastname = parts[2],
                        ExperienceLength = double.Parse(parts[3]),
                        Email = parts[4]
                    });
                }
            }
        }

        if (values.Length > 4 && !string.IsNullOrEmpty(values[4]))
        {
            var bookEntries = values[4].Split(';', StringSplitOptions.RemoveEmptyEntries);
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
        string managerString = Manager != null
            ? $"{Manager.IdCard},{Manager.Name},{Manager.Lastname},{Manager.ExperienceLength},{Manager.Email}"
            : "";

        string authorsString = Authors != null && Authors.Any()
            ? string.Join(";", Authors.Select(a => $"{a.IdCard},{a.Name},{a.Lastname},{a.ExperienceLength},{a.Email}"))
            : "";

        string booksString = Books != null && Books.Any()
            ? string.Join(";", Books.Select(b => $"{b.Isbn},{b.Name}"))
            : "";

        return new string[]
        {
        Id.ToString(),
        Name,
        managerString,
        authorsString,
        booksString
        };
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Id: {Id}, ");
        sb.Append($"Name: {Name}, ");
        sb.Append($"Manager: {Manager?.Name ?? "Nema"}, ");
        sb.Append($"Authors: {Authors?.Count ?? 0}, ");
        sb.Append($"Books: {Books?.Count ?? 0}");

        return sb.ToString();
    }
}