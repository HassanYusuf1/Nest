using InstagramMVC.Models;

namespace InstagramMVC.ViewModels
{
    public class NoteCommentViewModel
    {
        public Note? Note { get; set; } 
        public CommentViewModel? CommentViewModel { get; set; }  
    }
}
