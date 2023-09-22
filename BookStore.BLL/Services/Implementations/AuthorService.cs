using BookStore.BLL.Exceptions;
using BookStore.BLL.Services.Interfaces;
using BookStore.DAL.Repositories.Interfaces;
using BookStore.DAL.UnitOfWork.Interfaces;
using BookStore.Models;

namespace BookStore.BLL.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Author>> GetAuthorsAsync()
        {
            return await _unitOfWork.GetRepository<IAuthorRepository>().GetAllAsync();
        }

        public async Task<Author> GetAuthorAsync(int id)
        {
            return await GetAuthorByIdAsync(id);
        }

        public async Task AddAuthorAsync(Author author)
        {
            if (author == null)
            {
                throw new BookStoreNotFoundException(nameof(author));
            }

            await _unitOfWork.GetRepository<IAuthorRepository>().AddAsync(author);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            await GetAuthorByIdAsync(author.Id);

            _unitOfWork.GetRepository<IAuthorRepository>().Update(author);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAuthorAsync(int id)
        {
            _ = await GetAuthorByIdAsync(id);

            await _unitOfWork.GetRepository<IAuthorRepository>().RemoveAsync(id);
            await _unitOfWork.SaveAsync();
        }

        private async Task<Author> GetAuthorByIdAsync(int id)
        {
            var author = await _unitOfWork.GetRepository<IAuthorRepository>().GetByIdAsync(id);

            return author ?? throw new BookStoreNotFoundException($"Author with ID {id} not found.");
        }
    }
}
