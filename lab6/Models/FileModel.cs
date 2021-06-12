using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lab6.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(22, ErrorMessage = "The maximum length of the name is 22 symbols.")]
        public string Name { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }

        public byte[] File { get; set; }

        [Required]    
        public int FolderId { get; set; }
        public FolderModel Folder { get; set; }
    
    }
}
