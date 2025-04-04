using PM.Shared.Dtos;
using System;
using System.IO;
namespace PM.Shared.Files
{
    public class ActionFile
    {
        private string _fileName;
        private string _folderName;
        public ActionFile(string folderName, string fileName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentException("Folder name cannot be null or empty", nameof(folderName));
            }   
            _folderName = folderName;
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));
            }
            _fileName = fileName;
        }

        public string CustomPath()
        {
            var rootPath = Directory.GetCurrentDirectory();
            if (string.IsNullOrEmpty(rootPath))
            {
                throw new InvalidOperationException("Root path cannot be null or empty");
            }
            var pathProject = rootPath.Split("\\Project-Manager-Microservices-ASP.Net");
            return pathProject[0] + "\\Project-Manager-Microservices-ASP.Net";
        }
        public string FindLogFolder()
        {
            var path = CustomPath();
            if (Directory.Exists(path + "\\Logs" ))
            {
                return path+"\\Logs";
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(path + "\\Logs");
                    return path + "\\Logs";
                }
                catch
                {
                    throw new DirectoryNotFoundException("Unable to create Logs directory");
                }
            }
        }
        public string FindFordeService()
        {
            var path = FindLogFolder();
            if (Directory.Exists(path + $"{_folderName}-service"))
            {
                return path + $"{_folderName}-service";
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(path + $"{_folderName}-service");
                    return path + $"{_folderName}-service";
                }
                catch
                {
                    throw new DirectoryNotFoundException("Unable to create Logs directory");
                }
            }
        }
        public string CreateAndWriteFile(ContentLog content)
        {
            var path = FindFordeService();
            if (File.Exists(path + $"\\{_fileName}"))
            {
                throw new FileNotFoundException("File already exists");
            }
            try
            {
                using (StreamWriter sw = File.CreateText(path + $"\\{_fileName}"))
                {
                    sw.WriteLine($"Action Name: {content.ActionName}");
                    sw.WriteLine($"User Name: {content.UserName}");
                    sw.WriteLine($"Message: {content.Message}");
                    sw.WriteLine($"Status: {content.Status}");
                    sw.WriteLine($"Status Code: {content.StatusCode}");
                    sw.WriteLine($"Action At: {content.ActionAt}");
                }
                return path + $"\\{_fileName}";
            }
            catch (Exception ex)
            {
                throw new IOException("Unable to create or write to file", ex);
            }

        }
    }
}
