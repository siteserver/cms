using System.Collections.Generic;
using SSCMS.Utils;
using System.Threading.Tasks;
using System.IO;

namespace SSCMS.Core.Utils
{
    public class FakeConsoleUtils : IConsoleUtils
    {
        public FakeConsoleUtils() { }

        public TextWriter Out { get; }

        public void Report(double value)
        {

        }

        public void Dispose()
        {
            
        }

        public Task WriteSuccessAsync(string successMessage)
        {
            return Task.CompletedTask;
        }

        public Task WriteErrorAsync(string errorMessage)
        {
            return Task.CompletedTask;
        }

        public Task WriteLineAsync(string value)
        {
            return Task.CompletedTask;
        }

        public Task WriteLineAsync()
        {
            return Task.CompletedTask;
        }

        public Task WriteRowLineAsync()
        {
            return Task.CompletedTask;
        }

        public Task WriteRowAsync(params string[] columns)
        {
            return Task.CompletedTask;
        }

        public string GetSelect(string text, List<string> options)
        {
            return string.Empty;
        }

        public string GetString(string text)
        {
            return string.Empty;
        }

        public List<string> GetStringList(string text)
        {
            return new List<string>();
        }

        public string GetPassword(string text)
        {
            return string.Empty;
        }

        public int GetInt(string text)
        {
            return 0;
        }

        public decimal GetDecimal(string text)
        {
            return 0;
        }

        public bool GetYesNo(string text)
        {
            return false;
        }
    }
}
