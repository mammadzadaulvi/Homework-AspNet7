using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurpleBuzz_Backend.Models
{
    public class ContactIntroComponent
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title can't be null"), MinLength(3, ErrorMessage = "Title must contain 3 letter at least")]
        public string Title { get; set; }

        [Required(ErrorMessage ="Text can't be null")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Filepath can't be null")]
        public string? FilePath { get; set; }

        [NotMapped]
        [Required]
        public IFormFile Photo { get; set; }
    }
}
