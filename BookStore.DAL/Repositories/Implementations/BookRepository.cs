using BookStore.DAL.Data;
using BookStore.DAL.Helpers;
using BookStore.DAL.Repositories.Interfaces;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DAL.Repositories.Implementations
{
    public class BookRepository : Repository<Book>, IBookRepository
    {

        public BookRepository(BookstoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Book>> GetFilteredBooks(string? title, string? authorName, string? genreName)
        {
            IQueryable<Book> query = _context.Books
                .Include(book => book.Author)
                .Include(book => book.Genre);

            query = BookFilter.FilterByTitle(query, title);
            query = BookFilter.FilterByAuthorName(query, authorName);
            query = BookFilter.FilterByGenreName(query, genreName);

            return await query.ToListAsync();
        }
    }
}
