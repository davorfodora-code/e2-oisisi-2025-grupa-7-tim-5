using Core.DAO;
using Core.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Address adresa1 = new Address("DJordja Zlicica", "34", "Novi Sad", "Vojvodina", "Srbija");
            Address adresa2 = new Address("Kralja Petra 1", "15b", "Pristina", "KiM", "Srbija");
            Console.WriteLine(adresa1);

            //   Console.WriteLine(publisher1);

            List<Visitor> visitors = new List<Visitor>();
            List<Purchase> purchases = new List<Purchase>();
            List<Book> wishlist = new List<Book>();


            DateOnly purchaseDate = DateOnly.ParseExact("12.12.2000.", "dd.MM.yyyy.");



            Book book1 = new Book();
            // BookDAO bookDAO= new BookDAO();
            //  bookDAO.AddBook(book1);
            List<Book> books = new List<Book>();

            AuthorDAO authorDAO = new AuthorDAO();

            Author a1 = new Author("Ivo", "Andrić",purchaseDate, adresa1,"0601376895", "ivo@andric.rs", "ID001",40, new List<Book>());

            Author a2 = new Author("Misa", "Andrić",purchaseDate, adresa1,"060135454895", "ivo@andric.rs", "ID001",40, new List<Book>());

            authorDAO.AddAuthor(a1);

            List<Author> authors = authorDAO.GetAllAuthors();
            foreach (Author author in authors)
            {
                Console.WriteLine(author);
            }
            Publisher publisher1 = new Publisher("Mirko", a1, authors, books);

           
            authorDAO.UpdateAuthor(a1);

            foreach (Author author in authorDAO.GetAllAuthors())
            {
                Console.WriteLine(author);
            }
            

            authorDAO.RemoveAuthor("ID001");

            Book book2 = new Book("strinng", "ime", "zanr", "2022", 123.0, 15, authors, publisher1, visitors, visitors);
            BookDAO bookDAO = new BookDAO();
            bookDAO.AddBook(book2);
            
            //DateOnly purchaseDate2 = DateOnly.ParseExact("13.12.2000.", "dd.MM.yyyy.");

            WishListDAO wishlistDAO = new WishListDAO();
           






            Visitor visitor = new Visitor("Davor", "Fodora", "24-06-04", adresa1, "000", "11", "204", "12", "12", purchases, wishlist);
            wishlistDAO.AddWishList(visitor, book2);

           Purchase p1 = new Purchase(visitor, book2, purchaseDate, 4.3, "Odlicna knjiga");
           Purchase p4 = new Purchase(visitor, book2, purchaseDate, 4.1, "Sup knjiga");
            PurchaseDAO purchaseDAO = new PurchaseDAO();
            purchaseDAO.AddPurchase(p1);
            purchaseDAO.AddPurchase(p4);

            VisitorDAO visitors1 = new VisitorDAO(purchaseDAO);
            visitors1.AddVisitor(visitor);

            visitors1.UpdateAverageReview("204");

            PublisherDAO publisherDAO = new PublisherDAO();
            publisherDAO.AddPublisher(publisher1);

           









        }
    }
}
