using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteka.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [MaxLength(50)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [MaxLength(50)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Biografia")]
        public string? Bio { get; set; }

        // Właściwość nawigacyjna – jeden autor ma wiele książek (1:N)
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        // Pole pomocnicze do wyświetlania pełnego imienia
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}