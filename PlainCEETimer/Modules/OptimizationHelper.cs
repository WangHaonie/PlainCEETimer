using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public class OptimizationHelper : IDisposable
    {
        private OpenFileDialog Dialog;
        private const string Ngen = "ngen.exe";
        private readonly string NgenPath = @"C:\Windows\Microsoft.NET\Framework64\";
        private readonly string DNFVersion = "v4*";
        private readonly MessageBoxHelper MessageX = new();

        public void Optimize()
        {
            MessageX.Info("稍后进行优化操作，将无界面显示进度，请耐心等待。\n若弹出 UAC 对话框，请点击 是。");

            try
            {
                var dirs = Directory.GetDirectories(NgenPath, DNFVersion);
                var length = dirs.Length;

                if (length != 0)
                {
                    for (int i = length - 1; i < length; i--)
                    {
                        var path = Path.Combine(dirs[i], Ngen);

                        if (File.Exists(path))
                        {
                            Start(path);
                            return;
                        }
                    }
                }

                throw new Exception();
            }
            catch
            {
                MessageX.Warn($"无法自动搜索到 {Ngen}，请手动指定！");
                Retry();
            }
        }

        public void Dispose()
        {
            Dialog?.Dispose();
        }

        private void Retry()
        {
            Dialog ??= new()
            {
                Title = $"选择 {Ngen} - 高考倒计时",
                InitialDirectory = @"C:\Windows",
                Filter = "Windows 应用程序 (*.exe)|*.exe|所有文件 (*.*)|*.*",
                FileName = Ngen
            };

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.Equals(Dialog.SafeFileName, Ngen, StringComparison.OrdinalIgnoreCase))
                {
                    Start(Dialog.FileName);
                }
                else
                {
                    MessageX.Error($"选择的文件貌似不是 {Ngen}，请重新指定！");
                    Retry();
                }
            }
            else
            {
                MessageX.Info("你已取消本次操作！");
            }
        }

        private void Start(string path)
        {
            try
            {
                var code = (int)ProcessHelper.Run(path, $"install \"{App.CurrentExecutablePath}\" /verbose", 2, AdminRequired: true);

                if (MessageX.Info($"优化完成！\n命令返回值为 {code} (0x{code:X})\n(0 代表成功，其他值为失败)\n\n是否重启倒计时?", Buttons: MessageButtons.YesNo) == DialogResult.Yes)
                {
                    App.Shutdown(true);
                }
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                MessageX.Error("授权失败，请在 UAC 对话框弹出时点击 \"是\"。", ex);
                Start(path);
            }
        }

        ~OptimizationHelper() => Dispose();
    }
}
