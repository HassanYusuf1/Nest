using Microsoft.AspNetCore.Mvc;
using InstagramMVC.Models;
using InstagramMVC.DAL;
using InstagramMVC.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace InstagramMVC.Controllers
{
    public class BildeController : Controller
    {
        private readonly IBildeRepository _bildeRepository;
        private readonly IKommentarRepository _kommentarRepository;
        private readonly ILogger<BildeController> _logger;

        public BildeController(IBildeRepository bildeRepository, IKommentarRepository kommentarRepository, ILogger<BildeController> logger)
        {
            _kommentarRepository = kommentarRepository;
            _bildeRepository = bildeRepository;
            _logger = logger;
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Bilde nyttBilde, IFormFile BildeUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(nyttBilde);
            }

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

                nyttBilde.BildeUrl = "/images/" + uniqueFileName;
            }

            bool vellykket = await _bildeRepository.Create(nyttBilde);
            if (vellykket)
            {
                return RedirectToAction(nameof(Grid));
            }
            else
            {
                _logger.LogWarning("[BildeController] Could not create new image.");
                return View(nyttBilde);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                _logger.LogError("[BildeController] bilde sin id ble ikke funnet");
                return NotFound();
            }
            
            return View("BildeDetails", bilde);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                return NotFound();
            }
            return View(bilde);
        }

       [HttpPost]
public async Task<IActionResult> Edit(int id, Bilde updatedBilde, IFormFile? newBildeUrl)
{
    if (id != updatedBilde.BildeId || !ModelState.IsValid)
    {
        return View(updatedBilde);
    }

    // Hent eksisterende objekt fra databasen
    var eksisterendeBilde = await _bildeRepository.BildeId(id);
    if (eksisterendeBilde == null)
    {
        return NotFound();
    }

    // Oppdater feltene manuelt
    eksisterendeBilde.Tittel = updatedBilde.Tittel;
    eksisterendeBilde.Beskrivelse = updatedBilde.Beskrivelse;

    if (newBildeUrl != null && newBildeUrl.Length > 0)
    {
        // Håndter bildefil-opplastingen som vanlig
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(newBildeUrl.FileName);
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await newBildeUrl.CopyToAsync(fileStream);
        }

        // Slett det gamle bildet hvis det finnes
        if (!string.IsNullOrEmpty(eksisterendeBilde.BildeUrl))
        {
            string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", eksisterendeBilde.BildeUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        // Oppdater BildeUrl til den nye filen
        eksisterendeBilde.BildeUrl = "/images/" + uniqueFileName;
    }

    // Nå vil BildeUrl beholde sin verdi hvis ingen ny fil er lastet opp
    bool vellykket = await _bildeRepository.Oppdater(eksisterendeBilde);
    return vellykket ? RedirectToAction("Grid") : View(updatedBilde);
}

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                _logger.LogError("[BildeController] bilde med Id ble ikke funnet {id}", id);
                return NotFound();
            }
            return View(bilde);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);

            if (bilde == null)
            {
                _logger.LogError("[BildeController] bilde med Id ble ikke funnet {id}", id);
                return NotFound();
            }

            if (!string.IsNullOrEmpty(bilde.BildeUrl))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", bilde.BildeUrl.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            bool vellykket = await _bildeRepository.Delete(id);

            if (!vellykket)
            {
                _logger.LogError("[BildeController] bilde ble ikke slettet med {Id} ", id);
                return BadRequest("Bilde ble ikke slettet");
            }

            return RedirectToAction(nameof(Grid));
        }
    }
}