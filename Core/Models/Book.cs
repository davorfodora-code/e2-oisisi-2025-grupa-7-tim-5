using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

public class Book : ISerializable
{
	public string isbn { get; set; }
	public string name { get; set; }
	public string genre { get; set; }
	public string publication_year { get; set; }
	public double price { get; set; }
	public int number_of_pages { get; set; }
	public List<Author> authors { get; set; }
	public Publisher publisher { get; set; }
	public List<Visitor> visitor_bought_book { get; set; }
	public List<Visitor> visitor_wish_list { get; set; }

	public Book()
	{
		
		 visitor_bought_book = new List<Visitor>();
		 visitor_wish_list = new List<Visitor>();
	}
public Book(string isbn, string name, string genre, string publication_year,double price, int number_of_pages,Publisher publisher,List<Visitor> visitor_bought_book,List<Visitor> visitor_wish_list)
{
	isbn = isbn;
	name = name;
	genre = genre;
	publication_year = publication_year;
	price = price;
	number_of_pages = number_of_pages;
	publisher = publisher;	
	visitor_bought_book= visitor_bought_book;
	visitor_wish_list= visitor_wish_list;

}

public override string ToString()
{
    StringBuilder sb = new StringBuilder();

    sb.Append("ISBN: " + isbn + ", ");
    sb.Append("NAZIV: " + name + ", ");
    sb.Append("ŽANR: " + genre + ", ");
    sb.Append("GODINA: " + publication_year + ", ");
    sb.Append("CENA: " + price + ", ");
    sb.Append("BROJ STRANA: " + number_of_pages + ", ");

   //fali
    

    return sb.ToString();
}

    public string[] ToCSV()
    {
        throw new NotImplementedException();
    }

    public void FromCSV(string[] values)
    {
        throw new NotImplementedException();
    }
}
