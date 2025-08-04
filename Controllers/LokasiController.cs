// controller// LokasiController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using AparWebAdmin.Models;
using System.Text;

namespace AparWebAdmin.Controllers
{
    public class LokasiController : Controller
    {
        private readonly HttpClient _httpClient;
        public LokasiController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // Helper: load dropdown PIC Petugas
        private async Task LoadPetugasDropdownAsync(int? selectedPetugasId = null)
        {
            List<Petugas> list;
            try
            {
                var resp = await _httpClient.GetAsync("api/petugas");
                if (!resp.IsSuccessStatusCode)
                    list = new List<Petugas>();
                else
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<Petugas>>(json)
                           ?? new List<Petugas>();
                }
            }
            catch
            {
                list = new List<Petugas>();
            }

            ViewBag.PetugasSelectList = new SelectList(
                list.Select(p => new { p.Id, Display = $"{p.BadgeNumber} – {p.Role}" }),
                "Id",
                "Display",
                selectedPetugasId
            );
        }

        // GET: /Lokasi
        public async Task<IActionResult> Index()
        {
            // 1) Ambil lokasi
            var respLok = await _httpClient.GetAsync("api/lokasi");
            List<Lokasi> data;
            if (!respLok.IsSuccessStatusCode)
            {
                ViewBag.Error = $"API Lokasi error: {respLok.StatusCode}";
                data = new List<Lokasi>();
            }
            else
            {
                var jsonLok = await respLok.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<Lokasi>>(jsonLok)
                       ?? new List<Lokasi>();
            }

            // 2) Ambil petugas
            var respPet = await _httpClient.GetAsync("api/petugas");
            var petugasList = new List<Petugas>();
            if (respPet.IsSuccessStatusCode)
            {
                var jsonPet = await respPet.Content.ReadAsStringAsync();
                petugasList = JsonConvert.DeserializeObject<List<Petugas>>(jsonPet)
                              ?? new List<Petugas>();
            }

            // 3) Map PICBadgeNumber & PICJabatan
            foreach (var loc in data)
            {
                if (loc.PICPetugasId.HasValue)
                {
                    var p = petugasList.FirstOrDefault(x => x.Id == loc.PICPetugasId.Value);
                    loc.PICBadgeNumber = p?.BadgeNumber;
                    loc.PICJabatan     = p?.Role;
                }
            }

            return View(data);
        }

        // GET: /Lokasi/Create
        public async Task<IActionResult> Create()
        {
            await LoadPetugasDropdownAsync();
            return View();
        }

        // POST: /Lokasi/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lokasi lokasi)
        {
            if (!ModelState.IsValid)
            {
                await LoadPetugasDropdownAsync(lokasi.PICPetugasId);
                return View(lokasi);
            }

            var dto = new
            {
                nama         = lokasi.Nama,
                picPetugasId = lokasi.PICPetugasId,
                lat          = lokasi.Lat,
                @long        = lokasi.Long
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(dto),
                Encoding.UTF8,
                "application/json"
            );
            var resp = await _httpClient.PostAsync("api/lokasi", content);
            if (resp.IsSuccessStatusCode)
            {
                TempData["Success"] = "Lokasi berhasil ditambahkan";
                return RedirectToAction(nameof(Index));
            }

            var errJson = await resp.Content.ReadAsStringAsync();
            dynamic errObj = JsonConvert.DeserializeObject(errJson);
            ViewBag.Error = errObj?.message ?? "Gagal menyimpan lokasi";
            await LoadPetugasDropdownAsync(lokasi.PICPetugasId);
            return View(lokasi);
        }

        // GET: /Lokasi/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Ambil data lokasi
            var resp = await _httpClient.GetAsync($"api/lokasi/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = "Lokasi tidak ditemukan";
                return RedirectToAction(nameof(Index));
            }
            var json = await resp.Content.ReadAsStringAsync();
            var lokasi = JsonConvert.DeserializeObject<Lokasi>(json) ?? new Lokasi();

            // Dropdown
            await LoadPetugasDropdownAsync(lokasi.PICPetugasId);
            return View(lokasi);
        }

        // POST: /Lokasi/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lokasi lokasi)
        {
            if (!ModelState.IsValid)
            {
                await LoadPetugasDropdownAsync(lokasi.PICPetugasId);
                return View(lokasi);
            }

            var dto = new
            {
                id           = lokasi.Id,
                nama         = lokasi.Nama,
                picPetugasId = lokasi.PICPetugasId,
                lat          = lokasi.Lat,
                @long        = lokasi.Long
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(dto),
                Encoding.UTF8,
                "application/json"
            );
            var resp = await _httpClient.PutAsync($"api/lokasi/{id}", content);
            if (resp.IsSuccessStatusCode)
            {
                TempData["Success"] = "Lokasi berhasil diupdate";
                return RedirectToAction(nameof(Index));
            }

            var errJson2 = await resp.Content.ReadAsStringAsync();
            dynamic errObj2 = JsonConvert.DeserializeObject(errJson2);
            ViewBag.Error = errObj2?.message ?? "Gagal update lokasi";
            await LoadPetugasDropdownAsync(lokasi.PICPetugasId);
            return View(lokasi);
        }

        // POST: /Lokasi/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resp = await _httpClient.DeleteAsync($"api/lokasi/{id}");
                if (resp.IsSuccessStatusCode)
                    TempData["Success"] = "Lokasi berhasil dihapus";
                else
                {
                    var errJson3 = await resp.Content.ReadAsStringAsync();
                    dynamic errObj3 = JsonConvert.DeserializeObject(errJson3);
                    TempData["Error"] = errObj3?.message ?? "Gagal menghapus lokasi";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
