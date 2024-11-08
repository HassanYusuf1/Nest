using InstagramMVC.Models;

namespace InstagramMVC.ViewModels
{
    public class PictureCommentViewModel
    {
        public Picture? Picture { get; set; } 
        public CommentViewModel? CommentViewModel { get; set; }  
    }
}
