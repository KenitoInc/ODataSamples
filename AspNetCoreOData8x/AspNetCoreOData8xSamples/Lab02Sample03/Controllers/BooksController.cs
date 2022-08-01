﻿using Lab02Sample03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData;
using System.Linq;

namespace Lab02Sample03.Controllers
{
    public class BooksController : ODataController
    {
        #region CRUD operations
        // Get ~/Books
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1, PageSize = 100, MaxExpansionDepth = 5)]
        public IActionResult Get()
        {
            return Ok(DataSource.Instance.Books);
        }

        // GET ~/Books(1)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var book = DataSource.Instance.Books.FirstOrDefault(b => b.ID == key);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // POST ~/Books
        [EnableQuery]
        public IActionResult Post([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DataSource.Instance.Books.Add(book);

            return Created(book);
        }

        // PUT ~/Books(1)
        [EnableQuery]
        public IActionResult Put(int key, [FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = DataSource.Instance.Books.Where(b => b.ID == key).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // PATCH ~/Books(1)
        [EnableQuery]
        public IActionResult Patch([FromODataUri] int key, Delta<Book> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = DataSource.Instance.Books.FirstOrDefault(b => b.ID == key);
            if (book == null)
            {
                return NotFound($"Book with ID: {key} not found");
            }

            return Ok(book);
        }

        // DELETE ~/Books(1)
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var book = DataSource.Instance.Books.FirstOrDefault(b => b.ID == key);
            if (book == null)
            {
                ODataError error = new ODataError
                {
                    Message = $"Book with ID: {key} not found",
                    ErrorCode = "404"
                };

                return NotFound(error);
            }

            return Ok(book);
        }
        #endregion

        #region Non-Contained Navigation
        [EnableQuery]
        public IActionResult GetMainAuthor([FromODataUri] int key)
        {
            var mainAuthor = DataSource.Instance.Books.Where(m => m.ID == key).Select(m => m.MainAuthor).FirstOrDefault();

            if (mainAuthor == null)
            {
                return NotFound();
            }

            return Ok(mainAuthor);
        }

        // GET ~/Books(1)/Authors
        // Authors is a navigation property.
        [EnableQuery]
        public IActionResult GetAuthors([FromODataUri] int key)
        {
            var authors = DataSource.Instance.Books.AsQueryable<Book>().Where(b => b.ID == key).Select(b => b.Authors);
            return Ok(authors);
        }
        #endregion

        #region Contained Navigation
        //Contained entities don't have their own controller; the action is defined in the containing entity set controller.

        // GET ~/Books(1)/Translators
        [EnableQuery]
        public IActionResult GetTranslators(int key)
        {
            var translators = DataSource.Instance.Books.Single(b => b.ID == key).Translators;
            return Ok(translators);
        }

        // GET ~/Books(1)/Translators(100001)
        [EnableQuery]
        [HttpGet("odata/Books({bookId})/Translators({translatorId})")]
        public IActionResult GetSingleTranslator(int bookId, int translatorId)
        {
            var translators = DataSource.Instance.Books.FirstOrDefault(b => b.ID == bookId).Translators;
            var translator = translators.FirstOrDefault(t => t.TranslatorID == translatorId);

            if(translator == null)
            {
                return NotFound();
            }

            return Ok(translator);
        }

        // PUT ~/Books(1)/Translators(100001)
        [HttpPut("odata/Books({bookId})/Translators({translatorId})")]
        public IActionResult PutToTranslator(int bookId, int translatorId, [FromBody] Translator translator)
        {
            var book = DataSource.Instance.Books.Single(b => b.ID == bookId);
            var originalTranslator = book.Translators.Single(t => t.TranslatorID == translatorId);
            originalTranslator.TranslatorName = translator.TranslatorName;
            return Ok(translator);
        }

        // DELETE ~/Books(1)/Translators(100001)
        [HttpDelete("odata/Books({bookId})/Translators({TranslatorID})")]
        public IActionResult DeleteTranslatorFromBook(int bookId, int translatorId)
        {
            var book = DataSource.Instance.Books.Single(b => b.ID == bookId);
            var originalTranslator = book.Translators.Single(t => t.TranslatorID == translatorId);
            if (book.Translators.Remove(originalTranslator))
            {
                return Ok(originalTranslator);
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion

        #region Actions and Functions
        // GET /Books/MostRecent()
        // This is a bound function. It's bound to the entity set Books.
        [HttpGet("odata/Books/MostRecent()")]
        public IActionResult MostRecent()
        {
            var product = DataSource.Instance.Books.Max(x => x.ID);
            return Ok(product);
        }

        // POST /Books(1)/Rate
        // Body has { Rating: 7 }
        // This is bound Action. The action is bound to the Books entity set.
        [HttpPost("odata/Books({key})/Rate")]
        public IActionResult Rate([FromODataUri] int key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int rating = (int)parameters["Rating"];

            if (rating < 0)
            {
                return BadRequest();
            }

            return Ok(new BookRating() { BookID = key, Rating = rating });
        }

        // GET ReturnAllForKidsBooks()
        // This is an unbound Function.
        [HttpGet("odata/ReturnAllForKidsBooks")]
        public IActionResult ReturnAllForKidsBooks()
        {
            var forKidsBooks = DataSource.Instance.Books.Where(m => m.ForKids == true);
            return Ok(forKidsBooks);
        }
        #endregion
    }
}
