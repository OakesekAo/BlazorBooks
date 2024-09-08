namespace BlazorBooks.Shared.Dtos;

public record BookDetailDto(int Id, string Title, string Image, 
    AuthorDto Author, int NumPages, string Format, string Description, GenreDto[] Genres, string BuyLink)
{
    public string BookLink => string.IsNullOrWhiteSpace(BuyLink)
        ? $"https://www.bing.com/search?q={Title.Replace(" ", "+")}+by+{Author.Name.Replace(" ", "+")}"
        : BuyLink;
};



