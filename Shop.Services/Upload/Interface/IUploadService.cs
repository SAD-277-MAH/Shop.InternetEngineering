using Microsoft.AspNetCore.Http;
using Shop.Data.ViewModels.Upload;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Upload.Interface
{
    public interface IUploadService
    {
        Task<FileForUploadViewModel> UploadFile(IFormFile file, string url, string path);
        Task<FileForUploadViewModel> UploadFileToLocal(IFormFile file, string url, string path);
        FileForUploadViewModel RemoveFileFromLocal(string filePath);
    }
}
