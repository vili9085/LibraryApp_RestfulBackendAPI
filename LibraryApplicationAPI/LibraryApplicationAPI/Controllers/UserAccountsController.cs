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
    [Route("api/useraccounts")]
    [ApiController]
    public class UserAccountsController : ControllerBase
    {
        private readonly LibraryApplicationAPIContext _context;

        public UserAccountsController(LibraryApplicationAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates user entry with properties in param newUser. Param id and param newUser id must match.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newUser"></param>
        /// <returns></returns>
        [HttpPut("changepassword/{id}")]
        public async Task<IActionResult> ChangePassword([FromRoute] string id, [FromBody] UserAccountDTO newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != newUser.Id)
            {
                return BadRequest();
            }

            var user = await _context.UserAccounts.FindAsync(id);
            user.Password = newUser.Password;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAccountExists(id))
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
        /// Adds new UserAccount to context
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostUserAccount([FromBody] UserAccount userAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserAccounts.Add(userAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserAccountById), new { id = userAccount.Id }, userAccount);
        }

        private bool UserAccountExists(string id)
        {
            return _context.UserAccounts.Any(e => e.Id == id);
        }

        /// <summary>
        /// Checks if param userInfo matches existing user
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [HttpGet("login")]
        public async Task<IActionResult> LogIn([FromBody] UserAccountDTO userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.UserAccounts.FindAsync(userInfo.Id);

            if (user == null)
            {
                return NotFound("User id not found");
            }
            else if (user.Password != userInfo.Password)
            {
                return NotFound("Wrong password");
            }
            
            return Ok();

        }

        #region not necessary

        /// <summary>
        /// 
        /// </summary>
        /// <returns> Returns all users</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAccount>>> GetUserAccounts()
        {
            return Ok(await _context.UserAccounts.ToListAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns user with param id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAccountById([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userAccount = await _context.UserAccounts.FindAsync(id);

            if (userAccount == null)
            {
                return NotFound();
            }

            return Ok(userAccount);
        }


        /// <summary>
        /// Removes UserAccount with param id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAccount([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }

            _context.UserAccounts.Remove(userAccount);
            await _context.SaveChangesAsync();

            return Ok(userAccount);
        }

        #endregion

    }
}