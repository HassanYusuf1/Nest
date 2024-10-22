using System;
using System.ComponentModel.DataAnnotations;
namespace InstagramMVC.Models {
    public class Bilde {
        public int Id {get; set;}  
        public string? Tittel {get; set;} // Bildetekst


        // Beskrivelse for bilde bildet  Maks 500 tegn
        [StringLength(500)]
        public String? Beskrivelse {get; set;}

        public DateTime OpprettetDato {get; set;} // dato innlegget ble opprettet

        public byte[] BildeData {get; set;} // Dato bildet ble lastet opp

        public int BrukerId {get; set;} 


    
}

}