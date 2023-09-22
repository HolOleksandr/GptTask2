using BookStore.Models;

namespace BookStore.DAL.Repositories.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetFilteredBooks(string? title, string? authorName, string? genreName);
    }
}
