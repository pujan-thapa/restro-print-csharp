using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RestroPrint
{
    public class PrinterSelectionForm : Form
    {
        public string SelectedPrinter { get; private set; }

        public PrinterSelectionForm(List<string> printers, string preselectedPrinter = "")
        {
            this.SelectedPrinter = string.Empty;

            var listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                DataSource = printers,
                Padding = new Padding(10)
            };

            // Set the preselected printer in the Shown event
            this.Shown += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(preselectedPrinter) && printers.Contains(preselectedPrinter))
                {
                    listBox.SelectedItem = preselectedPrinter;
                }
            };

            var selectButton = new Button
            {
                Text = "Select",
                Dock = DockStyle.Bottom,
                Height = 30
            };

            selectButton.Click += (sender, e) =>
            {
                if (listBox.SelectedItem is string selectedPrinter)
                {
                    SelectedPrinter = selectedPrinter;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("No printer selected.");
                }
            };

            this.Controls.Add(listBox);
            this.Controls.Add(selectButton);
            this.Text = "Select Printer";
            this.Size = new System.Drawing.Size(300, 280);
        }
    }
}
