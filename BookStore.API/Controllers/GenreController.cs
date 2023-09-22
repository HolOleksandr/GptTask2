using BookStore.API.Configurations;
using BookStore.BLL.Services.Interfaces;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExceptionFilter]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenresAsync()
        {
            var genres = await _genreService.GetGenresAsync();
            return Ok(genres);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genre>> GetGenreAsync(int id)
        {
            var genre = await _genreService.GetGenreAsync(id);
            return Ok(genre);
        }

        [HttpPost]
        public async Task<ActionResult> AddGenreAsync([FromBody] Genre genre)
        {
            await _genreService.AddGenreAsync(genre);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateGenreAsync(int id, [FromBody] Genre genre)
        {
            if (id != genre.Id)
            {
                return BadRequest();
            }

            await _genreService.UpdateGenreAsync(genre);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGenreAsync(int id)
        {
            await _genreService.DeleteGenreAsync(id);
            return NoContent();
        }
    }
}
