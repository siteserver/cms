using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SSCMS.Utils;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace SSCMS.Cli.Core
{
    /// <summary>
    /// https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54
    /// </summary>
    public class ConsoleUtils : IConsoleUtils, IProgress<double>
    {
        private const int blockCount = 67;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string animation = @"|/-\";

        private readonly Timer timer;

        private double currentProgress = 0;
        private string currentText = string.Empty;
        private bool disposed = false;
        private int animationIndex = 0;
        private bool isProgress = false;

        public ConsoleUtils(bool isProgress)
        {
            this.isProgress = isProgress;
            if (isProgress)
            {
                timer = new Timer(TimerHandler);

                // A progress bar is only for temporary display in a console window.
                // If the console output is redirected to a file, draw nothing.
                // Otherwise, we'll end up with a lot of garbage in the target file.
                if (!Console.IsOutputRedirected)
                {
                    ResetTimer();
                }
            }
        }

        public TextWriter Out => Console.Out;

        public void Report(double value)
        {
            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref currentProgress, value);
        }

        private void TimerHandler(object state)
        {
            lock (timer)
            {
                if (disposed) return;

                int progressBlockCount = (int)(currentProgress * blockCount);
                int percent = (int)(currentProgress * 100);
                string text = string.Format("[{0}{1}] {2,3}% {3}",
                    new string('#', progressBlockCount), new string('-', blockCount - progressBlockCount),
                    percent,
                    animation[animationIndex++ % animation.Length]);
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            currentText = text;
        }

        private void ResetTimer()
        {
            timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
        }

        public void Dispose()
        {
            if (isProgress)
            {
                lock (timer)
                {
                    disposed = true;
                    UpdateText(string.Empty);
                }
            }
        }

        public async Task WriteSuccessAsync(string successMessage)
        {
            var backgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            await Console.Out.WriteAsync(" SUCCESS ");
            Console.BackgroundColor = backgroundColor;

            await Console.Out.WriteAsync($" {successMessage}");
            await Console.Out.WriteLineAsync();
        }

        public async Task WriteErrorAsync(string errorMessage)
        {
            var backgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            await Console.Out.WriteAsync(" ERROR ");
            Console.BackgroundColor = backgroundColor;

            await Console.Out.WriteAsync($" {errorMessage}");
            await Console.Out.WriteLineAsync();
        }

        public async Task WriteLineAsync(string value)
        {
            await Console.Out.WriteLineAsync(value);
        }

        public async Task WriteLineAsync()
        {
            await Console.Out.WriteLineAsync();
        }

        public async Task WriteRowLineAsync()
        {
            await Console.Out.WriteLineAsync(new string('-', CliConstants.ConsoleTableWidth));
        }

        public async Task WriteRowAsync(params string[] columns)
        {
            var width = (CliConstants.ConsoleTableWidth - columns.Length) / columns.Length;
            var row = columns.Aggregate("|", (current, column) => current + AlignCentre(column, width) + "|");

            await Console.Out.WriteLineAsync(row);
        }

        private string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            return string.IsNullOrEmpty(text)
                ? new string(' ', width)
                : text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }

        public string GetSelect(string text, List<string> options)
        {
            var option = Prompt.GetString($"{text}({ListUtils.ToString(options)}):");
            return ListUtils.ContainsIgnoreCase(options, option) ? option : GetSelect(text, options);
        }

        public string GetString(string text)
        {
            var value = Prompt.GetString(text);
            return !string.IsNullOrEmpty(value) ? value : GetString(text);
        }

        public List<string> GetStringList(string text)
        {
            var value = Prompt.GetString(text);
            return ListUtils.GetStringList(value);
        }

        public string GetPassword(string text)
        {
            var value = Prompt.GetPassword(text);
            return !string.IsNullOrEmpty(value) ? value : GetPassword(text);
        }

        public int GetInt(string text)
        {
            var value = Prompt.GetInt(text);
            return value > 0 ? value : GetInt(text);
        }

        public decimal GetDecimal(string text)
        {
            var value = Prompt.GetString(text);
            return TranslateUtils.ToDecimal(value);
        }

        public bool GetYesNo(string text)
        {
            return Prompt.GetYesNo(text, true);
        }
    }
}
