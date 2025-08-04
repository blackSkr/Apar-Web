using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AparWebAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Linq;

namespace AparWebAdmin.Controllers
{
    public class PetugasController : Controller
    {
        private readonly HttpClient _httpClient;

        public PetugasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // Helper: Load Dropdowns (Role, Interval, Lokasi)
        private async Task LoadDropdownAsync(string? selectedRole = null, int? selectedInterval = null, int? selectedLokasi = null)
        {
            // ROLE (ambil dari /api/petugas, distinct role)
            var roles = new List<string>();
            try
            {
                var resp = await _httpClient.GetAsync("api/petugas");
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var allPetugas = JsonConvert.DeserializeObject<List<Petugas>>(json) ?? new List<Petugas>();
                    roles = allPetugas.Where(p => !string.IsNullOrWhiteSpace(p.Role)).Select(p => p.Role!).Distinct().OrderBy(r => r).ToList();
                }
            }
            catch { }
            ViewBag.Roles = new SelectList(roles, selectedRole);

            // INTERVAL
            var intervals = new List<DropdownItem>();
            try
            {
                var resp = await _httpClient.GetAsync("api/interval-petugas");
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var raw = JsonConvert.DeserializeObject<List<dynamic>>(json) ?? new List<dynamic>();
                    foreach (var item in raw)
                    {
                        intervals.Add(new DropdownItem { Id = (int)item.Id, Nama = (string)item.NamaInterval });
                    }
                }
            }
            catch { }
            ViewBag.IntervalList = new SelectList(intervals, "Id", "Nama", selectedInterval);

            // LOKASI
            var lokasiList = new List<DropdownItem>();
            try
            {
                var resp = await _httpClient.GetAsync("api/lokasi");
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var raw = JsonConvert.DeserializeObject<List<dynamic>>(json) ?? new List<dynamic>();
                    foreach (var item in raw)
                    {
                        lokasiList.Add(new DropdownItem { Id = (int)item.Id, Nama = (string)item.Nama });
                    }
                }
            }
            catch { }
            ViewBag.LokasiList = new SelectList(lokasiList, "Id", "Nama", selectedLokasi);
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
                    var petugasList = JsonConvert.DeserializeObject<List<Petugas>>(json) ?? new List<Petugas>();
                    return View(petugasList);
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
        public async Task<IActionResult> Create()
        {
            await LoadDropdownAsync();
            return View();
        }

        // POST: Petugas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Petugas petugas)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownAsync(petugas.Role, petugas.IntervalPetugasId, petugas.LokasiId);
                return View(petugas);
            }

            try
            {
                var json = JsonConvert.SerializeObject(new
                {
                    badgeNumber = petugas.BadgeNumber,
                    role = petugas.Role,
                    intervalPetugasId = petugas.IntervalPetugasId,
                    lokasiId = petugas.LokasiId
                });
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
                await LoadDropdownAsync(petugas.Role, petugas.IntervalPetugasId, petugas.LokasiId);
                return View(petugas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                await LoadDropdownAsync(petugas.Role, petugas.IntervalPetugasId, petugas.LokasiId);
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
                    await LoadDropdownAsync(petugas?.Role, petugas?.IntervalPetugasId, petugas?.LokasiId);
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
                await LoadDropdownAsync(petugas.Role, petugas.IntervalPetugasId, petugas.LokasiId);
                return View(petugas);
            }

            try
            {
                var json = JsonConvert.SerializeObject(new
                {
                    badgeNumber = petugas.BadgeNumber,
                    role = petugas.Role,
                    intervalPetugasId = petugas.IntervalPetugasId,
                    lokasiId = petugas.LokasiId
                });
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
                await LoadDropdownAsync(petugas.Role, petugas.IntervalPetugasId, petugas.LokasiId);
                return View(petugas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                await LoadDropdownAsync(petugas.Role, petugas.IntervalPetugasId, petugas.LokasiId);
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
