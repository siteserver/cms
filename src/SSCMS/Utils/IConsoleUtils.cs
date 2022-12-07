using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SSCMS.Utils
{
    public interface IConsoleUtils : IDisposable
    {
        public TextWriter Out { get; }

        void Report(double value);

        Task WriteSuccessAsync(string successMessage);

        Task WriteErrorAsync(string errorMessage);

        Task WriteLineAsync(string value);
        
        Task WriteLineAsync();

        Task WriteRowLineAsync();

        Task WriteRowAsync(params string[] columns);

        string GetSelect(string text, List<string> options);

        string GetString(string text);

        List<string> GetStringList(string text);

        string GetPassword(string text);

        int GetInt(string text);

        decimal GetDecimal(string text);

        bool GetYesNo(string text);
    }
}