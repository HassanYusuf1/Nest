namespace InstagramMVC.Models{
public class Kommentar {

    public int Id {get; set;}
    public string? Innehold {get;set;}  //Kommentarens innehold
    public DateTime OppretteDato{get; set ;} //Dato Kommentarer ble opprettet

    public  int InnleggId {get; set;}
    public Innlegg? innlegg {get; set; }  //Forhold til Innlegget;

    public Bruker? Bruker {get; set;}  //Forhold til brukeren som la til kommentaren ss

}
}