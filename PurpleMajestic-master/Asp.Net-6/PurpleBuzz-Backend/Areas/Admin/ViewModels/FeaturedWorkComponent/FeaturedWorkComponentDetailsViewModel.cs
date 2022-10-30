namespace PurpleBuzz_Backend.Areas.Admin.ViewModels.FeaturedWorkComponent
{
    public class FeaturedWorkComponentDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public ICollection<Models.FeaturedWorkComponentPhoto> FeatureWorkComponentPhotos { get; set; }
    }
}
