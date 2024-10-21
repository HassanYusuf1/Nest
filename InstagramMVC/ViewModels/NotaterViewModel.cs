using InstagramMVC.Models;

namespace InstagramMVC.ViewModels
{
    public class NotaterViewModel
    {
        public IEnumerable<Note> Notater;
        public string? CurrentViewName;

        public NotaterViewModel(IEnumerable<Note> notater, string? currentViewName)
        {
            Notater = notater;
            CurrentViewName = currentViewName;
        }
    }
}