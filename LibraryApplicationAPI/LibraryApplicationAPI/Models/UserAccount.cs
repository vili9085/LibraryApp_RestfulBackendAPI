using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApplicationAPI.Models
{
    public class UserAccount
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public List<Book> BooksLoaned { get; set; } 
    }
}
