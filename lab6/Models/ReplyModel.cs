using System;
using System.Collections.Generic;

namespace lab6.Models
{
    public class ReplyModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string DateCreated { get; set; }
        public DateTime DateEdited { get; set; } = DateTime.Now;

        public int? TopicId { get; set; }
        public TopicModel Topic { get; set; }

        public int? AuthorId { get; set; }
        public UserModel Author { get; set; }

        public virtual ICollection<PictureModel> Pictures { get; set; }
        public int PictureCount { get; set; } = 0;

        public ReplyModel()
        {
            Pictures = new List<PictureModel>();
        }
    }
}   
