using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public class OptimizationHelper(bool isAuto) : IDisposable
    {
        private readonly bool Auto = isAuto;
        private OpenFileDialog Dialog;
        private const string Ngen = "ngen.exe";
        private readonly string NgenPath = @"C:\Windows\Microsoft.NET\Framework64\";
        private readonly string DNFVersion = "v4*";
        private readonly MessageBoxHelper MessageX = MessageBoxHelper.Instance;

        public void Optimize()
        {
            if (Auto || MessageX.Warn(
                """
                确认对本程序进行优化？此操作将有助于提升一定的运行速度。
                
                推荐在以下情况下使用：
                    1. 首次运行本程序
                    2. 清理过系统垃圾 (特别是 .NET 缓存) 之后
                    3. 其他情况导致的程序运行速度变慢
                
                """, Buttons: MessageButtons.YesNo) == DialogResult.Yes)
            {
                if (!Auto)
                {
                    MessageX.Info("稍后进行优化操作，将无界面显示进度，请耐心等待。\n\n>> 点击 确定 继续。");
                }

                if (Auto || UACHelper.EnsureUAC(MessageX))
                {
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
                        if (!Auto)
                        {
                            MessageX.Warn($"无法自动搜索到 {Ngen}，请手动指定！");
                            Retry();
                        }
                    }
                }
                else
                {
                    Cancel();
                }
            }
        }

        public void Dispose()
        {
            Dialog?.Dispose();
            GC.SuppressFinalize(this);
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
                if (Dialog.SafeFileName.Equals(Ngen, StringComparison.OrdinalIgnoreCase))
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
                Cancel();
            }
        }

        private void Start(string path)
        {
            try
            {
                var code = (int)ProcessHelper.Run(path, $"install \"{App.CurrentExecutablePath}\" /verbose", 2, AdminRequired: true);

                if (!Auto && MessageX.Info($"命令执行完成！\n返回值为 {code} (0x{code:X})\n(0 代表成功，其他值为失败)\n\n是否重启倒计时?", Buttons: MessageButtons.YesNo) == DialogResult.Yes)
                {
                    App.Exit(ExitReason.UserRestart);
                }
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == Constants.ERROR_CANCELLED)
            {
                if (!Auto)
                {
                    MessageX.Error("授权失败，请在 UAC 对话框弹出时点击 \"是\"。", ex);
                    Cancel();
                }
            }
        }

        private void Cancel()
        {
            MessageX.Info("本次操作已被取消！");
        }

        ~OptimizationHelper() => Dispose();
    }
}
