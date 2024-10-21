using InstagramMVC.Models;


namespace InstagramMVC.ViewModels

{
    public class BildeViewModel
    {
        public IEnumerable <Bilde> Bilder {get; set;}

        public string? CurrentViewName;

        public BildeViewModel(IEnumerable <Bilde> bilder, string? currentViewName)
        {
            Bilder = bilder;
            CurrentViewName = currentViewName;
        }
    }
}