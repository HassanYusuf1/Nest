using InstagramMVC.Models;

namespace InstagramMVC.ViewModels
{
    public class BilderViewModel
    {
        public IEnumerable<Bilde> Bilder;
        public string? CurrentViewName;

        public BilderViewModel(IEnumerable<Bilde> bilder, string? currentViewName)
        {
            Bilder = bilder;
            CurrentViewName = currentViewName;
        }
    }
}