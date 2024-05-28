using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DBPROJ_V2.Models
{
    public class UserProfile
    {
        [Key]
        public int UserProfileId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Location { get; set; }

        public IdentityUser User { get; set; }  // Navigation property
    }
}
