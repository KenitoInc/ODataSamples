﻿using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Lab02Sample03.Models;

namespace Lab02Sample03.Controllers
{
    public class AuthorsController : ODataController
    {
        #region CRUD operations
        // Get ~/Authors
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1, PageSize = 100, MaxExpansionDepth = 5)]
        public IActionResult Get()
        {
            return Ok(DataSource.Instance.Authors);
        }

        // GET ~/Authors(10001)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var author = DataSource.Instance.Authors.FirstOrDefault(a => a.ID == key);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // PUT ~/Authors(10001)
        [EnableQuery]
        public IActionResult Put(int key, [FromBody] Author Author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var author = DataSource.Instance.Authors.FirstOrDefault(a => a.ID == key);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // PATCH ~/Authors(10001)
        [EnableQuery]
        public IActionResult Patch([FromODataUri] int key, Delta<Author> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var author = DataSource.Instance.Authors.FirstOrDefault(a => a.ID == key);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // DELETE ~/Authors(10001)
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var author = DataSource.Instance.Authors.FirstOrDefault(a => a.ID == key);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }
        #endregion
    }
}
