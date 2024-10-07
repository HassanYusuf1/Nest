namespace Instagram.Models{
public class Bruker {
    public int Id { get; set;}
    public string BrukerNavn {get; set;}
    public string Epost {get; set;}
    public String Passord {get; set;}


    public List <Innlegg>  Innlegg {get; set;} //Lager Liste over brukerens innlegg
     public List <Kommentar> Kommentarer {get; set;} // L


}

    
}   