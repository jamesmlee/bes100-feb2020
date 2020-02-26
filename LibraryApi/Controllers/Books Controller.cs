using LibraryApi.Domain;
using LibraryApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class Books_Controller : Controller
    {
        LibraryDataContext Context;

        public Books_Controller(LibraryDataContext context)
        {
            Context = context;
        }

        // mini-put
        [HttpPut("books/{id:int}/numberofpages")]
        public async Task<ActionResult> UpdateNumberOfPages(int id, [FromBody] int newPages)
        {
            var book = await GetBooksInInventory()
                .Where(b => b.Id == id)
                .SingleOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            } else
            {
                book.NumberOfPages = newPages;
                await Context.SaveChangesAsync();
                return NoContent();
            }
        }

        [HttpDelete("books/{id:int}")]
        public async Task<ActionResult> RemoveABook(int id)
        {
            var book = await GetBooksInInventory()
                .Where(b => b.Id == id)
                .SingleOrDefaultAsync();

            if (book != null)
            {
                book.InInventory = false;
                await Context.SaveChangesAsync();
            }
            return NoContent();
        }

        /// <summary>
        /// Add a book to the inventory
        /// </summary>
        /// <param name="bookToAdd">The details of the book to add</param>
        /// <returns></returns>
        [HttpPost("books")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GetABookResponse>> AddABook([FromBody] PostBooksRequest bookToAdd)
        {
            // 1. Validate it
            //    If not valid, send a 400 Bad Request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Add it the database
            var book = new Book
            {
                Title = bookToAdd.Title,
                Author = bookToAdd.Author,
                Genre = bookToAdd.Genre,
                NumberOfPages = bookToAdd.NumberOfPages,
                InInventory = true
            };

            Context.Books.Add(book);
            await Context.SaveChangesAsync();
            var response = new GetABookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                NumberOfPages = book.NumberOfPages
            };

            // 3. Return:
            //    201 Created 
            //    Location header with the URL of the newly created resource (like a birth announcement)
            //    Attach a copy of whatever they would get by following the location header.

            return CreatedAtRoute("books#getabook", new { id = book.Id }, response);
        }

        // Name just needs to be unique
        [HttpGet("books/{id:int}", Name = "books#getabook")]
        public async Task<ActionResult<GetABookResponse>> GetABook(int id)
        {
            var response = await GetBooksInInventory()
                .Where(b => b.Id == id)
                .Select(b => new GetABookResponse
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    NumberOfPages = b.NumberOfPages
                }).SingleOrDefaultAsync();
            // throws an exception if it returns more than 1 result ... nothing is default / null

            if (response == null)
            {
                return NotFound();
            } else
            {
                return Ok(response);
            }
        }

        //[HttpGet("books")]
        //public ActionResult GetAllBooks()
        //{

        //    var response = new GetBooksResponse();
        //    response.Data = Context.Books
        //        .Where(b => b.InInventory)
        //        .Select(b => new BookSummaryItem { Id = b.Id, Title = b.Title, Author = b.Author })
        //        .ToList();
        //    return Ok(response);
        //}

        [HttpGet("books")]
        public async Task<ActionResult<GetBooksResponse>> GetAllBooks([FromQuery] string genre = "all")
        {
            // this leaks too much to the client
            //var response = Context.Books.ToList();
            var response = new GetBooksResponse();

            // refactor ... extract to helper method GetBooksInInventory
            //var data = Context.Books
            //    .Where(b => b.InInventory);
            var data = GetBooksInInventory();

            if (genre != "all")
            {
                data = data.Where(b => b.Genre == genre);
            }

            response.Data = await data.Select(b => new BookSummaryItem { Id = b.Id, Title = b.Title, Author = b.Author })
                .ToListAsync();
            response.Genre = genre;
            return Ok(response);
        }

        private IQueryable<Book> GetBooksInInventory()
        {
            return Context.Books.Where(b => b.InInventory);
        }
    }
}
