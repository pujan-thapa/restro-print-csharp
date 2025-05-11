using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;  // For Font class
using System.Linq;    // For LINQ

namespace RestroPrint
{
    public static class LogHelper
    {
        private static ListView? _logListView;
        private static readonly string logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "RestroPrint Logs", "logs.txt");

        // Initialize LogHelper and set font for the ListView
        public static void Init(ListView listView)
        {
            _logListView = listView ?? throw new ArgumentNullException(nameof(listView));

            try
            {
                // Set font to Consolas for monospaced alignment
                _logListView.Font = new Font("Consolas", 10, FontStyle.Regular);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting font: {ex.Message}");
            }

            string logDir = Path.GetDirectoryName(logPath)!;
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
        }

        // Append message to log and update ListView
        public static void Append(string message)
        {
            string timestamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
            bool isFirstLine = true;
            var entries = new List<ListViewItem>();

            foreach (var line in message.Split('\n'))
            {
                string entry;

                if (isFirstLine)
                {
                    entry = $"[{timestamp}] {line}{Environment.NewLine}";
                    isFirstLine = false;
                }
                else
                {
                    entry = $"{line}{Environment.NewLine}";
                }

                try
                {
                    File.AppendAllText(logPath, entry, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing to log file: {ex.Message}");
                }

                entries.Add(new ListViewItem(entry));
                Console.WriteLine(entry);
            }

            // Update the ListView once on UI thread
            if (_logListView != null)
            {
                if (_logListView.InvokeRequired)
                {
                    _logListView.Invoke(new Action(() =>
                    {
                        _logListView.Items.AddRange(entries.ToArray());
                        _logListView.EnsureVisible(_logListView.Items.Count - 1);
                        AdjustColumnWidth();
                        ScrollToBottom();
                    }));
                }
                else
                {
                    _logListView.Items.AddRange(entries.ToArray());
                    _logListView.EnsureVisible(_logListView.Items.Count - 1);
                    AdjustColumnWidth();
                    ScrollToBottom();
                }
            }
        }


        // Adjust the width of the column automatically to fit content
        private static void AdjustColumnWidth()
        {
            if (_logListView?.Columns.Count > 0)
            {
                _logListView.Columns[0].Width = -2; // -2 auto sizes to content
            }
        }

        // Ensure ListView scrolls to the bottom
        private static void ScrollToBottom()
        {
            if (_logListView != null && _logListView.Items.Count > 0)
            {
                // Ensure the last item is visible
                _logListView.Items.Cast<ListViewItem>().Last().EnsureVisible();
            }
        }
    }
}
