using System;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

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

    public Author(string name, string lastname, DateOnly dateOfBirth, Address address, string phoneNumber, string email, string idCard, double experienceLength, List<Book> bookList)
    {
        Name = name;
        Lastname = lastname;
        DateOfBirth = dateOfBirth;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        IdCard = idCard;
        ExperienceLength = experienceLength;
        BookList = bookList;
    }

    public void FromCSV(string[] values)
    {
        IdCard = values[0];
        Name = values[1];
        Lastname = values[2];
        DateOfBirth = DateOnly.Parse(values[3]);
        PhoneNumber = values[4];
        Email = values[5];
        ExperienceLength = double.Parse(values[6]);
    }

    public string[] ToCSV()
    {
        string[] values =
        {
          IdCard, Name, Lastname,
           DateOfBirth.ToString("dd.MM.yyyy."),
           PhoneNumber, Email,
           ExperienceLength.ToString(),
           
        };
        return values;
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("NAME: " + Name + ", ");
        sb.Append("LASTNAME: " + Lastname + ", ");
        sb.Append("DATE OF BIRTH: " + DateOfBirth + ", ");
        sb.Append("PHONE: " + PhoneNumber + ", ");
        sb.Append("EMAIL: " + Email + ", ");
        sb.Append("ID CARD: " + IdCard + ", ");
        sb.Append("EXPERIENCE: " + ExperienceLength + ", ");
        sb.Append("ADDRESS: " + Address + ", ");
        sb.Append("BOOKS: ");
        foreach(Book book in BookList)
        {
            sb.Append(book.ToString());
        }


        return sb.ToString();
    }


}
