using System;
using System.IO;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public unsafe static class PathUtils
{
    public static string GetSafeFileName(string path)
    {
        if (path is { Length: var len } && len > Windows.MAX_PATH)
        {
            return GetShortName(path);
        }

        return path;
    }

    public static string GetShortName(string file)
    {
        if (!string.IsNullOrEmpty(file))
        {
            fixed (char* pszFile = file)
            {
                var length = Win32.GetShortPathName(pszFile, null, 0);

                if (length > 0)
                {
                    var result = StringInternals.FastAllocateString(length - 1);

                    fixed (char* lpBuffer = result)
                    {
                        if (Win32.GetShortPathName(pszFile, lpBuffer, length) > 0)
                        {
                            return result;
                        }
                    }
                }
            }
        }

        return file;
    }

    /// <summary>
    /// 为指定的文件创建一个唯一的文件名。
    /// </summary>
    /// <param name="path">所在文件夹，以 \ 结尾</param>
    /// <param name="name">文件名</param>
    public static string MakeUniqueName(string path, string name)
    {
        if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(name))
        {
            using var cache = new ArrayCache<char>(Windows.MAX_PATH, out var buffer);

            fixed (char* ptr = buffer)
            {
                if (Win32.PathYetAnotherMakeUniqueName(ptr, path, NInt.Zero, name))
                {
                    return new(ptr);
                }
            }

            return Path.Combine(path, GetUniqueName(name));
        }

        return path ?? name;
    }

    public static string CompactPath(string path, int maxLength = 150)
    {
        if (path is { Length: var len } && len > maxLength)
        {
            using var cache = new ArrayCache<char>(++maxLength, out var buffer);

            fixed (char* ptr = buffer)
            {
                if (Win32.PathCompactPathEx(ptr, path, maxLength, 0))
                {
                    return new(ptr);
                }
            }

            var a = maxLength / 3;

            return path.Truncate(maxLength - a, a);
        }

        return path;
    }

    private static string GetUniqueName(string name)
    {
        return Guid.NewGuid().ToString("d") + Path.GetExtension(name);
    }
}
