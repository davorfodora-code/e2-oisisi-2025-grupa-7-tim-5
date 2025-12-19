using Core.DAO;
using Core.Serialization;

namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
        

            

            Address a = new Address("ulica","34","novi sad","vojvodina","srbija");
            Console.WriteLine(a);
            Publisher publisher = new Publisher();
            Console.WriteLine(publisher);

            List<Purchase> purchases = new List<Purchase>();
            List<Book> wishlist = new List<Book>();
            Visitor visitor = new Visitor("Davor", "Fodora", "24-06-04", a, "000", "11", "204", "12", "12", 5.4, purchases, wishlist);
            VisitorDAO visitors1 = new VisitorDAO();
            visitors1.AddVisitor(visitor);

            visitors1.RemoveVisitor("12");
            visitors1.ShowVisitors();

        }
    }
}
