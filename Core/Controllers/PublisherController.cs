using Core.DAO;
using Core.DTO;
using Core.Observer;
using System.Collections.Generic;
using System.Linq;

namespace Core.Controllers
{
    public class PublisherController : Subject
    {
        private readonly PublisherDAO _publisherDao;

        public PublisherController(PublisherDAO publisherDao)
        {
            _publisherDao = publisherDao;
        }

        public List<Publisher> GetAllPublishers()
        {
            return _publisherDao.GetAllPublishers();
        }

        public List<PublisherDTO> GetAllPublisherDTOs()
        {
            return _publisherDao.GetAllPublishers()
                .Select(PublisherDTO.FromModel)
                .ToList();
        }
    }
}