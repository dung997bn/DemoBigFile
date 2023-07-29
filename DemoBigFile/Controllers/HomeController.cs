using DemoBigFile.Extensions;
using DemoBigFile.Hubs;
using DemoBigFile.Models;
using DemoBigFile.Models.RelationalModels;
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
using System.Data;
using System.Data.SqlClient;
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
                var watch = new System.Diagnostics.Stopwatch();
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    watch.Start();
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
                    watch.Stop();
                    Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
                }
                return new OkObjectResult(filePath);
            }
            return new NoContentResult();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ImportBulkCopy(IList<IFormFile> files)
        {
            if (files != null && files.Count > 0)
            {
                try
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
                    var watch = new System.Diagnostics.Stopwatch();
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        watch.Start();
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                        await _hubContext.Clients.All.SendAsync("StartBulkCopy");
                        bool hasHeader = true;
                        DataTable tbl = new DataTable();
                        foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                        {
                            tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                        }
                        var startRow = hasHeader ? 2 : 1;
                        for (int rowNum = startRow; rowNum <= workSheet.Dimension.End.Row; rowNum++)
                        {
                            var wsRow = workSheet.Cells[rowNum, 1, rowNum, workSheet.Dimension.End.Column];
                            DataRow row = tbl.Rows.Add();
                            foreach (var cell in wsRow)
                            {
                                row[cell.Start.Column - 1] = cell.Text;
                            }
                        }

                        await _hubContext.Clients.All.SendAsync("CompleteConvertToDataTable");
                        string conStr = _constants.ConnectionStr;
                        using (SqlConnection sqlConn = new SqlConnection(conStr))
                        {
                            sqlConn.Open();
                            using (SqlBulkCopy sqlbc = new SqlBulkCopy(sqlConn))
                            {
                                //donation
                                sqlbc.DestinationTableName = "donation_1";
                                sqlbc.ColumnMappings.Add("id", "id");
                                sqlbc.ColumnMappings.Add("parent_id", "parent_id");
                                sqlbc.ColumnMappings.Add("elec_code", "elec_code");
                                sqlbc.ColumnMappings.Add("senator_id", "senator_id");
                                sqlbc.ColumnMappings.Add("amount", "amount");
                                sqlbc.ColumnMappings.Add("representative_name", "representative_name");
                                sqlbc.ColumnMappings.Add("postal_code", "postal_code");
                                sqlbc.ColumnMappings.Add("address", "address");
                                sqlbc.ColumnMappings.Add("occupation_id", "occupation_id");
                                sqlbc.ColumnMappings.Add("receipt_no", "receipt_no");

                                //history
                                //sqlbc.DestinationTableName = "history";
                                //sqlbc.ColumnMappings.Add("id", "id");
                                //sqlbc.ColumnMappings.Add("donation_id", "donation_id");
                                //sqlbc.ColumnMappings.Add("modified_datetime", "modified_datetime");
                                //sqlbc.ColumnMappings.Add("modified_person", "modified_person");
                                //sqlbc.ColumnMappings.Add("change_process", "change_process");
                                //sqlbc.ColumnMappings.Add("content_change", "content_change");
                                //sqlbc.ColumnMappings.Add("send_from_process", "send_from_process");
                                //sqlbc.BulkCopyTimeout = 0;
                                //sqlbc.BatchSize = 100;
                                sqlbc.WriteToServer(tbl);

                                await _hubContext.Clients.All.SendAsync("CompleteWriteToServer");

                                _repository.MergeTable();

                                await _hubContext.Clients.All.SendAsync("CompleteImport");
                            }
                            watch.Stop();
                            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
                        }
                        return new NoContentResult();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new BadRequestResult();
        }

        [HttpPost]
        public async Task<IActionResult> DemoRelationalDataInsert()
        {
            //Init data
            int stt = 1;
            List<ProductsEntity> productsList = new List<ProductsEntity>();
            List<ProductsVariantsEntity> productVariantsList = new List<ProductsVariantsEntity>();
            while (stt < 1000000)
            {
                int index = 1;

                ProductsEntity productData = new ProductsEntity();
                productData.product_code = $"Product_code aaaaaaaaaaaaaaaaaaaaa dddddddddddddsaaaaaaaaaaaaaaaaaaaaaaaaaaaaa {stt}";
                productData.product_name = $"Dell Laptop bbbbbbbbbbbbbbbbbbbbbbbbbbbb fewrtreyyyyyyyyyyyyyyyyyyyy   dfsfd{stt}";
                productData.base_price = stt % 2 == 0 ? 6700 : 7200;
                productData.reference_id = Guid.NewGuid().ToString();

                productsList.Add(productData);

                while (index < 4)
                {
                    var prodVariant = new ProductsVariantsEntity();
                    prodVariant.variant_name = $"12 GB RAM Variant yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy trrrrrrrrrrrrrrrrrrrrrrrrrrrr  dsfsdfsdfds{index}";
                    prodVariant.reference_id = productData.reference_id;
                    productVariantsList.Add(prodVariant);
                    index++;
                }
                stt++;
            }

            var tableProduct = productsList.ToDataTable(new List<string>());
            var tableProductVariant = productVariantsList.ToDataTable(new List<string>());

            await _hubContext.Clients.All.SendAsync("StartDemoRelationalDataInsert");
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            await _repository.DemoRelationalDataInsert(tableProduct, tableProductVariant);
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            await _hubContext.Clients.All.SendAsync("CompleteDemoRelationalDataInsert");
            return new NoContentResult();
        }

    }
}
