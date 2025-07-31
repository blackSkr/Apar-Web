using Microsoft.AspNetCore.Mvc;
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

        // GET: Lokasi
        public async Task<IActionResult> Index()
        {
            try
            {
                Console.WriteLine("🔍 Calling API: api/lokasi");
                
                var response = await _httpClient.GetAsync("api/lokasi");
                Console.WriteLine($"🔍 Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"🔍 Response JSON: {json}");
                    
                    var lokasi = JsonConvert.DeserializeObject<List<Lokasi>>(json) ?? new List<Lokasi>();
                    return View(lokasi);
                }
                
                ViewBag.Error = $"API returned: {response.StatusCode}";
                return View(new List<Lokasi>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<Lokasi>());
            }
        }

        // GET: Lokasi/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lokasi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lokasi lokasi)
        {
            if (!ModelState.IsValid)
            {
                return View(lokasi);
            }

            try
            {
                var json = JsonConvert.SerializeObject(lokasi);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/lokasi", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Lokasi berhasil ditambahkan";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                ViewBag.Error = errorObj?.message ?? "Gagal menyimpan lokasi";
                
                return View(lokasi);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(lokasi);
            }
        }

        // GET: Lokasi/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Console.WriteLine($"🔍 Editing lokasi ID: {id}");
                
                var response = await _httpClient.GetAsync($"api/lokasi/{id}");
                Console.WriteLine($"🔍 Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"🔍 API Response JSON: {json}");
                    
                    var lokasi = JsonConvert.DeserializeObject<Lokasi>(json);
                    Console.WriteLine($"🔍 Deserialized Lokasi: Id={lokasi?.Id}, Nama={lokasi?.Nama}");
                    
                    return View(lokasi);
                }
                
                TempData["Error"] = "Lokasi tidak ditemukan";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Lokasi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lokasi lokasi)
        {
            if (!ModelState.IsValid)
            {
                return View(lokasi);
            }

            try
            {
                var json = JsonConvert.SerializeObject(lokasi);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"api/lokasi/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Lokasi berhasil diupdate";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                ViewBag.Error = errorObj?.message ?? "Gagal update lokasi";
                
                return View(lokasi);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(lokasi);
            }
        }

        // POST: Lokasi/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/lokasi/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Lokasi berhasil dihapus";
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                    TempData["Error"] = errorObj?.message ?? "Gagal menghapus lokasi";
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