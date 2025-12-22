using Core.DAO;
using Core.Serialization;

namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Address adresa1 = new Address("DJordja Zlicica","34","Novi Sad","Vojvodina","Srbija");
            Address adresa2 = new Address("Kralja Petra 1", "15b", "Pristina", "KiM", "Srbija");
            Console.WriteLine(adresa1);
            Publisher publisher = new Publisher();
            Console.WriteLine(publisher);
          

            List<Purchase> purchases = new List<Purchase>();
            List<Book> wishlist = new List<Book>();
            Visitor visitor = new Visitor("Davor", "Fodora", "24-06-04", adresa1, "000", "11", "204", "12", "12", 5.4, purchases, wishlist);
            VisitorDAO visitors1 = new VisitorDAO();
            visitors1.AddVisitor(visitor);

            visitors1.RemoveVisitor("12");
            visitors1.ShowVisitors();


            AuthorDAO authorDAO = new AuthorDAO();

            Author a1= new Author("Ivo", "Andrić", "1892-10-09",  "064123456", "ivo@andric.rs" , "ID001",adresa1, 40, new List<Book>());

            Author a2 = new Author( "Mesa", "Selimović", "1910-04-26", "065987654", "mesa@selimovic.rs", "ID002",adresa2,35, new List<Book>()
        );
            authorDAO.AddAuthor(a1);

            List<Author> authors = authorDAO.GetAllAuthors();
            foreach (Author author in authors)
            {
                Console.WriteLine(author);
            }

            a1.Phone_number = "066111222";
            authorDAO.UpdateAuthor(a1);

            foreach (Author author in authorDAO.GetAllAuthors())
            {
                Console.WriteLine(author);
            }

            authorDAO.RemoveAuthor("ID002");





        }
    }
}
