using Core.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Core.DTO
{
    public class AuthorDTO
    {
        public string Name { get; set; } = "";
        public string Lastname { get; set; } = "";
        public DateOnly DateOfBirth { get; set; }
        public AddressDTO Address { get; set; }
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string IdCard { get; set; } = "";
        public double ExperienceLength { get; set; }
        public List<BookDTO> BookList { get; set; } = new List<BookDTO>();

        public string FullName => $"{Name} {Lastname}";

        public int BooksCount => BookList?.Count ?? 0;

        public (bool IsValid, string ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return (false, "Author name is required.");

            if (string.IsNullOrWhiteSpace(Lastname))
                return (false, "Author lastname is required.");

            if (DateOfBirth == default)
                return (false, "Date of birth is required.");

            if (string.IsNullOrWhiteSpace(IdCard))
                return (false, "IdCard is required.");

            if (string.IsNullOrWhiteSpace(Email))
                return (false, "Email is required.");

            return (true, "");
        }

        public Author ToModel()
        {
            return new Author
            {
                Name = this.Name,
                Lastname = this.Lastname,
                DateOfBirth = this.DateOfBirth,
                Address = this.Address?.ToModel(),
                PhoneNumber = this.PhoneNumber,
                Email = this.Email,
                IdCard = this.IdCard,
                ExperienceLength = this.ExperienceLength,
                BookList = this.BookList?.Select(b => b.ToModel()).ToList() ?? new List<Book>()
            };
        }

        public static AuthorDTO FromModel(Author author)
        {
            if (author == null) return null;

            return new AuthorDTO
            {
                Name = author.Name,
                Lastname = author.Lastname,
                DateOfBirth = author.DateOfBirth,
                Address = AddressDTO.FromModel(author.Address),
                PhoneNumber = author.PhoneNumber,
                Email = author.Email,
                IdCard = author.IdCard,
                ExperienceLength = author.ExperienceLength,
                BookList = author.BookList?.Select(BookDTO.FromModel).ToList() ?? new List<BookDTO>()
            };
        }
    }
}

