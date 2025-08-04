// Models/Lokasi.cs
using System.ComponentModel.DataAnnotations;

namespace AparWebAdmin.Models
{
    public class Lokasi
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nama lokasi wajib diisi")]
        [Display(Name = "Nama Lokasi")]
        public string Nama { get; set; } = string.Empty;

        [Display(Name = "PIC Petugas")]
        public int? PICPetugasId { get; set; }

        // Properti read-only untuk display
        [Display(Name = "Badge Number PIC")]
        public string? PICBadgeNumber { get; set; }

        [Display(Name = "Jabatan PIC")]
        public string? PICJabatan { get; set; }

        [Display(Name = "Latitude")]
        public decimal? Lat { get; set; }

        [Display(Name = "Longitude")]
        public decimal? Long { get; set; }
    }
}
