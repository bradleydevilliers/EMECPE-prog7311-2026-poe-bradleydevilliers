using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
//(Microsft,2026)
namespace TechMoveGLMS.Shared.Models.ViewModels
{
    public class ServiceRequestViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Contract")]
        public int ContractId { get; set; }
        
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = "Basic";
        
        [Display(Name = "Distance (km)")]
        public decimal Distance { get; set; }
        
        [Display(Name = "Priority Service")]
        public bool IsPriority { get; set; }
        
        public List<SelectListItem>? Contracts { get; set; }
        public List<SelectListItem>? ServiceLevels { get; set; }
    }
}
// Microsoft, 2026. Entity Framework Core Documentation.[Online]  Available at:
//  https://learn.microsoft.com/en-us/ef/core/