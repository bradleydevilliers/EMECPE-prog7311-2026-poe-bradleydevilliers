using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
//(Microsoft,2026)
namespace TechMoveGLMS.Shared.Models.Entities
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }
        
        // Foreign Key to Client
        public int ClientId { get; set; }
        
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        
        [Required]
        public string ServiceLevel { get; set; } // "Basic", "Premium", "Enterprise"
        
        public string Status { get; set; } = "Draft"; // "Draft", "Active", "Expired"
        
        // File handling - path to signed agreement PDF
        public string? SignedAgreementPath { get; set; }
        
        // Navigation property - One contract has many service requests
        [JsonIgnore]
        public List<ServiceRequest> ServiceRequests { get; set; } = new();
        
        // Computed property to check if contract is expired
        [NotMapped]
        public bool IsExpired => EndDate < DateTime.Now;
        
        // Computed property to check if contract is active
        [NotMapped]
        public bool IsActive => Status == "Active" && !IsExpired;
    }
}
// Microsoft, 2026. Entity Framework Core Documentation.[Online]  Available at:
//  https://learn.microsoft.com/en-us/ef/core/