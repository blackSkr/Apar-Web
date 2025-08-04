using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AparWebAdmin.Models;

namespace AparWebAdmin.Controllers
{
    public class IntervalPetugasController : Controller
    {
        private readonly HttpClient _httpClient;

        public IntervalPetugasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: IntervalPetugas
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/interval-petugas");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var intervalPetugasList = JsonSerializer.Deserialize<List<IntervalPetugas>>(jsonData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<IntervalPetugas>();
                    
                    return View(intervalPetugasList);
                }
                else
                {
                    ViewBag.Error = "Gagal mengambil data interval petugas dari server";
                    return View(new List<IntervalPetugas>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<IntervalPetugas>());
            }
        }

        // GET: IntervalPetugas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IntervalPetugas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IntervalPetugas intervalPetugas)
        {
            if (!ModelState.IsValid)
            {
                return View(intervalPetugas);
            }

            try
            {
                var jsonContent = JsonSerializer.Serialize(intervalPetugas);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/interval-petugas", httpContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Interval petugas berhasil ditambahkan!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                        ViewBag.Error = errorResponse?.ContainsKey("message") == true ? errorResponse["message"].ToString() : "Gagal menambah interval petugas";
                    }
                    catch
                    {
                        ViewBag.Error = "Gagal menambah interval petugas";
                    }
                    return View(intervalPetugas);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(intervalPetugas);
            }
        }

        // GET: IntervalPetugas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/interval-petugas/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var intervalPetugas = JsonSerializer.Deserialize<IntervalPetugas>(jsonData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (intervalPetugas == null)
                    {
                        TempData["Error"] = "Interval petugas tidak ditemukan";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    return View(intervalPetugas);
                }
                else
                {
                    TempData["Error"] = "Interval petugas tidak ditemukan";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: IntervalPetugas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IntervalPetugas intervalPetugas)
        {
            if (!ModelState.IsValid)
            {
                return View(intervalPetugas);
            }

            try
            {
                var jsonContent = JsonSerializer.Serialize(intervalPetugas);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"api/interval-petugas/{id}", httpContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Interval petugas berhasil diupdate!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                        ViewBag.Error = errorResponse?.ContainsKey("message") == true ? errorResponse["message"].ToString() : "Gagal update interval petugas";
                    }
                    catch
                    {
                        ViewBag.Error = "Gagal update interval petugas";
                    }
                    return View(intervalPetugas);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(intervalPetugas);
            }
        }

        // POST: IntervalPetugas/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/interval-petugas/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Interval petugas berhasil dihapus!";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                        TempData["Error"] = errorResponse?.ContainsKey("message") == true ? errorResponse["message"].ToString() : "Gagal menghapus interval petugas";
                    }
                    catch
                    {
                        TempData["Error"] = "Gagal menghapus interval petugas";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // API helper untuk dropdown
        public async Task<List<DropdownItem>> GetIntervalDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/interval-petugas");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var intervalList = JsonSerializer.Deserialize<List<IntervalPetugas>>(jsonData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<IntervalPetugas>();
                    
                    return intervalList.Select(i => new DropdownItem
                    {
                        Id = i.Id,
                        Nama = $"{i.NamaInterval} ({i.Deskripsi})"
                    }).ToList();
                }
                
                return new List<DropdownItem>();
            }
            catch
            {
                return new List<DropdownItem>();
            }
        }
    }
}