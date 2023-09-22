using BookStore.BLL.Exceptions;
using BookStore.BLL.Services.Implementations;
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
    public class AuthorServiceTests
    {
        private AuthorService _authorService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IAuthorRepository> _authorRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _authorRepositoryMock = new Mock<IAuthorRepository>();
            _unitOfWorkMock.Setup(uow => uow.GetRepository<IAuthorRepository>()).Returns(_authorRepositoryMock.Object);

            _authorService = new AuthorService(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task GetAuthorById_AuthorExists_ReturnsAuthor()
        {
            // Arrange
            var authorId = 1;
            var expectedAuthor = new Author { Id = authorId, Name = "John Doe" };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(expectedAuthor);

            // Act
            var result = await _authorService.GetAuthorAsync(authorId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expectedAuthor));
        }

        [Test]
        public void GetAuthorById_AuthorDoesNotExist_ThrowsAuthorNotFoundException()
        {
            // Arrange
            var authorId = 1;
            _ = _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).Returns(Task.FromResult<Author?>(null));

            // Act & Assert
            Assert.ThrowsAsync<BookStoreNotFoundException>(() => _authorService.GetAuthorAsync(authorId));
        }

        [Test]
        public async Task GetAllAuthors_ReturnsAuthorList()
        {
            // Arrange
            var expectedAuthors = new List<Author>
            {
                new Author { Id = 1, Name = "John Doe" },
                new Author { Id = 2, Name = "Jane Doe" },
            };
            _authorRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedAuthors);

            // Act
            var result = await _authorService.GetAuthorsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expectedAuthors));
        }

        [Test]
        public async Task AddAuthor_AuthorIsValid_ReturnsAddedAuthor()
        {
            // Arrange
            var newAuthor = new Author { Name = "John Doe" };
            var authors = new List<Author>();

            _authorRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Author>())).Callback<Author>((a) => authors.Add(a));
            _authorRepositoryMock.Setup(repo => repo.GetAllAsync()).Returns(() => Task.FromResult(authors as IEnumerable<Author>));

            // Act
            var authorsCount = (await _authorService.GetAuthorsAsync()).Count();
            await _authorService.AddAuthorAsync(newAuthor);
            var resultCount = (await _authorService.GetAuthorsAsync()).Count();

            // Assert
            Assert.That(resultCount, Is.EqualTo(authorsCount + 1));
        }

        [Test]
        public async Task UpdateAuthor_AuthorExists_ReturnsUpdatedAuthor()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { Id = authorId, Name = "John Doe" };
            var updatedAuthor = new Author { Id = authorId, Name = "Updated John Doe" };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(author);
            _authorRepositoryMock.Setup(repo => repo.Update(It.IsAny<Author>())).Callback<Author>((a) => author.Name = a.Name);

            // Act
            await _authorService.UpdateAuthorAsync(updatedAuthor);
            var result = await _authorService.GetAuthorAsync(authorId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(updatedAuthor.Name));
        }

        [Test]
        public void UpdateAuthor_AuthorDoesNotExist_ThrowsAuthorNotFoundException()
        {
            // Arrange
            var authorId = 1;
            var updatedAuthor = new Author { Id = authorId, Name = "Updated John Doe" };
            _ = _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).Returns(Task.FromResult<Author?>(null));

            // Act & Assert
            Assert.ThrowsAsync<BookStoreNotFoundException>(() => _authorService.UpdateAuthorAsync(updatedAuthor));
        }

        [Test]
        public async Task DeleteAuthor_AuthorExists_RemovesAuthor()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { Id = authorId, Name = "John Doe" };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).ReturnsAsync(author);
            _authorRepositoryMock.Setup(repo => repo.RemoveAsync(authorId)).Returns(Task.CompletedTask);

            // Act
            await _authorService.DeleteAuthorAsync(authorId);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.RemoveAsync(authorId), Times.Once);
        }

        [Test]
        public void DeleteAuthor_AuthorDoesNotExist_ThrowsAuthorNotFoundException()
        {
            // Arrange
            var authorId = 1;
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(authorId)).Returns(Task.FromResult<Author?>(null));

            // Act & Assert
            Assert.ThrowsAsync<BookStoreNotFoundException>(async () => await _authorService.DeleteAuthorAsync(authorId));
        }
    }
}
