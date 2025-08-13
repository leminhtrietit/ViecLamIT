using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ViecLamIT.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private static List<LocationData> _locations; // Cache dữ liệu để không phải đọc file nhiều lần

        public LocationsController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        private async Task LoadLocationsAsync()
        {
            if (_locations == null)
            {
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Data", "danh-sach-3321-xa-phuong.xls");
                var lines = await System.IO.File.ReadAllLinesAsync(filePath);

                _locations = lines.Skip(1) // Bỏ qua dòng tiêu đề
                                  .Select(line => line.Split(','))
                                  .Where(parts => parts.Length >= 3)
                                  .Select(parts => new LocationData
                                  {
                                      Province = parts[0].Trim(),
                                      District = parts[1].Trim(),
                                      Ward = parts[2].Trim()
                                  }).ToList();
            }
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            await LoadLocationsAsync();
            var provinces = _locations.Select(l => l.Province).Distinct().OrderBy(p => p).ToList();
            return Ok(provinces);
        }

        [HttpGet("districts/{province}")]
        public async Task<IActionResult> GetDistricts(string province)
        {
            await LoadLocationsAsync();
            var districts = _locations.Where(l => l.Province == province)
                                      .Select(l => l.District)
                                      .Distinct()
                                      .OrderBy(d => d)
                                      .ToList();
            return Ok(districts);
        }
    }

    // Lớp helper để chứa dữ liệu
    public class LocationData
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
    }
}
