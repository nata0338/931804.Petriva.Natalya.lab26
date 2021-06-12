using System.Collections.Generic;


namespace lab6.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        public int RoleId { get; set; }
        public RoleModel Role { get; set; }

        public virtual ICollection<TopicModel> Posts { get; set; }
        public virtual ICollection<ReplyModel> Replies { get; set; }

        public UserModel()
        {
            Posts = new List<TopicModel>();
            Replies = new List<ReplyModel>();
        }

    }

    public class RoleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
