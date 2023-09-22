using BookStore.DAL.Data;
using BookStore.DAL.Helpers;
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
    public class BookRepositoryTests
    {
        private Mock<IBookRepository> _bookRepository;
        private BookstoreContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BookstoreContext>()
                .UseInMemoryDatabase(databaseName: "BookstoreDb")
                .Options;

            _context = new BookstoreContext(options);
            _bookRepository = new Mock<IBookRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task AddBook_ShouldAddBookToRepository()
        {
            // Arrange
            var book = new Book() { Title = "The Catcher in the Rye", AuthorId = 1, GenreId = 1 };
            _bookRepository.Setup(r => r.AddAsync(book))
                .Callback(() => _context.AddAsync(book).AsTask())
                .Returns(Task.CompletedTask);

            // Act
            await _bookRepository.Object.AddAsync(book);
            await _context.SaveChangesAsync();
            var allBooks = _context.Books.ToList();
            var savedBook = await _context.Books.FindAsync(book.Id);

            // Assert
            Assert.That(savedBook, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(allBooks, Has.Count.EqualTo(1));
                Assert.That(savedBook.Title, Is.EqualTo(book.Title));
                Assert.That(savedBook.AuthorId, Is.EqualTo(book.AuthorId));
                Assert.That(savedBook.GenreId, Is.EqualTo(book.GenreId));
            });
            _bookRepository.Verify(r => r.AddAsync(book), Times.Once());
        }

        [Test]
        public async Task Update_ShouldUpdateBookInRepository()
        {
            // Arrange
            var book = new Book() { Title = "The Catcher in the Rye", AuthorId = 1, GenreId = 1 };
            await _context.AddAsync(book);
            await _context.SaveChangesAsync();

            book.Title = "Updated Title";
            _bookRepository.Setup(r => r.Update(book))
                .Callback(() => _context.Entry(book).State = EntityState.Modified);

            // Act
            _bookRepository.Object.Update(book);
            await _context.SaveChangesAsync();
            var updatedBook = await _context.Books.FindAsync(book.Id);

            // Assert
            Assert.That(updatedBook, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(updatedBook.Title, Is.EqualTo("Updated Title"));
                Assert.That(updatedBook.AuthorId, Is.EqualTo(book.AuthorId));
                Assert.That(updatedBook.GenreId, Is.EqualTo(book.GenreId));
            });
            _bookRepository.Verify(r => r.Update(book), Times.Once());
        }

        [Test]
        public async Task RemoveAsync_ShouldRemoveBookFromRepository()
        {
            // Arrange
            var book = new Book() { Title = "The Catcher in the Rye", AuthorId = 1, GenreId = 1 };
            await _context.AddAsync(book);
            await _context.SaveChangesAsync();

            var bookId = book.Id;
            _bookRepository.Setup(r => r.RemoveAsync(bookId))
                .Callback(async () =>
                {
                    var entity = await _context.Books.FindAsync(bookId);
                    if (entity != null)
                    {
                        _context.Remove(entity);
                    }
                });

            // Act
            await _bookRepository.Object.RemoveAsync(bookId);
            await _context.SaveChangesAsync();
            var deletedBook = await _context.Books.FindAsync(bookId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(deletedBook, Is.Null);
            });
            _bookRepository.Verify(r => r.RemoveAsync(bookId), Times.Once());
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllBooksFromRepository()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book() { Title = "The Catcher in the Rye", AuthorId = 1, GenreId = 1 },
                new Book() { Title = "To Kill a Mockingbird", AuthorId = 2, GenreId = 1 }
            };
            await _context.AddRangeAsync(books);
            await _context.SaveChangesAsync();

            _bookRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_context.Books.ToList());

            // Act
            var result = await _bookRepository.Object.GetAllAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(2));
            });
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnBookWithGivenIdFromRepository()
        {
            // Arrange
            var book = new Book() { Title = "The Catcher in the Rye", AuthorId = 1, GenreId = 1 };
            await _context.AddAsync(book);
            await _context.SaveChangesAsync();

            var bookId = book.Id;
            _bookRepository.Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(() => _context.Books.Find(bookId));

            // Act
            var result = await _bookRepository.Object.GetByIdAsync(bookId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Title, Is.EqualTo(book.Title));
                Assert.That(result.Id, Is.EqualTo(book.Id));
                Assert.That(result.AuthorId, Is.EqualTo(book.AuthorId));
                Assert.That(result.GenreId, Is.EqualTo(book.GenreId));
            });
        }

        [Test]
        public async Task GetFilteredBooks_ShouldReturnFilteredBooksFromRepository()
        {
            // Arrange
            var author1 = new Author() { Name = "John Doe" };
            var author2 = new Author() { Name = "Jane Doe" };
            var genre1 = new Genre() { Name = "Fiction" };
            var genre2 = new Genre() { Name = "Non-Fiction" };
            var books = new List<Book>
                {
                    new Book() { Title = "Fiction Book 1", Author = author1, Genre = genre1 },
                    new Book() { Title = "Fiction Book 2", Author = author2, Genre = genre1 },
                    new Book() { Title = "Non-Fiction Book 1", Author = author1, Genre = genre2 }
                };
            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();

            _bookRepository.Setup(r => r.GetFilteredBooks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string, string>(async (title, authorName, genreName) =>
                {
                    IQueryable<Book> query = _context.Books
                        .Include(book => book.Author)
                        .Include(book => book.Genre);

                    query = BookFilter.FilterByTitle(query, title);
                    query = BookFilter.FilterByAuthorName(query, authorName);
                    query = BookFilter.FilterByGenreName(query, genreName);

                    return await query.ToListAsync();
                });

            // Act
            var booksByTitle = await _bookRepository.Object.GetFilteredBooks("Fiction Book", null, null);
            var booksByAuthorName = await _bookRepository.Object.GetFilteredBooks(null, "John Doe", null);
            var booksByGenreName = await _bookRepository.Object.GetFilteredBooks(null, null, "Non-Fiction");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(booksByTitle.Count(), Is.EqualTo(3));
                Assert.That(booksByAuthorName.Count(), Is.EqualTo(2));
                Assert.That(booksByGenreName.Count(), Is.EqualTo(1));
            });
        }
    }
}
