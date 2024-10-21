using InstagramMVC.Models;

namespace InstagramMVC.DAL;

public interface INotatRepository
{
    Task<IEnumerable<Note>> GetAll();
    Task<Note?> GetNoteById(int id);
    Task Create(Note note);
    Task Update(Note note);
    Task<bool> Delete(int id);
    Task<bool> DeleteConfirmed(int id);
}