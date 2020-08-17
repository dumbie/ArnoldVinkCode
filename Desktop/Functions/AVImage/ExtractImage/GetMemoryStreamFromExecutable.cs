using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ArnoldVinkCode
{
    public partial class ExtractImage
    {
        [DllImport("Shell32.dll")]
        private static extern int SHDefExtractIconW(IntPtr pszIconFile, int iIndex, uint uFlags, ref IntPtr phiconLarge, ref IntPtr phiconSmall, uint nIconSize);

        [DllImport("User32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public static MemoryStream GetMemoryStreamFromExecutable(string executablePath, int iconIndex, ref MemoryStream imageMemoryStream)
        {
            try
            {
                IntPtr ptrIconLarge = IntPtr.Zero;
                IntPtr ptrIconSmall = IntPtr.Zero;
                uint largestSmallest = (1 << 16) | (256 & 0xFFFF);

                IntPtr intPtrExePath = Marshal.StringToHGlobalUni(executablePath);
                int iconExtractResult = SHDefExtractIconW(intPtrExePath, iconIndex, 0, ref ptrIconLarge, ref ptrIconSmall, largestSmallest);
                if (iconExtractResult == 0)
                {
                    if (ptrIconLarge != IntPtr.Zero)
                    {
                        using (Icon IconRaw = Icon.FromHandle(ptrIconLarge))
                        {
                            Bitmap iconBitmap = IconRaw.ToBitmap();
                            if (ptrIconLarge != IntPtr.Zero) { DestroyIcon(ptrIconLarge); }
                            if (ptrIconSmall != IntPtr.Zero) { DestroyIcon(ptrIconSmall); }
                            iconBitmap.Save(imageMemoryStream, ImageFormat.Png);
                            imageMemoryStream.Seek(0, SeekOrigin.Begin);
                            iconBitmap.Dispose();
                            return imageMemoryStream;
                        }
                    }
                    if (ptrIconSmall != IntPtr.Zero)
                    {
                        using (Icon IconRaw = Icon.FromHandle(ptrIconSmall))
                        {
                            Bitmap iconBitmap = IconRaw.ToBitmap();
                            if (ptrIconLarge != IntPtr.Zero) { DestroyIcon(ptrIconLarge); }
                            if (ptrIconSmall != IntPtr.Zero) { DestroyIcon(ptrIconSmall); }
                            iconBitmap.Save(imageMemoryStream, ImageFormat.Png);
                            imageMemoryStream.Seek(0, SeekOrigin.Begin);
                            iconBitmap.Dispose();
                            return imageMemoryStream;
                        }
                    }
                }
            }
            catch { }
            return null;
        }
    }
}