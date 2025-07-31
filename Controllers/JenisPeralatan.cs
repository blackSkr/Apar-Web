using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AparWebAdmin.Models;
using System.Text;

namespace AparWebAdmin.Controllers
{
    public class JenisPeralatanController : Controller
    {
        private readonly HttpClient _httpClient;

        public JenisPeralatanController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: JenisPeralatan
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/jenis-peralatan");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jenisPeralatan = JsonConvert.DeserializeObject<List<JenisPeralatan>>(json) ?? new List<JenisPeralatan>();
                    return View(jenisPeralatan);
                }
                
                ViewBag.Error = $"API returned: {response.StatusCode}";
                return View(new List<JenisPeralatan>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<JenisPeralatan>());
            }
        }

        // GET: JenisPeralatan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JenisPeralatan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JenisPeralatan jenisPeralatan)
        {
            if (!ModelState.IsValid)
            {
                return View(jenisPeralatan);
            }

            try
            {
                var json = JsonConvert.SerializeObject(jenisPeralatan);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/jenis-peralatan", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Jenis peralatan berhasil ditambahkan";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                ViewBag.Error = errorObj?.message ?? "Gagal menyimpan jenis peralatan";
                
                return View(jenisPeralatan);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(jenisPeralatan);
            }
        }

        // GET: JenisPeralatan/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/jenis-peralatan/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jenisPeralatan = JsonConvert.DeserializeObject<JenisPeralatan>(json);
                    return View(jenisPeralatan);
                }
                
                TempData["Error"] = "Jenis peralatan tidak ditemukan";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: JenisPeralatan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JenisPeralatan jenisPeralatan)
        {
            if (!ModelState.IsValid)
            {
                return View(jenisPeralatan);
            }

            try
            {
                var json = JsonConvert.SerializeObject(jenisPeralatan);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"api/jenis-peralatan/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Jenis peralatan berhasil diupdate";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                ViewBag.Error = errorObj?.message ?? "Gagal update jenis peralatan";
                
                return View(jenisPeralatan);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(jenisPeralatan);
            }
        }

        // POST: JenisPeralatan/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/jenis-peralatan/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Jenis peralatan berhasil dihapus";
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                    TempData["Error"] = errorObj?.message ?? "Gagal menghapus jenis peralatan";
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