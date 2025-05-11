using System;
using System.Windows.Forms;

namespace RestroPrint
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var appKey = AppConfig.GetSetting("AppKey");
            var cluster = AppConfig.GetSetting("Cluster");

            // Prompt the user if settings are missing
            if (string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(cluster))
            {
                using var form = new SettingsPromptForm(); // Custom form with input fields
                if (form.ShowDialog() == DialogResult.OK)
                {
                    AppConfig.SetSetting("AppKey", form.AppKey ?? string.Empty);
                    AppConfig.SetSetting("Cluster", form.Cluster ?? string.Empty);

                    appKey = form.AppKey;
                    cluster = form.Cluster;
                }
                else
                {
                    MessageBox.Show("WebSockets settings are required to run the application.");
                    return;
                }
            }

            // Pass appKey and cluster to LogForm constructor
            Application.Run(new LogForm(appKey ?? string.Empty, cluster ?? string.Empty));
        }
    }
}
