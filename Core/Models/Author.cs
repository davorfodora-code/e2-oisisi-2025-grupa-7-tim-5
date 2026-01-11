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
	public string Id_card { get; set; }
	public double ExperienceLength { get; set; }
	public List<Book> BookList { get; set; }


	public Author()
	{
		 string name, lastname, date_of_birth, phone_number, email, id_card;
		 Address address;
		 double experience_length;
		BookList  = new List<Book>();
	}
    public Author(
    string Name,
    string Lastname,
    string Date_of_birth,
    string Phone_number,
    string Email,
    string Id_card,
    Address Address,
    double Experience_length,
    List<Book> bookList)
    {
		this.Name = Name;
		this.Lastname = Lastname;
		this.DateOfBirth = DateOfBirth;
		this.PhoneNumber = PhoneNumber;
		this.Email = Email;
		this.Id_card = Id_card;
		this.Address = Address;
		this.ExperienceLength = ExperienceLength;
        BookList = new List<Book>();
    }
    public void FromCSV(string[] values)
    {

        Name = values[0];
        Lastname = values[1];
        DateOfBirth = DateOnly.Parse(values[2]);
        PhoneNumber = values[3];
        Email = values[4];
        Id_card = values[5];
        ExperienceLength = double.Parse(values[6]);
    }

    public string[] ToCSV()
    {
        string[] values =
        {
           Name, Lastname, PhoneNumber, Email, Id_card,
           ExperienceLength.ToString(),
           DateOfBirth.ToString()
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
        sb.Append("ID CARD: " + Id_card + ", ");
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
