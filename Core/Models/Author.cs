using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Author : ISerializable
{
    public string Name { get; set; }
    public string Lastname { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public Address Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string IdCard { get; set; }
    public double ExperienceLength { get; set; }
    public List<Book> BookList { get; set; }

    public Author()
    {
        BookList = new List<Book>();
        Address = new Address();
    }

    public Author(string name, string lastname, DateOnly dateOfBirth, Address address,
                  string phoneNumber, string email, string idCard, double experienceLength,
                  List<Book> bookList)
    {
        Name = name;
        Lastname = lastname;
        DateOfBirth = dateOfBirth;
        Address = address ?? new Address();
        PhoneNumber = phoneNumber;
        Email = email;
        IdCard = idCard;
        ExperienceLength = experienceLength;
        BookList = bookList ?? new List<Book>();
    }

    public void FromCSV(string[] values)
    {
        try
        {
            IdCard = values[0];
            Name = values[1];
            Lastname = values[2];

            if (!string.IsNullOrEmpty(values[3]))
                DateOfBirth = DateOnly.Parse(values[3]);

            PhoneNumber = values[4];
            Email = values[5];

            if (!string.IsNullOrEmpty(values[6]))
                ExperienceLength = double.Parse(values[6]);

            Address = new Address();
            if (values.Length > 7)
                Address.Street = values[7] ?? "";
            if (values.Length > 8)
                Address.Number = values[8] ?? "";
            if (values.Length > 9)
                Address.City = values[9] ?? "";
            if (values.Length > 10)
                Address.State = values[10] ?? "";
            if (values.Length > 11)
                Address.Country = values[11] ?? "";

            BookList = new List<Book>();
            if (values.Length > 12 && !string.IsNullOrEmpty(values[12]))
            {
                var bookEntries = values[12].Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in bookEntries)
                {
                    var parts = entry.Split(',');
                    if (parts.Length >= 2)
                    {
                        BookList.Add(new Book
                        {
                            Isbn = parts[0],
                            Name = parts.Length > 1 ? parts[1] : ""
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška u FromCSV: {ex.Message}");
        }
    }

    public string[] ToCSV()
    {
        string street = Address?.Street ?? "";
        string number = Address?.Number ?? "";
        string city = Address?.City ?? "";
        string state = Address?.State ?? "";
        string country = Address?.Country ?? "";

        string booksStr = BookList != null && BookList.Any()
            ? string.Join(";", BookList.Select(b => $"{b.Isbn},{b.Name}"))
            : "";

        string[] values = new string[]
        {
            IdCard ?? "",
            Name ?? "",
            Lastname ?? "",
            DateOfBirth.ToString("dd.MM.yyyy."),
            PhoneNumber ?? "",
            Email ?? "",
            ExperienceLength.ToString(),
            street,
            number,
            city,
            state,
            country,
            booksStr 
        };
        return values;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"ID: {IdCard}, ");
        sb.Append($"Ime: {Name} {Lastname}, ");
        sb.Append($"Datum: {DateOfBirth}, ");
        sb.Append($"Tel: {PhoneNumber}, ");
        sb.Append($"Email: {Email}, ");
        sb.Append($"Iskustvo: {ExperienceLength}, ");
        sb.Append($"Adresa: {Address?.Street} {Address?.Number}, {Address?.City}, ");
        sb.Append($"Knjige: {BookList?.Count ?? 0}");
        return sb.ToString();
    }
}