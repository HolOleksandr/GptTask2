using BookStore.Models;

namespace BookStore.BLL.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<Book> GetBookAsync(int id);
        Task<IEnumerable<Book>> SearchBooksAsync(string? title, string? authorName, string? genreName);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
    }
}
