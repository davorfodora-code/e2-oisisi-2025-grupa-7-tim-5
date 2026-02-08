using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class VisitorBoughtBookDAO
{
    private readonly List<VisitorBoughtBook> visitorBoughtBookLists;
    private readonly Storage<VisitorBoughtBook> storage;

    public VisitorBoughtBookDAO()
    {
        storage = new Storage<VisitorBoughtBook>("VisitorBoughtBook.txt");
        visitorBoughtBookLists = storage.Load();
    }

    public VisitorBoughtBook AddVisitorBoughtBook(Visitor v, Book b)
    {
        if (visitorBoughtBookLists.Any(vb =>
           vb.VisitorMemberShipNumber == v.MembershipNumber &&
           vb.BookIsbn == b.Isbn))
        {
            return null;
        }

        var visitorBoughtBook = new VisitorBoughtBook
        {
            VisitorMemberShipNumber = v.MembershipNumber,
            VistorName = v.Name,
            VisitorLastName = v.LastName,
            BookIsbn = b.Isbn,
            BookName = b.Name
        };

        visitorBoughtBookLists.Add(visitorBoughtBook);
        storage.Save(visitorBoughtBookLists);
        return visitorBoughtBook;
    }

    public bool RemoveFromVisitorBoughtBook(string visitorId, string bookIsbn)
    {
        var item = visitorBoughtBookLists.FirstOrDefault(w =>
            w.VisitorMemberShipNumber == visitorId &&
            w.BookIsbn == bookIsbn);

        if (item != null)
        {
            visitorBoughtBookLists.Remove(item);
            storage.Save(visitorBoughtBookLists);
            return true;
        }
        return false;
    }

}

