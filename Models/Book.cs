using System;
using System.ComponentModel.DataAnnotations;

namespace HomeLib.Models
{
    public class Book
    {
        public int BookID { get; set; }
        
        [Required(ErrorMessage = "Название книги обязательно")]
        [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Автор обязателен")]
        [StringLength(150, ErrorMessage = "Имя автора не должно превышать 150 символов")]
        public string Author { get; set; }
        
        [StringLength(100, ErrorMessage = "Жанр не должен превышать 100 символов")]
        public string Genre { get; set; }
        
        [Required(ErrorMessage = "Год издания обязателен")]
        [Range(1000, 2025, ErrorMessage = "Год издания должен быть от 1000 до 2025")]
        public int PublishYear { get; set; }
        
        [StringLength(150, ErrorMessage = "Издательство не должно превышать 150 символов")]
        public string Publisher { get; set; }
        
        [Required(ErrorMessage = "Количество страниц обязательно")]
        [Range(1, 10000, ErrorMessage = "Количество страниц должно быть больше 0")]
        public int PageCount { get; set; }
        
        [StringLength(50, ErrorMessage = "Язык не должен превышать 50 символов")]
        public string Language { get; set; }
        
        public DateTime DateAdded { get; set; }
        
        public string TableOfContents { get; set; }
    }
}