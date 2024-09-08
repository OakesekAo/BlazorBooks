using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlazorBooks.Web.Data.Entities
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(80), Unicode(false)]
        public string Name { get; set; } = string.Empty;


        [Required, MaxLength(800), Unicode(false)]
        public string Slug { get; set; } = string.Empty;
    }
}
