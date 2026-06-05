using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
//(Microsoft,2026)
namespace TechMoveGLMS.Shared.Models.ViewModels
{
    public class ContractViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Client")]
        public int ClientId { get; set; }
        
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;
        
        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now.AddYears(1);
        
        [Required]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = "Basic";
        
        [Display(Name = "Signed Agreement (PDF only)")]
        public IFormFile? SignedAgreement { get; set; }
        
        public List<SelectListItem>? Clients { get; set; }
        public List<SelectListItem>? ServiceLevels { get; set; }
    }
}
// Microsoft, 2026. Entity Framework Core Documentation.[Online]  Available at:
//  https://learn.microsoft.com/en-us/ef/core/