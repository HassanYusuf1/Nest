namespace InstagramMVC.Models{
public class Kommentar {

    public int KommentarId {get; set;}  // PK
    // Bilde sin id
    public int? BildeId{get; set;} //FK

    public int? NoteId {get; set;}

    //Kommentar innehold
    public string?  KommentarBeskrivelse  {get;set;}  

    public DateTime KommentarTid {get; set;}

    // Relasjon til Bilde
    public virtual Bilde? Bilde {get; set;} 

    public virtual Note? Note { get; set; }

    //public virtual IdentityUser? Bruker // legger til dette når vi har lagt inn identityuser
    public string? UserName {get; set;} 

    
}
}