using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webappinsta.Models;
using webappinsta.ViewModels;

namespace webappinsta.Controllers;

public class NotatController : Controller
{
    private readonly MediaDbContext _mediaDbContext;
    private readonly ILogger<NotatController> _logger;


    public NotatController(MediaDbContext mediaDbContext, ILogger<FeedController> logger)
    {
        _mediaDbContext = mediaDbContext;
        _logger = logger;
    }

    public async Task<IActionResult> Notes()
    {
        _logger.LogInformation("This is notes");
        _logger.LogWarning("This is a warning");
        _logger.LogError("This is an error");
        List<Note> notes = await _noteDbContext.Notes.ToListAsync();
        var notesViewModel = new NotesViewModel(notes, "Notes");
        return View(notesViewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var note = await _noteDbContext.Notes.FirstOrDefaultAsync(i => i.NoteId == id);
        if (note == null)
            return NotFound();
        return View(note);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _noteDbContext.Notes.FindAsync(id);
        if (note == null)
        {
            return NotFound();
        }
        return View(note);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var note = await _noteDbContext.Notes.FindAsync(id);
        if (note == null)
        {
            return NotFound();
        }
        _noteDbContext.Notes.Remove(note);
        await _noteDbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Notes));
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
            _noteDbContext.Notes.Add(note);
            await _noteDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Notes));
        }
        return View(note);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var note = await _noteDbContext.Notes.FindAsync(id);
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
            _noteDbContext.Notes.Update(note);
            await _noteDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Notes));
        }
        return View(note);
    }
}
