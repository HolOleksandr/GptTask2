using BookStore.BLL.Exceptions;
using BookStore.BLL.Services.Implementations;
using BookStore.BLL.Services.Interfaces;
using BookStore.DAL.Repositories.Interfaces;
using BookStore.DAL.UnitOfWork.Interfaces;
using BookStore.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.UnitTests.Services
{
    public class BookServiceTests
    {
        private BookService _bookService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IBookRepository> _bookRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _unitOfWorkMock.Setup(uow => uow.GetRepository<IBookRepository>()).Returns(_bookRepositoryMock.Object);

            _bookService = new BookService(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task GetBookById_BookExists_ReturnsBook()
        {
            // Arrange
            var bookId = 1;
            var expectedBook = new Book { Id = bookId, Title = "Sample Book" };
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(expectedBook);

            // Act
            var result = await _bookService.GetBookAsync(bookId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expectedBook));
        }

        [Test]
        public void GetBookById_BookDoesNotExist_ThrowsBookNotFoundException()
        {
            // Arrange
            var bookId = 1;
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).Returns(Task.FromResult<Book?>(null));

            // Act & Assert
            Assert.ThrowsAsync<BookStoreNotFoundException>(() => _bookService.GetBookAsync(bookId));
        }

        [Test]
        public async Task GetAllBooks_ReturnsBookList()
        {
            // Arrange
            var expectedBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Sample Book 1" },
                new Book { Id = 2, Title = "Sample Book 2" },
            };
            _bookRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedBooks);

            // Act
            var result = await _bookService.GetBooksAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expectedBooks));
        }

        [Test]
        public async Task AddBook_BookIsValid_ReturnsAddedBook()
        {
            // Arrange
            var newBook = new Book { Title = "New Sample Book" };
            var books = new List<Book>();

            _bookRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Book>())).Callback<Book>((b) => books.Add(b));
            _bookRepositoryMock.Setup(repo => repo.GetAllAsync()).Returns(() => Task.FromResult(books as IEnumerable<Book>));

            // Act
            var booksCount = (await _bookService.GetBooksAsync()).Count();
            await _bookService.AddBookAsync(newBook);
            var resultCount = (await _bookService.GetBooksAsync()).Count();

            // Assert
            Assert.That(resultCount, Is.EqualTo(booksCount + 1));
        }

        [Test]
        public async Task UpdateBook_BookExists_ReturnsUpdatedBook()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { Id = bookId, Title = "Sample Book" };
            var updatedBook = new Book { Id = bookId, Title = "Updated Sample Book" };
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(repo => repo.Update(It.IsAny<Book>())).Callback<Book>((b) => book.Title = b.Title);

            // Act
            await _bookService.UpdateBookAsync(updatedBook);
            var result = await _bookService.GetBookAsync(bookId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(updatedBook.Title));
        }

        [Test]
        public void UpdateBook_BookDoesNotExist_ThrowsBookNotFoundException()
        {
            // Arrange
            var bookId = 1;
            var updatedBook = new Book { Id = bookId, Title = "Updated Sample Book" };
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).Returns(Task.FromResult<Book?>(null));

            // Act & Assert
            Assert.ThrowsAsync<BookStoreNotFoundException>(() => _bookService.UpdateBookAsync(updatedBook));
        }

        [Test]
        public async Task DeleteBook_BookExists_RemovesBook()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { Id = bookId, Title = "Sample Book" };
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(repo => repo.RemoveAsync(bookId)).Returns(Task.CompletedTask);

            // Act
            await _bookService.DeleteBookAsync(bookId);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.RemoveAsync(bookId), Times.Once);
        }

        [Test]
        public void DeleteBook_BookDoesNotExist_ThrowsBookNotFoundException()
        {
            // Arrange
            var bookId = 1;
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId)).Returns(Task.FromResult<Book?>(null));

            // Act & Assert
            Assert.ThrowsAsync<BookStoreNotFoundException>(() => _bookService.DeleteBookAsync(bookId));
        }


        [Test]
        public async Task SearchBooksAsync_GivenTitleAuthorNameAndGenreName_ReturnsFilteredBooks()
        {
            // Arrange
            var author1 = new Author { Name = "John Doe" };
            var author2 = new Author { Name = "Jane Doe" };
            var genre1 = new Genre { Name = "Fiction" };
            var genre2 = new Genre { Name = "Non-Fiction" };

            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Fiction Book 1", Author = author1, Genre = genre1 },
                new Book { Id = 2, Title = "Fiction Book 2", Author = author2, Genre = genre1 },
                new Book { Id = 3, Title = "Non-Fiction Book 1", Author = author1, Genre = genre2 },
            };

            _ = _bookRepositoryMock.Setup(repo => repo.GetFilteredBooks(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
                .Returns<string?, string?, string?>(async (title, authorName, genreName) =>
                {
                    IQueryable<Book> query = books.AsQueryable();

                    query = query.Where(book => string.IsNullOrWhiteSpace(title) || book.Title.Contains(title));
                    query = query.Where(book => authorName == null || book.Author.Name == authorName);
                    query = query.Where(book => genreName == null || book.Genre.Name == genreName);

                    return await Task.FromResult(query.ToList());
                });

            // Act
            var booksByTitle = await _bookService.SearchBooksAsync("Fiction Book", null, null);
            var booksByAuthorName = await _bookService.SearchBooksAsync(null, "John Doe", null);
            var booksByGenreName = await _bookService.SearchBooksAsync(null, null, "Non-Fiction");

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
