using Simploud.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Simploud.Api.Providers
{
    public interface ICloudProvider
    {
        Account GetAccountInfo();

        Simploud.Api.Models.File GetFiles(string path);

        Simploud.Api.Models.FileSystemInfo CreateFolder(string path, string name);

        Simploud.Api.Models.FileSystemInfo Rename(string old_name, string new_name);

        Simploud.Api.Models.FileSystemInfo Move(string fromPath, string toPath);

        Simploud.Api.Models.FileSystemInfo Delete(string path);

        byte[] DownloadFile(string path);

        Simploud.Api.Models.FileSystemInfo UploadFile(string path, string filename, MemoryStream data);
    }
}
