using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace JetEazy.BasicSpace
{
    public class ImageFromMemory
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int nSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        //public static Bitmap GetImageFromMemory(int processId, IntPtr memoryAddress, int width, int height, PixelFormat format, int stride)
        //{
        //    int bytesRead;
        //    IntPtr processHandle = OpenProcess(0x1F0FFF, false, processId);
        //    byte[] buffer = new byte[width * height * 4]; // Assuming 32bppARGB
        //    bool success = ReadProcessMemory(processHandle, memoryAddress, buffer, buffer.Length, out bytesRead);
        //    CloseHandle(processHandle);

        //    if (success && bytesRead == buffer.Length)
        //    {
        //        Bitmap bmp = new Bitmap(width, height, stride, format, buffer);
        //        return bmp;
        //    }

        //    return null;
        //}

        //public static void SaveImage(Bitmap image, string path)
        //{
        //    image.Save(path, ImageFormat.Png);
        //}
    }
}
