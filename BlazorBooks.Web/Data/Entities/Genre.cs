﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlazorBooks.Web.Data.Entities
{
    public class Genre
    {
        [Key]
        public short Id { get; set; }

        [Required, MaxLength(50), Unicode(false)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(75), Unicode(false)]
        public string Slug { get; set; } = string.Empty;
        public bool IsTop { get; set; }

        public virtual ICollection<GenreBooks> GenreBooks { get; set; } = new List<GenreBooks>();


    }
}
