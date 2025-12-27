using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    public class PurchaseDAO
    {
        private readonly List<Purchase> purchases;
        private readonly Storage<Purchase> storage;

        public PurchaseDAO()
        {
            storage = new Storage<Purchase>("Purchases.txt");
            purchases = storage.Load();
        }

      
        public Purchase AddPurchase(Purchase purchase)
        {
            if(FindPurchase(purchase) != null)
            {
                return null;
            }
          
            purchases.Add(purchase);
            storage.Save(purchases);
            return purchase;
        }

        public Purchase? UpdatePurchase(Purchase p)
        {
            Purchase? oldPurchase = FindPurchase(p);
            if (oldPurchase == null)
                return null;

            oldPurchase.Visitor = p.Visitor;
            oldPurchase.Book = p.Book;
            oldPurchase.DateOfPurchase = p.DateOfPurchase;
            oldPurchase.Review = p.Review;
            oldPurchase.Comment = p.Comment;

            storage.Save(purchases);
            return oldPurchase;
        }

      
        public Purchase? RemovePurchase(Purchase p)
        {
            Purchase? purchase = FindPurchase(p);
            if (purchase == null)
                return null;

            purchases.Remove(purchase);
            storage.Save(purchases);
            return purchase;
        }

      
        private Purchase? FindPurchase(Purchase p)
        {
            return purchases.Find(x =>
               x.Visitor.MembershipNumber == p.Visitor.MembershipNumber &&
               x.Book.Isbn == p.Book.Isbn &&
               x.DateOfPurchase == p.DateOfPurchase
            );
        }

       
        public List<Purchase> GetAllPurchases()
        {
            return new List<Purchase>(purchases);
        }
    }

}
