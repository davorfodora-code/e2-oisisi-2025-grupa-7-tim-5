using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.DAO
{
    public class PublisherDAO
    {
        private readonly List<Publisher> publishers;
        private readonly Storage<Publisher> storage;
        private readonly AuthorDAO _authorDAO;
        private int CreatePublisherID()
        {
            if (publishers.Count == 0)
            {
                return 1;
            }

           
            var lastPublisher = publishers[publishers.Count - 1];

            return lastPublisher.Id + 1;
        }

        public PublisherDAO()
        {

            storage = new Storage<Publisher>("Publishers.txt");
            publishers = storage.Load();

            _authorDAO = new AuthorDAO();

            LinkAuthorsWithFullData();
        }

        private void LinkAuthorsWithFullData()
        {
            var allAuthors = _authorDAO.GetAllAuthors();

            foreach (var publisher in publishers)
            {
                if (publisher.Manager != null && !string.IsNullOrEmpty(publisher.Manager.IdCard))
                {
                    var fullManager = allAuthors.FirstOrDefault(a => a.IdCard == publisher.Manager.IdCard);
                    if (fullManager != null)
                        publisher.Manager = fullManager;
                }

                if (publisher.Authors != null && publisher.Authors.Any())
                {
                    var fullAuthors = new List<Author>();
                    foreach (var author in publisher.Authors)
                    {
                        if (!string.IsNullOrEmpty(author.IdCard))
                        {
                            var fullAuthor = allAuthors.FirstOrDefault(a => a.IdCard == author.IdCard);
                            if (fullAuthor != null)
                                fullAuthors.Add(fullAuthor);
                            else
                                fullAuthors.Add(author);
                        }
                    }
                    publisher.Authors = fullAuthors;
                }
            }
        }

        public Publisher AddPublisher(Publisher p)
        {
            var existing = publishers.FirstOrDefault(existingPublisher => existingPublisher.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                return existing;
            }

            p.Id = CreatePublisherID();
            
            publishers.Add(p);
            storage.Save(publishers);
            return p;
        }

        public Publisher? UpdateAddress(Publisher p)
        {
            Publisher? oldPublisher = FindPublisher(p.Id);
            if (oldPublisher == null)
                return null;

            oldPublisher.Name = p.Name;
            oldPublisher.Manager = p.Manager;
            oldPublisher.Authors = p.Authors;
            oldPublisher.Books = p.Books;

            storage.Save(publishers);
            return oldPublisher;
        }

        public Publisher? RemovePublisher(Publisher p)
        {
            Publisher? publisher = FindPublisher(p.Id);
            if (publisher == null)
                return null;

            publishers.Remove(publisher);
            storage.Save(publishers);
            return publisher;
        }


        private Publisher? FindPublisher(int id)
        {
            return publishers.Find(p => p.Id == id);
        }


        public List<Publisher> GetAllPublishers()
        {
            return new List<Publisher>(publishers);
        }
    }
}
