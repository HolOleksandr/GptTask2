using BookStore.DAL.Data;
using BookStore.DAL.Repositories.Interfaces;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.UnitTests.Repositories
{
    public class AuthorRepositoryTests
    {
        private Mock<IAuthorRepository> _authorRepository;
        private BookstoreContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BookstoreContext>()
                .UseInMemoryDatabase(databaseName: "BookstoreDb")
                .Options;

            _context = new BookstoreContext(options);
            _authorRepository = new Mock<IAuthorRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task AddAuthor_ShouldAddAuthorToRepository()
        {
            // Arrange
            var author = new Author() { Name = "John Doe" };
            _authorRepository.Setup(r => r.AddAsync(author))
                .Callback(() => _context.AddAsync(author).AsTask())
                .Returns(Task.CompletedTask);

            // Act
            await _authorRepository.Object.AddAsync(author);
            await _context.SaveChangesAsync();
            var allAuthors = _context.Authors.ToList();
            var savedAuthor = await _context.Authors.FindAsync(author.Id);

            // Assert
            Assert.That(savedAuthor, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(allAuthors, Has.Count.EqualTo(1));
                Assert.That(savedAuthor.Name, Is.EqualTo(author.Name));
            });
            _authorRepository.Verify(r => r.AddAsync(author), Times.Once());
        }

        [Test]
        public async Task Update_ShouldUpdateAuthorInRepository()
        {
            // Arrange
            var author = new Author() { Name = "John Doe" };
            await _context.AddAsync(author);
            await _context.SaveChangesAsync();

            author.Name = "Jane Doe";
            _ = _authorRepository.Setup(r => r.Update(author))
                .Callback(() => _context.Entry(author).State = EntityState.Modified);

            // Act
            _authorRepository.Object.Update(author);
            await _context.SaveChangesAsync();
            var updatedAuthor = await _context.Authors.FindAsync(author.Id);

            // Assert
            Assert.That(updatedAuthor, Is.Not.Null);
            Assert.That(updatedAuthor.Name, Is.EqualTo("Jane Doe"));
            _authorRepository.Verify(r => r.Update(author), Times.Once());
        }

        [Test]
        public async Task RemoveAsync_ShouldRemoveAuthorFromRepository()
        {
            // Arrange
            var author = new Author() { Name = "John Doe" };
            await _context.AddAsync(author);
            await _context.SaveChangesAsync();

            var authorId = author.Id;
            _ = _authorRepository.Setup(r => r.RemoveAsync(authorId))
                .Callback(async () =>
                {
                    var entity = await _context.Authors.FindAsync(authorId);
                    if (entity != null)
                    {
                        _ = _context.Authors.Remove(entity);
                    }
                });

            // Act
            await _authorRepository.Object.RemoveAsync(authorId);
            await _context.SaveChangesAsync();
            var deletedAuthor = await _context.Authors.FindAsync(authorId);

            // Assert
            Assert.That(deletedAuthor, Is.Null);
            _authorRepository.Verify(r => r.RemoveAsync(authorId), Times.Once());
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllAuthorsFromRepository()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author() { Name = "John Doe" },
                new Author() { Name = "Jane Doe" }
            };
            await _context.AddRangeAsync(authors);
            await _context.SaveChangesAsync();

            _ = _authorRepository.Setup(r => r.GetAllAsync())
                .Returns(async () => await _context.Authors.ToListAsync());

            // Act
            var result = await _authorRepository.Object.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnAuthorWithGivenIdFromRepository()
        {
            // Arrange
            var author = new Author() { Name = "John Doe" };
            await _context.AddAsync(author);
            await _context.SaveChangesAsync();

            var authorId = author.Id;
            _ = _authorRepository.Setup(r => r.GetByIdAsync(authorId))
                .Returns(async () => await _context.Authors.FindAsync(authorId));

            // Act
            var result = await _authorRepository.Object.GetByIdAsync(authorId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.EqualTo(author.Name));
                Assert.That(result.Id, Is.EqualTo(author.Id));
            });
        }
    }
}
