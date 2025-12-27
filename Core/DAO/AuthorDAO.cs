using Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{

    public class AuthorDAO
    {
        private readonly List<Author> authors;
        private readonly Storage<Author> storage;

        public AuthorDAO()
        {
            storage = new Storage<Author>("Authors.txt");
            authors = storage.Load();
        }

        //#dodavanje_autora 
        public Author? AddAuthor(Author author)
        {
            if (GetAuthorByIdCard(author.Id_card) != null)
                return null;

            authors.Add(author);
            storage.Save(authors);
            return author;
        }

        // #izmena_autora 
        public Author? UpdateAuthor(Author a)
        {
            Author? oldAuthor = GetAuthorByIdCard(a.Id_card);
            if (oldAuthor == null) 
                return null;

            oldAuthor.Name = a.Name;
            oldAuthor.Lastname = a.Lastname;
            oldAuthor.DateOfBirth = a.DateOfBirth;
            oldAuthor.PhoneNumber = a.PhoneNumber;
            oldAuthor.Email = a.Email;
            oldAuthor.Address = a.Address;
            oldAuthor.ExperienceLength = a.ExperienceLength;
            oldAuthor.BookList = a.BookList;

            storage.Save(authors);
            return oldAuthor;
        }

        // #brisanje_autora  
        public Author? RemoveAuthor(string idCard)
        {
            Author? author = GetAuthorByIdCard(idCard);
            if (author == null) 
                return null;

            authors.Remove(author);
            storage.Save(authors);
            return author;
        }

        //  #prikaz_autora (jednog)
        private Author? GetAuthorByIdCard(string idCard)
        {
            return authors.Find(a => a.Id_card == idCard);
        }

        //  #prikaz_autora (svih)
        public List<Author> GetAllAuthors()
        {
            return new List<Author>(authors);
        }
    }

}
