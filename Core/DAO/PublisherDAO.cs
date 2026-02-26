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
        }

        public void LinkAuthorsWithFullData(AuthorDAO authorDAO)
        {
            var allAuthors = authorDAO.GetAllAuthors();

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

        public Publisher? UpdatePublisher(Publisher p)
        {
            Publisher? oldPublisher = FindPublisher(p.Id);
            if (oldPublisher == null)
                return null;

            oldPublisher.Name = p.Name;
            oldPublisher.Authors = p.Authors ?? new List<Author>();
            oldPublisher.Books = p.Books ?? new List<Book>();

            if (p.Manager != null &&
                oldPublisher.Authors.Any(a => a.IdCard == p.Manager.IdCard))
            {
                oldPublisher.Manager = p.Manager;
            }
            else
            {
                oldPublisher.Manager = null;
            }

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