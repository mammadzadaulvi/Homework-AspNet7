using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Models;
using PurpleBuzz_Backend.ViewModels.Work;

namespace PurpleBuzz_Backend.Controllers
{
    public class WorkController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public WorkController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _appDbContext.Categories.Include(c => c.CategoryComponents).ToListAsync();
            var model = new WorkIndexViewModel
            {
                Categories = categories,

                FeaturedWorkComponent = await _appDbContext.FeaturedWorkComponent.Include(fwc => fwc.FeatureWorkComponentPhotos
                                             .OrderBy(fwcp => fwcp.Order))
                                             .FirstOrDefaultAsync()
            };
            return View(model);
        }
    }
}

