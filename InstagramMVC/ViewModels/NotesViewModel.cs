using InstagramMVC.Models;

namespace InstagramMVC.ViewModels
{
    public class NotesViewModel
    {
        public IEnumerable<Note> Notes;
        public string? CurrentViewName;

        public NotesViewModel(IEnumerable<Note> notes, string? currentViewName)
        {
            Notes = notes;
            CurrentViewName = currentViewName;
        }
    }
}