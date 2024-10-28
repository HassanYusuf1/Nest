using InstagramMVC.Models;

namespace InstagramMVC.ViewModels
{
    public class BildeKommentarViewModel
    {
         public Bilde Bilde { get; set; } 
        public IEnumerable<Kommentar> Kommentarer { get; set; }  
        public Kommentar NyKommentar { get; set; }   
    }
}
