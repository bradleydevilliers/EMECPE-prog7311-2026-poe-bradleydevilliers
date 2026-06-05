using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
//(Microsoft,2026)
namespace TechMoveGLMS.Shared.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required (ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
         [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Display(Name = "Link to Client Account")]
        public int? ClientId { get; set; }
         public List<SelectListItem>? Clients { get; set; }
    }
}
// Microsoft, 2026. Entity Framework Core Documentation.[Online]  Available at:
//  https://learn.microsoft.com/en-us/ef/core/