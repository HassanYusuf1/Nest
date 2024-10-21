using InstagramMVC.Models;


namespace InstagramMVC.ViewModel

{
    public class BildeViewModel
    {
        public IEnumerable <Bilde> Bilder;

        public string? CurrentViewName;

        public BildeViewModel(IEnumerable <Bilde> bilder, string? currentViewName)
        {
            Bilder = bilder;
            CurrentViewName = currentViewName;
        }
    }
}