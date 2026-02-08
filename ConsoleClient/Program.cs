using Core.DAO;
using Core.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ========== 1. ENUM DEFINICIJA ==========


        // ========== 2. ADRESE ==========
        Address adresa1 = new Address("DJordja Zlicica", "34", "Novi Sad", "Vojvodina", "Srbija");
        Address adresa2 = new Address("Kralja Petra 1", "15b", "Pristina", "KiM", "Srbija");
        Address adresa3 = new Address("Bulevar Kralja Aleksandra", "127", "Beograd", "Centralna Srbija", "Srbija");
        Address adresa4 = new Address("Svetozara Markovića", "22a", "Niš", "Centralna Srbija", "Srbija");
        Address adresa5 = new Address("Mihajla Pupina", "5", "Subotica", "Vojvodina", "Srbija");
        Address adresa6 = new Address("Knez Mihailova", "48", "Beograd", "Grad Beograd", "Srbija");

        // ========== 3. DATUMI ==========
        DateOnly purchaseDate = DateOnly.ParseExact("12.12.2000.", "dd.MM.yyyy.");
        DateOnly purchaseDate1 = DateOnly.ParseExact("12.12.2023.", "dd.MM.yyyy.");
        DateOnly purchaseDate2 = DateOnly.ParseExact("05.01.2024.", "dd.MM.yyyy.");
        DateOnly purchaseDate3 = DateOnly.ParseExact("20.02.2024.", "dd.MM.yyyy.");
        DateOnly purchaseDate4 = DateOnly.ParseExact("15.03.2024.", "dd.MM.yyyy.");
        DateOnly purchaseDate5 = DateOnly.ParseExact("08.04.2024.", "dd.MM.yyyy.");

        DateOnly datumRodjenja1 = DateOnly.ParseExact("15.05.1990.", "dd.MM.yyyy.");
        DateOnly datumRodjenja2 = DateOnly.ParseExact("03.11.1985.", "dd.MM.yyyy.");
        DateOnly datumRodjenja3 = DateOnly.ParseExact("22.08.1978.", "dd.MM.yyyy.");
        DateOnly datumRodjenja4 = DateOnly.ParseExact("09.01.1950.", "dd.MM.yyyy.");
        DateOnly datumRodjenja5 = DateOnly.ParseExact("30.03.1995.", "dd.MM.yyyy.");

        // ========== 4. AUTORI ==========
        // Prvo kreiramo prazne liste za knjige autora
        List<Book> knjigeDanilaKisa = new List<Book>();
        List<Book> knjigeMomeKapora = new List<Book>();

        Author a1 = new Author("Ivo", "Andrić", datumRodjenja1, adresa1, "0601376895", "ivo@andric.rs", "ID001", 40, new List<Book>());
        Author a2 = new Author("Misa", "Andrić", datumRodjenja5, adresa1, "060135454895", "ivo@andric.rs", "ID001", 40, new List<Book>());
        Author a3 = new Author("Danilo", "Kiš", datumRodjenja1, adresa3, "0612345678", "danilo.kis@email.rs", "ID002", 54, knjigeDanilaKisa);
        Author a4 = new Author("Momo", "Kapor", datumRodjenja2, adresa6, "0641122334", "momo.kapor@email.rs", "ID003", 73, knjigeMomeKapora);
        Author a5 = new Author("Miroslav", "Antić", datumRodjenja3, adresa5, "0639876543", "miroslav.antic@email.rs", "ID004", 58, new List<Book>());
        Author a6 = new Author("Milorad", "Pavić", datumRodjenja4, adresa6, "0625551212", "pavic@hazarskirecnik.rs", "ID005", 80, new List<Book>());
        Author a7 = new Author("Jelena", "Lengold", datumRodjenja2, adresa4, "0657778889", "j.lengold@email.com", "ID006", 35, new List<Book>());
        Author a9 = new Author("Svetlana", "Velmar-Janković", datumRodjenja4, adresa6, "0698887777", "svetlana.vj@email.rs", "ID008", 84, new List<Book>());
        Author a10 = new Author("Goran", "Petrović", datumRodjenja5, adresa3, "0669990001", "goran.petrovic@email.rs", "ID009", 42, new List<Book>());

        List<Author> authors = new List<Author> { a1, a2, a3, a4, a5, a6, a7, a9, a10 };

        // ========== 5. POSETIOCI ==========
        List<Purchase> purchases1 = new List<Purchase>();
        List<Book> wishlist1 = new List<Book>();

        List<Purchase> purchases2 = new List<Purchase>();
        List<Book> wishlist2 = new List<Book>();

        List<Purchase> purchases3 = new List<Purchase>();
        List<Book> wishlist3 = new List<Book>();

        List<Purchase> purchases4 = new List<Purchase>();
        List<Book> wishlist4 = new List<Book>();

        Visitor v1 = new Visitor("Davor", "Fodora", "24-06-04", adresa1, "000", "11", "204", "12", "12", purchases1, wishlist1);
        Visitor v2 = new Visitor("Ana", "Marković", "15-03-02", adresa3, "001", "15", "305", "8", "10", purchases2, wishlist2);
        Visitor v3 = new Visitor("Marko", "Petrović", "10-11-05", adresa5, "002", "22", "118", "15", "20", purchases3, wishlist3);
        Visitor v4 = new Visitor("Jovana", "Nikolić", "03-07-01", adresa6, "003", "9", "412", "5", "7", purchases4, wishlist4);

        List<Visitor> visitors = new List<Visitor> { v1, v2, v3, v4 };

        // ========== 6. KNJIGE ==========
        // Lista autora za knjige
        List<Author> authors1 = new List<Author> { a1 };
        List<Author> authors2 = new List<Author> { a3 };
        List<Author> authors3 = new List<Author> { a4 };
        List<Author> authors4 = new List<Author> { a6 };
        List<Author> authors5 = new List<Author> { a5, a7 };

        // Kreiranje knjiga sa enum Genre
        Book book1 = new Book("ISBN9788610036547", "Na Drini ćuprija", Genre.Roman, "1945", 1200.0, 314, authors1, null, new List<Visitor>(), new List<Visitor>());
        Book book2 = new Book("ISBN9788661452071", "Hazarski rečnik", Genre.Roman, "1984", 1500.0, 450, authors4, null, new List<Visitor>(), new List<Visitor>());
        Book book3 = new Book("ISBN9788663791239", "Una", Genre.Roman, "1995", 1100.0, 280, authors3, null, new List<Visitor>(), new List<Visitor>());
        Book book4 = new Book("ISBN9788674461835", "Grobnica za Borisa Davidoviča", Genre.Roman, "1976", 1350.0, 210, authors2, null, new List<Visitor>(), new List<Visitor>());
        Book book5 = new Book("ISBN9788660311452", "Sabrana pesme", Genre.Poezija, "1965", 900.0, 180, authors5, null, new List<Visitor>(), new List<Visitor>());

        // Knjige za knjige autora
        Book book6 = new Book("ISBN9788610023547", "Grobnica za Borisa Davidoviča", Genre.Roman, "1976", 1350.0, 210, authors2, null, new List<Visitor>(), new List<Visitor>());
        Book book7 = new Book("ISBN9788610023548", "Peščanik", Genre.Roman, "1972", 1200.0, 250, authors2, null, new List<Visitor>(), new List<Visitor>());
        Book book8 = new Book("ISBN9788639104523", "Život je nekad siv, nekad žut", Genre.Esej, "1985", 950.0, 190, authors3, null, new List<Visitor>(), new List<Visitor>());
        Book book9 = new Book("ISBN9788639104524", "Una", Genre.Roman, "1995", 1100.0, 280, authors3, null, new List<Visitor>(), new List<Visitor>());

        // Dodaj knjige listama autora
        knjigeDanilaKisa.Add(book6);
