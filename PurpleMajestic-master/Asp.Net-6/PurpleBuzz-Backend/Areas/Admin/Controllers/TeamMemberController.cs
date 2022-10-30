using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.Areas.Admin.ViewModels;
using PurpleBuzz_Backend.Areas.Admin.ViewModels.TeamMember;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Helpers;
using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamMemberController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public TeamMemberController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new TeamMemberIndexViewModel
            {
                TeamMembers = await _appDbContext.TeamMembers.ToListAsync()
            };
            return View(model);
        }


        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(TeamMember teamMember)
        {
            if (!ModelState.IsValid) return View(teamMember);
            if (!teamMember.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(teamMember);
            }
            if (teamMember.Photo.Length / 1024 > 60)
            {
                ModelState.AddModelError("Photo", "Photo size is greater than 60kB");
                return View(teamMember);
            }

            teamMember.PhotoPath = await _fileService.UploadAsync(teamMember.Photo, _webHostEnvironment.WebRootPath);
            await _appDbContext.TeamMembers.AddAsync(teamMember);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        #endregion


        #region Update

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();

            var model = new TeamMemberUpdateViewModel
            {
                Id = dbTeamMember.id,
                Name = dbTeamMember.Name,
                Surname = dbTeamMember.Surname,
                Position = dbTeamMember.Position,
                PhotoPath = dbTeamMember.PhotoPath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, TeamMemberUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();

            if (id != model.Id) return BadRequest();

            dbTeamMember.Name = model.Name;
            dbTeamMember.Surname = model.Surname;
            dbTeamMember.Position = model.Position;

            if (model.Photo != null)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                    return View(model);
                }

                if (!_fileService.CheckSize(model.Photo, 60))
                {
                    ModelState.AddModelError("Photo", "Photo size is greater than 60kB");
                    return View(model);
                }
                _fileService.Delete(_webHostEnvironment.WebRootPath, dbTeamMember.PhotoPath);
                dbTeamMember.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        #endregion


        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();
            return View(dbTeamMember);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();

            _fileService.Delete(_webHostEnvironment.WebRootPath, dbTeamMember.PhotoPath);

            _appDbContext.TeamMembers.Remove(dbTeamMember);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        #endregion


        public async Task<IActionResult> Details(int id)
        {
            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();
            return View(dbTeamMember);
        }
    }
}
