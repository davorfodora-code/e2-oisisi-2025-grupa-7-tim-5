using Core.DAO;
using Core.DTO;
using Core.Observer;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void SubscribeObserver(IObserver observer) => Subscribe(observer);
        public void UnsubscribeObserver(IObserver observer) => Unsubscribe(observer);

        public List<Visitor> GetAllVisitors() => _visitorDAO.GetAllVisitors();

        public (bool Success, string Message) AddVisitor(VisitorDTO dto)
        {
            var (isValid, errorMessage) = dto.Validate();
            if (!isValid)
                return (false, errorMessage);

            if (_visitorDAO.GetVisitorByMemebershipId(dto.MembershipNumber) != null)
                return (false, $"Visitor with membership number '{dto.MembershipNumber}' already exists!");

            var visitor = dto.ToModel();
            var result = _visitorDAO.AddVisitor(visitor);

            if (result == null)
                return (false, "Failed to add visitor!");

            NotifyObservers();
            return (true, "Visitor added successfully!");
        }

        public (bool Success, string Message) UpdateVisitor(VisitorDTO dto)
        {
            var (isValid, errorMessage) = dto.Validate();
            if (!isValid)
                return (false, errorMessage);

            var existingVisitor = _visitorDAO.GetVisitorByMemebershipId(dto.MembershipNumber);
            if (existingVisitor == null)
                return (false, "Visitor not found!");

            var visitor = dto.ToModel();

            // Čuvamo podatke koji se ne menjaju kroz VisitorDTO
            visitor.Purchases = existingVisitor.Purchases;
            visitor.MembershipYear = existingVisitor.MembershipYear;
            visitor.AverageReview = existingVisitor.AverageReview;

            // ← NOVO: DateOfBirth, ContactPhone, Email sada dolaze iz DTO
            // (već postavljeni kroz dto.ToModel() gore)

            var result = _visitorDAO.UpdateVisitor(visitor);

            if (result == null)
                return (false, "Visitor not found!");

            NotifyObservers();
            return (true, "Visitor updated successfully!");
        }

        public (bool Success, string Message) DeleteVisitor(string membershipNumber)
        {
            var result = _visitorDAO.RemoveVisitor(membershipNumber);

            if (result == null)
                return (false, "Visitor not found!");

            NotifyObservers();
            return (true, "Visitor deleted successfully!");
        }

        public bool ExistsMembershipNumber(string membershipNumber)
            => _visitorDAO.GetVisitorByMemebershipId(membershipNumber) != null;

        public double GetAverageReview(string membershipNumber)
            => _visitorDAO.GetAverageReview(membershipNumber);

        public void UpdateAverageReview(string membershipNumber)
            => _visitorDAO.UpdateAverageReview(membershipNumber);

        public List<Visitor> SearchVisitors(string searchQuery)
            => _visitorDAO.SearchVisitors(searchQuery);

        public List<VisitorDTO> SearchVisitorsDTO(string searchQuery)
        {
            var visitors = _visitorDAO.SearchVisitors(searchQuery);
            return visitors.Select(VisitorDTO.FromModel).ToList();
        }
    }
}