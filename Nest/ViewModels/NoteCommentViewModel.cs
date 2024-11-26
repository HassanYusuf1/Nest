using Nest.Models;

namespace Nest.ViewModels
{
    public class NoteCommentViewModel
    {
        public Note? Note { get; set; } 
        public CommentViewModel? CommentViewModel { get; set; }  
    }
}
