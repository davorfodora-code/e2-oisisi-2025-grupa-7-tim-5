using Core.DAO;
using Core.Serialization;
using System.Collections.Generic;
using System;

namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ========== 1. ADRESE ==========
            Address adresa1 = new Address("DJordja Zlicica", "34", "Novi Sad", "Vojvodina", "Srbija");
            Address adresa2 = new Address("Kralja Petra 1", "15b", "Pristina", "KiM", "Srbija");
            Address adresa3 = new Address("Bulevar Kralja Aleksandra", "127", "Beograd", "Centralna Srbija", "Srbija");
            Address adresa4 = new Address("Svetozara Markovića", "22a", "Niš", "Centralna Srbija", "Srbija");
            Address adresa5 = new Address("Mihajla Pupina", "5", "Subotica", "Vojvodina", "Srbija");
            Address adresa6 = new Address("Knez Mihailova", "48", "Beograd", "Grad Beograd", "Srbija");

            // ========== 2. DATUMI ==========
            DateOnly purchaseDate1 = DateOnly.ParseExact("12.12.2023.", "dd.MM.yyyy.");
            DateOnly purchaseDate2 = DateOnly.ParseExact("05.01.2024.", "dd.MM.yyyy.");

            DateOnly datumRodjenja1 = DateOnly.ParseExact("15.05.1990.", "dd.MM.yyyy.");
            DateOnly datumRodjenja2 = DateOnly.ParseExact("03.11.1985.", "dd.MM.yyyy.");
            DateOnly datumRodjenja3 = DateOnly.ParseExact("22.08.1978.", "dd.MM.yyyy.");
            DateOnly datumRodjenja4 = DateOnly.ParseExact("09.01.1950.", "dd.MM.yyyy.");
            DateOnly datumRodjenja5 = DateOnly.ParseExact("30.03.1995.", "dd.MM.yyyy.");

            // ========== 3. AUTORI ==========
            List<Book> knjigeDanilaKisa = new List<Book>();
            List<Book> knjigeMomeKapora = new List<Book>();

            Author a1 = new Author("Ivo", "Andrić", datumRodjenja1, adresa1, "0601376895", "ivo@andric.rs", "ID001", 40.0, new List<Book>());
            Author a3 = new Author("Danilo", "Kiš", datumRodjenja1, adresa3, "0612345678", "danilo.kis@email.rs", "ID002", 54.0, knjigeDanilaKisa);
            Author a4 = new Author("Momo", "Kapor", datumRodjenja2, adresa6, "0641122334", "momo.kapor@email.rs", "ID003", 73.0, knjigeMomeKapora);
            Author a5 = new Author("Miroslav", "Antić", datumRodjenja3, adresa5, "0639876543", "miroslav.antic@email.rs", "ID004", 58.0, new List<Book>());
            Author a6 = new Author("Milorad", "Pavić", datumRodjenja4, adresa6, "0625551212", "pavic@hazarskirecnik.rs", "ID005", 80.0, new List<Book>());
            Author a7 = new Author("Jelena", "Lengold", datumRodjenja2, adresa4, "0657778889", "j.lengold@email.com", "ID006", 35.0, new List<Book>());
            Author a9 = new Author("Svetlana", "Velmar-Janković", datumRodjenja4, adresa6, "0698887777", "svetlana.vj@email.rs", "ID008", 84.0, new List<Book>());
            Author a10 = new Author("Goran", "Petrović", datumRodjenja5, adresa3, "0669990001", "goran.petrovic@email.rs", "ID009", 42.0, new List<Book>());

            List<Author> sviAutori = new List<Author> { a1, a3, a4, a5, a6, a7, a9, a10 };

            // ========== 4. POSETIOCI ==========
            List<Purchase> purchases2 = new List<Purchase>();
            List<Book> wishlist2 = new List<Book>();
            List<Purchase> purchases3 = new List<Purchase>();
            List<Book> wishlist3 = new List<Book>();
            List<Purchase> purchases4 = new List<Purchase>();
            List<Book> wishlist4 = new List<Book>();

            Visitor v1 = new Visitor("Davor", "Fodora", "24-06-04", adresa1, "000", "11", "204", "1V2", "V");
            Visitor v2 = new Visitor("Ana", "Marković", "15-03-02", adresa3, "001", "15", "305", "8", "10", purchases2, wishlist2);
            Visitor v3 = new Visitor("Marko", "Petrović", "10-11-05", adresa5, "002", "22", "118", "15", "20", purchases3, wishlist3);
            Visitor v4 = new Visitor("Jovana", "Nikolić", "03-07-01", adresa6, "003", "9", "412", "5", "7", purchases4, wishlist4);

            // ========== 5. KNJIGE ==========
            List<Author> autoriIvo = new List<Author> { a1 };
            List<Author> autoriDanilo = new List<Author> { a3 };
            List<Author> autoriMomo = new List<Author> { a4 };
            List<Author> autoriMilorad = new List<Author> { a6 };
            List<Author> autoriMiroslav = new List<Author> { a5, a7 };

            // KREIRAJ PUBLISHER PRVO
            Publisher publisher1 = new Publisher();
            publisher1.Name = "Laguna";
            publisher1.Id = 1;
            publisher1.Manager = a1;
            publisher1.Authors = sviAutori;

            // KREIRAJ SVE KNJIGE SA 10 ARGUMENATA (BEZ IMENOVANIH PARAMETARA)
            Book book1 = new Book(
                "ISBN9788610036547",           // isbn
                "Na Drini ćuprija",             // name
                Genre.Roman,                     // genre
                "1945",                          // publicationYear
                1200.0,                          // price
                314,                             // numberOfPages
                autoriIvo,                       // authors
                publisher1,                      // publisher
                new List<Visitor>(),             // visitorBoughtBook
                new List<Visitor>()              // visitorWishList
            );

            Book book2 = new Book(
                "ISBN9788661452071",
                "Hazarski rečnik",
                Genre.Roman,
                "1984",
                1500.0,
                450,
                autoriMilorad,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book book3 = new Book(
                "ISBN9788663791239",
                "Una",
                Genre.Roman,
                "1995",
                1100.0,
                280,
                autoriMomo,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book book4 = new Book(
                "ISBN9788674461835",
                "Grobnica za Borisa Davidoviča",
                Genre.Roman,
                "1976",
                1350.0,
                210,
                autoriDanilo,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book book5 = new Book(
                "ISBN9788660311452",
                "Sabrana pesme",
                Genre.Poezija,
                "1965",
                900.0,
                180,
                autoriMiroslav,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book book6 = new Book(
                "ISBN9788610023547",
                "Grobnica za Borisa Davidoviča",
                Genre.Roman,
                "1976",
                1350.0,
                210,
                autoriDanilo,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book book7 = new Book(
                "ISBN9788610023548",
                "Peščanik",
                Genre.Roman,
                "1972",
                1200.0,
                250,
                autoriDanilo,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book book8 = new Book(
                "ISBN9788639104523",
                "Život je nekad siv, nekad žut",
                Genre.Esej,
                "1985",
                950.0,
                190,
                autoriMomo,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book book9 = new Book(
                "ISBN9788639104524",
                "Una",
                Genre.Roman,
                "1995",
                1100.0,
                280,
                autoriMomo,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book bookKupovina1 = new Book(
                "ISBN9788610036548",
                "Na Drini ćuprija",
                Genre.Roman,
                "1945",
                1200.0,
                314,
                autoriIvo,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book bookKupovina2 = new Book(
                "ISBN9788652114520",
                "Prokleta avlija",
                Genre.Roman,
                "1954",
                1250.0,
                256,
                autoriIvo,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            Book bookKupovina3 = new Book(
                "ISBN9788645123975",
                "Gospođica",
                Genre.Novela,
                "1945",
                800.0,
                112,
                autoriIvo,
                publisher1,
                new List<Visitor>(),
                new List<Visitor>()
            );

            // Dodaj knjige u liste autora
            knjigeDanilaKisa.Add(book6);
            knjigeDanilaKisa.Add(book7);
            knjigeMomeKapora.Add(book8);
            knjigeMomeKapora.Add(book9);

            // Lista svih knjiga
            List<Book> sveKnjige = new List<Book> {
                book1, book2, book3, book4, book5, book6, book7, book8, book9,
                bookKupovina1, bookKupovina2, bookKupovina3
            };

            publisher1.Books = sveKnjige;

            // ========== 6. KUPOVINE ==========
            Purchase p1 = new Purchase(v1, bookKupovina2, purchaseDate1, 4.3, "Odlična knjiga!");
            Purchase p4 = new Purchase(v4, bookKupovina3, purchaseDate2, 4.2, "Lepo opisana istorija");

            v1.Purchases = new List<Purchase> { p1 };
            v4.Purchases = new List<Purchase> { p4 };

            // ========== 7. DAO ==========
            AuthorDAO authorDAO = new AuthorDAO(null);
            foreach (var autor in sviAutori)
            {
                authorDAO.AddAuthor(autor);
            }

            BookDAO bookDAO = new BookDAO();
            foreach (var knjiga in sveKnjige)
            {
                bookDAO.AddBook(knjiga);
            }

            PurchaseDAO purchaseDAO = new PurchaseDAO();
            purchaseDAO.AddPurchase(p1);
            purchaseDAO.AddPurchase(p4);

            VisitorDAO visitorDAO = new VisitorDAO(purchaseDAO);
            visitorDAO.AddVisitor(v1);
            visitorDAO.AddVisitor(v2);
            visitorDAO.AddVisitor(v3);
            visitorDAO.AddVisitor(v4);

            PublisherDAO publisherDAO = new PublisherDAO();
            publisherDAO.AddPublisher(publisher1);

            Console.WriteLine("Svi podaci uspešno dodati!");
            Console.WriteLine($"Dodato autora: {sviAutori.Count}");
            Console.WriteLine($"Dodato knjiga: {sveKnjige.Count}");
            Console.WriteLine($"Dodato posetilaca: 4");
        }
    }
}