using System.ComponentModel.DataAnnotations;
//(Microsoft,2026)
namespace TechMoveGLMS.Shared.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
         [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
         [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
// Microsoft, 2026. Entity Framework Core Documentation.[Online]  Available at:
//  https://learn.microsoft.com/en-us/ef/core/