using BookStore.API.Configurations;
using BookStore.BLL.Services.Interfaces;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExceptionFilter]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksAsync([FromQuery] string title, [FromQuery] string authorName, [FromQuery] string genreName)
        {
            var books = await GetFilteredOrAllBooksAsync(title, authorName, genreName);
            return Ok(books);
        }

        private async Task<IEnumerable<Book>> GetFilteredOrAllBooksAsync(string title, string authorName, string genreName)
        {
            if (IsSearchingStringEmpty(title, authorName, genreName))
            {
                return await _bookService.GetBooksAsync();
            }
            else
            {
                return await _bookService.SearchBooksAsync(title, authorName, genreName);
            }
        }

        private static bool IsSearchingStringEmpty(string? title, string? authorName, string? genreName)
        {
            return string.IsNullOrEmpty(title) || string.IsNullOrEmpty(authorName) || string.IsNullOrEmpty(genreName);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetBookAsync(int id)
        {
            var book = await _bookService.GetBookAsync(id);
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult> AddBookAsync([FromBody] Book book)
        {
            await _bookService.AddBookAsync(book);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBookAsync(int id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            await _bookService.UpdateBookAsync(book);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
    }
}
