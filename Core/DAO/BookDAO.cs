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

        public Book AddBook(Book book)
        {
            books.Add(book);
            storage.Save(books);
            return book;
        }

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

        private Book? GetBookByIsbn(string bIsbn)
        {
            return books.Find(v => v.Isbn == bIsbn);
        }

    }
}

