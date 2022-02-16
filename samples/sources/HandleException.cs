using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    internal class Program
    {
        /*
         * 在平台调用过程中，引发异常或错误情形主要可分为两中类型：
         *  - 由非托管函数的错误托管定义导致的异常或错误
         *      在为非托管函数编写托管定义时，需要知道该函数的名称、参数类型等信息，以及将其导出的DLL的名称。
         *      因此，在声明非托管函数的过程中，有可能因为设置了错误的DLL名称而引发DLLNotFoundException异常，
         *      也有可能由于设置了错误的函数入口点等因素导致平台调用无法在非托管DLL中找到相应的函数名，从而引
         *      发EntryPointNotFoundException异常。
         *
         *      这种错误或异常一般都能通过【try...catch】语句进行捕获
         *
         *  - 由非托管函数本身的错误导致的异常或错误
         *      这种异常或错误一般是由非托管函数接受了错误的参数，或者在计算时由于溢出错误、下标越界、空指针等
         *      一些常见的C/C++错误而导致的平台调用异常或错误。
         *
         *      如果异常在.NET平台上由对应的版本，则捕获为对应的.NET异常。
         *      如果没有或者是使用【throw】抛出的异常只能捕获到【SEHException】异常
         */

        // SomeDLL.dll 不存在
        [DllImport("SomeDLL.dll")]
        private static extern void DoSomeThingFunc(int paramInt);

        // 这里声明一个NativeLib.dll中并不存在的除法函数
        [DllImport("NativeLib.dll")]
        private static extern float Divide(float factorA, float factorB);

        // 不正确的参数类型或参数数目不匹配可能产生不同的结果
        // 1. 引发EntryPointNotFoundException异常
        // 2. 非期望值或随机值
        [DllImport("NativeLib.dll")]
        private static extern int Multiply(int factorA, string factorB);

        private static void Main()
        {
            try
            {
                DoSomeThingFunc(100);
                Console.WriteLine("Finish to call function DoSomeThingFunc.");
            }
            catch (DllNotFoundException dllNotFoundExc)
            {
                Console.WriteLine("DllNotFoundException was detected, error message: \r\n{0}", dllNotFoundExc.Message);
            }
            Console.WriteLine("================================================");

            try
            {
                var result = Divide(100F, 818F);
                Console.WriteLine("Divide result from unmanaged function is {0}.", result);
            }
            catch (EntryPointNotFoundException entryPointExc)
            {
                Console.WriteLine("EntryPointNotFoundException was detected, error message: \r\n{0}", entryPointExc.Message);
            }
            Console.WriteLine("================================================");


            try
            {
                var result = Multiply(100, "100");
                Console.WriteLine("Multiply result from unmanaged function is {0}.", result);
            }
            catch (EntryPointNotFoundException entryPointExc)
            {
                Console.WriteLine("EntryPointNotFoundException was detected, error message: \r\n{0}", entryPointExc.Message);
            }

            Console.WriteLine("\r\n按任意键退出...");
            Console.Read();
        }
    }
}