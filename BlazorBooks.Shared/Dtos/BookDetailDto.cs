namespace BlazorBooks.Shared.Dtos;

public record BookDetailDto(int Id, string Title, string Image, 
    AuthorDto Author, int NumPages, string Format, string Description, GenreDto[] Genres);



