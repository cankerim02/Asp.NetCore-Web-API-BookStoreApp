﻿using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute), Order = 2)]
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParameters bookParameters)
        {

            var pagedResult = await _manager
                .BookService
                .GetAllBooksAsync(bookParameters, false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.books);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
        {
            var books = await _manager
                           .BookService
                           .GetOneBookByIdAsync(id, false);

            return Ok(books);


        }

        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {

            var book = await _manager.BookService.CreateOneBookAsync(bookDto);

            return StatusCode(201, book); //CreatedAtRoute()
        }

        //[ServiceFilter(typeof(LogFilterAttribute),Order =2)]
        [ServiceFilter(typeof(ValidationFilterAttribute),Order =1)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] BookDtoForUpdate bookDto)
        {
            if (bookDto is null)
                return BadRequest(); // 400
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState); // 422
            await _manager.BookService.UpdateOneBookAsync(id, bookDto, false);

            return NoContent(); // 204



        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAllBooksAsync([FromRoute(Name = "id")] int id)
        {

            await _manager.BookService.DeleteOneBookAsync(id, false);

            return NoContent();

        }


        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest(); // 400

            var result = await _manager.BookService.GetOneBookForPatchAsync(id, false);

            bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);

            TryValidateModel(result.bookDtoForUpdate);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState); //422

            await _manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);
            return NoContent(); //204

        }
    }
}
