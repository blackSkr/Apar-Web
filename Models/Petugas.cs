// Models/Petugas.cs - Updated sesuai struktur database
using System.ComponentModel.DataAnnotations;

namespace AparWebAdmin.Models
{
    public class Petugas
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Badge number harus diisi")]
        [StringLength(50, ErrorMessage = "Badge number maksimal 50 karakter")]
        [Display(Name = "Badge Number")]
        public string BadgeNumber { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "Role maksimal 50 karakter")]
        [Display(Name = "Role/Jabatan")]
        public string? Role { get; set; }
        
        [Display(Name = "Interval Petugas")]
        public int? IntervalPetugasId { get; set; }
        
        [Display(Name = "Lokasi")]
        public int? LokasiId { get; set; }
        
        // Navigation properties untuk join
        [Display(Name = "Interval")]
        public string? IntervalNama { get; set; }
        
        [Display(Name = "Lokasi")]
        public string? LokasiNama { get; set; }
        
        [Display(Name = "Interval Bulan")]
        public int? IntervalBulan { get; set; }
    }
}