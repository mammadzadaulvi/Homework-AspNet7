using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.Areas.Admin.ViewModels;
using PurpleBuzz_Backend.Areas.Admin.ViewModels.ContactIntro;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Helpers;
using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactIntroController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public ContactIntroController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ContactIntroIndexViewModel
            {
                ContactIntroComponents = await _appDbContext.ContactIntroComponent.ToListAsync()
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        #region Create

        [HttpPost]
        public async Task<IActionResult> Create(ContactIntroComponent contactIntroComponent)
        {
            if (!ModelState.IsValid) return View(contactIntroComponent);

            if (!_fileService.IsImage(contactIntroComponent.Photo))
            {
                ModelState.AddModelError("Photo", "The image must be img format");
                return View(contactIntroComponent);
            }
            if (!_fileService.CheckSize(contactIntroComponent.Photo, 1000))
            {
                ModelState.AddModelError("Photo", $"Şəkilin ölçüsü 1000 kb-dan böyükdür");
                return View(contactIntroComponent);
            }

            contactIntroComponent.FilePath = await _fileService.UploadAsync(contactIntroComponent.Photo, _webHostEnvironment.WebRootPath);
            await _appDbContext.ContactIntroComponent.AddAsync(contactIntroComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Update

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var contactIntroComponent = await _appDbContext.ContactIntroComponent.FindAsync(id);
            if (contactIntroComponent == null) return NotFound();

            var model = new ContactIntroUpdateViewModel
            {
                Id = contactIntroComponent.Id,
                Title = contactIntroComponent.Title,
                Description = contactIntroComponent.Description,
                //Text1 = contactIntroComponent.Text1,
                PhotoPath = contactIntroComponent.FilePath
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ContactIntroUpdateViewModel contactIndexUpdateView)
        {
            if (!ModelState.IsValid) return View(contactIndexUpdateView);
            if (id != contactIndexUpdateView.Id) return BadRequest();

            var contactIntroComponent = await _appDbContext.ContactIntroComponent.FindAsync(contactIndexUpdateView.Id);
            if (contactIntroComponent is null) return NotFound();

            contactIntroComponent.Title = contactIndexUpdateView.Title;
            contactIntroComponent.Description = contactIndexUpdateView.Description;

            if (!_fileService.IsImage(contactIndexUpdateView.Photo))
            {
                ModelState.AddModelError("Photo", "The image must be img format");
                return View(contactIndexUpdateView);
            }


            if (!_fileService.CheckSize(contactIndexUpdateView.Photo, 1000))
            {
                ModelState.AddModelError("Photo", $"Şəkilin ölçüsü 1000 kb-dan böyükdür");
                return View(contactIndexUpdateView);
            }
            if (contactIndexUpdateView.Photo != null)
            {
                _fileService.Delete(contactIntroComponent.FilePath, _webHostEnvironment.WebRootPath);

                contactIntroComponent.FilePath = await _fileService.UploadAsync(contactIndexUpdateView.Photo, _webHostEnvironment.WebRootPath);
            }

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        #endregion



        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var contactIntroComponent = await _appDbContext.ContactIntroComponent.FindAsync(id);
            if (contactIntroComponent == null) return NotFound();
            return View(contactIntroComponent);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var contactIntroComponent = await _appDbContext.ContactIntroComponent.FindAsync(id);
            if (contactIntroComponent == null) return NotFound();

            _fileService.Delete(contactIntroComponent.FilePath , _webHostEnvironment.WebRootPath);

            _appDbContext.ContactIntroComponent.Remove(contactIntroComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        #endregion


        #region Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var contactIntroComponent = await _appDbContext.ContactIntroComponent.FindAsync(id);
            if (contactIntroComponent == null) return NotFound();
            return View(contactIntroComponent);
        }
        #endregion

    }
}
