// Controllers/PeralatanController.cs
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AparWebAdmin.Models;
using System.Text;

namespace AparWebAdmin.Controllers
{
    public class PeralatanController : Controller
    {
        private readonly HttpClient _httpClient;

        public PeralatanController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: Peralatan
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/peralatan/admin");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var peralatan = JsonConvert.DeserializeObject<List<Peralatan>>(json) ?? new List<Peralatan>();
                    return View(peralatan);
                }
                
                ViewBag.Error = $"API returned: {response.StatusCode}";
                return View(new List<Peralatan>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<Peralatan>());
            }
        }

        // GET: Peralatan/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                await LoadDropdownData();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading form: {ex.Message}";
                return View();
            }
        }

        // POST: Peralatan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Peralatan peralatan)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(peralatan);
            }

            try
            {
                var json = JsonConvert.SerializeObject(peralatan);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/peralatan/admin", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    
                    TempData["Success"] = "Peralatan berhasil ditambahkan";
                    TempData["TokenQR"] = result?.tokenQR?.ToString();
                    return RedirectToAction(nameof(Index));
                }
                
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                ViewBag.Error = errorObj?.message ?? "Gagal menyimpan peralatan";
                
                await LoadDropdownData();
                return View(peralatan);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                await LoadDropdownData();
                return View(peralatan);
            }
        }

        // GET: Peralatan/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/peralatan/admin/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var peralatan = JsonConvert.DeserializeObject<Peralatan>(json);
                    
                    await LoadDropdownData();
                    return View(peralatan);
                }
                
                TempData["Error"] = "Peralatan tidak ditemukan";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Peralatan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Peralatan peralatan)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(peralatan);
            }

            try
            {
                var json = JsonConvert.SerializeObject(peralatan);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"api/peralatan/admin/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Peralatan berhasil diupdate";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                ViewBag.Error = errorObj?.message ?? "Gagal update peralatan";
                
                await LoadDropdownData();
                return View(peralatan);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                await LoadDropdownData();
                return View(peralatan);
            }
        }

        // POST: Peralatan/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/peralatan/admin/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Peralatan berhasil dihapus";
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                    TempData["Error"] = errorObj?.message ?? "Gagal menghapus peralatan";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Peralatan/QR/5 - FIXED!
        public async Task<IActionResult> QR(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/peralatan/admin/{id}/qr");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var qrData = JsonConvert.DeserializeObject<QRCodeResponse>(json);
                    return View(qrData);
                }
                
                TempData["Error"] = "Peralatan tidak ditemukan";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Private method untuk load dropdown data
        private async Task LoadDropdownData()
        {
            try
            {
                // Load Lokasi
                var lokasiResponse = await _httpClient.GetAsync("api/lokasi");
                if (lokasiResponse.IsSuccessStatusCode)
                {
                    var lokasiJson = await lokasiResponse.Content.ReadAsStringAsync();
                    var lokasiList = JsonConvert.DeserializeObject<List<Lokasi>>(lokasiJson) ?? new List<Lokasi>();
                    ViewBag.LokasiList = lokasiList.Select(l => new DropdownItem 
                    { 
                        Id = l.Id, 
                        Nama = l.Nama 
                    }).ToList();
                }
                else
                {
                    ViewBag.LokasiList = new List<DropdownItem>();
                }

                // Load Jenis Peralatan
                var jenisResponse = await _httpClient.GetAsync("api/jenis-peralatan");
                if (jenisResponse.IsSuccessStatusCode)
                {
                    var jenisJson = await jenisResponse.Content.ReadAsStringAsync();
                    var jenisList = JsonConvert.DeserializeObject<List<JenisPeralatan>>(jenisJson) ?? new List<JenisPeralatan>();
                    ViewBag.JenisList = jenisList.Select(j => new DropdownItem 
                    { 
                        Id = j.Id, 
                        Nama = $"{j.Nama} ({j.IntervalPemeriksaanBulan} bulan)" 
                    }).ToList();
                }
                else
                {
                    ViewBag.JenisList = new List<DropdownItem>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dropdown data: {ex.Message}");
                ViewBag.LokasiList = new List<DropdownItem>();
                ViewBag.JenisList = new List<DropdownItem>();
            }
        }
    }
}