using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstagramMVC.Models;
using InstagramMVC.ViewModels;
using InstagramMVC.DAL;

namespace InstagramMVC.Controllers;

public class NotatController : Controller
{
    private readonly ILogger<NotatController> _logger;
    private readonly INotatRepository _notatRepository;


    public NotatController(INotatRepository notatRepository, ILogger<NotatController> logger)
    {
        _notatRepository = notatRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Notat()
    {
        _logger.LogInformation("This is notes");
        _logger.LogWarning("This is a warning");
        _logger.LogError("This is an error");
        var notater = await _notatRepository.GetAll();
        var notaterViewModel = new NotaterViewModel(notater, "Notes");
        return View(notaterViewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
            return NotFound();
        return View(note);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
        {
            return NotFound();
        }
        return View(note);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var notat = await _notatRepository.GetNoteById(id);
        if (notat == null)
        {
            return NotFound();
        }
        await _notatRepository.DeleteConfirmed(id);
        
        return RedirectToAction(nameof(Notat));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Note note)
    {
        if (ModelState.IsValid)
        {
            await _notatRepository.Create(note);
            
            return RedirectToAction(nameof(Notat));
        }
        return View(note);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var note = await _notatRepository.GetNoteById(id);
        if (note == null)
        {
            return NotFound();
        }
        return View(note);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(Note note)
    {
        if (ModelState.IsValid)
        {
            await _notatRepository.Update(note);
            
            return RedirectToAction(nameof(Notat));
        }
        return View(note);
    }
}
