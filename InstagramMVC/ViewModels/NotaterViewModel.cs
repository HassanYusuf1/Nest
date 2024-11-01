using InstagramMVC.Models;

namespace InstagramMVC.ViewModels
{
    public class NotaterViewModel
    {
        public IEnumerable<Note> Notes;
        public string? CurrentViewName;

        public NotaterViewModel(IEnumerable<Note> notes, string? currentViewName)
        {
            Notes = notes;
            CurrentViewName = currentViewName;
        }
    }
}