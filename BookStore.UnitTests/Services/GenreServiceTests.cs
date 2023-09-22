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
    public class GenreServiceTests
    {
        private GenreService _genreService;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IGenreRepository> _genreRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _unitOfWorkMock.Setup(uow => uow.GetRepository<IGenreRepository>()).Returns(_genreRepositoryMock.Object);

            _genreService = new GenreService(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task GetGenreById_GenreExists_ReturnsGenre()
        {
            // Arrange
            var genreId = 1;
            var expectedGenre = new Genre { Id = genreId, Name = "Fiction" };
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).ReturnsAsync(expectedGenre);

            // Act
            var result = await _genreService.GetGenreAsync(genreId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expectedGenre));
        }

        [Test]
        public void GetGenreById_GenreDoesNotExist_ThrowsGenreNotFoundException()
        {
            // Arrange
            var genreId = 1;
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).Returns(Task.FromResult<Genre?>(null));

            // Act & Assert
            Assert.ThrowsAsync<BookStoreNotFoundException>(() => _genreService.GetGenreAsync(genreId));
        }

        [Test]
        public async Task GetAllGenres_ReturnsGenreList()
        {
            // Arrange
            var expectedGenres = new List<Genre>
            {
                new Genre { Id = 1, Name = "Fiction" },
                new Genre { Id = 2, Name = "Non-Fiction" },
            };
            _genreRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedGenres);

            // Act
            var result = await _genreService.GetGenresAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expectedGenres));
        }

        [Test]
        public async Task AddGenre_GenreIsValid_ReturnsAddedGenre()
        {
            // Arrange
            var newGenre = new Genre { Name = "Fiction" };
            var genres = new List<Genre>();

            _genreRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Genre>())).Callback<Genre>((g) => genres.Add(g));
            _genreRepositoryMock.Setup(repo => repo.GetAllAsync()).Returns(() => Task.FromResult(genres as IEnumerable<Genre>));

            // Act
            var genresCount = (await _genreService.GetGenresAsync()).Count();
            await _genreService.AddGenreAsync(newGenre);
            var resultCount = (await _genreService.GetGenresAsync()).Count();

            // Assert
            Assert.That(resultCount, Is.EqualTo(genresCount + 1));
        }

        [Test]
        public async Task UpdateGenre_GenreExists_ReturnsUpdatedGenre()
        {
            // Arrange
            var genreId = 1;
            var genre = new Genre { Id = genreId, Name = "Fiction" };
            var updatedGenre = new Genre { Id = genreId, Name = "Updated Fiction" };
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).ReturnsAsync(genre);
            _genreRepositoryMock.Setup(repo => repo.Update(It.IsAny<Genre>())).Callback<Genre>((g) => genre.Name = g.Name);

            // Act
            await _genreService.UpdateGenreAsync(updatedGenre);
            var result = await _genreService.GetGenreAsync(genreId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(updatedGenre.Name));
        }

        [Test]
        public void UpdateGenre_GenreDoesNotExist_ThrowsGenreNotFoundException()
        {
            // Arrange
            var genreId = 1;
            var updatedGenre = new Genre { Id = genreId, Name = "Updated Fiction" };
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).Returns(Task.FromResult<Genre?>(null));

            // Act & Assert
            Assert.ThrowsAsync<BookStoreNotFoundException>(() => _genreService.UpdateGenreAsync(updatedGenre));
        }

        [Test]
        public async Task DeleteGenre_GenreExists_RemovesGenre()
        {
            // Arrange
            var genreId = 1;
            var genre = new Genre { Id = genreId, Name = "Fiction" };
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).ReturnsAsync(genre);
            _genreRepositoryMock.Setup(repo => repo.RemoveAsync(genreId)).Returns(Task.CompletedTask);

            // Act
            await _genreService.DeleteGenreAsync(genreId);

            // Assert
            _genreRepositoryMock.Verify(repo => repo.RemoveAsync(genreId), Times.Once);
        }

        [Test]
        public void DeleteGenre_GenreDoesNotExist_ThrowsGenreNotFoundException()
        {
            // Arrange
            var genreId = 1;
            _genreRepositoryMock.Setup(repo => repo.GetByIdAsync(genreId)).Returns(Task.FromResult<Genre?>(null));

            // Act & Assert
            Assert.ThrowsAsync<BookStoreNotFoundException>(() => _genreService.DeleteGenreAsync(genreId));
        }
    }
}
