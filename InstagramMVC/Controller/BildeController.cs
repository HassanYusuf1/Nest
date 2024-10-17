using Microsoft.AspNetCore.Mvc;
using InstagramMVC.Models;
using InstagramMVC.DAL;
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

        [HttpGet]
        public async Task<IActionResult> HentAlle()
        {
            var bilder = await _bildeRepository.HentAlle();
            return Ok(bilder);
        }  




    }
}