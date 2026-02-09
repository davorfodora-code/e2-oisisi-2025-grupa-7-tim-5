using Core.DAO;
using Core.DTO;
using Core.Observer;
using System.Collections.Generic;

namespace Core.Controllers
{
    public class BookController : Subject
    {
        private readonly BookDAO _bookDao;
        private readonly PublisherDAO _publisherDao;

        public BookController(BookDAO bookDao, PublisherDAO publisherDao)
        {
            _bookDao = bookDao;
            _publisherDao = publisherDao;
        }
        public List<Book> GetAllBooks()
        {
            return _bookDao.GetAllBooks();
        }

        public void RegisterObserver(IObserver observer) { Subscribe(observer); }
        public void UnregisterObserver(IObserver observer) { Unsubscribe(observer); }


        public List<Book> RetrieveAllBooks() => _bookDao.GetAllBooks();

      
        public (bool Success, string Message) CreateBook(BookDTO bookDto)
        {
            var (isValid, error) = bookDto.Validate();
            if (!isValid)
                return (false, error);

            if (_bookDao.GetBookByIsbn(bookDto.Isbn) != null)
                return (false, $"Knjiga sa ISBN '{bookDto.Isbn}' već postoji.");

            var book = bookDto.ToModel();
            var addedBook = _bookDao.AddBook(book);

            if (addedBook == null)
                return (false, "Dodavanje knjige nije uspelo.");

            NotifyObservers();
            return (true, "Knjiga uspešno dodata!");
        }


        public (bool Success, string Message) EditBook(BookDTO bookDto)
        {
            var (isValid, error) = bookDto.Validate();
            if (!isValid)
                return (false, error);

            var book = bookDto.ToModel();
            var updatedBook = _bookDao.UpdateBook(book);

            if (updatedBook == null)
                return (false, "Knjiga nije pronađena za ažuriranje.");

            NotifyObservers();
            return (true, "Knjiga uspešno ažurirana!");
        }

        public (bool Success, string Message) RemoveBook(string isbn)
        {
            var removedBook = _bookDao.RemoveBook(isbn);
            if (removedBook == null)
                return (false, "Knjiga sa tim ISBN-om nije pronađena.");

            NotifyObservers();
            return (true, "Knjiga uspešno obrisana.");
        }


        public bool Exists(string isbn) => _bookDao.GetBookByIsbn(isbn) != null;
    }
}

