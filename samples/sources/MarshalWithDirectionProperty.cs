using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct ManagedStruct
    {
        public uint Id;
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    class ManagedClass
    {
        public uint Id;
        public string Name;
    }

    // 方向属性强制执行以下功能
    //                   托管数据                        副本                           非托管数据
    // 非托管函数执行前：                    >>                           >> 
    // 非托管函数执行后：                    <<                           <<
    //                           通过方向属性控制拷贝方向        通过参数类型控结果输入输出
    //                                   方向属性                      参数属性（假想）
    //                                   [In][Out]                    [In][out]

    class ParameterIsStruct
    {
        // void __stdcall Direction_ParameterIsStruct(TestDirection d)

        // 方向属性：无，参数属性：[In]，数据有去无回
        // 默认方向属性为[In]
        [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsStruct", CallingConvention = CallingConvention.StdCall)]
        public static extern void DirectionIsDefault(ManagedStruct employee);

        // 方向属性：[In]，参数属性：[In]，数据有去无回
        // 默认方向属性为[In]
        [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsStruct", CallingConvention = CallingConvention.StdCall)]
        public static extern void DirectionIsIn([In]ManagedStruct employee);

        // 方向属性：[Out]，参数属性：[In]，数据有去无回
        // 仅指定[Out]方向属性，编译器会自动增加[In]方向属性
        [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsStruct", CallingConvention = CallingConvention.StdCall)]
        public static extern void DirectionIsOut([Out]ManagedStruct employee);

        // 方向属性：[In, Out]，参数属性：[In]，数据有去无回
        [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsStruct", CallingConvention = CallingConvention.StdCall)]
        public static extern void DirectionIsInOut([In, Out]ManagedStruct employee);
    }

    class ParameterIsPointer
    {
        // 方向属性：Default，参数属性：[In, Out]，数据有去有回
        // ref 关键字默认使用[In, Out]方向属性
        [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsPointer", CallingConvention = CallingConvention.StdCall)]
        public static extern void DirectionIsRefDefault(ref ManagedStruct employee);

        // 方向属性：[In]，参数属性：[In, Out]，数据有去无回
        // 同时使用ref关键字与[In]方向属性，由于显示指定了方向属性，ref关键字带的默认关键字属性被替换为显示指定的[In]方向属性
        [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsPointer", CallingConvention = CallingConvention.StdCall)]
        public static extern void DirectionIsRefIn([In] ref ManagedStruct employee);

        // 方向属性：[Out]，参数属性：[In, Out]
        // ref 关键字 不能与方向属性[Out]一起使用，除非同时指定方向属性[In]
        // [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsPointer", CallingConvention = CallingConvention.StdCall)]
        // public static extern void DirectionIsRefOut([Out] ref ManagedStruct employee);

        // 方向属性：[In, Out]，参数属性：[In, Out]，数据有去有回
        // 等价于未使用任何方向属性
        [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsPointer", CallingConvention = CallingConvention.StdCall)]
        public static extern void DirectionIsRefInOut([In, Out] ref ManagedStruct employee);
    }

    class ParameterIsPointerPointer
    {
        // 方向属性：Default，参数属性：[In, Out]，数据无去有回
        [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsPointerPointer", CallingConvention = CallingConvention.StdCall)]
        public static extern void DirectionIsOutDefault(out ManagedClass employee);

        // 方向属性：[In]，参数属性：[In, Out]
        // 关键字out不能与方向属性[In]一起使用
        // [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsPointerPointer", CallingConvention = CallingConvention.StdCall)]
        // public static extern void DirectionIsOutIn([In]out ManagedClass employee);

        // 方向属性：[Out]，参数属性：[In, Out]，数据无去有回
        // 等价于不使用任何方向属性
        [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsPointerPointer", CallingConvention = CallingConvention.StdCall)]
        public static extern void DirectionIsOutOut([Out]out ManagedClass employee);

        // 方向属性：[In, Out]，参数属性：[In, Out]
        // 关键字out不能与方向属性[In]一起使用
        // [DllImport("NativeLib.dll", EntryPoint = "Direction_ParameterIsPointerPointer", CallingConvention = CallingConvention.StdCall)]
        // public static extern void DirectionIsOutInOut([In, Out]out ManagedClass employee);
    }

    internal class Program
    {
        private static void Main()
        {
            {
                ManagedStruct managedStruct = new ManagedStruct();
                managedStruct.Id = 10001;
                ParameterIsStruct.DirectionIsDefault(managedStruct);

                Console.WriteLine("  managed, the id is {0}", managedStruct.Id);
                Console.WriteLine("  managed, the name is {0}", managedStruct.Name);
            }

            {
                ManagedStruct managedStruct = new ManagedStruct();
                managedStruct.Id = 10002;
                ParameterIsStruct.DirectionIsIn(managedStruct);

                Console.WriteLine("  managed, the id is {0}", managedStruct.Id);
                Console.WriteLine("  managed, the name is {0}", managedStruct.Name);
            }

            {
                ManagedStruct managedStruct = new ManagedStruct();
                managedStruct.Id = 10003;
                managedStruct.Name = "xxx";
                ParameterIsStruct.DirectionIsOut(managedStruct);

                Console.WriteLine("  managed, the id is {0}", managedStruct.Id);
                Console.WriteLine("  managed, the name is {0}", managedStruct.Name);
            }

            {
                ManagedStruct managedStruct = new ManagedStruct();
                managedStruct.Id = 10004;
                managedStruct.Name = "xxx";
                ParameterIsStruct.DirectionIsInOut(managedStruct);

                Console.WriteLine("  managed, the id is {0}", managedStruct.Id);
                Console.WriteLine("  managed, the name is {0}", managedStruct.Name);
            }
        
            {
                ManagedStruct managedStruct = new ManagedStruct();
                managedStruct.Id = 10005;
                managedStruct.Name = "xxx";
                ParameterIsPointer.DirectionIsRefDefault(ref managedStruct);

                Console.WriteLine("  managed, the id is {0}", managedStruct.Id);
                Console.WriteLine("  managed, the name is {0}", managedStruct.Name);
            }

            {
                ManagedStruct managedStruct = new ManagedStruct();
                managedStruct.Id = 10006;
                managedStruct.Name = "xxx";
                ParameterIsPointer.DirectionIsRefIn(ref managedStruct);

                Console.WriteLine("  managed, the id is {0}", managedStruct.Id);
                Console.WriteLine("  managed, the name is {0}", managedStruct.Name);
            }

            {
                ManagedStruct managedStruct = new ManagedStruct();
                managedStruct.Id = 10007;
                managedStruct.Name = "xxx";
                ParameterIsPointer.DirectionIsRefInOut(ref managedStruct);

                Console.WriteLine("  managed, the id is {0}", managedStruct.Id);
                Console.WriteLine("  managed, the name is {0}", managedStruct.Name);
            }

            {
                ManagedClass managedClass = new ManagedClass();
                managedClass.Id = 10008;
                managedClass.Name = "xxx";
                ParameterIsPointerPointer.DirectionIsOutDefault(out managedClass);

                Console.WriteLine("  managed, the id is {0}", managedClass.Id);
                Console.WriteLine("  managed, the name is {0}", managedClass.Name);
            }

            {
                ManagedClass managedClass = new ManagedClass();
                managedClass.Id = 10009;
                managedClass.Name = "xxx";
                ParameterIsPointerPointer.DirectionIsOutOut(out managedClass);

                Console.WriteLine("  managed, the id is {0}", managedClass.Id);
                Console.WriteLine("  managed, the name is {0}", managedClass.Name);
            }
        }
    }
}