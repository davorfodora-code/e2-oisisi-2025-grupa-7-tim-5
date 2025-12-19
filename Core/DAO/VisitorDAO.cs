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

        public VisitorDAO()
        {
            storage = new Storage<Visitor>("DataBase.txt");
            visitors = storage.Load();
        }

        public Visitor AddVisitor(Visitor visitor)
        {
            visitors.Add(visitor);
            storage.Save(visitors);
            return visitor;
        }

        public Visitor? UpdateVisitor(Visitor v)
        {
            Visitor? oldVisitor = GetVisitorByMemebershipId(v.Membership_number);
            if (oldVisitor == null) return null;
            oldVisitor.Name = v.Name;
            oldVisitor.Address = v.Address;
            oldVisitor.Wishlist = v.Wishlist;
            oldVisitor.Purchases = v.Purchases;
            oldVisitor.Status = v.Status;
            oldVisitor.Average_review = v.Average_review;
            oldVisitor.Date_of_birth = v.Date_of_birth;
            oldVisitor.Membership_year = v.Membership_year;
            oldVisitor.Contact_phone = v.Contact_phone;
            oldVisitor.Email = v.Email;
            storage.Save(visitors);

            return oldVisitor;
        }

        public Visitor? RemoveVisitor(string mmId)
        {
            Visitor? visitor = GetVisitorByMemebershipId(mmId);
            if (visitor == null)
            {
                Console.WriteLine("Visitor NOT FOUND: " + mmId);
                return null;

            }
            

            visitors.Remove(visitor);
            storage.Save(visitors);

            return visitor;
        }

        private Visitor? GetVisitorByMemebershipId(string mmId)
        {
            return visitors.Find(v => v.Membership_number == mmId);
        }

        public void ShowVisitors()
        {
            foreach(Visitor v in visitors)
            {
                Console.WriteLine(v);
            }
        }

    }
}
