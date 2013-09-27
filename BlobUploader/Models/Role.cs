using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlobUploader.Models
{
    [Table("webpages_Roles")]
    public class Role
    {
        public Role()
        {
            Members = new List<Membership>();
        }

        [Key]
        public int RoleId { get; set; }
        [StringLength(256)]
        public string RoleName { get; set; }

        public ICollection<Membership> Members { get; set; }
    }
}