using InstagramMVC.Models;

namespace InstagramMVC.ViewModels
{
    public class KommentarViewModel
    {
        public IEnumerable<Kommentar> Kommentarer;
        public string? CurrentViewName;

        public KommentarViewModel(IEnumerable<Kommentar> kommentarer, string? currentViewName)
        {
            Kommentarer = kommentarer;
            CurrentViewName = currentViewName;
        }
    }
}