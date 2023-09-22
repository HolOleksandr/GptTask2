using BookStore.DAL.Data;
using BookStore.DAL.Repositories.Interfaces;
using BookStore.Models;

namespace BookStore.DAL.Repositories.Implementations
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        public AuthorRepository(BookstoreContext context) : base(context)
        {
        }
    }
}
