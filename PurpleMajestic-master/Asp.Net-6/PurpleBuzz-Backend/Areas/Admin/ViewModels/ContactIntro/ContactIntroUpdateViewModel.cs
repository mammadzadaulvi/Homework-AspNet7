using System.ComponentModel.DataAnnotations;

namespace PurpleBuzz_Backend.Areas.Admin.ViewModels.ContactIntro
{
    public class ContactIntroUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Text can't be null")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Text can't be null")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Text can't be null")]
        public string Text1 { get; set; }

        public string? PhotoPath { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
