using InstgarmMVC.Models;

namespace InstagramMVC.ViewModel

{
    public class BildeViewModel
    {
        public IEnumerable <Bilde> Bilder { get; set;}

        public string? CurrentViewName {get; set;}

        public BildeViewModel(IEnumerable <Bilde> bilder, string? currentViewName)
        {
            Bilder = bilder;
            CurrentViewName = currentViewName
        }
    }
}