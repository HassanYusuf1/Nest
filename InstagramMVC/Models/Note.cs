using System;
using System.ComponentModel.DataAnnotations;

namespace InstagramMVC.Models
{
    public class Note
    {
        public int NoteId {get; set;}
        [Required]
        public string Tittel {get; set;} = string.Empty;
        [Required]
        public string Innhold {get; set;} = string.Empty;

        public DateTime OpprettetDato {get; set;} // dato innlegget ble opprettet
        public virtual ICollection<Kommentar> Kommentarer { get; set; } = new List<Kommentar>();
        public string? username {get; set;}

        
    }
}