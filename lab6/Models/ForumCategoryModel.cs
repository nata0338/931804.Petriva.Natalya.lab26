using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace lab6.Models
{
    public class ForumCategoryModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<TopicModel> Topics { get; set; }

        public ForumCategoryModel()
        {
            Topics = new List<TopicModel>();
        }

    }
}
