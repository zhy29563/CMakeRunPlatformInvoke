using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    internal class Program
    {
        /*
         * 非托管代码中分配的内存主要有三种方法：
         *  - 在C语言中，用于分配内存的主要方法是【malloc】，而释放内存的方法是【free】。
         *  - 在C++中，用于分配内存的是【new】，而释放内存的方法是【delete】。
         *  - 在COM中，分配用【CoTaskMemAlloc】，释放【CoTaskMemFree】。
         *
         *  在上面3种非托管代码分配内存的主要方法中，如果非托管内存是由【前两种方法分配的(malloc, new)】，那么在平台调用中【不能对其直接进行释放】。
         *  原因主要在于，托管代码无法确切获悉非托管代码是采用那种方法类分配内存。并且，如果是用C语言编写的代码，则更无法知道是采
         *  用那个版本的C运行库。要释放由这两种方法分配的内存，必须在非托管代码中实现一个能够释放此非托管内存的方法，然后在托管代
         *  码中调用还方法对非托管内存进行释放。
         *
         *  如果内存是由第三种方法进行分配的，那么封送拆收器是能够将其释放掉的。
         *  原因在于【封送拆收器】在对【非托管内存进行处理】时，会将COM的内存分配方法【CoTaskMemAlloc】作为非托管内存的【默认分配方法】。
         *  因此，当【封送拆收器】将一个非托管内存指针封送成【.NET】数据类型时，【封送拆收器】会使用非托管数据的【副本】创建一个
         *  【.NET】对象。由于非托管数据已经被【封送拆收器】获取，因此【封送拆收器】就会使用相应的COM释放内存的方法CoTaskMemFree
         *  来释放内存。
         */
        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetStringMalloc();

        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void FreeMallocMemory(IntPtr pbuffer);


        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetStringNew();

        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void FreeNewMemory(IntPtr pbuffer);


        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern string GetStringCoTaskMemAlloc();

        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "GetStringCoTaskMemAlloc")]
        static extern IntPtr GetStringCoTaskMemAllocViaIntPtr();

        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void FreeCoTaskMemAllocMemory(IntPtr pbuffer);

        private static void Main()
        {
            var mallocStringPtr = GetStringMalloc();
            var stringFromMalloc = Marshal.PtrToStringUni(mallocStringPtr);
            Console.WriteLine(stringFromMalloc);
            FreeMallocMemory(mallocStringPtr);
            Console.WriteLine("================================================");


            var newStringPtr = GetStringNew();
            var stringFromNew = Marshal.PtrToStringUni(newStringPtr);
            Console.WriteLine(stringFromNew);
            FreeNewMemory(newStringPtr);
            Console.WriteLine("================================================");

            
            // 内存自动释放
            var stringViaCoTaskMemAlloc = GetStringCoTaskMemAlloc();
            Console.WriteLine(stringViaCoTaskMemAlloc);

            // 内存手动释放
            var coTaskMemAllocIntPtr = GetStringCoTaskMemAllocViaIntPtr();
            var stringFromCoTaskMemAlloc = Marshal.PtrToStringUni(coTaskMemAllocIntPtr);
            Console.WriteLine(stringFromCoTaskMemAlloc);
            FreeCoTaskMemAllocMemory(coTaskMemAllocIntPtr);
            //Marshal.FreeCoTaskMem(coTaskMemAllocIntPtr);

            Console.WriteLine("\r\n按任意键退出...");
            Console.Read();
        }
    }
}