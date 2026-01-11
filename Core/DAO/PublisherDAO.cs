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

        public Publisher AddPublisher(Publisher p)
        {
            

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
