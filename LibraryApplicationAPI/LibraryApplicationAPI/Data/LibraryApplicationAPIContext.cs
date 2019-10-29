using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryApplicationAPI.Models;

namespace LibraryApplicationAPI.Models
{
    public class LibraryApplicationAPIContext : DbContext
    {
        public LibraryApplicationAPIContext (DbContextOptions<LibraryApplicationAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<UserAccount> UserAccounts { get; set; }
    }
}
