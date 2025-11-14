using System;

namespace Autosoft_Licensing.Services
{
    public interface IFileService
    {
        string ReadFileBase64(string path);
        void WriteFileBase64(string path, string base64content);
        byte[] ReadBytes(string path);
        void SaveBytes(string path, byte[] content);
    }
}