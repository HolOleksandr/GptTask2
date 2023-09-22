using BookStore.DAL.Data;
using BookStore.DAL.Repositories.Interfaces;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BookStore.UnitTests.Repositories
{
    public class GenreRepositoryTests
    {
        private Mock<IGenreRepository> _genreRepository;
        private BookstoreContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BookstoreContext>()
                .UseInMemoryDatabase(databaseName: "BookstoreDb")
                .Options;

            _context = new BookstoreContext(options);
            _genreRepository = new Mock<IGenreRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task AddGenre_ShouldAddGenreToRepository()
        {
            // Arrange
            var genre = new Genre() { Name = "Fiction" };
            _genreRepository.Setup(r => r.AddAsync(genre))
                .Callback(() => _context.AddAsync(genre).AsTask())
                .Returns(Task.CompletedTask);

            // Act
            await _genreRepository.Object.AddAsync(genre);
            await _context.SaveChangesAsync();
            var allGenres = _context.Genres.ToList();
            var savedGenre = await _context.Genres.FindAsync(genre.Id);

            // Assert
            Assert.That(savedGenre, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(allGenres, Has.Count.EqualTo(1));
                Assert.That(savedGenre.Name, Is.EqualTo(genre.Name));
            });
            _genreRepository.Verify(r => r.AddAsync(genre), Times.Once());
        }

        [Test]
        public async Task Update_ShouldUpdateGenreInRepository()
        {
            // Arrange
            var genre = new Genre() { Name = "Fiction" };
            await _context.AddAsync(genre);
            await _context.SaveChangesAsync();

            genre.Name = "Mystery";
            _ = _genreRepository.Setup(r => r.Update(genre))
                .Callback(() => _context.Entry(genre).State = EntityState.Modified);

            // Act
            _genreRepository.Object.Update(genre);
            await _context.SaveChangesAsync();
            var updatedGenre = await _context.Genres.FindAsync(genre.Id);

            // Assert
            Assert.That(updatedGenre, Is.Not.Null);
            Assert.That(updatedGenre.Name, Is.EqualTo("Mystery"));
            _genreRepository.Verify(r => r.Update(genre), Times.Once());
        }

        [Test]
        public async Task RemoveAsync_ShouldRemoveGenreFromRepository()
        {
            // Arrange
            var genre = new Genre() { Name = "Fiction" };
            await _context.AddAsync(genre);
            await _context.SaveChangesAsync();

            var genreId = genre.Id;
            _ = _genreRepository.Setup(r => r.RemoveAsync(genreId))
                .Callback(async () =>
                {
                    var entity = await _context.Genres.FindAsync(genreId);
                    if (entity != null)
                    {
                        _ = _context.Genres.Remove(entity);
                    }
                });

            // Act
            await _genreRepository.Object.RemoveAsync(genreId);
            await _context.SaveChangesAsync();
            var deletedGenre = await _context.Genres.FindAsync(genreId);

            // Assert
            Assert.That(deletedGenre, Is.Null);
            _genreRepository.Verify(r => r.RemoveAsync(genreId), Times.Once());
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllGenresFromRepository()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre() { Name = "Fiction" },
                new Genre() { Name = "Mystery" }
            };
            await _context.AddRangeAsync(genres);
            await _context.SaveChangesAsync();

            _ = _genreRepository.Setup(r => r.GetAllAsync())
                .Returns(async () => await _context.Genres.ToListAsync());

            // Act
            var result = await _genreRepository.Object.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnGenreWithGivenIdFromRepository()
        {
            // Arrange
            var genre = new Genre() { Name = "Fiction" };
            await _context.AddAsync(genre);
            await _context.SaveChangesAsync();

            var genreId = genre.Id;
            _ = _genreRepository.Setup(r => r.GetByIdAsync(genreId))
                .Returns(async () => await _context.Genres.FindAsync(genreId));

            // Act
            var result = await _genreRepository.Object.GetByIdAsync(genreId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo(genre.Name));
                Assert.That(result.Id, Is.EqualTo(genre.Id));
            });
        }
    }
}
