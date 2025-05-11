using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

namespace RestroPrint
{
    public partial class LogForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListView logListView;
        private System.Windows.Forms.Label printStatusLabel;
        private Button selectPrinterButton; // Button to select printer

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            logListView = new ListView();
            printStatusLabel = new Label();
            selectPrinterButton = new Button();
            SuspendLayout();
            // 
            // logListView
            // 
            logListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logListView.FullRowSelect = true;
            logListView.GridLines = true;
            logListView.Location = new Point(12, 10);
            logListView.Margin = new Padding(0, 0, 0, 10);
            logListView.Name = "logListView";
            logListView.Size = new Size(409, 264);
            logListView.TabIndex = 0;
            logListView.UseCompatibleStateImageBehavior = false;
            logListView.View = View.Details;
            logListView.Columns.Add("Activity");
            // 
            // printStatusLabel
            // 
            printStatusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            printStatusLabel.Location = new Point(12, 284);
            printStatusLabel.Name = "printStatusLabel";
            printStatusLabel.Size = new Size(400, 30);
            printStatusLabel.TabIndex = 1;
            printStatusLabel.Text = "Status: Waiting...";
            // 
            // selectPrinterButton
            // 
            selectPrinterButton.Anchor = AnchorStyles.Bottom;
            selectPrinterButton.Location = new Point(143, 317);
            selectPrinterButton.Name = "selectPrinterButton";
            selectPrinterButton.Size = new Size(148, 30);
            selectPrinterButton.TabIndex = 2;
            selectPrinterButton.Text = "Select Printer";
            selectPrinterButton.Click += SelectPrinterButton_Click;
            // 
            // LogForm
            // 
            ClientSize = new Size(450, 350);
            Controls.Add(logListView);
            Controls.Add(printStatusLabel);
            Controls.Add(selectPrinterButton);
            Name = "LogForm";
            Text = "RestroPrint";
            ResumeLayout(false);
        }

        private void SelectPrinterButton_Click(object sender, EventArgs e)
        {
            List<string> printers = GetUsbPrinters();
            if (printers == null || printers.Count == 0)
            {
                MessageBox.Show("No printers available.");
                return;  // Exit the method early since no printers are available
            }
            string previouslySelectedPrinter = LoadPrinterFromRegistry();

            // Pass previously selected printer to the form
            var printerSelectionForm = new PrinterSelectionForm(printers, previouslySelectedPrinter);

            var dialogResult = printerSelectionForm.ShowDialog();

            if (dialogResult == DialogResult.OK && !string.IsNullOrEmpty(printerSelectionForm.SelectedPrinter))
            {
                string selectedPrinter = printerSelectionForm.SelectedPrinter;
                SavePrinterToRegistry(selectedPrinter);
                MessageBox.Show("Printer selected: " + selectedPrinter);
            }
        }
        private string LoadPrinterFromRegistry()
        {
            try
            {
                return Registry.GetValue(@"HKEY_CURRENT_USER\Software\RestroPrint", "SelectedPrinter", "")?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }
        private List<string> GetAllDevicesForTesting()
        {
            var deviceNames = new List<string>();
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");

                foreach (ManagementBaseObject obj in searcher.Get())
                {
                    ManagementObject device = (ManagementObject)obj;
                    string name = device["Name"]?.ToString() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(name))
                        deviceNames.Add(name);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Append("Error fetching devices: " + ex.Message);
            }

            return deviceNames;
        }
        private List<string> GetUsbPrinters()
        {
            var printerNames = new List<string>();
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");

                foreach (ManagementBaseObject obj in searcher.Get())
                {
                    ManagementObject printer = (ManagementObject)obj;
                    string printerPort = printer["PortName"]?.ToString() ?? string.Empty; // Handle null values
                    string printerName = printer["Name"]?.ToString() ?? string.Empty; // Handle null values

                    if (!string.IsNullOrEmpty(printerPort) && printerPort.Contains("USB"))
                    {
                        printerNames.Add(printerName);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Append("Error finding USB printers: " + ex.Message);
            }
            return printerNames;
        }

        private void SavePrinterToRegistry(string printerName)
        {
            try
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\RestroPrint", "SelectedPrinter", printerName);
            }
            catch (Exception ex)
            {
                LogHelper.Append("Error saving printer to registry: " + ex.Message);
            }
        }

        private static Stream GetEmbeddedResourceStream(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
