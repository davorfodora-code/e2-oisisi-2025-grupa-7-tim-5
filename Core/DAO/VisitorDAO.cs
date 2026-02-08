using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    public class VisitorDAO
    {
        private readonly List<Visitor> visitors;
        private readonly Storage<Visitor> storage;
        private PurchaseDAO _purchaseDAO;
        private PurchaseDAO PurchaseDAO
        {   
            get
            {
                if (_purchaseDAO == null)
                    _purchaseDAO = new PurchaseDAO();
                return _purchaseDAO;
            }
        }

        public VisitorDAO(PurchaseDAO purchaseDAO)
        {
            storage = new Storage<Visitor>("Visitors.txt");
            visitors = storage.Load();
        }

        public double GetAverageReview(string membershipNumber)
        {
            Visitor visitor = GetVisitorByMemebershipId(membershipNumber);

            if(visitor == null)
            {
                return 0.0;
            }

            double avgReview = _purchaseDAO.CalculateAverageReview(visitor);

            return avgReview;
        }

        public void UpdateAverageReview(string membershipNumber)
        {
            Visitor? visitor = GetVisitorByMemebershipId(membershipNumber);
            if (visitor == null) return;

            double avgReview = PurchaseDAO.CalculateAverageReview(visitor);
            visitor.AverageReview = Math.Round(avgReview, 2);

            storage.Save(visitors);
        }

        public Visitor AddVisitor(Visitor visitor)
        {
            if(GetVisitorByMemebershipId(visitor.MembershipNumber) != null)
            {
                return null;
            }

            visitors.Add(visitor);
            storage.Save(visitors);
            return visitor;
        }

        public Visitor? UpdateVisitor(Visitor v)
        {
            Visitor? oldVisitor = GetVisitorByMemebershipId(v.MembershipNumber);
            if (oldVisitor == null) return null;
            oldVisitor.Name = v.Name;
            oldVisitor.LastName = v.LastName;
            oldVisitor.Address = v.Address;
            oldVisitor.Wishlist = v.Wishlist;
            oldVisitor.Purchases = v.Purchases;
            oldVisitor.Status = v.Status;
            oldVisitor.AverageReview = v.AverageReview;
            oldVisitor.DateOfBirth = v.DateOfBirth;
            oldVisitor.MembershipYear = v.MembershipYear;
            oldVisitor.MembershipNumber = v.MembershipNumber;
            oldVisitor.ContactPhone = v.ContactPhone;
            oldVisitor.Email = v.Email;
            storage.Save(visitors);

            return oldVisitor;
        }

        public Visitor? RemoveVisitor(string mmId)
        {
            Visitor? visitor = GetVisitorByMemebershipId(mmId);
            if (visitor == null)
            {
                return null;
            }
            
            visitors.Remove(visitor);
            storage.Save(visitors);

            return visitor;
        }

        private Visitor? GetVisitorByMemebershipId(string mmId)
        {
            return visitors.Find(v => v.MembershipNumber == mmId);
        }

        public void ShowVisitors()
        {
            foreach(Visitor v in visitors)
            {
                Console.WriteLine(v);
            }
        }

        public List<Visitor> GetAllVisitors()
        {
            return new List<Visitor>(visitors);
        }

    }
}
