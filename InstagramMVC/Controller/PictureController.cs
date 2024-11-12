using Microsoft.AspNetCore.Mvc;
using InstagramMVC.Models;
using InstagramMVC.DAL;
using InstagramMVC.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using InstagramMVC.Utilities;

namespace InstagramMVC.Controllers
{
    public class PictureController : Controller
    {
        private readonly IPictureRepository _pictureRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<PictureController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public PictureController(IPictureRepository pictureRepository, ICommentRepository commentRepository, ILogger<PictureController> logger, UserManager<IdentityUser> userManager)
        {
            _commentRepository = commentRepository;
            _pictureRepository = pictureRepository;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyPage()
        {
            var currentUserName = _userManager.GetUserName(User);
            if (string.IsNullOrEmpty(currentUserName))
            {
                _logger.LogError("[PictureController] Current user is null or empty when accessing MyPage.");
                return Unauthorized();
            }

            var allPictures = await _pictureRepository.GetAll();
            if (allPictures == null)
            {
                _logger.LogError("[PictureController] Could not retrieve images for user {UserName}", currentUserName);
                allPictures = Enumerable.Empty<Picture>();
            }

            var userPictures = allPictures.Where(b => b.UserName == currentUserName).ToList();

            var pictureViewModel = new PicturesViewModel(userPictures, "MyPage");

            ViewData["IsMyPage"] = true; // Set flag to indicate it's MyPage
            return View("MyPage", pictureViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Picture()
        {
            var pictures = await _pictureRepository.GetAll();
            var pictureViewModel = new PicturesViewModel(pictures, "Picture");

            if (pictures == null)
            {
                _logger.LogError("[PictureController] Picture list, not found.");
            }

            return View(pictureViewModel);
        }

        public async Task<IActionResult> Grid()
        {
            var pictures = await _pictureRepository.GetAll();
            var pictureViewModel = new PicturesViewModel(pictures, "Picture");

            if (pictures == null)
            {
                _logger.LogError("[PictureController] Picture list, not found.");
                return NotFound("Pictures not found");
            }

            ViewData["IsMyPage"] = false; // Set IsMyPage flag to false for general feed (Grid)
            return View(pictureViewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Picture newImage, IFormFile PictureUrl)
        {
            var time = DateTime.Now;
            newImage.UploadDate = time;

            if (!ModelState.IsValid)
            {
                return View(newImage);
            }

            var UserName = _userManager.GetUserName(User);
            newImage.UserName = UserName;

            if (PictureUrl != null && PictureUrl.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(PictureUrl.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureUrl.CopyToAsync(fileStream);
                }

                newImage.PictureUrl = "/images/" + uniqueFileName;
            }

            bool success = await _pictureRepository.Create(newImage);
            if (success)
            {
                return RedirectToAction(nameof(MyPage));
            }
            else
            {
                _logger.LogWarning("[PictureController] Could not create new image.");
                return View(newImage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string source = "Grid")
        {
            var picture = await _pictureRepository.PictureId(id);
            if (picture == null)
            {
                _logger.LogError("[PictureController] picture id not found");
                return NotFound();
            }

            ViewBag.Source = source; // Save source in ViewBag
            return View("PictureDetails", picture);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id, string source = "Grid")
        {
            var picture = await _pictureRepository.PictureId(id);
            if (picture == null)
            {
                _logger.LogError("The image with id {PictureId} was not found", id);
                return NotFound();
            }

            var currentUserName = _userManager.GetUserName(User);
            if (picture.UserName != currentUserName)
            {
                _logger.LogWarning("Unauthorized edit attempt by user {UserId} for image {PictureId}", currentUserName, id);
                return Forbid();
            }

            TempData["Source"] = source; // Store source in TempData
            return View(picture);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Picture updatedPicture, IFormFile? newPictureUrl, string source)
        {
            if (id != updatedPicture.PictureId || !ModelState.IsValid)
            {
                TempData["Source"] = source; // Preserve source value in case of validation error
                return View(updatedPicture);
            }

            var existingPicture = await _pictureRepository.PictureId(id);
            if (existingPicture == null)
            {
                _logger.LogError("The image with id {PictureId} was not found", id);
                return NotFound();
            }

            var currentUserName = _userManager.GetUserName(User);
            if (existingPicture.UserName != currentUserName)
            {
                _logger.LogWarning("Unauthorized edit attempt by user {UserName} for image {PictureId}", currentUserName, id);
                return Forbid();
            }

            // Update title and description
            existingPicture.Title = updatedPicture.Title;
            existingPicture.Description = updatedPicture.Description;

            // Update the image if a new one is uploaded
            if (newPictureUrl != null && newPictureUrl.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(newPictureUrl.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await newPictureUrl.CopyToAsync(fileStream);
                }

                // Delete the old image
                if (!string.IsNullOrEmpty(existingPicture.PictureUrl))
                {
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingPicture.PictureUrl.TrimStart('/'));
                    if (FileUtil.FileExists(oldFilePath))
                    {
                        FileUtil.FileDelete(oldFilePath);
                    }
                }

                existingPicture.PictureUrl = "/images/" + uniqueFileName;
            }

            bool success = await _pictureRepository.Edit(existingPicture);
            if (success)
            {
                // Redirect to the correct page based on the Source parameter
                return RedirectToAction(source == "MyPage" ? "MyPage" : "Grid");
            }
            else
            {
                _logger.LogWarning("[PictureController] Could not update the image.");
                TempData["Source"] = source; // Preserve source value if the update fails
                return View(updatedPicture);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id, string source = "Grid")
        {
            var picture = await _pictureRepository.PictureId(id);
            if (picture == null)
            {
                _logger.LogError("[PictureController] picture with Id not found {id}", id);
                return NotFound();
            }

            var currentUserName = _userManager.GetUserName(User);
            if (picture.UserName != currentUserName)
            {
                _logger.LogWarning("Unauthorized delete attempt by user {UserName} for image {PictureId}", currentUserName, id);
                return Forbid();
            }

            TempData["Source"] = source; // Store source in TempData
            return View(picture);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id, string source)
        {
            var picture = await _pictureRepository.PictureId(id);
            if (picture == null)
            {
                _logger.LogError("[PictureController] picture with Id not found {id}", id);
                return NotFound();
            }

            var currentUserName = _userManager.GetUserName(User);
            if (picture.UserName != currentUserName)
            {
                _logger.LogWarning("Unauthorized delete attempt by user {UserName} for image {PictureId}", currentUserName, id);
                return Forbid();
            }

            if (!string.IsNullOrEmpty(picture.PictureUrl))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", picture.PictureUrl.TrimStart('/'));

                if (FileUtil.FileExists(fullPath))
                {
                    FileUtil.FileDelete(fullPath);
                }
            }

            bool success = await _pictureRepository.Delete(id);
            if (!success)
            {
                _logger.LogError("[PictureController] picture not deleted with {Id}", id);
                return BadRequest("Picture not deleted");
            }

            // Redirect to the correct page based on the Source parameter
            return RedirectToAction(source == "MyPage" ? "MyPage" : "Grid");
        }
    }
}
