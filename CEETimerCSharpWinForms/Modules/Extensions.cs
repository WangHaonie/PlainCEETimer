﻿using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CEETimerCSharpWinForms.Modules
{
    public static class Extensions
    {
        public static int DpiRatio { get; private set; } = 0;

        public static string FormatLog(this string UpdateLog, string LatestVersion)
            => Regex.Replace(UpdateLog.RemoveIllegalChars(), @"[#\>]", "").Replace($"v{LatestVersion}更新日志新功能修复移除", "").Replace("+", "\n● ");

        public static bool IsVersionNumber(this string VersionNumber)
            => Regex.IsMatch(VersionNumber, @"^\d+(\.\d+){1,3}$");

        public static double ToLuminance(this Color _Color)
            => _Color.R * 0.299 + _Color.G * 0.587 + _Color.B * 0.114;

        public static string ToMessage(this Exception ex)
            => $"\n\n错误信息: \n{ex.Message}\n\n错误详情: \n{ex}";

        public static void ReActivate(this Form _Form)
        {
            _Form.WindowState = FormWindowState.Normal;
            _Form.Activate();
        }

        public static int WithDpi(this int Pixel, Form _Form)
        {
            Graphics _Graphics = null;
            int _Pixel = Pixel;

            if (DpiRatio == 0)
            {
                _Graphics = _Form.CreateGraphics();
                DpiRatio = (int)(_Graphics.DpiX / 96);
            }
            else
            {
                _Pixel = Pixel * DpiRatio;
            }

            _Graphics?.Dispose();
            return _Pixel;
        }

        #region 来自网络
        /*

        移除字符串里不可见的空格 (Unicode 控制字符) 参考：

        c# - Removing hidden characters from within strings - Stack Overflow
        https://stackoverflow.com/a/40888424/21094697

        */
        public static string RemoveIllegalChars(this string Text)
            => new(Text.Trim().Replace(" ", "").Where(c => char.IsLetterOrDigit(c) || (c >= ' ' && c <= byte.MaxValue)).ToArray());
        #endregion
    }
}