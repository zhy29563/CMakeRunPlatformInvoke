using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    internal class Program
    {
        [DllImport("NativeLib.dll")]
        private static extern void PrintMsg(string msg);

        private static void Main()
        {
            PrintMsg("Hello world!");

            Console.WriteLine("\r\n按任意键退出...");
            Console.Read();
        }
    }
}