using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DAL.Helpers
{
    public class BookFilter
    {
        public static IQueryable<Book> FilterByTitle(IQueryable<Book> query, string? title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(book => book.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            return query;
        }

        public static IQueryable<Book> FilterByAuthorName(IQueryable<Book> query, string? authorName)
        {
            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(book => book.Author.Name.Contains(authorName, StringComparison.OrdinalIgnoreCase));
            }

            return query;
        }

        public static IQueryable<Book> FilterByGenreName(IQueryable<Book> query, string? genreName)
        {
            if (!string.IsNullOrEmpty(genreName))
            {
                query = query.Where(book => book.Genre.Name.Contains(genreName, StringComparison.OrdinalIgnoreCase));
            }

            return query;
        }
    }
}