knjigeDanilaKisa.Add(book7);
knjigeMomeKapora.Add(book8);
knjigeMomeKapora.Add(book9);

// Knjige za kupovine
Book bookKupovina1 = new Book("ISBN9788610036548", "Na Drini ćuprija", Genre.Roman, "1945", 1200.0, 314, authors1, null, new List<Visitor>(), new List<Visitor>());
        Book bookKupovina2 = new Book("ISBN9788652114520", "Prokleta avlija", Genre.Roman, "1954", 1250.0, 256, authors1, null, new List<Visitor>(), new List<Visitor>());
        Book bookKupovina3 = new Book("ISBN9788645123975", "Gospođica", Genre.Novela, "1945", 800.0, 112, authors1, null, new List<Visitor>(), new List<Visitor>());
        Book bookKupovina4 = new Book("ISBN9788654321098", "Most na Žepi", Genre.Pripovetka, "1925", 750.0, 85, authors1, null, new List<Visitor>(), new List<Visitor>());
        Book bookKupovina5 = new Book("ISBN9788611123345", "Anikina vremena", Genre.Roman, "1931", 1050.0, 275, authors1, null, new List<Visitor>(), new List<Visitor>());

        List<Book> books = new List<Book> { book1, book2, book3, book4, book5, book6, book7, book8, book9 };

        // ========== 7. IZDAVAČ ==========
        Publisher publisher1 = new Publisher("Mirko", a1, authors, books);

        // Postavi izdavača za knjige
        book1.Publisher = publisher1;
