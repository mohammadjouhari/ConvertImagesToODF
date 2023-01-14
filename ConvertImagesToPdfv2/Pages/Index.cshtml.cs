using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Pechkin.Synchronized;
using Pechkin;
using Microsoft.Extensions.FileProviders;

namespace ConvertImagesToPdfv2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        public IndexModel(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public IFormFile UploadPdf { get; set; }
        public FileResult OnPost()
        {
            // Check if uploaded file is image;
            var fileName = Path.GetFileName(UploadPdf.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                UploadPdf.CopyTo(fileStream);
            }
            var tem = "<img width=\"100%\" src=\"{0}\" />";
            var fullPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["BaseUrl"] + fileName;
            var html = string.Format(tem, fullPath);
            var htmlContent = String.Format("<html><body>{0}</body></html>", html);
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
            return File(pdfBytes, "application/octet-stream", "A.PDF");

        }
        public void OnGet()
        {

        }
    }
}