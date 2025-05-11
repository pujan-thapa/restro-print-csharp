using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestroPrint
{
    public partial class LogForm : Form
    {
        private PusherService? _pusherService;
        private void LogForm_Resize(object? sender, EventArgs e)
        {
            int padding = 12;
            int buttonHeight = 30;
            int labelHeight = 30;

            logListView.Height = this.ClientSize.Height - (buttonHeight + labelHeight + 3 * padding);

            printStatusLabel.Top = logListView.Bottom + padding;
            selectPrinterButton.Top = printStatusLabel.Bottom + padding;

            // Re-center the button
            selectPrinterButton.Left = (this.ClientSize.Width - selectPrinterButton.Width) / 2;
        }
        // Constructor that accepts appKey and cluster
        public LogForm(string appKey, string cluster)
        {
            InitializeComponent();
            this.Resize += LogForm_Resize;
            LogHelper.Init(logListView);
            StartListeningToServer(appKey, cluster);  // Pass the parameters to StartListeningToServer
        }

        // Method to start listening to the server with provided appKey and cluster
        public async void StartListeningToServer(string appKey, string cluster)
        {
            // Initialize the PusherService with the correct appKey and cluster
            _pusherService = new PusherService(appKey, cluster, (text, ip, port, printerType) =>
            {
                // Using Invoke to ensure that UI updates happen on the UI thread
                this.Invoke(new Action(() =>
                {
                    printStatusLabel.Text = "Printing...";

                    // Handle LAN printer logic
                    if (printerType == "lan" && !string.IsNullOrEmpty(ip) && port.HasValue)
                    {
                        LogHelper.Append($"Sending to LAN printer at {ip}:{port}");
                        LogHelper.Append($"Content: \n {text}");

                        PrinterHelper.PrintViaTcp(text, ip, port ?? 9100, status =>
                        {
                            // Update the UI thread after printing is complete
                            this.Invoke(() => {
                                printStatusLabel.Text = status;  // Example: updating the status label
                            });
                        });
                    }
                    // Handle USB printer logic
                    else if (printerType == "usb")
                    {
                        LogHelper.Append("Sending to USB printer");
                        // Enqueue via the queue (uses ip = "" to signal USB)
                        PrinterHelper.PrintToUsb(text, status =>
                        {
                            // Update the UI thread after printing is complete
                            this.Invoke(() =>
                            {
                                printStatusLabel.Text = status;
                            });
                        });
                    }
                }));
            });

            try
            {
                // Now connecting with the correct appKey and cluster
                await _pusherService.ConnectAsync();
            }
            catch (Exception ex)
            {
                LogHelper.Append($"Error connecting to server: {ex.Message}");
                MessageBox.Show("Error connecting to Pusher service.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
