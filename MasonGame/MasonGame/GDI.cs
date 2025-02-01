using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace MasonGame
{
    class GDI
    {
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll")]
        static extern int GetBitmapBits(IntPtr hbmp, int cbBuffer, IntPtr lpvBits);

        [DllImport("gdi32.dll")]
        static extern int SetBitmapBits(IntPtr hbmp, int cBytes, IntPtr lpvBits);

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAlloc(IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);
        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;
        const uint MEM_COMMIT = 0x1000;
        const uint MEM_RESERVE = 0x2000;
        const uint PAGE_READWRITE = 0x04;
        const uint SRCCOPY = 0x00CC0020;
        const uint SRCERASE = 0x00440328;
        public static void MasonGDI()
        {
            int w = GetSystemMetrics(SM_CXSCREEN);
            int h = GetSystemMetrics(SM_CYSCREEN);
            IntPtr data = VirtualAlloc(IntPtr.Zero, (IntPtr)((w * h + w) * Marshal.SizeOf(typeof(RGBQUAD))), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (data == IntPtr.Zero)
            {
                return;
            }
            IntPtr hdc, hdcMem, hbm;
            for (int i = 0; ; i++, i %= 6)
            {
                hdc = GetDC(IntPtr.Zero);
                hdcMem = CreateCompatibleDC(hdc);
                hbm = CreateBitmap(w, h, 1, 32, data);
                SelectObject(hdcMem, hbm);
                BitBlt(hdcMem, 0, 0, w, h, hdc, 0, 0, SRCCOPY);
                GetBitmapBits(hbm, w * h * 4, data);
                for (int j = 0; j < w * h; j++)
                {
                    int x = j * w + j;
                    int y = j * h + j;
                    int f = (y | (y + y + -1 + x + w / h));
                    Marshal.WriteByte(data, j * 4 + 2, (byte)(f / 1));
                }
                SetBitmapBits(hbm, w * h * 4, data);
                BitBlt(hdc, 0, 0, w, h, hdcMem, 0, 0, SRCERASE);
                Graphics g = Graphics.FromHdc(hdc);
                Font largeFont = new Font("Impact", 100, FontStyle.Bold);
                Font smallFont = new Font("Impact", 30, FontStyle.Regular);
                string largeText = "GAME OVER";
                SizeF largeTextSize = g.MeasureString(largeText, largeFont);
                PointF largeTextPosition = new PointF((w - largeTextSize.Width) / 2, (h - largeTextSize.Height) / 2);
                g.DrawString(largeText, largeFont, Brushes.Black, largeTextPosition);
                g.Dispose();
                DeleteObject(hbm);
                DeleteObject(hdcMem);
                ReleaseDC(IntPtr.Zero, hdc);
                System.Threading.Thread.Sleep(new Random().Next(100));
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }
    }
}
