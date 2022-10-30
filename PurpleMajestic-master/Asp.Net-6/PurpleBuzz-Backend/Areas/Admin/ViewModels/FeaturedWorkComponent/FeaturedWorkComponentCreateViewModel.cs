namespace PurpleBuzz_Backend.Areas.Admin.ViewModels.FeaturedWorkComponent
{
    public class FeaturedWorkComponentCreateViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}
