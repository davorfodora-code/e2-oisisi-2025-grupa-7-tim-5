using Core.Serialization;

namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Storage<Visitor> storage = new Storage<Visitor>("DataBase.txt");
            List<Visitor> visitors = new List<Visitor>();
            Visitor visitor = new Visitor();
            visitors.Add(visitor); 
            storage.Save(visitors);
            List<Visitor> load = storage.Load();


            Address a = new Address("ulica","34","novi sad","vojvodina","srbija");
            Address a1;
            Console.WriteLine(a);
            Publisher publisher = new Publisher();
            Console.WriteLine(publisher);

        }
    }
}
