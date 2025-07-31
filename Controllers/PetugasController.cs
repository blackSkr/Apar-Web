using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AparWebAdmin.Models;
using System.Text;

namespace AparWebAdmin.Controllers
{
    public class PetugasController : Controller
    {
        private readonly HttpClient _httpClient;

        public PetugasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: Petugas
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/petugas");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var petugas = JsonConvert.DeserializeObject<List<Petugas>>(json) ?? new List<Petugas>();
                    return View(petugas);
                }
                
                ViewBag.Error = $"API returned: {response.StatusCode}";
                return View(new List<Petugas>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<Petugas>());
            }
        }

        // GET: Petugas/Create
        public IActionResult Create()
        {
            ViewBag.Roles = PetugasRoles.AvailableRoles;
            return View();
        }

        // POST: Petugas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Petugas petugas)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = PetugasRoles.AvailableRoles;
                return View(petugas);
            }

            try
            {
                var json = JsonConvert.SerializeObject(petugas);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/petugas", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Petugas berhasil ditambahkan";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                ViewBag.Error = errorObj?.message ?? "Gagal menyimpan petugas";
                
                ViewBag.Roles = PetugasRoles.AvailableRoles;
                return View(petugas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                ViewBag.Roles = PetugasRoles.AvailableRoles;
                return View(petugas);
            }
        }

        // GET: Petugas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/petugas/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var petugas = JsonConvert.DeserializeObject<Petugas>(json);
                    ViewBag.Roles = PetugasRoles.AvailableRoles;
                    return View(petugas);
                }
                
                TempData["Error"] = "Petugas tidak ditemukan";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Petugas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Petugas petugas)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = PetugasRoles.AvailableRoles;
                return View(petugas);
            }

            try
            {
                var json = JsonConvert.SerializeObject(petugas);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"api/petugas/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Petugas berhasil diupdate";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                ViewBag.Error = errorObj?.message ?? "Gagal update petugas";
                
                ViewBag.Roles = PetugasRoles.AvailableRoles;
                return View(petugas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                ViewBag.Roles = PetugasRoles.AvailableRoles;
                return View(petugas);
            }
        }

        // POST: Petugas/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/petugas/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Petugas berhasil dihapus";
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorObj = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                    TempData["Error"] = errorObj?.message ?? "Gagal menghapus petugas";
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