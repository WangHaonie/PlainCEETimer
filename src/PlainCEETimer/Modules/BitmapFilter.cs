using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public static class BitmapFilter
{
    /*
    
    WinForms 高斯模糊覆盖层 参考：

    .net - Layer effects (blur, etc.) in WinForms - Stack Overflow
    https://stackoverflow.com/a/3599954/21094697
    
     */

    [StructLayout(LayoutKind.Sequential)]
    private class ConvMatrix
    {
        public int TopLeft;
        public int TopRight;
        public int BottomLeft;
        public int BottomRight;

        public int TopMid;
        public int MidLeft;
        public int MidRight;
        public int BottomMid;

        public int Pixel = 1;
        public int Factor = 1;
        public int Offset;

        public ConvMatrix(int pixel, int factor, int nVal, int midVal)
        {
            for (int i = 0; i < 4; i++)
            {
                Marshal.WriteInt32(this, i * sizeof(int), nVal);
            }

            for (int i = 4; i < 4 + 4; i++)
            {
                Marshal.WriteInt32(this, i * sizeof(int), midVal);
            }

            Pixel = pixel;
            Factor = factor;
        }
    }

    public static bool GaussianBlur(Bitmap b, int nWeight = 4)
    {
        return Conv3x3(b, new(nWeight, nWeight + 12, 1, 2));
    }

    private static bool Conv3x3(Bitmap b, ConvMatrix m)
    {
        if (m.Factor == 0)
        {
            return false;
        }

        var bSrc = b.Copy();
        var bmData = b.LockBits(new(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        var bmSrc = bSrc.LockBits(new(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

        var stride = bmData.Stride;
        var stride2 = stride * 2;
        var Scan0 = bmData.Scan0;
        var SrcScan0 = bmSrc.Scan0;

        var nOffset = stride + 6 - b.Width * 3;
        var nWidth = b.Width - 2;
        var nHeight = b.Height - 2;

        int nPixel;

        unsafe
        {
            byte* p = (byte*)(void*)Scan0;
            byte* pSrc = (byte*)(void*)SrcScan0;

            for (int y = 0; y < nHeight; ++y)
            {
                for (int x = 0; x < nWidth; ++x)
                {
                    nPixel = ((((pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
                        (pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight) +
                        (pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;

                    p[5 + stride] = (byte)nPixel;

                    nPixel = ((((pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
                        (pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight) +
                        (pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;

                    p[4 + stride] = (byte)nPixel;

                    nPixel = ((((pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight) +
                        (pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight) +
                        (pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                    if (nPixel < 0) nPixel = 0;
                    if (nPixel > 255) nPixel = 255;

                    p[3 + stride] = (byte)nPixel;

                    p += 3;
                    pSrc += 3;
                }

                p += nOffset;
                pSrc += nOffset;
            }
        }

        b.UnlockBits(bmData);
        bSrc.UnlockBits(bmSrc);
        return true;
    }
}
