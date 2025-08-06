// Controllers/MaintenanceController.cs
using AparWebAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace AparWebAdmin.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly HttpClient _http;
        private readonly string _apiUrl = "http://localhost:3000/api/perawatan";

        public MaintenanceController()
        {
            _http = new HttpClient();
        }

        public async Task<IActionResult> Index(int? id)
        {
            HttpResponseMessage res;

            if (id == null)
                res = await _http.GetAsync($"{_apiUrl}/all");
            else
            {
                res = await _http.GetAsync($"{_apiUrl}/history/{id}");
                ViewBag.AparId = id;
            }

            if (!res.IsSuccessStatusCode)
                return View("Error", "Gagal load data maintenance");

            var json = await res.Content.ReadAsStringAsync();
            var wrapper = JsonConvert.DeserializeObject<dynamic>(json);
            var list = JsonConvert.DeserializeObject<List<Maintenance>>(Convert.ToString(wrapper.data));
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var res = await _http.GetAsync($"{_apiUrl}/details/{id}");
            if (!res.IsSuccessStatusCode)
                return View("Error", "Gagal load detail maintenance");

            var json = await res.Content.ReadAsStringAsync();
            var wrapper = JsonConvert.DeserializeObject<JObject>(json);

            var data = wrapper["data"]?.ToObject<Maintenance>();
            if (data == null) return View("Error", "Data tidak ditemukan");

            // Map checklist
            var checklistJson = wrapper["data"]?["checklist"]?.ToObject<List<JObject>>() ?? new();
            var checklist = checklistJson.Select(c => new ChecklistJawaban
            {
                ChecklistId = (int)(c["ChecklistId"] ?? 0),
                Jawaban = (bool)(c["Dicentang"] ?? false),
                Keterangan = (string?)c["Keterangan"] ?? "",
                PertanyaanChecklist = (string?)c["Pertanyaan"] ?? ""
            }).ToList();
            data.Checklist = checklist;

            // Map photos
            data.Photos = wrapper["data"]?["photos"]?.ToObject<List<FotoPemeriksaan>>() ?? new();

            return View(data);
        }



        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            string badge = "BN-01"; // TODO: Replace with user login
            var res = await _http.GetAsync($"http://localhost:3000/api/peralatan/with-checklist?id={id}&badge={badge}");
            if (!res.IsSuccessStatusCode)
                return View("Error", "Gagal load data apar");

            var json = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Maintenance>(json);
            data.TanggalPemeriksaan = DateTime.Now;
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Maintenance model, List<IFormFile> fotos)
        {
            var multipart = new MultipartFormDataContent();

            multipart.Add(new StringContent(model.PeralatanId.ToString()), "aparId");
            multipart.Add(new StringContent(model.BadgeNumber), "badgeNumber");
            multipart.Add(new StringContent(model.TanggalPemeriksaan.ToString("yyyy-MM-dd")), "tanggal");
            multipart.Add(new StringContent(model.IntervalPetugasId?.ToString() ?? ""), "intervalPetugasId");
            multipart.Add(new StringContent(model.Kondisi ?? ""), "kondisi");
            multipart.Add(new StringContent(model.CatatanMasalah ?? ""), "catatanMasalah");
            multipart.Add(new StringContent(model.Rekomendasi ?? ""), "rekomendasi");
            multipart.Add(new StringContent(model.TindakLanjut ?? ""), "tindakLanjut");
            multipart.Add(new StringContent(model.Tekanan?.ToString() ?? ""), "tekanan");
            multipart.Add(new StringContent(model.JumlahMasalah?.ToString() ?? ""), "jumlahMasalah");

            var checklistJson = JsonConvert.SerializeObject(model.Checklist.Select(c => new
            {
                checklistId = c.ChecklistId,
                condition = c.Jawaban ? "Baik" : "Tidak",
                alasan = c.Alasan ?? ""
            }));
            multipart.Add(new StringContent(checklistJson, Encoding.UTF8, "application/json"), "checklist");

            foreach (var file in fotos)
            {
                if (file.Length > 0)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    multipart.Add(fileContent, "fotos", file.FileName);
                }
            }

            var response = await _http.PostAsync($"{_apiUrl}/submit", multipart);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Gagal menyimpan data maintenance";
                return View(model);
            }

            return RedirectToAction("Index", new { id = model.PeralatanId });
        }
    }
}
