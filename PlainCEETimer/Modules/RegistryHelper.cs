using Microsoft.Win32;
using System;

namespace PlainCEETimer.Modules
{
    public class RegistryHelper : IDisposable
    {
        private readonly RegistryKey OpenedKey;

        private RegistryHelper(string path)
            => OpenedKey = Registry.CurrentUser.OpenSubKey(path, true);

        public static RegistryHelper Open(string path)
            => new(path);

        public bool GetState(string KeyName, string Expectation)
            => OpenedKey?.GetValue(KeyName, false) is string tmp && tmp.Equals(Expectation, StringComparison.OrdinalIgnoreCase);

        public void Set(string KeyName, object Value)
            => OpenedKey.SetValue(KeyName, Value);

        public void Delete(string KeyName)
            => OpenedKey?.DeleteValue(KeyName, false);

        public void Dispose()
        {
            OpenedKey?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~RegistryHelper() => Dispose();
    }
}
