using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApplicationAPI.Models;

namespace LibraryApplicationAPI.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryApplicationAPIContext _context;

        public BooksController(LibraryApplicationAPIContext context)
        {
            _context = context;

            /*
            if (_context.Books.Count() == 0)
            {
                // Create a new Book if collection is empty,
                // which means you can't delete all Book.
                _context.Books.Add(new Book(1, "Book1", "Author1"));
                _context.SaveChanges();
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> All books </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            return Ok(await _context.Books.ToListAsync());
        }

        /// <summary>
        /// Returns book with param id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Returns true if book with param id exists </returns>
        private bool BookExists(long id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        /// <summary>
        /// Querys the books from the dbcontext and ceeps the ones where the name contains the param searchString
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns> List with books with names that contains param</returns>
        [HttpGet("search/{searchString}")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchForBook(string searchString)
        {
            var books = from m in _context.Books
                        select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Name.Contains(searchString));
            }

            return await books.ToListAsync();
        }

        /// <summary>
        /// Checks if book and user exists and that book is available, adds book to users loaned books and sets book to not available
        /// </summary>
        /// <param name="id"> Id of book to loan</param>
        /// <param name="userAccount"> User that wants to loan book </param>
        /// <returns> returns user </returns>
        [HttpPut("loanbook/{id}")]
        public async Task<IActionResult> LoanBook([FromRoute] long id, [FromBody] UserAccount userAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.UserAccounts.FindAsync(userAccount.Id);
            var book = await _context.Books.FindAsync(id);

            if (user == null || book == null)
            {
                return NotFound();
            }

            if (book.IsAvailable != true)
            {
                return NotFound("Book is not available"); // Not sure what to return
            }

            user.BooksLoaned.Add(book);
            book.IsAvailable = false;

            _context.Entry(book).State = EntityState.Modified;
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        /// <summary>
        /// Checks if book and user exists, removes book from user and sets book as available
        /// </summary>
        /// <param name="id"> Id of book to return</param>
        /// <param name="userAccount"> User that returns book </param>
        /// <returns></returns>
        [HttpPut("returnbook/{id}")]
        public async Task<IActionResult> ReturnBook([FromRoute] long id, [FromBody] UserAccount userAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.UserAccounts.FindAsync(userAccount.Id);
            var book = await _context.Books.FindAsync(id);

            if (user == null || book == null)
            {
                return NotFound();
            }

            user.BooksLoaned.Remove(book);
            book.IsAvailable = true;

            _context.Entry(book).State = EntityState.Modified;
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        #region not necessary
        /// <summary>
        /// Updates book entry with properties in param book. Param id and param book id must match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook([FromRoute] long id, [FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                return BadRequest();
            }

            // Marks all the properties of entry as modified
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Adds new book to context
        /// </summary>
        /// <param name="book"></param>
        /// <returns> If successful returns a 201 status code, the new book in the body and URI to get the book </returns>
        [HttpPost]
        public async Task<IActionResult> PostBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        
        /// <summary>
        /// Deletes book with param id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return Ok(book);
        }
        #endregion

    }
}