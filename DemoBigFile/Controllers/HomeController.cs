using DemoBigFile.Models;
using DemoBigFile.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment,
            IOptions<CommonConstants> constants, IRepository repository)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _constants = constants.Value;
            _repository = repository;
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
        public IActionResult ImportBigFile(IList<IFormFile> files)
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
                    for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                    {
                        donation = new DonationViewModel();
                        donation.id = int.Parse(workSheet.Cells[i, 1].Value.ToString());
                        donation.type_id = int.Parse(workSheet.Cells[i, 2].Value.ToString());
                        donation.class_id = int.Parse(workSheet.Cells[i, 3].Value.ToString());
                        donation.source_id = int.Parse(workSheet.Cells[i, 4].Value.ToString());
                        donation.parent_id = int.Parse(workSheet.Cells[i, 5].Value.ToString());
                        donation.elec_code = workSheet.Cells[i, 6].Value.ToString();
                        donation.senator_id = workSheet.Cells[i, 7].Value.ToString();
                        donation.state = int.Parse(workSheet.Cells[i, 8].Value.ToString());
                        donation.no_history = int.Parse(workSheet.Cells[i, 9].Value.ToString());
                        donation.image_url = workSheet.Cells[i, 10].Value.ToString();
                        donation.year = workSheet.Cells[i, 11].Value.ToString();
                        donation.month = workSheet.Cells[i, 12].Value.ToString();
                        donation.day = workSheet.Cells[i, 13].Value.ToString();
                        donation.amount = int.Parse(workSheet.Cells[i, 14].Value.ToString());
                        donation.total_number = int.Parse(workSheet.Cells[i, 15].Value.ToString());
                        donation.company_name = workSheet.Cells[i, 16].Value.ToString();
                        donation.representative_name = workSheet.Cells[i, 17].Value.ToString();
                        donation.postal_code = workSheet.Cells[i, 18].Value.ToString();
                        donation.address = workSheet.Cells[i, 19].Value.ToString();
                        donation.occupation_id = int.Parse(workSheet.Cells[i, 20].Value.ToString());
                        donation.deduction = int.Parse(workSheet.Cells[i, 21].Value.ToString());
                        donation.missed_message = workSheet.Cells[i, 22].Value.ToString();
                        donation.created_at = DateTime.Parse(workSheet.Cells[i, 23].Value.ToString());
                        donation.created_user_id = int.Parse(workSheet.Cells[i, 24].Value.ToString());
                        donation.created_process = workSheet.Cells[i, 25].Value.ToString();
                        donation.updated_at = DateTime.Parse(workSheet.Cells[i, 26].Value.ToString());
                        donation.updated_user_id = int.Parse(workSheet.Cells[i, 27].Value.ToString());
                        donation.updated_process = workSheet.Cells[i, 28].Value.ToString();
                        donation.deleted_at = DateTime.Parse(workSheet.Cells[i, 29].Value.ToString());
                        donation.deleted_user_id = int.Parse(workSheet.Cells[i, 30].Value.ToString());
                        donation.deleted_process = workSheet.Cells[i, 31].Value.ToString();
                        donation.times_object_last_batch_process = int.Parse(workSheet.Cells[i, 32].Value.ToString());
                        donation.batch_processing = int.Parse(workSheet.Cells[i, 33].Value.ToString());
                        donation.receipt_no = workSheet.Cells[i, 34].Value.ToString();

                        _repository.Create(donation);
                    }
                }
                return new OkObjectResult(filePath);
            }
            return new NoContentResult();
        }
    }
}
