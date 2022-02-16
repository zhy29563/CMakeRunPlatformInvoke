using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    internal class Program
    {
        [DllImport("user32.dll", EntryPoint = "MessageBox")]
        public static extern int MessageBox(int hWnd, string lpText, string lpCaption, int wType);
        private static void Main()
        {
            MessageBox(0, "Hello world!", "Welcome", 0);

            Console.WriteLine("\r\n按任意键退出...");
            Console.Read();
        }
    }
}