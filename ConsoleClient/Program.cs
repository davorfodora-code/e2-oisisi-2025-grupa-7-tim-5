using Core.DAO;
using Core.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //TESTIRANJE ADRESE
            Address adresa1 = new Address("DJordja Zlicica", "34", "Novi Sad", "Vojvodina", "Srbija");
            Address adresa2 = new Address("Kralja Petra 1", "15b", "Pristina", "KiM", "Srbija");
            AddressDAO addressDAO   = new AddressDAO();
            addressDAO.AddAddress(adresa1);
            Console.WriteLine(adresa1);
            Console.WriteLine(adresa2);

            //KREIRANJE DateOnly
            DateOnly purchaseDate = DateOnly.ParseExact("12.12.2000.", "dd.MM.yyyy.");

            //TESTIRANJE AUTORA
            AuthorDAO authorDAO = new AuthorDAO();
            Author a1 = new Author("Ivo", "Andrić", purchaseDate, adresa1, "0601376895", "ivo@andric.rs", "ID001", 40, new List<Book>());
            Author a2 = new Author("Misa", "Andrić", purchaseDate, adresa1, "060135454895", "ivo@andric.rs", "ID002", 40, new List<Book>());
            authorDAO.AddAuthor(a1);
            authorDAO.AddAuthor(a2);
            List<Author> authors = authorDAO.GetAllAuthors();
            authorDAO.RemoveAuthor("ID001");

            //TESTIRANJE VISITORA
            Visitor visitor = new Visitor("Davor", "Fodora", "24-06-04", adresa1, "000", "11", "204", "12", "12");
            List<Visitor> visitors = new List<Visitor>();
            PurchaseDAO purchaseDAO = new PurchaseDAO();
            VisitorDAO visitors1 = new VisitorDAO(purchaseDAO);
            visitors1.AddVisitor(visitor);
            visitors1.UpdateAverageReview("204");

            //TESTIRANJE PUBLISHERA
            List<Book> books = new List<Book>();
            Publisher publisher1 = new Publisher("Mirko", a1, authors, books);
            PublisherDAO publisherDAO = new PublisherDAO();
            publisherDAO.AddPublisher(publisher1);

            //TESTIRANJE KNJIGE
            Book book2 = new Book("strinng", "ime", "zanr", "2022", 123.0, 15, authors, publisher1, visitors, visitors);
            BookDAO bookDAO = new BookDAO();
            bookDAO.AddBook(book2);

            //TESTIRANJE PURCHASE
            Purchase p1 = new Purchase(visitor, book2, purchaseDate, 4.3, "Odlicna knjiga");
            Purchase p4 = new Purchase(visitor, book2, purchaseDate, 4.1, "Sup knjiga");
            purchaseDAO.AddPurchase(p1);
            purchaseDAO.AddPurchase(p4);
            List<Purchase> purchases = new List<Purchase>();
            purchases.Add(p1);

            //TESTIRANJE WISHLSTE
            List<Book> wishlist = new List<Book>();
            WishListDAO wishlistDAO = new WishListDAO();
            wishlistDAO.AddWishList(visitor, book2);
            wishlist.Add(book2);

            //TESTIRANJE VISITORBOUGHTBOOK
            VisitorBoughtBookDAO vbbDAO = new VisitorBoughtBookDAO();
            vbbDAO.AddVisitorBoughtBook(visitor, book2);


            //TESTIRANJE VISITORA SA LISTAMA ISPISA
            Visitor visitor11 = new Visitor("Davor", "Fodora", "24-06-04", adresa1, "000", "11", "909", "12", "12", purchases, wishlist);
            visitors1.AddVisitor(visitor11);
        }
    }
}
