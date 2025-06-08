using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.Contracts;
using Repositories.EFCore;

namespace BookStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepositoryManager _manager;

        public BooksController(IRepositoryManager repositoryManager)
        {
            _manager = repositoryManager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _manager.Book.GetAllBooks(false);
                return Ok(books);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)
        {
            try
            {
                var books = _manager
                               .Book
                               .GetOneBookById(id, false);
                if (books is null)
                    return NotFound();
                return Ok(books);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            try
            {

                if (book is null)
                    return BadRequest();

                _manager.Book.CreateOneBook(book);
                _manager.Save();

                return StatusCode(201, book);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id,
            [FromBody] Book book)
        {
            try
            {
                //check book
                var entity = _manager
                    .Book
                    .GetOneBookById(id, true);

                if (entity is null)
                    return NotFound();

                // check id
                if (id != book.Id)
                    return BadRequest();

                entity.Title = book.Title;
                entity.Price = book.Price;

                _manager.Save();

                return Ok(book);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteAllBooks([FromRoute(Name = "id")] int id)
        {
            try
            {

                var entity = _manager
                    .Book
                   .GetOneBookById(id, false);

                if (entity is null)
                    return NotFound(new
                    {
                        statusCode = 404,
                        message = $"Book with id {id} not found"
                    });

                _manager.Book.DeleteOneBook(entity);
                _manager.Save();

                return NoContent();

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            try
            {
                // check entity
                var entity = _manager
                    .Book
                    .GetOneBookById(id, true);
                if (entity is null)
                    return NotFound(); //404
                bookPatch.ApplyTo(entity);
                _manager.Book.Update(entity);
                return NoContent(); //204
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
