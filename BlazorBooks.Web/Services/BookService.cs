using BlazorBooks.Shared.Dtos;
using BlazorBooks.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorBooks.Web.Services
{
    public class BookService
    {
        private readonly IDbContextFactory<BookContext> _dbContextFactory;

        public BookService(IDbContextFactory<BookContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<GenreDto[]> GetGenresAsync(bool topOnly)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var query = context.Genres.AsQueryable();
            if (topOnly)
            {
                query = query.Where(g => g.IsTop);
            }

            var genres = await query.Select(g => new GenreDto(g.Name, g.Slug))
                .ToArrayAsync();

            return genres;
        }

        public async Task<PagedResult<BookListDto>> GetBooksAsync(int pageNo, int pageSize, string? genreSlug = null)
        {

            using var context = _dbContextFactory.CreateDbContext();

            var query = context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(genreSlug))
            {
                query = context.Genres
                    .Where(g => g.Slug == genreSlug)
                    .SelectMany(g => g.GenreBooks)
                    .Select(gb => gb.Book);
            }

            var totalCount = await query.CountAsync();
            var books = await query
                .OrderByDescending(b => b.Id)
                .Skip((pageNo - 1)* pageSize)
                .Take(pageSize)
                .Select(b=> new BookListDto(b.Id, b.Title, b.Image, new AuthorDto(b.Author.Name, b.Author.Slug)))
                .ToArrayAsync();

            return new PagedResult<BookListDto> ( books, totalCount );
        }

        public async Task<BookListDto[]> GetPopularBooksAsync(int count, string? genreSlug = null)
        {

        }

        public async Task<BookDetailDto> GetBookAsync(int bookId)
        {

        }

        public async Task<BookListDto[]> GetSimilarBooksAsync(int bookId, int count)
        {

        }

        public async Task<PagedResult<BookListDto>> GetBooksByAuthorAsync(int pageNo, int pageSize, string authorSlug)
        {

        }
    }
}
