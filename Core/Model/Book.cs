using System;

public class Class1
{
	public Class1()
	{
		private string isbn, name, genre, publication_year;
		private double price;
		private int number_of_pages;
		List<Author> authors = new List<Author>();
		Publisher publisher;
		List<Visitor> visitor_bought_book = new List<Visitor>();
		List<Visitor> visitor_wish_list = new List<Visitor>();
	}
}
