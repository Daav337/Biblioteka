using Biblioteka.Models;
using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa gatunku jest wymagana")]
        [MaxLength(50)]
        [Display(Name = "Gatunek")]
        public string Name { get; set; } = string.Empty;

        // Właściwość nawigacyjna – gatunek może mieć wiele książek (N:M)
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}