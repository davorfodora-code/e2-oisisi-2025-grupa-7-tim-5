using Core.DAO;
using Core.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Controllers
{
    public class VisitorController : Subject
    {
        private readonly VisitorDAO _visitorDAO;
        private readonly PurchaseDAO _purchaseDAO;

        public VisitorController(VisitorDAO visitorDAO, PurchaseDAO purchaseDAO)
        {
            _visitorDAO = visitorDAO;
            _purchaseDAO = purchaseDAO;
        }

        public void SubscribeObserver(IObserver observer)
        {
            Subscribe(observer);
        }

        public void UnsubscribeObserver(IObserver observer)
        {
            Unsubscribe(observer);
        }

        public List<Visitor> GetAllVisitors()
        {
            return _visitorDAO.GetAllVisitors();
        }
    }
}
