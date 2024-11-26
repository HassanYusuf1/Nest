using Nest.Models;

namespace Nest.ViewModels
{
    public class PictureCommentViewModel
    {
        public Picture? Picture { get; set; } 
        public CommentViewModel? CommentViewModel { get; set; }  
    }
}
