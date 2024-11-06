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
    public class BildeController : Controller
    {
        private readonly IBildeRepository _bildeRepository;
        private readonly IKommentarRepository _kommentarRepository;
        private readonly ILogger<BildeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public BildeController(IBildeRepository bildeRepository, IKommentarRepository kommentarRepository, ILogger<BildeController> logger,UserManager<IdentityUser> userManager)
        {
            _kommentarRepository = kommentarRepository;
            _bildeRepository = bildeRepository;
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
                _logger.LogError("[BildeController] Current user is null or empty when accessing MyPage.");
                return Unauthorized();
            }

            var allBilder = await _bildeRepository.GetAll();
            if (allBilder == null)
            {
                _logger.LogError("[BildeController] Could not retrieve images for user {UserName}", currentUserName);
                allBilder = Enumerable.Empty<Bilde>();
            }

            var userBilder = allBilder.Where(b => b.UserName == currentUserName).ToList();

            var bildeViewModel = new BilderViewModel(userBilder, "MyPage");

            return View("MyPage", bildeViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Bilde()
        {
            var bilder = await _bildeRepository.GetAll();
            var bildeViewModel = new BilderViewModel(bilder, "Bilde");

            if (bilder == null)
            {
                _logger.LogError("[BildeController] Bilde liste, ble ikke funnet.");
            }

            return View(bildeViewModel);
        }

        public async Task<IActionResult> Grid()
        {
            var bilder = await _bildeRepository.GetAll();
            var bildeViewModel = new BilderViewModel(bilder, "Bilde");

            if (bilder == null)
            {
                _logger.LogError("[BildeController] Bilde liste, ble ikke funnet.");
                return NotFound("Bildene ble ikke funnet");
            }

            return View(bildeViewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Bilde newImage, IFormFile BildeUrl)
        {
            var time = DateTime.Now;
            newImage.OpprettetDato = time;
            

            if (!ModelState.IsValid)
            {
                return View(newImage);
            }
            var UserName = _userManager.GetUserName(User);
            newImage.UserName = UserName; 


            if (BildeUrl != null && BildeUrl.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(BildeUrl.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await BildeUrl.CopyToAsync(fileStream);
                }

                newImage.BildeUrl = "/images/" + uniqueFileName;
            }

            bool vellykket = await _bildeRepository.Create(newImage);
            if (vellykket)
            {
                return RedirectToAction(nameof(MyPage));
            }
            else
            {
                _logger.LogWarning("[BildeController] Could not create new image.");
                return View(newImage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, string source = "Grid")
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                _logger.LogError("[BildeController] bilde sin id ble ikke funnet");
                return NotFound();
            }
    
            ViewBag.Source = source; // Lagre source i ViewBag
            return View("BildeDetails", bilde);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                _logger.LogError("The image with id {BildeId} was not found", id);
                return NotFound();
            }
            var currentUserName =  _userManager.GetUserName(User);
            if(bilde.UserName != currentUserName)
            {
                _logger.LogWarning("Unartorized edit attempt by user {UserId} for image {BildeId}", currentUserName,id);
                return Forbid();
            }
            return View(bilde);
        }

       [HttpPost]
       [Authorize]
    public async Task<IActionResult> Edit(int id, Bilde updatedBilde, IFormFile? newBildeUrl)
    {
        if (id != updatedBilde.BildeId || !ModelState.IsValid)
        {
            return View(updatedBilde);
        }

        var eksisterendeBilde = await _bildeRepository.BildeId(id);
        if (eksisterendeBilde == null)
        {
            return NotFound();
        }

        var currentUserName = _userManager.GetUserName(User);
        if(eksisterendeBilde.UserName != currentUserName)
        {
            _logger.LogWarning("Unaauthorized edit attempt by use {UserName} for image {BildeId}", currentUserName, id);
            return Forbid();
        }

        eksisterendeBilde.Tittel = updatedBilde.Tittel;
        eksisterendeBilde.Beskrivelse = updatedBilde.Beskrivelse;

        if (newBildeUrl != null && newBildeUrl.Length > 0)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(newBildeUrl.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await newBildeUrl.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(eksisterendeBilde.BildeUrl))
            {
                string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", eksisterendeBilde.BildeUrl.TrimStart('/'));
                if (FileUtil.FileExists(oldFilePath))
                {
                    FileUtil.FileDelete(oldFilePath);
                }
            }   

            eksisterendeBilde.BildeUrl = "/images/" + uniqueFileName;
        }

        bool vellykket = await _bildeRepository.Oppdater(eksisterendeBilde);
        return vellykket ? RedirectToAction("Grid") : View(updatedBilde);
    }

   [HttpGet]
[Authorize]
public async Task<IActionResult> Delete(int id, string? returnUrl = null)
{
    var bilde = await _bildeRepository.BildeId(id);
    if (bilde == null)
    {
        _logger.LogError("[BildeController] bilde med Id ble ikke funnet {id}", id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (bilde.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for image {BildeId}", currentUserName, id);
        return Forbid();
    }

    // Store the returnUrl in TempData so we can pass it to the Delete view.
    TempData["ReturnUrl"] = returnUrl ?? Url.Action("Grid"); // Lagre returnUrl i TempData

    return View(bilde);
}



[HttpPost]
[Authorize]
public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl = null)
{
    var bilde = await _bildeRepository.BildeId(id);
    if (bilde == null)
    {
        _logger.LogError("[BildeController] bilde med Id ble ikke funnet {id}", id);
        return NotFound();
    }

    var currentUserName = _userManager.GetUserName(User);
    if (bilde.UserName != currentUserName)
    {
        _logger.LogWarning("Unauthorized delete attempt by user {UserName} for image {BildeId}", currentUserName, id);
        return Forbid();
    }

    if (!string.IsNullOrEmpty(bilde.BildeUrl))
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", bilde.BildeUrl.TrimStart('/'));

        if (FileUtil.FileExists(fullPath))
        {
            FileUtil.FileDelete(fullPath);
        }
    }

    bool vellykket = await _bildeRepository.Delete(id);

    if (!vellykket)
    {
        _logger.LogError("[BildeController] bilde ble ikke slettet med {Id}", id);
        return BadRequest("Bilde ble ikke slettet");
    }

    return Redirect(returnUrl ?? Url.Action("Grid")); // Endret: Sikrer at `Redirect` får en ikke-null verdi
}




    }
}