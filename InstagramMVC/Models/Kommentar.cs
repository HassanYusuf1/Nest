namespace InstagramMVC.Models{
public class Kommentar {

    public int KommentarId {get; set;}  // PK
    // Bilde sin id
    public int id {get; set;} //FK

    //Kommentar innehold
    public string?  KommentarBeskrivelse  {get;set;}  

    public DateTime KommentarTid {get; set;}

    // Relasjon til Bilde
    public virtual Bilde? Bilde {get; set;} 

    //public virtual IdentityUser? Bruker // legger til dette når vi har lagt inn identityuser
    
}
}