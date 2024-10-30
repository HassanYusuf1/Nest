using System;

namespace InstagramMVC.Models
{
    public class Note
    {
        public int NoteId {get; set;}
        public string Tittel {get; set;}
        public string Innhold {get; set;}
        public int BrukerId {get; set;}

        public DateTime OpprettetDato {get; set;} // dato innlegget ble opprettet

        
    }
}