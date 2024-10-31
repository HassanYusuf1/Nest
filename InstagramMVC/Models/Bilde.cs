using System;
using System.ComponentModel.DataAnnotations;
namespace InstagramMVC.Models {
    public class Bilde {
        public int BildeId { get; set; }  
        public string? BildeUrl {get; set;}  // Lagrer filstien til bilde
        public string? Tittel {get; set;} // Bildetekst


        // Beskrivelse for bilde bildet  Maks 500 tegn
        [StringLength(500)]
        public String? Beskrivelse {get; set;}

        public DateTime OpprettetDato {get; set;} // dato innlegget ble opprettet

        public virtual ICollection<Kommentar> Kommentarer { get; set; } = new List<Kommentar>();

        public string? UserName {get; set;} 


    
}

}