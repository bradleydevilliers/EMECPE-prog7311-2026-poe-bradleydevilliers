using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//(Microsoft,2026)
namespace TechMoveGLMS.Shared.Models.Entities
{
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }
        
        // Foreign Key to Contract
        public int ContractId { get; set; }
        
        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        public string Status { get; set; } = "Draft"; // "Draft", "Active", "Completed", "Cancelled"
        
        [Required]
        public string ServiceLevel { get; set; }
        
        [DataType(DataType.Currency)]
        public decimal LocalCost { get; set; } // Stored in ZAR (Rand)
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
// Microsoft, 2026. Entity Framework Core Documentation.[Online]  Available at:
//  https://learn.microsoft.com/en-us/ef/core/