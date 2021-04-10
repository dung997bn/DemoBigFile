using DemoBigFile.Hubs;
using DemoBigFile.Models;
using DemoBigFile.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DemoBigFile.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly CommonConstants _constants;
        private readonly IRepository _repository;
        private readonly IHubContext<DocumentHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment,
            IOptions<CommonConstants> constants, IRepository repository, IHubContext<DocumentHub> hubContext)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _constants = constants.Value;
            _repository = repository;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ImportBigFile()
        {
            return View();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ImportBigFile(IList<IFormFile> files)
        {
            if (files != null && files.Count > 0)
            {
                var file = files[0];
                var filename = ContentDispositionHeaderValue
                                   .Parse(file.ContentDisposition)
                                   .FileName
                                   .Trim('"');

                string folder = _hostingEnvironment.WebRootPath + $@"\uploaded\excels";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filePath = Path.Combine(folder, filename);

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }

                //Import
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    DonationViewModel donation;
                    await _hubContext.Clients.All.SendAsync("ShowProgress", workSheet.Dimension.End.Row);
                    await Task.Delay(3000);
                    for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                    {
                        donation = new DonationViewModel();

                        int.TryParse(workSheet.Cells[i, 1].Value?.ToString() ?? null, out var id);
                        donation.id = id;

                        int.TryParse(workSheet.Cells[i, 2].Value?.ToString() ?? null, out var parent_id);
                        donation.parent_id = parent_id;

                        donation.elec_code = workSheet.Cells[i, 3].Value?.ToString() ?? "";
                        donation.senator_id = workSheet.Cells[i, 4].Value?.ToString() ?? "";

                        int.TryParse(workSheet.Cells[i, 5].Value.ToString() ?? null, out var amount);
                        donation.amount = amount;

                        donation.representative_name = workSheet.Cells[i, 6].Value?.ToString() ?? "";
                        donation.postal_code = workSheet.Cells[i, 7].Value?.ToString() ?? "";
                        donation.address = workSheet.Cells[i, 8].Value?.ToString() ?? "";

                        int.TryParse(workSheet.Cells[i, 9].Value?.ToString() ?? null, out var occupation_id);
                        donation.occupation_id = occupation_id;

                        donation.receipt_no = workSheet.Cells[i, 10].Value?.ToString() ?? "";

                        _repository.Create(donation);

                        await _hubContext.Clients.All.SendAsync("UpdateProgress", workSheet.Dimension.End.Row, i);
                    }
                }
                return new OkObjectResult(filePath);
            }
            return new NoContentResult();
        }
    }
}
