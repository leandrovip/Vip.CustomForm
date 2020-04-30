using System;

namespace Vip.CustomForm
{
    public static class DpiAware
    {
        private static float dpiValue;

        public static float GetCurrentDpi()
        {
            if (dpiValue == 0f)
            {
                IntPtr dC = NativeMethods.GetDC(IntPtr.Zero);
                dpiValue = NativeMethods.GetDeviceCaps(dC, 88);
                NativeMethods.ReleaseDC(IntPtr.Zero, dC);
            }

            return dpiValue;
        }
    }
}