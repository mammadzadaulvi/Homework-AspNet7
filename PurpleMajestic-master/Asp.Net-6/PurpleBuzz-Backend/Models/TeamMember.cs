using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurpleBuzz_Backend.Models
{
    public class TeamMember
    {
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string Position { get; set; }
        public string? PhotoPath { get; set; }
        
        [NotMapped]
        [Required]
        public IFormFile Photo { get; set; }
    }
}
