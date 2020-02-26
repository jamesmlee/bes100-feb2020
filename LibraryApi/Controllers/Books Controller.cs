using LibraryApi.Domain;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("books")]
        public ActionResult GetAllBooks([FromQuery] string genre = "all")
        {
            // this leaks too much to the client
            //var response = Context.Books.ToList();
            var response = new GetBooksResponse();
            var data = Context.Books
                .Where(b => b.InInventory);

            if (genre != "all")
            {
                data = data.Where(b => b.Genre == genre);
            }

            response.Data = data.Select(b => new BookSummaryItem { Id = b.Id, Title = b.Title, Author = b.Author })
                .ToList();
            return Ok(response);
        }
    }
}
