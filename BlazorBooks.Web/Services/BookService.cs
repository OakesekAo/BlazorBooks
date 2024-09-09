using BlazorBooks.Shared.Dtos;
using BlazorBooks.Shared.Interfaces;
using BlazorBooks.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorBooks.Web.Services
{


    public class BookService : IBookService
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
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookListDto(b.Id, b.Title, b.Image, new AuthorDto(b.Author.Name, b.Author.Slug)))
                .ToArrayAsync();

            return new PagedResult<BookListDto>(books, totalCount);
        }

        public async Task<BookListDto[]> GetPopularBooksAsync(int count, string? genreSlug = null)
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

            var books = await query
                .Select(b => new BookListDto(b.Id, b.Title, b.Image, new AuthorDto(b.Author.Name, b.Author.Slug)))
                .OrderBy(b => Guid.NewGuid())
                .Take(count)
                .ToArrayAsync();

            if (books.Length < count)
            {
                //we dont have enough{count in this genre
                // add random books
                var alreadyFetchedBookIds = books.Select(b => b.Id);

                var additionalBooks = await context.Books
                    .Where(b => !alreadyFetchedBookIds.Contains(b.Id))
                    .Select(b => new BookListDto(b.Id, b.Title, b.Image, new AuthorDto(b.Author.Name, b.Author.Slug)))
                    .OrderBy(b => Guid.NewGuid())
                    .Take(count - books.Length)
                    .ToArrayAsync();

                books = [.. books, .. additionalBooks];
            }

            return books;
        }

        public async Task<BookDetailDto> GetBookAsync(int bookId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var book = await context.Books.Where(b => b.Id == bookId)
                        .Select(b => new BookDetailDto(b.Id, b.Title, b.Image,
                                    new AuthorDto(b.Author.Name, b.Author.Slug), b.NumPages, b.Format, b.Description,
                                    b.GenreBooks.Select(bg => new GenreDto(bg.Genre.Name, bg.Genre.Slug)).ToArray(),
                                    b.BuyLink
                                ))
                        .FirstOrDefaultAsync();

            return book;

        }

        public async Task<BookListDto[]> GetSimilarBooksAsync(int bookId, int count)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var similarBooks = await context.GenreBooks.Where(gb => gb.BookId == bookId)
                .SelectMany(gb => gb.Genre.GenreBooks)
                .Select(gb => gb.Book)
                .Where(b => b.Id != bookId)// Skip this current book
                .Select(b => new BookListDto(b.Id, b.Title, b.Image, new AuthorDto(b.Author.Name, b.Author.Slug)))
                .OrderBy(b => Guid.NewGuid())//randomize
                .Take(count)
                .ToArrayAsync();

            return similarBooks;

        }

        public async Task<PagedResult<BookListDto>> GetBooksByAuthorAsync(int pageNo, int pageSize, string authorSlug)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var query = context.Books.Where(b => b.Author.Slug == authorSlug);

            var totalCount = await query.CountAsync();
            var books = await query
                .OrderByDescending(b => b.Id)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookListDto(b.Id, b.Title, b.Image, new AuthorDto(b.Author.Name, b.Author.Slug)))
                .ToArrayAsync();

            return new PagedResult<BookListDto>(books, totalCount);

        }
    }
}
