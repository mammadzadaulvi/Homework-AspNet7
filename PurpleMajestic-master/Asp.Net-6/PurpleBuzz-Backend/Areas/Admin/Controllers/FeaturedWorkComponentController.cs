using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.Areas.Admin.ViewModels.FeaturedWorkComponent;
using PurpleBuzz_Backend.Areas.Admin.ViewModels.FeaturedWorkComponent.FeaturedWorkComponentPhoto;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Helpers;
using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeaturedWorkComponentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FeaturedWorkComponentController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new FeaturedWorkComponentIndexViewModel
            {
                FeaturedWorkComponent = await _appDbContext.FeaturedWorkComponent.FirstOrDefaultAsync()
            };
            return View(model);
        }


        #region Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var featuredWorkComponent = await _appDbContext.FeaturedWorkComponent.FirstOrDefaultAsync();
            if (featuredWorkComponent != null) return NotFound();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FeaturedWorkComponentCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var featuredWorkComponent = new FeaturedWorkComponent
            {
                Title = model.Title,
                Description = model.Description
            };

            await _appDbContext.FeaturedWorkComponent.AddAsync(featuredWorkComponent);
            await _appDbContext.SaveChangesAsync();

            bool hasError = false;
            foreach (var photo in model.Photos)
            {
                if (!_fileService.IsImage(photo))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} yuklediyiniz file sekil formatinda olmalidir");
                    hasError = true;

                }
                else if (!_fileService.CheckSize(photo, 300))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} ci yuklediyiniz sekil 300 kb dan az olmalidir");
                    hasError = true;

                }

            }

            if (hasError) { return View(model); }

            int order = 1;
            foreach (var photo in model.Photos)
            {
                var featuredWorkComponentPhoto = new FeaturedWorkComponentPhoto
                {
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,
                    FeaturedWorkComponentId = featuredWorkComponent.Id
                };
                await _appDbContext.FeaturedWorkComponentPhotos.AddAsync(featuredWorkComponentPhoto);
                await _appDbContext.SaveChangesAsync();

                order++;
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var featureWorkComponent = await _appDbContext.FeaturedWorkComponent
                .Include(fwc => fwc.FeatureWorkComponentPhotos)
                .FirstOrDefaultAsync();

            if (featureWorkComponent == null) return NotFound();

            foreach (var photo in featureWorkComponent.FeatureWorkComponentPhotos)
            {
                _fileService.Delete(photo.Name, _webHostEnvironment.WebRootPath);

            }
            _appDbContext.FeaturedWorkComponent.Remove(featureWorkComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var featuredworkComponent = await _appDbContext.FeaturedWorkComponent.Include(fwc => fwc.FeatureWorkComponentPhotos)
                .FirstOrDefaultAsync();

            if (featuredworkComponent == null) return NotFound();

            var model = new FeaturedWorkComponentDetailsViewModel
            {
                Id = featuredworkComponent.Id,
                Title = featuredworkComponent.Title,
                Description = featuredworkComponent.Description,
                FeatureWorkComponentPhotos = _appDbContext.FeaturedWorkComponentPhotos.ToList(),

            };

            return View(model);
        }

        #endregion

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var featureWorkComponent = await _appDbContext.FeaturedWorkComponent.Include(fwc => fwc.FeatureWorkComponentPhotos).FirstOrDefaultAsync();
            if (featureWorkComponent == null) return NotFound();

            var model = new FeaturedWorkComponentUpdateViewModel
            {
                Title = featureWorkComponent.Title,
                Description = featureWorkComponent.Description,
                FeaturedWorkComponentPhotos = featureWorkComponent.FeatureWorkComponentPhotos.ToList(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(FeaturedWorkComponentUpdateViewModel model)
        {
            var featureWorkComponent = await _appDbContext.FeaturedWorkComponent.Include(fwc => fwc.FeatureWorkComponentPhotos).FirstOrDefaultAsync();

            if (featureWorkComponent == null) return NotFound();

            if (!ModelState.IsValid) return View(model);

            featureWorkComponent.Title = model.Title;
            featureWorkComponent.Description = model.Description;

            await _appDbContext.SaveChangesAsync();

            bool hasError = false;

            if (model.Photos != null)
            {

                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} yuklediyiniz file sekil formatinda olmalidir");
                        hasError = true;

                    }
                    else if (!_fileService.CheckSize(photo, 300))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} ci yuklediyiniz sekil 300 kb dan az olmalidir");
                        hasError = true;

                    }

                }

                if (hasError) return View(model);

                int order = 1;
                foreach (var photo in model.Photos)
                {
                    var featuredWorkComponentPhoto = new FeaturedWorkComponentPhoto
                    {
                        Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                        Order = order,
                        FeaturedWorkComponentId = featureWorkComponent.Id
                    };
                    await _appDbContext.FeaturedWorkComponentPhotos.AddAsync(featuredWorkComponentPhoto);
                    await _appDbContext.SaveChangesAsync();

                    order++;
                }
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Photo

        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {

            var featuredWorkComponentPhoto = await _appDbContext.FeaturedWorkComponentPhotos.FindAsync(id);
            if (featuredWorkComponentPhoto == null) return NotFound();

            var model = new FeaturedWorkComponentPhotoUpdateViewModel
            {
                Order = featuredWorkComponentPhoto.Order
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, FeaturedWorkComponentPhotoUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();

            var featureWorkComponentPhoto = await _appDbContext.FeaturedWorkComponentPhotos.FindAsync(model.Id);
            if (featureWorkComponentPhoto == null) return NotFound();

            featureWorkComponentPhoto.Order = model.Order;
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("update", "featuredworkcomponent", new { id = featureWorkComponentPhoto.FeaturedWorkComponentId });
        }

        [HttpGet]

        public async Task<IActionResult> Deletephoto(int id)
        {
            var featureWorkComponentPhoto = await _appDbContext.FeaturedWorkComponentPhotos.FindAsync(id);
            if (featureWorkComponentPhoto == null) return NotFound();


            _fileService.Delete(featureWorkComponentPhoto.Name, _webHostEnvironment.WebRootPath);

            _appDbContext.FeaturedWorkComponentPhotos.Remove(featureWorkComponentPhoto);
            await _appDbContext.SaveChangesAsync();


            return RedirectToAction("update", "featuredworkcomponent", new { id = featureWorkComponentPhoto.FeaturedWorkComponentId });
        }
        #endregion
    }
}
