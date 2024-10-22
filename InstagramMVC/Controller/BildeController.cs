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
        private readonly ILogger<BildeController> _logger;

        public BildeController(IBildeRepository bildeRepository, ILogger<BildeController> logger)
        {
            _bildeRepository = bildeRepository;
            _logger = logger;
        }

        // Henter alle bilende 
        [HttpGet]
        public async Task<IActionResult> Bilde()
        {
            // Henter alle bildene 
            var bilder = await _bildeRepository.GetAll();
            var bildeViewModel = new BilderViewModel(bilder, "Bilde");

            if (bilder == null) {
                
                _logger.LogError("[BildeController] Bilde liste, ble ikke funnet.");
                // return NotFound("Bildene ble ikke funnet"); HVA FAEN ER DENNE KODEN HER????!!!! 

            }
        
            return View(bildeViewModel);  
        }

        public async Task<IActionResult> Grid()
        {
             // Henter alle bildene 
            var bilder = await _bildeRepository.GetAll();
            var bildeViewModel = new BilderViewModel(bilder, "Bilde");

            if (bilder == null){
                
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

        //bildeLAGRE
        [HttpPost]
        public async Task<IActionResult> Create(Bilde nyttBilde)
        {
            if (!ModelState.IsValid)
            {
                return View(nyttBilde);  
            }
            bool vellykket = await _bildeRepository.Opprette(nyttBilde);
            if (vellykket)
            {
                return RedirectToAction(nameof(Grid));  
            }
            else
            {
                _logger.LogWarning("[BildeController] kunne ikke opprett nytt bilde ");
                return View(nyttBilde);  // Show form again if creation failed
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                _logger.LogError(" [BildeController] bilde sin id ble ikke funnet");
                return NotFound();  // Return 404 if the image is not found
            }
            return View(bilde);  
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                return NotFound();  // Return 404 if the image is not found
            }
            return View(bilde);  // Return the Edit form with the image details
        }

        // Handle form submission to update an image
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Bilde bilde)
        {
            if (id != bilde.Id || !ModelState.IsValid)
            {
                return View(bilde);  // Re-render the form with validation errors
            }

            bool vellykket = await _bildeRepository.Oppdater(bilde);
            if (vellykket)
            {
                return RedirectToAction("Grid");  // Redirect to Index after successful update
            }
            else
            {
                ModelState.AddModelError("", "Kunne ikke oppdatere bilde.");
                return View(bilde);  // Show form again if update failed
            }
        }

        // Show form to confirm deletion (Razor view)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                _logger.LogError("[BildeController] bilde med Id ble ikke funnet {id}", id);
                return NotFound();  // Return 404 if the image is not found
            }
            return View(bilde);  // Return Delete confirmation page
        }

        
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
