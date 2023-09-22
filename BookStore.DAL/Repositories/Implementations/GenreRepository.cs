using BookStore.DAL.Data;
using BookStore.DAL.Repositories.Interfaces;
using BookStore.Models;

namespace BookStore.DAL.Repositories.Implementations
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        public GenreRepository(BookstoreContext context) : base(context)
        {
        }
    }
}
