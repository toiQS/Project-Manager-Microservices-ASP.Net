using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Infrastructers.Interfaces
{
    public interface ILoggerServices<T> where T : class
    {
        public void LogInformation(string actioName,string message);
        public void LogWarning(string actionName, string message);
        public void LogError(string actionName,string message);
        public void LogCritical( string actionName, string message);
    }
}
