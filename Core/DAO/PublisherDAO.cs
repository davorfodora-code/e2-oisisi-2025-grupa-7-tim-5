using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    public class PublisherDAO
    {
        private readonly List<Publisher> publishers;
        private readonly Storage<Publisher> storage;

        public PublisherDAO()
        {

            storage = new Storage<Publisher>("Publishers.txt");
            publishers = storage.Load();
        }

        public Publisher AddPublisher(Publisher p)
        {
            if(FindPublisher(p.Id) != null)
            {
                return null;
            }
            
            publishers.Add(p);
            storage.Save(publishers);
            return p;
        }

        public Publisher? UpdateAddress(Publisher p)
        {
            Publisher? oldPublisher = FindPublisher(p.Id);
            if (oldPublisher == null)
                return null;

            oldPublisher.Id = p.Id;
            oldPublisher.Name = p.Name;


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


        private Publisher? FindPublisher(string id)
        {
            return publishers.Find(p => p.Id == id);
        }


        public List<Publisher> GetAllPublishers()
        {
            return new List<Publisher>(publishers);
        }
    }
}
