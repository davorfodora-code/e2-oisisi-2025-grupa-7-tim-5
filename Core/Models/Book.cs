using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Book : ISerializable
{
	public string Isbn { get; set; }
	public string Name { get; set; }
	public string Genre { get; set; }
	public string Publication_year { get; set; }
	public double Price { get; set; }
	public int Number_of_pages { get; set; }
	public List<Author> Authors { get; set; }
	public Publisher Publisher { get; set; }
	public List<Visitor> Visitor_bought_book { get; set; }
	public List<Visitor> Visitor_wish_list { get; set; }

	public Book()
	{
		
		 Visitor_bought_book = new List<Visitor>();
		 Visitor_wish_list = new List<Visitor>();
	}
public Book(string isbn, string name, string genre, string publication_year,double price, int number_of_pages,Publisher publisher,List<Visitor> visitor_bought_book,List<Visitor> visitor_wish_list)
{
	this.Isbn = isbn;
	this.Name = name;
	this.Genre = genre;
	this.Publication_year = publication_year;
	this.Price = price;
	this.Number_of_pages = number_of_pages;
	this.Publisher = publisher;	
	this.Visitor_bought_book= visitor_bought_book;
	this.Visitor_wish_list= visitor_wish_list;

}
	public void FromCSV(string[] values)
	{

		Isbn = values[0];
		Name = values[1];
		Genre = values[2];
		Publication_year = values[3];
		Price = double.Parse(values[4]);
    }

    public string[] ToCSV()
    {
        string[] values =
        {
           Isbn, Name, Genre, Publication_year,
		   Price.ToString()
        };
        return values;
    }

    public override string ToString()
{
    StringBuilder sb = new StringBuilder();

    sb.Append("ISBN: " + Isbn + ", ");
    sb.Append("NAZIV: " + Name + ", ");
    sb.Append("ŽANR: " + Genre + ", ");
    sb.Append("GODINA: " + Publication_year + ", ");
    sb.Append("CENA: " + Price + ", ");
    sb.Append("BROJ STRANA: " + Number_of_pages + ", ");

   //fali
    

    return sb.ToString();
}

   
}
