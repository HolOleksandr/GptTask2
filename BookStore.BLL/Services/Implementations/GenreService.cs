using BookStore.BLL.Exceptions;
using BookStore.BLL.Services.Interfaces;
using BookStore.DAL.Repositories.Interfaces;
using BookStore.DAL.UnitOfWork.Interfaces;
using BookStore.Models;

namespace BookStore.BLL.Services.Implementations
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Genre>> GetGenresAsync()
        {
            return await _unitOfWork.GetRepository<IGenreRepository>().GetAllAsync();
        }

        public async Task<Genre> GetGenreAsync(int id)
        {
            return await GetGenreByIdAsync(id);
        }

        public async Task AddGenreAsync(Genre genre)
        {
            if (genre == null)
            {
                throw new BookStoreNotFoundException(nameof(genre));
            }

            await _unitOfWork.GetRepository<IGenreRepository>().AddAsync(genre);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            await GetGenreByIdAsync(genre.Id);

            _unitOfWork.GetRepository<IGenreRepository>().Update(genre);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteGenreAsync(int id)
        {
            _ = await GetGenreByIdAsync(id);

            await _unitOfWork.GetRepository<IGenreRepository>().RemoveAsync(id);
            await _unitOfWork.SaveAsync();
        }

        private async Task<Genre> GetGenreByIdAsync(int id)
        {
            var genre = await _unitOfWork.GetRepository<IGenreRepository>().GetByIdAsync(id);

            return genre ?? throw new BookStoreNotFoundException($"Genre with ID {id} not found.");
        }
    }
}
