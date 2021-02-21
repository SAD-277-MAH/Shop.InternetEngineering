using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Shop.Services.Upload.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Data.ViewModels.Upload;
using Shop.Repo.Infrastructure;
using Shop.Data.Context;

namespace Shop.Services.Upload.Service
{
    public class UploadService : IUploadService
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UploadService(IUnitOfWork<DatabaseContext> db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<FileForUploadViewModel> UploadFile(IFormFile file, string url, string path)
        {
            return await UploadFileToLocal(file, url, path);
        }

        public async Task<FileForUploadViewModel> UploadFileToLocal(IFormFile file, string url, string path)
        {
            if (file.Length > 0)
            {
                try
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtension = Path.GetExtension(fileName);
                    string fileNewName = string.Format("{0}{1}", Guid.NewGuid().ToString(), fileExtension);
                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images\\" + path);
                    //string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images\\Profile\\" + userId);
                    string fullPath = Path.Combine(filePath, fileNewName);
                    Directory.CreateDirectory(filePath);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);

                        return new FileForUploadViewModel()
                        {
                            Status = true,
                            Message = "فایل با موفقیت در فضای ابری آپلود شد",
                            Url = string.Format("{0}/{1}", url, "wwwroot/images/" + path.Replace('\\', '/') + "/" + fileNewName)
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new FileForUploadViewModel()
                    {
                        Status = false,
                        Message = ex.Message
                    };
                }
            }
            else
            {
                return new FileForUploadViewModel()
                {
                    Status = false,
                    Message = "فایلی برای آپلود یافت نشد"
                };
            }
        }

        public FileForUploadViewModel RemoveFileFromLocal(string filePath)
        {
            string path = Path.Combine(_hostingEnvironment.WebRootPath, filePath);

            if (File.Exists(path))
            {
                File.Delete(path);

                return new FileForUploadViewModel()
                {
                    Status = true,
                    Message = "فایل با موفقیت از فضای ابری حذف شد"
                };
            }
            else
            {
                return new FileForUploadViewModel()
                {
                    Status = false,
                    Message = "فایل یافت نشد"
                };
            }
        }
    }
}
