using System;
using System.IO;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.Services
{
    public class FileService : IFileService
    {
        public string ReadFileBase64(string path)
        {
            var bytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }

        public void WriteFileBase64(string path, string base64content)
        {
            var bytes = Convert.FromBase64String(base64content);
            SaveBytes(path, bytes);
        }

        public byte[] ReadBytes(string path) => File.ReadAllBytes(path);

        public void SaveBytes(string path, byte[] content)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllBytes(path, content);
        }
    }
}