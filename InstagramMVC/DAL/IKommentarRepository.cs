using InstagramMVC.Models;


namespace InstagramMVC.DAL
{
    public interface IKommentarRepository
    {
        Task<IEnumerable<Kommentar>> GetALL();
        Task <Kommentar> GetKommentarById();

        Task<int?> GetBildeId(int id);
        Task Create(Kommentar kommentar);
        //Task Update(Kommentar kommentar);
        //Task<bool> Delete(int id);

    }
}