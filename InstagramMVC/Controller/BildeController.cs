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

            
            return View(bildeViewModel);  
        }

        // Show form to create a new image (Razor view)
        [HttpGet]
        public IActionResult Create()
        {
            return View();  // This will return Create.cshtml
        }

        // Handle form submission to create a new image
        [HttpPost]
        public async Task<IActionResult> Create(Bilde nyttBilde)
        {
            if (!ModelState.IsValid)
            {
                return View(nyttBilde);  // Re-render form with validation errors
            }

            bool vellykket = await _bildeRepository.Opprette(nyttBilde);
            if (vellykket)
            {
                return RedirectToAction("Index");  // Redirect to Index after successful creation
            }
            else
            {
                ModelState.AddModelError("", "Kunne ikke opprette bilde.");
                return View(nyttBilde);  // Show form again if creation failed
            }
        }

        // Display details of a specific image (Razor view)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bilde = await _bildeRepository.BildeId(id);
            if (bilde == null)
            {
                return NotFound();  // Return 404 if the image is not found
            }
            return View(bilde);  // Pass the image to the Details.cshtml view
        }

        // Show form to update an image (Razor view)
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
                return RedirectToAction("Index");  // Redirect to Index after successful update
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
                return NotFound();  // Return 404 if the image is not found
            }
            return View(bilde);  // Return Delete confirmation page
        }

        // Handle the image deletion
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool vellykket = await _bildeRepository.Slett(id);
            if (vellykket)
            {
                return RedirectToAction("Index");  // Redirect to Index after successful deletion
            }
            else
            {
                ModelState.AddModelError("", "Kunne ikke slette bilde.");
                var bilde = await _bildeRepository.BildeId(id);
                return View(bilde);  // Show the form again if deletion failed
            }
        }
    }
}
