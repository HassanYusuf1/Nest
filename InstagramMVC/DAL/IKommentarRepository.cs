using InstagramMVC.Models;


namespace InstagramMVC.DAL
{
    public interface IKommentarRepository
    {
        Task<IEnumerable<Kommentar>> GetAll();
        Task <Kommentar?> GetKommentarById(int id);

        Task<int?> GetBildeId(int id);
        Task<int?> GetNoteId(int id);
        
        Task Create(Kommentar kommentar);
        Task Update(Kommentar kommentar);
        Task<bool> Delete(int id);

    }
}