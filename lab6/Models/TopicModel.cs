using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace lab6.Models
{
    public class TopicModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="The title needs to be filled.")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string DateCreated { get; set; }
        public DateTime DateEdited { get; set; } = DateTime.Now;   
        public int ReplyCount { get; set; }

        public int? ForumId { get; set; }
        public ForumCategoryModel Forum { get; set; }

        public int? AuthorId { get; set; }
        public UserModel Author { get; set; }

        public int? LastReplyId { get; set; }

        public virtual ICollection<ReplyModel> Replies { get; set; }
        public virtual ICollection<PictureModel> Pictures { get; set; }
        public int PictureCount { get; set; } = 0;

        public TopicModel()
        {
            Replies = new List<ReplyModel>();
            Pictures = new List<PictureModel>();
        }
    }
}
