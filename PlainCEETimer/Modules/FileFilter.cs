namespace PlainCEETimer.Modules
{
    public readonly struct FileFilter(string name, params string[] exts)
    {
        public static readonly FileFilter Application = new("应用程序", "*.exe");
        public static readonly FileFilter Shortcut = new("快捷方式", "*.lnk");
        public static readonly FileFilter AllFiles = new("所有文件", "*.*");

        public override string ToString()
        {
            return string.Format("{0} ({1})|{1}", name, string.Join(";", exts));
        }
    }
}
