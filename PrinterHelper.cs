using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using Microsoft.Win32; // For registry access

namespace RestroPrint
{
    public static class PrinterHelper
    {
        private static readonly Queue<(string text, string ip, int port, int retries, Action<string> updateStatusCallback)> queue = new();
        private static readonly object queueLock = new(); // lock object for thread safety
        private static bool isPrinting = false;

        public static void Enqueue(string text, string ip, int port, Action<string> updateStatusCallback)
        {
            lock (queueLock)
            {
                queue.Enqueue((text, ip, port, 0, updateStatusCallback));
                if (!isPrinting)
                {
                    ProcessQueue();
                }
            }
        }

        private static async void ProcessQueue()
        {
            lock (queueLock)
            {
                if (isPrinting || queue.Count == 0) return;
                isPrinting = true;
            }

            while (true)
            {
                (string text, string ip, int port, int retries, Action<string> updateStatusCallback) job;

                lock (queueLock)
                {
                    if (queue.Count == 0)
                    {
                        isPrinting = false;
                        return;
                    }

                    job = queue.Dequeue();
                }

                bool success = job.ip == "" ? PrintViaUsb(job.text, job.updateStatusCallback)
                                            : await SendToPrinterAsync(job.text, job.ip, job.port, job.updateStatusCallback);

                if (success)
                {
                    LogHelper.Append("Printed successfully.");
                    await Task.Delay(1000);
                    job.updateStatusCallback?.Invoke("Job Completed");
                }
                else if (job.retries < 3)
                {
                    LogHelper.Append($"Retrying job ({job.retries + 1}/3)...");
                    await Task.Delay(1000);
                    job.updateStatusCallback?.Invoke("Retrying");

                    lock (queueLock)
                    {
                        queue.Enqueue((job.text, job.ip, job.port, job.retries + 1, job.updateStatusCallback!));
                    }
                }

                await Task.Delay(100); // Prevent flooding
            }
        }

        public static void PrintViaTcp(string text, string ip, int port, Action<string> updateStatusCallback)
        {
            Enqueue(text, ip, port, updateStatusCallback);
        }

        public static void PrintToUsb(string text, Action<string> updateStatusCallback)
        {
            Enqueue(text, "", 0, updateStatusCallback);
        }

        private static async Task<bool> SendToPrinterAsync(string text, string ip, int port, Action<string> updateStatusCallback)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(ip, port);

                // Wait for the connection to complete with a timeout
                if (await Task.WhenAny(connectTask, Task.Delay(3000)) != connectTask) // Timeout after 2 seconds
                {
                    LogHelper.Append($"Connection to {ip}:{port} timed out.");
                    updateStatusCallback?.Invoke("Timeout");
                    return false;
                }

                byte[] bytes = Encoding.ASCII.GetBytes(text + "\n\n\n\x1D\x56\x00");
                using var stream = client.GetStream();

                // Create a CancellationTokenSource for managing the cancellation
                var cancellationTokenSource = new CancellationTokenSource();

                // Optionally set a timeout for the operation
                cancellationTokenSource.CancelAfter(5000);  // Cancel the operation after 5 seconds

                // Use the WriteAsync method with CancellationToken
                await stream.WriteAsync(new ReadOnlyMemory<byte>(bytes), cancellationTokenSource.Token);
                await stream.FlushAsync();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Append("Print failed: " + ex.Message);
                await Task.Delay(1000);  // Adding a delay before callback to simulate some waiting time
                updateStatusCallback?.Invoke("Failed");
                return false;
            }
        }

        private static bool PrintViaUsb(string text, Action<string> updateStatusCallback)
        {
            try
            {
                // Get the printer name from the registry (whether USB or network)
                string? printerName = GetPrinterNameFromRegistry();

                if (string.IsNullOrEmpty(printerName))
                {
                    LogHelper.Append("Please select a printer.");
                    updateStatusCallback?.Invoke("No printer selected");
                    return false;
                }

                using var printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = printerName;
                bool printSuccessful = false;

                printDoc.PrintPage += (sender, args) =>
                {
                    if (args.Graphics != null)
                    {
                        args.Graphics.DrawString(text, new System.Drawing.Font("Arial", 12), System.Drawing.Brushes.Black, 0, 0);
                        printSuccessful = true;
                    }
                    else
                    {
                        LogHelper.Append("Graphics object is null. Print failed.");
                        printSuccessful = false;
                    }
                };

                printDoc.Print();

                if (printSuccessful)
                {
                    LogHelper.Append("Printed to printer successfully.");
                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        updateStatusCallback?.Invoke("Job Completed");
                    });
                    return true;
                }
                else
                {
                    LogHelper.Append("Print failed: No print was executed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Append("Print failed: " + ex.Message);
                return false;
            }
        }

        // Get the printer name saved in the registry
        private static string? GetPrinterNameFromRegistry()
        {
            try
            {
                return Registry.GetValue(@"HKEY_CURRENT_USER\Software\RestroPrint", "SelectedPrinter", null) as string;
            }
            catch (Exception ex)
            {
                LogHelper.Append("Error reading printer from registry: " + ex.Message);
                return null;
            }
        }
    }

}
