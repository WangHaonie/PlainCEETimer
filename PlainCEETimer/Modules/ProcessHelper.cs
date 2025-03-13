using System.Diagnostics;

namespace PlainCEETimer.Modules
{
    public static class ProcessHelper
    {
        /// <summary>
        /// 启动一个进程。若要打开文件夹请改用 Process.Start()
        /// </summary>
        /// <param name="ProcessPath">文件路径</param>
        /// <param name="Args">启动参数</param>
        /// <param name="Return">返回类型：0 - 不等待, 1 - 返回进程输出, 2 - 返回进程返回值</param>
        /// <param name="AdminRequired">是否需要管理员权限</param>
        /// <returns>object (string or int)</returns>
        public static object Run(string ProcessPath, string Args = "", int Return = 0, bool AdminRequired = false)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = ProcessPath,
                Arguments = Args,
                UseShellExecute = AdminRequired,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = Return == 1,
                Verb = AdminRequired ? "runas" : ""
            });

            if (Return != 0)
            {
                process.WaitForExit();
            }
            else
            {
                return null;
            }

            return Return == 1 ? process.StandardOutput.ReadToEnd().Trim() : process.ExitCode;
        }
    }
}