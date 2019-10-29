using System.Collections.Generic;

namespace LibraryApplicationAPI.Models
{
    public class Book
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public bool IsAvailable { get; set; }
        public List<Link> Links { get; set; }

        /// <summary>
        /// Creates a book with links to get book and loan book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="author"></param>
        public Book (long id, string name, string author)
        {
            Id = id;
            Name = name;
            Author = author;
            IsAvailable = true;

            Link getLink = new Link()
            {
                Rel = "self",
                Href = "https://localhost:44377/api/books/" + Id,
                Action = "GET",
                Types = new string[] { "text/xml", "application/json" }
            };

            Link loanLink = new Link()
            {
                Rel = "self",
                Href = "https://localhost:44377/api/books/loanbook/" + Id,
                Action = "PUT",
                Types = new string[] { "text/xml", "application/json" }
            };

            Links = new List<Link>()
            {
                getLink,
                loanLink
            };
        }

    }
}
// https://localhost:44377/api/books
