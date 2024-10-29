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
                _logger.LogWarning("[BildeController] ModelState is not valid.");
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
            else
            {
                _logger.LogWarning("[BildeController] No file selected for upload.");
            }

            bool vellykket = await _bildeRepository.Create(nyttBilde);
            if (vellykket)
            {
                _logger.LogInformation("[BildeController] Image created successfully with ID: {0}", nyttBilde.BildeId);
                return RedirectToAction(nameof(Grid));
            }
            else
            {
                _logger.LogWarning("[BildeController] Could not create new image.");
                return View(nyttBilde);
            }
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
        public async Task<IActionResult> Edit(int id, Bilde bilde)
        {
            if (id != bilde.BildeId || !ModelState.IsValid)
            {
                return View(bilde);
            }

            bool vellykket = await _bildeRepository.Oppdater(bilde);
            if (vellykket)
            {
                return RedirectToAction("Grid");
            }
            else
            {
                ModelState.AddModelError("", "Kunne ikke oppdatere bilde.");
                return View(bilde);
            }
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