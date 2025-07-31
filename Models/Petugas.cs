// Models/Petugas.cs
using System.ComponentModel.DataAnnotations;

namespace AparWebAdmin.Models
{
    public class Petugas
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Badge Number wajib diisi")]
        [Display(Name = "Badge Number")]
        [StringLength(50, ErrorMessage = "Badge Number maksimal 50 karakter")]
        public string BadgeNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Role wajib diisi")]
        [Display(Name = "Role")]
        [StringLength(50, ErrorMessage = "Role maksimal 50 karakter")]
        public string Role { get; set; } = string.Empty;
    }
    
    // Helper class untuk dropdown role
    public static class PetugasRoles
    {
        public static readonly List<string> AvailableRoles = new()
        {
            "Supervisor",
            "Technician", 
            "Inspector",
            "Operator",
            "Maintenance Staff",
            "Safety Officer"
        };
    }
}