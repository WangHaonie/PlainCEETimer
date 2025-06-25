using System;
using System.ComponentModel;
using System.Diagnostics;

namespace PlainCEETimer.Modules
{
    public static class ProcessHelper
    {
        public static int Run(string path, string args, bool showWindow = false, bool returnExitCode = false)
        {
            var proc = MakeProc(path, args, false, false, showWindow, false);
            proc.Start();

            if (returnExitCode)
            {
                proc.WaitForExit();
                return proc.ExitCode;
            }

            return -1;
        }

        public static void Run(string path, string args, EventHandler onExited, DataReceivedEventHandler onOutputDataReceived)
        {
            var proc = MakeProc(path, args, false, false, false, true);
            proc.Exited += onExited;
            proc.OutputDataReceived += onOutputDataReceived;
            proc.Start();
            proc.BeginOutputReadLine();
            proc.WaitForExit();
        }

        public static Process RunElevated(string path, string args)
        {
            var proc = MakeProc(path, args, true, true, false, false);
            proc.Start();
            return proc;
        }

        public static string GetExitMessage(object process)
        {
            return
                $"""

                ===================================
                命令执行完成，返回值为 0x{((Process)process).ExitCode:X}。可以关闭此窗口。
                ===================================
                """;
        }

        public static string GetExceptionMessage(Exception ex)
        {
            if (ex is Win32Exception w32ex && w32ex.NativeErrorCode == 1223)
            {
                /*
                            
                检测用户是否点击了 UAC 提示框的 "否" 参考:

                c# - Run process as administrator from a non-admin application - Stack Overflow
                https://stackoverflow.com/a/20872219/21094697

                */

                return $"{ex}\n\n授权失败，请在 UAC 对话框弹出时点击 \"是\"。";
            }

            return $"{ex}\n\n出现未知错误，请及时向我们反馈相关信息。";
        }

        private static Process MakeProc(string path, string args, bool elevate, bool useShExec, bool showWindow, bool redirectOutput)
        {
            return new()
            {
                StartInfo = new()
                {
                    FileName = path,
                    Arguments = args,
                    UseShellExecute = useShExec && !redirectOutput && (elevate || !showWindow),
                    Verb = elevate ? "runas" : "",
                    CreateNoWindow = !showWindow,
                    RedirectStandardOutput = redirectOutput && !elevate,
                    WindowStyle = showWindow ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                },

                EnableRaisingEvents = true
            };
        }
    }
}