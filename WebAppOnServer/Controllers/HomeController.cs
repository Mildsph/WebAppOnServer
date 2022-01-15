using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAppOnServer.Models;

namespace WebAppOnServer.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment Environment;
        public HomeController(IHostingEnvironment _environment)
        {
            Environment = _environment;
        }

        //**********Implement Pages********************
            public IActionResult Login()
            {
            
            return View();
            }

            public IActionResult Index()
        {
            string[] filepaths = Directory.GetFiles(Path.Combine(this.Environment.WebRootPath, "Files/"));

            List<FileModel> list = new List<FileModel>();
            foreach (string filepath in filepaths)
            {
                list.Add(new FileModel { FileName = Path.GetFileName(filepath) });
            }
            return View(list);
        }

        public FileResult DownloadFile(string filename)
        {
            string path = Path.Combine(this.Environment.WebRootPath, "Files/") + filename;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", filename);
        }

        public IActionResult Info()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //******File Function Controll************

        public IActionResult FileUpload()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Privacy(IFormFile file)
        {
            await UploadFile(file);
            TempData["msg"] = "File Uploaded successfully.";
            return View();
        }
        // Upload file on server
        public async Task<bool> UploadFile(IFormFile file)
        {
            string path = "";
            bool iscopied = false;
            try
            {
                if (file.Length > 0)
                {
                    string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    path = Path.GetFullPath(Path.Combine(this.Environment.WebRootPath, "Files/"));
                    using (var filestream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(filestream);
                    }
                    iscopied = true;
                }
                else
                {
                    iscopied = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return iscopied;
        }

        public IActionResult Delete(string imgdel)
        {
            imgdel = Path.Combine(this.Environment.WebRootPath, "Files/", imgdel);
            FileInfo fi = new FileInfo(imgdel);
            if (fi != null)
            {
                System.IO.File.Delete(imgdel);
                fi.Delete();
            }
            return RedirectToAction("Index");
        }
    }

}

