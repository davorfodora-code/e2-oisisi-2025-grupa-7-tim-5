using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    internal class BookDAO
    {
        private readonly List<Book> books;
        private readonly Storage<Book> storage;

        public BookDAO()
        {
            storage = new Storage<Book>("DataBase.txt");
            books = storage.Load();
        }
    //#brisanje_knjige

        public Book? RemoveBook(String bIsbn)
        {
            Book? book = GetBookByIsbn(bIsbn);
            if (book == null)
                return null;
            books.Remove(book);
            storage.Save(books);
            return book;
        }
        //#dodavanje_knjige 
        public Book AddBook(Book book)
        {
            if (GetBookByIsbn(book.Isbn) != null)
                return null;

            books.Add(book);
            storage.Save(books);
            return book;
        }
       //#izmena_knjige 
        public Book? UpdateBook(Book b)
        {
            Book? oldBook = GetBookByIsbn(b.Isbn);
            if (oldBook == null) return null;

            oldBook.Publisher = b.Publisher;
            oldBook.Visitor_wish_list = b.Visitor_wish_list;
            oldBook.Visitor_bought_book = b.Visitor_bought_book;
            oldBook.Number_of_pages = b.Number_of_pages;
            oldBook.Authors = b.Authors;
            oldBook.Genre = b.Genre;
            oldBook.Name = b.Name;
            oldBook.Price = b.Price;
            oldBook.Publication_year = b.Publication_year;

            storage.Save(books);


            return oldBook;
        }

    //prikaz_knjige(jedne)
        private Book? GetBookByIsbn(string bIsbn)
        {
            return books.Find(v => v.Isbn == bIsbn);
        }

     //prikaz_knjige(svih)

        public List<Book> GetAllBooks()
        {
            return new List<Book>(books);
        }



    }
}