book2.Publisher = publisher1;
book3.Publisher = publisher1;
book4.Publisher = publisher1;
book5.Publisher = publisher1;
book6.Publisher = publisher1;
book7.Publisher = publisher1;
book8.Publisher = publisher1;
book9.Publisher = publisher1;
bookKupovina1.Publisher = publisher1;
bookKupovina2.Publisher = publisher1;
bookKupovina3.Publisher = publisher1;
bookKupovina4.Publisher = publisher1;
bookKupovina5.Publisher = publisher1;

// ========== 8. KUPOVINE ==========
Purchase p1 = new Purchase(v1, bookKupovina2, purchaseDate1, 4.3, "Odlična knjiga!");
        Purchase p4 = new Purchase(v4, bookKupovina3, purchaseDate2, 4.2, "Lepo opisana istorija");

        List<Purchase> purchases = new List<Purchase> { p1, p4 };

        // ========== 9. TESTIRANJE DAO KLASA ==========
        // AuthorDAO
        AuthorDAO authorDAO = new AuthorDAO();
        authorDAO.AddAuthor(a1);

Console.WriteLine("Svi autori nakon dodavanja:");
foreach (Author author in authorDAO.GetAllAuthors())
{
    Console.WriteLine(author);
}

    authorDAO.UpdateAuthor(a1);

Console.WriteLine("\nSvi autori nakon update-a:");
foreach (Author author in authorDAO.GetAllAuthors())
{
    Console.WriteLine(author);
}

authorDAO.RemoveAuthor("ID001");

Console.WriteLine("\nSvi autori nakon brisanja ID001:");
foreach (Author author in authorDAO.GetAllAuthors())
{
    Console.WriteLine(author);
}

// BookDAO
BookDAO bookDAO = new BookDAO();
Book testBook = new Book("strinng", "ime", Genre.Roman, "2022", 123.0, 15, authors, publisher1, visitors, visitors);
bookDAO.AddBook(testBook);

// PurchaseDAO
PurchaseDAO purchaseDAO = new PurchaseDAO();
purchaseDAO.AddPurchase(p1);
purchaseDAO.AddPurchase(p4);

// WishListDAO
WishListDAO wishlistDAO = new WishListDAO();
wishlistDAO.AddWishList(v1, book2);

// VisitorDAO
VisitorDAO visitorDAO = new VisitorDAO(purchaseDAO);
visitorDAO.AddVisitor(v1);
visitorDAO.UpdateAverageReview("204");

// PublisherDAO
PublisherDAO publisherDAO = new PublisherDAO();
publisherDAO.AddPublisher(publisher1);

        }
    }
}
