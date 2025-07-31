// Models/Peralatan.cs
using System.ComponentModel.DataAnnotations;

namespace AparWebAdmin.Models
{
    public class Peralatan
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Kode peralatan wajib diisi")]
        [Display(Name = "Kode Peralatan")]
        [StringLength(50, ErrorMessage = "Kode maksimal 50 karakter")]
        public string Kode { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Jenis peralatan wajib dipilih")]
        [Display(Name = "Jenis Peralatan")]
        public int JenisId { get; set; }
        
        [Required(ErrorMessage = "Lokasi wajib dipilih")]
        [Display(Name = "Lokasi")]
        public int LokasiId { get; set; }
        
        [Required(ErrorMessage = "Spesifikasi wajib diisi")]
        [Display(Name = "Spesifikasi")]
        [StringLength(500, ErrorMessage = "Spesifikasi maksimal 500 karakter")]
        public string Spesifikasi { get; set; } = string.Empty;
        
        public Guid? TokenQR { get; set; }
        
        // Navigation properties untuk display
        public string? JenisNama { get; set; }
        public string? LokasiNama { get; set; }
        public int? IntervalPemeriksaanBulan { get; set; }
    }
}