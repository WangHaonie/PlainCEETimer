using System;
using Microsoft.Win32;

namespace PlainCEETimer.Modules
{
    public class RegistryHelper : IDisposable
    {
        private readonly RegistryKey BaseKey;
        private readonly RegistryKey OpenedKey;

        private RegistryHelper(string path, bool isReadonly, RegistryHive rootKey)
            => OpenedKey = (BaseKey = RegistryKey.OpenBaseKey(rootKey, RegistryView.Registry64)).OpenSubKey(path, !isReadonly);

        public static RegistryHelper Open(string path, bool isReadonly = true, RegistryHive rootKey = RegistryHive.CurrentUser)
            => new(path, isReadonly, rootKey);

        public object GetValue(string key, int defaultValue)
            => OpenedKey?.GetValue(key, defaultValue);

        public bool GetState(string key, string expectation, string defaultValue)
            => OpenedKey?.GetValue(key, defaultValue) is string tmp && tmp.Equals(expectation, StringComparison.OrdinalIgnoreCase);

        public bool GetState(string key, int expectation, int defaultValue)
            => OpenedKey?.GetValue(key, defaultValue) is int tmp && tmp == expectation;

        public void Set(string key, object Value)
            => OpenedKey.SetValue(key, Value);

        public void Delete(string key)
            => OpenedKey?.DeleteValue(key, false);

        public void Dispose()
        {
            OpenedKey?.Dispose();
            BaseKey?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~RegistryHelper() => Dispose();
    }
}
