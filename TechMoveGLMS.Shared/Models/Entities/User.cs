using System.ComponentModel.DataAnnotations;
//(Microsoft,2026)
namespace TechMoveGLMS.Shared.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = "Client"; // "Admin" or "Client"
        
        // Link to Client if role is Client
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
// Microsoft, 2026. Entity Framework Core Documentation.[Online]  Available at:
//  https://learn.microsoft.com/en-us/ef/core/