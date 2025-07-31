using System;
using Microsoft.Win32;

namespace PlainCEETimer.Modules
{
    public class RegistryHelper : IDisposable
    {
        private readonly RegistryKey BaseKey;
        private readonly RegistryKey OpenedKey;

        private RegistryHelper(string path, bool isReadonly, RegistryHive rootKey)
        {
            BaseKey = RegistryKey.OpenBaseKey(rootKey, RegistryView.Registry64);
            OpenedKey = BaseKey.OpenSubKey(path, !isReadonly);
        }

        public static RegistryHelper Open(string path, bool isReadonly = true, RegistryHive rootKey = RegistryHive.CurrentUser)
        {
            return new(path, isReadonly, rootKey);
        }

        public T Get<T>(string key, T defaultValue)
        {
            return (T)OpenedKey?.GetValue(key, defaultValue);
        }

        public bool Check(string key, string expectation, string defaultValue)
        {
            return OpenedKey?.GetValue(key, defaultValue) is string tmp && tmp.Equals(expectation, StringComparison.OrdinalIgnoreCase);
        }

        public bool Check(string key, int expectation, int defaultValue)
        {
            return OpenedKey?.GetValue(key, defaultValue) is int tmp && tmp == expectation;
        }

        public void Set(string key, object Value)
        {
            OpenedKey?.SetValue(key, Value);
        }

        public void Delete(string key)
        {
            OpenedKey?.DeleteValue(key, false);
        }

        public void Dispose()
        {
            OpenedKey?.Dispose();
            BaseKey?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~RegistryHelper() => Dispose();
    }
}
