using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    // 将托管类/结构体封送给非托管结构体的区别
    // 
    // 在.NET平台下类与结构体的主要区别是：
    //  - 类是引用类型（址传递），结构体是值类型（值类型）
    //  - 类字段的默认访问属性为【private】，结构体的默认访问属性为【public】

    // 除以下的区别外，封送托管类与托管结构体到非托管结构体大体相同。
    // | UnmanagedStruct    | ManagedStruct         | ManagedClass         |
    // | ------------------ | --------------------- | -------------------- |
    // | UnmanagedStruct    | ManagedStruct         |                      |
    // | UnmanagedStruct *  | ref/out ManagedStruct | ManagedClass         |
    // | UnmanagedStruct ** |                       | ref/out ManagedClass |

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class ManagedClass
    {
        public uint Id;
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct ManagedStruct
    {
        public uint Id;
        public string Name;
    }

    class ParameterIsStruct
    {
        // 由于非托管函数的参数时一个结构体变量，而CLR封送托管类时使用的时托管类的地址
        // 因此这种情况下的数据封送仅能使用托管结构体
        [DllImport("NativeLib.dll",
                   CharSet = CharSet.Ansi,
                   EntryPoint = "TMSC_ParameterIsStruct",
                   CallingConvention = CallingConvention.StdCall)]
        public static extern void TMSC_ParameterIsStruct(ManagedStruct employee);
    }


    class ParameterIsStructPointer
    {
        [DllImport("NativeLib.dll",
                   CharSet = CharSet.Ansi,
                   EntryPoint = "TMSC_ParameterIsStructPointer",
                   CallingConvention = CallingConvention.StdCall)]
        public static extern void TMSC_ParameterIsManagedClass([In, Out]ManagedClass employee);

        [DllImport("NativeLib.dll",
                   CharSet = CharSet.Ansi,
                   EntryPoint = "TMSC_ParameterIsStructPointer",
                   CallingConvention = CallingConvention.StdCall)]
        public static extern void TMSC_ParameterIsManagedStruct(ref ManagedStruct employee);
    }

    class ParameterIsStructPointerPointer
    {
        [DllImport("NativeLib.dll",
                   CharSet = CharSet.Ansi,
                   EntryPoint = "TMSC_ParameterIsStructPointerPointer",
                   CallingConvention = CallingConvention.StdCall)]
        public static extern void TMSC_ParameterIsManagedClass(out ManagedClass employee);
    }

    internal class Program
    {
        private static void Main()
        {
            {
                ManagedStruct managedStruct = new ManagedStruct();
                managedStruct.Id = 10001;
                ParameterIsStruct.TMSC_ParameterIsStruct(managedStruct);

                Console.WriteLine("  managed, the id is {0}", managedStruct.Id);
                Console.WriteLine("  managed, the name is {0}", managedStruct.Name);
            }

            {
                ManagedStruct managedStruct = new ManagedStruct();
                managedStruct.Id = 10002;
                ParameterIsStructPointer.TMSC_ParameterIsManagedStruct(ref managedStruct);

                Console.WriteLine("  managed, the id is {0}", managedStruct.Id);
                Console.WriteLine("  managed, the name is {0}", managedStruct.Name);
            }

            {
                ManagedClass managedClass = new ManagedClass();
                managedClass.Id = 10003;
                ParameterIsStructPointer.TMSC_ParameterIsManagedClass(managedClass);

                Console.WriteLine("  managed, the id is {0}", managedClass.Id);
                Console.WriteLine("  managed, the name is {0}", managedClass.Name);
            }

            {
                ManagedClass managedClass;
                ParameterIsStructPointerPointer.TMSC_ParameterIsManagedClass(out managedClass);

                Console.WriteLine("  managed, the id is {0}", managedClass.Id);
                Console.WriteLine("  managed, the name is {0}", managedClass.Name);
            }
        }
    }
}