using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    internal class Program
    {
        /*
         * 重新声明非托管函数时的注意事项
         * 
         * - 在声明函数时，必须使用 extern 修饰符。这样做的目的是为了告诉编译器，此函数是在外部实现的，没有方法体，更不需要在托管
         *   代码中搜索此函数。
         * - 在声明函数时，必须同时使用 static 修饰符，原因在于非托管 DLL 带出的非托管方法都时可以直接调用的，因此定义托管方法时，
         *   无需对类进行实例化节能调用此方法。
         * - 必须为定义的托管方法加上`DllImport`属性。
         */
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