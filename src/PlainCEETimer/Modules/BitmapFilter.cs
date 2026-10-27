using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public static class BitmapFilter
{
    /*

    WinForms 高斯模糊覆盖层 参考：

    .net - Layer effects (blur, etc.) in WinForms - Stack Overflow
    https://stackoverflow.com/a/3599954/21094697

     */

    public static void GaussianBlur(Bitmap b, int nWeight = 4)
    {
        var w = b.Width;
        var h = b.Height;

        if (w < 3 || h < 3)
        {
            return;
        }

        var factor = nWeight + 12;
        var mul = ((1L << 24) + factor - 1) / factor;

        using var bSrc = b.Copy();
        var rect = new Rectangle(0, 0, w, h);
        var bmData = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        var bmSrc = bSrc.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

        try
        {
            var stride = bmData.Stride;
            var src = bmSrc.Scan0;
            var dst = bmData.Scan0;

            if (w * h > 512 * 512)
            {
                Parallel.For(1, h - 1, y => ConvRow(src, dst, stride, y, w, nWeight, mul));
            }
            else
            {
                for (var y = 1; y < h - 1; y++)
                {
                    ConvRow(src, dst, stride, y, w, nWeight, mul);
                }
            }
        }
        finally
        {
            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
        }
    }

    private static unsafe void ConvRow(IntPtr pSrc, IntPtr pDst, int stride, int y, int w, int nWeight, long mul)
    {
        var src = (byte*)pSrc + (y - 1) * stride;
        var dst = (byte*)pDst + y * stride;

        for (var x = 1; x < w - 1; x++)
        {
            var i = x * 3;
            var q = src + i - 3;

            for (var c = 0; c < 3; c++)
            {
                var sum = q[c] + 2 * q[c + 3] + q[c + 6]
                    + 2 * (q[c + stride] + q[c + 6 + stride]) + nWeight * q[c + 3 + stride]
                    + q[c + 2 * stride] + 2 * q[c + 3 + 2 * stride] + q[c + 6 + 2 * stride];

                dst[i + c] = (byte)(sum * mul >> 24);
            }
        }
    }
}
