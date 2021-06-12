using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lab6.Models
{
    public class FolderModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(22, ErrorMessage = "The maximum length of the name is 22 symbols.")]
        public string Name { get; set; }

        public int? ParentFolderId { get; set; }

        public virtual ICollection<FolderModel> Folders { get; set; }
        public virtual ICollection<FileModel> Files { get; set; }

        public FolderModel()
        {
            Folders = new List<FolderModel>();
            Files = new List<FileModel>();
        }
    }
}
