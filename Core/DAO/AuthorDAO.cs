using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.DAO
{
    public class AuthorDAO
    {
        private readonly List<Author> authors;
        private readonly Storage<Author> storage;
        private readonly PublisherDAO _publisherDAO;

        public AuthorDAO(PublisherDAO publisherDAO)
        {
            storage = new Storage<Author>("Authors.txt");
            authors = storage.Load();
            _publisherDAO = publisherDAO;
        }

        public Author? AddAuthor(Author author)
        {
            if (GetAuthorByIdCard(author.IdCard) != null)
                return null;

            authors.Add(author);
            storage.Save(authors);
            return author;
        }

        public Author? UpdateAuthor(Author a)
        {
            Author? oldAuthor = GetAuthorByIdCard(a.IdCard);
            if (oldAuthor == null)
                return null;

            oldAuthor.Name = a.Name;
            oldAuthor.Lastname = a.Lastname;
            oldAuthor.DateOfBirth = a.DateOfBirth;
            oldAuthor.PhoneNumber = a.PhoneNumber;
            oldAuthor.Email = a.Email;
            oldAuthor.Address = a.Address;
            oldAuthor.ExperienceLength = a.ExperienceLength;
            oldAuthor.BookList = a.BookList;

            storage.Save(authors);
            return oldAuthor;
        }

        public Author? RemoveAuthor(string idCard)
        {
            Author? author = GetAuthorByIdCard(idCard);
            if (author == null)
                return null;

            var publishers = _publisherDAO.GetAllPublishers();

            foreach (var publisher in publishers)
            {
                if (publisher.Authors != null)
                {
                    publisher.Authors.RemoveAll(a => a.IdCard == idCard);
                }

                if (publisher.Manager != null && publisher.Manager.IdCard == idCard)
                {
                    publisher.Manager = null;
                }

                _publisherDAO.UpdatePublisher(publisher);
            }

            authors.Remove(author);
            storage.Save(authors);

            _publisherDAO.LinkAuthorsWithFullData(this);

            return author;
        }

        public Author? GetAuthorByIdCard(string idCard)
        {
            return authors.Find(a => a.IdCard == idCard);
        }

        public List<Author> GetAllAuthors()
        {
            return new List<Author>(authors);
        }
    }
}