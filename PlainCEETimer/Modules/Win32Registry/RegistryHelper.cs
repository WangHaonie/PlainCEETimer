using Microsoft.Win32;
using System;

namespace PlainCEETimer.Modules.Win32Registry
{
    public class RegistryHelper : IDisposable
    {
        private readonly RegistryKey OpenedKey;

        private RegistryHelper(string path)
            => OpenedKey = Registry.CurrentUser.OpenSubKey(path, true);

        public static RegistryHelper Open(string path)
            => new(path);

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
            GC.SuppressFinalize(this);
        }

        ~RegistryHelper() => Dispose();
    }
}
