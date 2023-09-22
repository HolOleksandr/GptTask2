using BookStore.API.Configurations;
using BookStore.BLL.Services.Interfaces;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExceptionFilter]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthorsAsync()
        {
            var authors = await _authorService.GetAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> GetAuthorAsync(int id)
        {
            var author = await _authorService.GetAuthorAsync(id);
            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult> AddAuthorAsync([FromBody] Author author)
        {
            await _authorService.AddAuthorAsync(author);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthorAsync(int id, [FromBody] Author author)
        {
            if (id != author.Id)
            {
                return BadRequest();
            }

            await _authorService.UpdateAuthorAsync(author);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthorAsync(int id)
        {
            await _authorService.DeleteAuthorAsync(id);
            return NoContent();
        }
    }
}
