using Core.DAO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.DTO
{
    public class BookDTO
    {
        public string Isbn { get; set; } = "";
        public string Name { get; set; } = "";
        public Genre Genre { get; set; }
        public string PublicationYear { get; set; } = "";
        public double Price { get; set; }
        public int NumberOfPages { get; set; }
        public List<AuthorDTO> Authors { get; set; } = new List<AuthorDTO>();
        public PublisherDTO Publisher { get; set; }
        public List<VisitorDTO> VisitorBoughtBook { get; set; } = new List<VisitorDTO>();
        public List<VisitorDTO> VisitorWishList { get; set; } = new List<VisitorDTO>();

        // DISPLAY PROPERTIES
        public string DisplayAuthors => Authors != null && Authors.Count > 0
            ? string.Join(", ", Authors.Select(a => $"{a.Name} {a.Lastname}"))
            : "";

        public string DisplayPublisher => Publisher != null ? Publisher.Name : "";

        // VALIDACIJA
        public (bool IsValid, string ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Isbn))
                return (false, "ISBN is required.");
            if (string.IsNullOrWhiteSpace(Name))
                return (false, "Book name is required.");
            if (Authors == null || Authors.Count == 0)
                return (false, "Book must have at least one author.");
            if (Publisher == null)
                return (false, "Book must have a publisher.");
            return (true, "");
        }

        // KONVERZIJA U MODEL
        public Book ToModel()
        {
            return new Book
            {
                Isbn = this.Isbn,
                Name = this.Name,
                Genre = this.Genre,
                PublicationYear = this.PublicationYear,
                Price = this.Price,
                NumberOfPages = this.NumberOfPages,
                Authors = this.Authors?.Select(a => a.ToModel()).ToList() ?? new List<Author>(),
                Publisher = this.Publisher?.ToModel(),
                VisitorBoughtBook = this.VisitorBoughtBook?.Select(v => v.ToModel()).ToList() ?? new List<Visitor>(),
                VisitorWishList = this.VisitorWishList?.Select(v => v.ToModel()).ToList() ?? new List<Visitor>()
            };
        }

        // KONVERZIJA IZ MODELA
        public static BookDTO FromModel(Book book)
        {
            if (book == null) return null;

            return new BookDTO
            {
                Isbn = book.Isbn,
                Name = book.Name,
                Genre = book.Genre,
                PublicationYear = book.PublicationYear,
                Price = book.Price,
                NumberOfPages = book.NumberOfPages,
                Authors = book.Authors?.Select(AuthorDTO.FromModel).ToList() ?? new List<AuthorDTO>(),
                Publisher = PublisherDTO.FromModel(book.Publisher),
                VisitorBoughtBook = book.VisitorBoughtBook?.Select(VisitorDTO.FromModel).ToList() ?? new List<VisitorDTO>(),
                VisitorWishList = book.VisitorWishList?.Select(VisitorDTO.FromModel).ToList() ?? new List<VisitorDTO>()
            };
        }
    }
    
    }


