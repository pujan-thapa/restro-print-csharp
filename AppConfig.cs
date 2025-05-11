using Microsoft.Win32;
using System;
using System.Text;

namespace RestroPrint
{
    public static class AppConfig
    {
        private const string RegistryPath = @"SOFTWARE\RestroPrint";

        public static void SetSetting(string key, string value)
        {
            using var regKey = Registry.CurrentUser.CreateSubKey(RegistryPath);
            if (regKey != null)
            {
                var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                regKey.SetValue(key, encoded);
            }
        }

        public static string? GetSetting(string key)
        {
            using var regKey = Registry.CurrentUser.OpenSubKey(RegistryPath);
            if (regKey != null)
            {
                var encoded = regKey.GetValue(key)?.ToString();
                if (!string.IsNullOrEmpty(encoded))
                {
                    try
                    {
                        var decodedBytes = Convert.FromBase64String(encoded);
                        return Encoding.UTF8.GetString(decodedBytes);
                    }
                    catch
                    {
                        return null; // in case of invalid format
                    }
                }
            }
            return null;
        }
    }
}
