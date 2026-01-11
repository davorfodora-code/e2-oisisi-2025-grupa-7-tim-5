using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class WishListDAO
{
    private readonly List<WishList> wishLists;
    private readonly Storage<WishList> storage;

    public WishListDAO()
    {
        storage = new Storage<WishList>("WishLists.txt");
        wishLists = storage.Load();
    }

    public WishList AddWishList(Visitor v, Book b)
    {
        if (wishLists.Any(w =>
           w.VisitorMemberShipNumber == v.MembershipNumber &&
           w.BookIsbn == b.Isbn))
        {
            return null;
        }

        var oneWishList = new WishList
        {
            VisitorMemberShipNumber = v.MembershipNumber,
            VistorName = v.Name,
            VisitorLastName = v.LastName,
            BookIsbn = b.Isbn,
            BookName = b.Name
        };

        wishLists.Add(oneWishList);
        storage.Save(wishLists);
        return oneWishList;
    }

   

}

