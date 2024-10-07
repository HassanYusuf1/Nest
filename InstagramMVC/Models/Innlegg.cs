using System;
namespace InstagramMVC.Models {
    public class Innlegg {
        public int Id {get; set;}  
        public string? BildeSti {get; set;}  // Lagrer filstien til bilde
        public string? BildeTekst {get; set;} // Bildetekst

        public DateTime OpprettetDato {get; set;} // dato innlegget ble opprettet

        public int BrukerId {get; set;} // Forhold til brukeren som eier innlegget 

        public List <Kommentar>? kommentarer {get; set;} //Liste over kommentarer til innlegget

    
}

}