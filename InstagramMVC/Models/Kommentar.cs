namespace InstagramMVC.Models{
public class Comment {

    public int CommentId {get; set;}  // PK
    // Picture sin id
    public int? PictureId{get; set;} //FK

    public int? NoteId {get; set;}

    //Comment innehold
    public string?  CommentDescription  {get;set;}  

    public DateTime CommentTime {get; set;}

    // Relasjon til Picture
    public virtual Picture? Picture {get; set;} 

    public virtual Note? Note { get; set; }

    //public virtual IdentityUser? Bruker // legger til dette når vi har lagt inn identityuser
    public string? UserName {get; set;} 

    
}
}