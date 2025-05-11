using System;
using System.Windows.Forms;

namespace RestroPrint
{
    public partial class SettingsPromptForm : Form
    {
        public string? AppKey { get; private set; }
        public string? Cluster { get; private set; }

        public SettingsPromptForm()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Get the values entered by the user
            AppKey = txtAppKey.Text;
            Cluster = txtCluster.Text;

            // Check if both fields are filled
            if (string.IsNullOrEmpty(AppKey) || string.IsNullOrEmpty(Cluster))
            {
                MessageBox.Show("Both AppKey and Cluster are required.");
                return;
            }

            // Close the form if everything is fine
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // Close the form without saving
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
