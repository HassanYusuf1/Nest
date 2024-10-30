using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using InstagramMVC.Models;
using InstagramMVC.DAL;



namespace InstagramMVC.DAL;

public class NotatRepository : INotatRepository
{
    private readonly MediaDbContext _db;
    private readonly ILogger<NotatRepository> _logger;

    public NotatRepository(MediaDbContext db, ILogger<NotatRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Note>> GetAll()
    {
        try
        {
            return await _db.Notes.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[NotatRepository] Notes ToListAsync() failed when GetAll, error message: {e}", e);
            throw new InvalidOperationException("[NotatRepository] GetAll() failed");
        }
    }

    public async Task<Note?> GetNoteById(int NoteId)
    {
        try
        {
            return await _db.Notes.FindAsync(NoteId);
        }
        catch (Exception e)
        {
            _logger.LogError("[NotatRepository] note FirstOrDefault() failed when GetNoteById for NoteId {NoteId:0000}, error message: {e}", NoteId, e);
            throw new InvalidOperationException("[NotatRepository] GetNoteById() failed");
        }
    }

    public async Task Create(Note note)
    {
        try
        {
            await _db.Notes.AddAsync(note);
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[NotatRepository] note creation failed for note {@note}, error message: {e}", note, e.Message);
        }
    }

    public async Task Update(Note note)
    {
        try
        {
            _db.Notes.Update(note);
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[NotatRepository] note update failed for note {@note}, error message: {e}", note, e.Message);
        }
    }

    public async Task<bool> Delete(int NoteId)
    {   
        var note = await _db.Notes.FindAsync(NoteId); // Find the Note object by its ID
        if (note != null)
        {
        _db.Notes.Remove(note);  // Now pass the object to Remove, not the ID
        await _db.SaveChangesAsync();
        return true;
    }
    else
    {
        _logger.LogError("Finner ikke notat med id: {NoteId}", NoteId);
        return false;
    }
    }

    public async Task<bool> DeleteConfirmed(int NoteId)
    {
        var notat = await _db.Notes.FindAsync(NoteId); // Use FindAsync to get the note by ID
        if (notat != null)
        {
            _db.Notes.Remove(notat); // Remove the found note
            await _db.SaveChangesAsync(); // Save changes
            return true;
        }
        return false;
}
}
