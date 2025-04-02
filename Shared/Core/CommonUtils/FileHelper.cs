using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Core.CommonUtils
{
    public static class FileHelper
    {
        public static async Task WriteToFileAsync(string filePath, string content)
        {
            await File.WriteAllTextAsync(filePath, content);
        }

        public static async Task<string> ReadFromFileAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }

        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }

}
