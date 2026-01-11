using Core.DAO;
using Core.Serialization;

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
            Visitor visitor = new Visitor("Davor", "Fodora", "24-06-04", adresa1, "000", "11", "204", "12", "12", 5.4, purchases, wishlist);
            VisitorDAO visitors1 = new VisitorDAO();
            visitors1.AddVisitor(visitor);

            visitors1.RemoveVisitor("12");
            visitors1.ShowVisitors();

            AddressDAO addressDAO = new AddressDAO();
            addressDAO.AddAddress(adresa1);
            

            Book book1 = new Book();
            // BookDAO bookDAO= new BookDAO();
            //  bookDAO.AddBook(book1);
            List<Book> books = new List<Book>();

            AuthorDAO authorDAO = new AuthorDAO();

            Author a1 = new Author("Ivo", "Andrić", "1892-10-09", "064123456", "ivo@andric.rs", "ID001", adresa1, 40, new List<Book>());

            Author a2 = new Author("Mesa", "Selimović", "1910-04-26", "065987654", "mesa@selimovic.rs", "ID002", adresa2, 35, new List<Book>()
        );
            authorDAO.AddAuthor(a1);

            List<Author> authors = authorDAO.GetAllAuthors();
            foreach (Author author in authors)
            {
                Console.WriteLine(author);
            }
            Publisher publisher1 = new Publisher("Mirko", a1, authors, books);

            a1.PhoneNumber = "066111222";
            authorDAO.UpdateAuthor(a1);

            foreach (Author author in authorDAO.GetAllAuthors())
            {
                Console.WriteLine(author);
            }
            

            authorDAO.RemoveAuthor("ID002");

            Book book2 = new Book("strinng", "ime", "zanr", "godina", 123.0, 15, authors, publisher1, visitors, visitors);
            BookDAO bookDAO = new BookDAO();
            bookDAO.AddBook(book2);
            Purchase p1 = new Purchase(visitor,book2,"12.12.2000",4.3,"Odlicna knjiga");

            PublisherDAO publisherDAO = new PublisherDAO();
            publisherDAO.AddPublisher(publisher1);
            PurchaseDAO purchaseDAO = new PurchaseDAO();
            purchaseDAO.AddPurchase(p1);





        }
    }
}
