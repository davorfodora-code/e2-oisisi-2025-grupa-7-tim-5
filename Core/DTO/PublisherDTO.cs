using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.DTO
{
    public class PublisherDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public AuthorDTO Manager { get; set; } = new AuthorDTO();
        public List<AuthorDTO> Authors { get; set; } = new List<AuthorDTO>();
        public List<BookDTO> Books { get; set; } = new List<BookDTO>();

        public string DisplayManager => Manager != null ? $"{Manager.Name} {Manager.Lastname}" : "";
        public int AuthorsCount => Authors?.Count ?? 0;
        public int BooksCount => Books?.Count ?? 0;

        public (bool IsValid, string ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return (false, "Publisher name is required.");

            if (Manager == null)
                return (false, "Publisher must have a manager.");

            var managerValid = Manager.Validate();
            if (!managerValid.IsValid)
                return managerValid;

            if (Authors == null || Authors.Count == 0)
                return (false, "Publisher must have at least one author.");

            foreach (var author in Authors)
            {
                var authorValid = author.Validate();
                if (!authorValid.IsValid)
                    return authorValid;
            }

            return (true, "");
        }

        public Publisher ToModel()
        {
            return new Publisher
            {
                Id = this.Id,
                Name = this.Name,
                Manager = this.Manager.ToModel(),
                Authors = this.Authors.Select(a => a.ToModel()).ToList() ?? new List<Author>(),
                Books = this.Books?.Select(b => b.ToModel()).ToList() ?? new List<Book>()
            };
        }


        public static PublisherDTO FromModel(Publisher publisher)
        {
         

            return new PublisherDTO
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Manager = AuthorDTO.FromModel(publisher.Manager),
                Authors = publisher.Authors.Select(AuthorDTO.FromModel).ToList() ?? new List<AuthorDTO>(),
                Books = publisher.Books?.Select(BookDTO.FromModel).ToList() ?? new List<BookDTO>()
            };
        }
    }
}

