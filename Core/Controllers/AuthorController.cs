using Core.DAO;
using Core.DTO;
using Core.Observer;
using System;
using System.Collections.Generic;

namespace Core.Controllers
{
    public class AuthorController : Subject
    {
        private readonly AuthorDAO _authorDAO;

        public AuthorController(AuthorDAO authorDAO)
        {
            _authorDAO = authorDAO;
        }

        public void SubscribeObserver(IObserver observer)
        {
            Subscribe(observer);
        }

        public void UnsubscribeObserver(IObserver observer)
        {
            Unsubscribe(observer);
        }

     
        public List<Author> GetAllAuthors()
        {
            return _authorDAO.GetAllAuthors();
        }

        // Dodavanje autora
        public (bool Success, string Message) AddAuthor(AuthorDTO dto)
        {
            var (isValid, errorMessage) = dto.Validate();
            if (!isValid)
            {
                return (false, errorMessage);
            }

            if (_authorDAO.GetAuthorByIdCard(dto.IdCard) != null)
            {
                return (false, $"Author with ID Card '{dto.IdCard}' already exists!");
            }

            var author = dto.ToModel();
            var result = _authorDAO.AddAuthor(author);

            if (result == null)
                return (false, "Failed to add author!");

            NotifyObservers();
            return (true, "Author added successfully!");
        }

        public (bool Success, string Message) UpdateAuthor(AuthorDTO dto)
        {
            var (isValid, errorMessage) = dto.Validate();
            if (!isValid)
            {
                return (false, errorMessage);
            }

            var author = dto.ToModel();
            var result = _authorDAO.UpdateAuthor(author);

            if (result == null)
                return (false, "Author not found!");

            NotifyObservers();
            return (true, "Author updated successfully!");
        }

        public (bool Success, string Message) DeleteAuthor(string idCard)
        {
            var result = _authorDAO.RemoveAuthor(idCard);

            if (result == null)
                return (false, "Author not found!");

            NotifyObservers();
            return (true, "Author deleted successfully!");
        }

        // Provera da li autor postoji 
        public bool ExistsIdCard(string idCard)
        {
            return _authorDAO.GetAuthorByIdCard(idCard) != null;
        }
    }
}

