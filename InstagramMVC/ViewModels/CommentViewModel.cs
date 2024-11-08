using InstagramMVC.Models;

namespace InstagramMVC.ViewModels
{
    public class CommentViewModel
    {
        public IEnumerable<Comment> Comments;
        public string? CurrentViewName;

        public CommentViewModel(IEnumerable<Comment> comments, string? currentViewName)
        {
            Comments = comments;
            CurrentViewName = currentViewName;
        }
    }
}