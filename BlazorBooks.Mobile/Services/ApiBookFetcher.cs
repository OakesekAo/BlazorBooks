using BlazorBooks.Shared.Dtos;
using BlazorBooks.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBooks.Mobile.Services
{
    public class ApiBookFetcher : IBookService
    {
        private readonly IBookApi _bookApi;
        public ApiBookFetcher(IBookApi bookApi)
        {
            _bookApi = bookApi;   
        }
        public async Task<BookDetailDto> GetBookAsync(int bookId)
        {
            return await _bookApi.GetBookAsync(bookId);
        }

        public async Task<PagedResult<BookListDto>> GetBooksAsync(int pageNo, int pageSize, string? genreSlug = null)
        {
            return await _bookApi.GetBooksAsync(pageNo, pageSize, genreSlug);
        }

        public async Task<PagedResult<BookListDto>> GetBooksByAuthorAsync(int pageNo, int pageSize, string authorSlug)
        {
            return await _bookApi.GetBooksByAuthorAsync(pageNo,pageSize, authorSlug);
        }

        public async Task<GenreDto[]> GetGenresAsync(bool topOnly)
        {
            return await _bookApi.GetGenresAsync(topOnly);
        }

        public async Task<BookListDto[]> GetPopularBooksAsync(int count, string? genreSlug = null)
        {
            return await _bookApi.GetPopularBooksAsync(count, genreSlug);
        }

        public async Task<BookListDto[]> GetSimilarBooksAsync(int bookId, int count)
        {
            return await _bookApi.GetSimilarBooksAsync(bookId, count);
        }
    }
}
