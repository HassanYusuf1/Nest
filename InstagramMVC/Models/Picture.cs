using System;
using System.ComponentModel.DataAnnotations;
using InstagramMVC.Models;
namespace InstagramMVC.Models {
    public class Picture {
        public int PictureId { get; set; }  
        public string? PictureUrl {get; set;}  // Lagrer filstien til picture
        public string? Title {get; set;} // Picturetekst


        // Beskrivelse for picture picturet  Maks 500 tegn
        [StringLength(500)]
        public String? Description {get; set;}

        public DateTime UploadDate {get; set;} // dato innlegget ble opprettet

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        
        public string? UserName {get; set;} 


    
}

}