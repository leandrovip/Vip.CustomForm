using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace Vip.CustomForm
{
    [ComVisible(false)]
    [SuppressUnmanagedCodeSecurity]
    internal class NativeMethods
    {
        internal struct WINDOWPOS
        {
            public IntPtr hwnd;

            public IntPtr hwndInsertAfter;

            public int x;

            public int y;

            public int cx;

            public int cy;

            public int flags;
        }

        internal struct POINT
        {
            internal int X;

            internal int Y;

            internal POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        internal struct SIZE
        {
            internal int CX;

            internal int CY;

            internal SIZE(int cx, int cy)
            {
                CX = cx;
                CY = cy;
            }
        }

        public struct RECT
        {
            public int left;

            public int top;

            public int right;

            public int bottom;

            public int Width => right - left;

            public int Height => bottom - top;

            public RECT(Rectangle rect)
            {
                bottom = rect.Bottom;
                left = rect.Left;
                right = rect.Right;
                top = rect.Top;
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.bottom = bottom;
                this.left = left;
                this.right = right;
                this.top = top;
            }

            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }

            public override string ToString()
            {
                return "Left = " + left + " Top " + top + " Right = " + right + " Bottom = " + bottom;
            }

            public static implicit operator Rectangle(RECT rect)
            {
                return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class COMRECT
        {
            internal int left;

            internal int top;

            internal int right;

            internal int bottom;

            internal COMRECT() { }

            internal COMRECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public override string ToString()
            {
                return "Left = " + left + " Top " + top + " Right = " + right + " Bottom = " + bottom;
            }

            internal static COMRECT FromXYWH(int x, int y, int width, int height)
            {
                return new COMRECT(x, y, x + width, y + height);
            }
        }

        internal struct LOGFONT
        {
            internal int lfHeight;

            internal int lfWidth;

            internal int lfEscapement;

            internal int lfOrientation;

            internal int lfWeight;

            internal byte lfItalic;

            internal byte lfUnderline;

            internal byte lfStrikeOut;

            internal byte lfCharSet;

            internal byte lfOutPrecision;

            internal byte lfClipPrecision;

            internal byte lfQuality;

            internal byte lfPitchAndFamily;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            internal string lfFaceName;
        }

        internal struct NONCLIENTMETRICS
        {
            internal int cbSize;

            internal int iBorderWidth;

            internal int iScrollWidth;

            internal int iScrollHeight;

            internal int iCaptionWidth;

            internal int iCaptionHeight;

            internal LOGFONT lfCaptionFont;

            internal int iSmCaptionWidth;

            internal int iSmCaptionHeight;

            internal LOGFONT lfSmCaptionFont;

            internal int iMenuWidth;

            internal int iMenuHeight;

            internal LOGFONT lfMenuFont;

            internal LOGFONT lfStatusFont;

            internal LOGFONT lfMessageFont;
        }

        internal struct TRACKMOUSEEVENT
        {
            internal int cbSize;

            internal int dwFlags;

            internal IntPtr hwndTrack;

            internal int dwHoverTime;
        }

        public struct MINMAXINFO
        {
            public POINT ptReserved;

            public POINT ptMaxSize;

            public POINT ptMaxPosition;

            public POINT ptMinTrackSize;

            public POINT ptMaxTrackSize;
        }

        public struct XFORM
        {
            public float eM11;

            public float eM12;

            public float eM21;

            public float eM22;

            public float eDx;

            public float eDy;
        }

        internal static int HIWORD(int n)
        {
            return (short) ((n >> 16) & 0xFFFF);
        }

        internal static int LOWORD(int n)
        {
            return (short) (n & 0xFFFF);
        }

        internal static int LOWORD(IntPtr n)
        {
            return LOWORD((int) n);
        }

        internal static int HIWORD(IntPtr n)
        {
            return HIWORD((int) n);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int CombineRgn(IntPtr hRgn, IntPtr hRgn1, IntPtr hRgn2, int nCombineMode);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("USER32.dll")]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("gdi32")]
        internal static extern int SetTextColor(IntPtr hDC, int crColor);

        [DllImport("gdi32")]
        internal static extern int SetBkMode(IntPtr hdc, int iBkMode);

        [DllImport("gdi32")]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool DeleteDC(IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool PtInRect(ref RECT rc, POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool LockWindowUpdate(IntPtr hWndLock);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        internal static extern int SystemParametersInfo(int uAction, int uParam, ref NONCLIENTMETRICS lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        internal static extern int GetWindowRect(int hwnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        internal static extern bool GetClientRect(IntPtr hwnd, ref RECT lpRect);

        [DllImport("gdi32.dll")]
        internal static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int DrawText(IntPtr hDC, string lpszString, int nCount, ref RECT lpRect, int nFormat);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ExcludeClipRect(IntPtr hdc, int nLeft, int nTop, int nRight, int nBottom);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int width, int height);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetRegionData(IntPtr hRgn, int size, byte[] data);

        [DllImport("gdi32.dll")]
        public static extern int GetRegionData(IntPtr hRgn, int dwCount, IntPtr lpRgnData);

        [DllImport("gdi32.dll")]
        public static extern IntPtr ExtCreateRegion(ref XFORM lpXform, int nCount, IntPtr lpRgnData);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetCursor(IntPtr hCursor);
    }
}