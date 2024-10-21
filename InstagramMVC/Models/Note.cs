namespace webappinsta.Models
{
    public class Note
    {
        public int NoteId {get; set;}
        public string Tittel {get; set;}
        public string Innhold {get; set;}
        public Datetime OpprettetDato {get; set;}
        public int BrukerId {get; set;}
        public List <Kommentar>? Kommentarer {get; set;}
    }
}