using BookStore.BLL.Exceptions;
using BookStore.BLL.Services.Interfaces;
using BookStore.DAL.Repositories.Interfaces;
using BookStore.DAL.UnitOfWork.Interfaces;
using BookStore.Models;

namespace BookStore.BLL.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _unitOfWork.GetRepository<IBookRepository>().GetAllAsync();
        }

        public async Task<Book> GetBookAsync(int id)
        {
            return await GetBookByIdAsync(id);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string? title, string? authorName, string? genreName)
        {
            var books = await _unitOfWork.GetRepository<IBookRepository>()
                .GetFilteredBooks(title, authorName, genreName);

            return books;
        }

        public async Task AddBookAsync(Book book)
        {
            if (book == null)
            {
                throw new BookStoreNotFoundException(nameof(book));
            }

            await _unitOfWork.GetRepository<IBookRepository>().AddAsync(book);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
            await GetBookByIdAsync(book.Id);

            _unitOfWork.GetRepository<IBookRepository>().Update(book);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            _ = await GetBookByIdAsync(id);

            await _unitOfWork.GetRepository<IBookRepository>().RemoveAsync(id);
            await _unitOfWork.SaveAsync();
        }

        private async Task<Book> GetBookByIdAsync(int id)
        {
            var book = await _unitOfWork.GetRepository<IBookRepository>().GetByIdAsync(id);

            return book ?? throw new BookStoreNotFoundException($"Book with ID {id} not found.");
        }
    }
}
