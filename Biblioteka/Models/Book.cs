using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteka.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [MaxLength(200)]
        [Display(Name = "Tytuł")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(13)]
        [Display(Name = "ISBN")]
        public string? ISBN { get; set; }

        [Range(1000, 2100, ErrorMessage = "Podaj prawidłowy rok (1000-2100)")]
        [Display(Name = "Rok wydania")]
        public int? PublishedYear { get; set; }

        // Klucz obcy do Author (relacja 1:N)
        [Required(ErrorMessage = "Autor jest wymagany")]
        [Display(Name = "Autor")]
        public int AuthorId { get; set; }

        // Właściwość nawigacyjna do autora
        public virtual Author? Author { get; set; }

        // Właściwość nawigacyjna do gatunków (N:M)
        public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}