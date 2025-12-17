using System;
using System.Text;

public class Author

{
	public string name { get; set; }
	public string lastname { get; set; }
	public string date_of_birth { get; set; }
	public Address address { get; set; }
	public string phone_number { get; set; }
	public string email { get; set; }
	public string id_card { get; set; }
	public double experience_length { get; set; }
	public List<Book> BookList { get; set; }


	public Author()
	{
		 string name, lastname, date_of_birth, phone_number, email, id_card;
		 Address address;
		 double experience_length;
		BookList  = new List<Book>();
	}
    public Author(
    string name,
    string lastname,
    string date_of_birth,
    string phone_number,
    string email,
    string id_card,
    Address address,
    double experience_length,
    List<Book> bookList)
    {
		name = name;
		lastname = lastname;
		date_of_birth = date_of_birth;
		phone_number = phone_number;
		email = email;
		id_card = id_card;
		address = address;
		experience_length = experience_length;
        BookList = new List<Book>();
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("NAME: " + name + ", ");
        sb.Append("LASTNAME: " + lastname + ", ");
        sb.Append("DATE OF BIRTH: " + date_of_birth + ", ");
        sb.Append("PHONE: " + phone_number + ", ");
        sb.Append("EMAIL: " + email + ", ");
        sb.Append("ID CARD: " + id_card + ", ");
        sb.Append("EXPERIENCE: " + experience_length + ", ");
        sb.Append("ADDRESS: " + address + ", ");
        sb.Append("BOOKS: ");
        //fali za listu


        return sb.ToString();
    }


}
