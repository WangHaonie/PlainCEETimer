using System;
using System.IO;
using System.Windows.Forms;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Dialogs;

namespace PlainCEETimer.Modules
{
    public class OptimizationHelper(bool isAuto) : IDisposable
    {
        private OpenFileDialog Dialog;
        private const string NGen = "ngen.exe";
        private readonly string NGenPath = @"C:\Windows\Microsoft.NET\Framework64\";
        private readonly string NetFxVersion = "v4*";
        private readonly MessageBoxHelper MessageX = MessageBoxHelper.Instance;

        public void Optimize()
        {
            if (isAuto || MessageX.Warn(
                """
                确认对本程序进行优化？此操作将有助于提升一定的运行速度。
                
                推荐在以下情况下使用：
                    1. 首次运行本程序
                    2. 清理过系统垃圾 (特别是 .NET 缓存) 之后
                    3. 其他情况导致的程序运行速度变慢
                """, MessageButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    var dirs = Directory.GetDirectories(NGenPath, NetFxVersion);
                    var length = dirs.Length;

                    if (length != 0)
                    {
                        for (int i = length - 1; i < length; i--)
                        {
                            var path = Path.Combine(dirs[i], NGen);

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
                    if (!isAuto)
                    {
                        MessageX.Warn($"无法自动搜索到 {NGen}，请手动指定！");
                        Retry();
                    }
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
                Title = $"选择 {NGen} - 高考倒计时",
                InitialDirectory = @"C:\Windows",
                Filter = "Windows 应用程序 (*.exe)|*.exe|所有文件 (*.*)|*.*",
                FileName = NGen
            };

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                if (Dialog.SafeFileName.Equals(NGen, StringComparison.OrdinalIgnoreCase))
                {
                    Start(Dialog.FileName);
                }
                else
                {
                    MessageX.Error($"选择的文件貌似不是 {NGen}，请重新指定！");
                    Retry();
                }
            }
            else
            {
                MessageX.Info("本次操作已被取消！");
            }
        }

        private void Start(string path)
        {
            var param = ConsoleParam.ShowLeftButton;
            Action<ConsoleWindow> complete = null;

            if (isAuto)
            {
                param |= ConsoleParam.AutoClose | ConsoleParam.NoMenu;
            }

            if (!isAuto)
            {
                complete = c => c.UpdateState("是否重启倒计时?");
            }

            var result = ConsoleWindow.Run(path, $"install \"{App.CurrentExecutablePath}\" /verbose", complete, param);

            if (!isAuto && result == DialogResult.OK)
            {
                App.Exit(ExitReason.UserRestart);
            }
        }

        ~OptimizationHelper() => Dispose();
    }
}
