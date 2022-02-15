using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    internal class Program
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int puts(string msg);
        
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _flushall();

        private static void Main()
        {
            puts("Hello world!");
            _flushall();

            Console.WriteLine("\r\n按任意键退出...");
            Console.Read();
        }
    }
}