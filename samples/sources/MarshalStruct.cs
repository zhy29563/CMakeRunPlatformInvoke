using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    struct ManagedSimpleStruct
    {
        public int intValue;
        public short shortValue;
        public float floatValue;
        public double doubleValue;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct MsEmployee
    {
        public uint EmployeeID;
        public short EmployedYear;
        public string DisplayName;
        public string Alias;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MsEmployee_IntPtrString
    {
        public uint EmployeeID;
        public short EmployedYear;
        public IntPtr DisplayName;
        public IntPtr Alias;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct MsEmployee2
    {
        public uint EmployeeID;
        public short EmployedYear;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string DisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string Alias;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PersonName
    {
        public string first;
        public string last;
        public string displayName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Person
    {
        public IntPtr name;
        public int age;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Person2
    {
        public PersonName name;
        public int age;
    }

    internal class Program
    {
        // 结构体作为输入参数(值传递)
        [DllImport("NativeLib.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TestStructArgumentByVal(ManagedSimpleStruct argStruct);

        // 结构体作为输入参数(址传递)
        [DllImport("NativeLib.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TestStructArgumentByRef(ref ManagedSimpleStruct argStruct);

        // 托管结构体的定义
        //
        //  封送结构体【最重要的环节】是要保证【管代码中定义的结构体】与【非托管代码中的结构体】的定义一致，其中包括【字段的顺序】，
        //  【类型】和【大小】。一般需要使用【StructLayout】特性进行修饰。

        //  StructLayout有3个比较重要，它们用来控制结构体和类的封送处理行为。
        //  - CharSet
        //      指定结构体或类中的字符串字段是按照【LPWSTR】还是【LPSTR】封送
        //  - Pack
        //      控制结构体或类的数据字段在内存中的对其方式。
        //      也可以使用编译器指令【#pragma(n)】对编译器做出指示，以调整结构体的内存布局。

        //      内存对齐是指结构体、类或联合体中的字段总是要与特定的【内存边界】对齐。
        //      边界值来源于Pack值的倍数和字段大小的倍数二者中较小的那个数值。
        //      在托管代码中，如果没有显式指定【Pack】值，则其默认值为8，与Visual C++中的默认值一致。
        //  - Size
        //      指定了结构体或类在非托管内存中的绝对大小

        //  StructLayout的构造函数使用一个LayoutKind参数。该参数是一个枚举类型，值与含义如下：
        //  - Sequential
        //      当结构体被封送到非托管内存中时，各个字段按照它们被定义的顺序在内存中布局。
        //      该布局同时会收到Pack字段的影响，在内存中有可能是不连续的。【默认值】。
        //  - Explicit
        //      该选项可以精确控制结构体中各个字段在非托管内存中的精确位置。每个字段必须用FieldOffset属性指定其位置。
        //  - Auto
        //      CLR会自动为指定了该值的结构体选择合适的内存布局。在平台调用中定义为此值的类型，会导致数据封送发生异常。


        // 封送从函数内部返回结构体(返回值)
        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr TestReturnStruct();

        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr TestReturnNewStruct();

        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void FreeStruct(IntPtr pStruct);

        // 封送从函数内部返回结构体(参数)
        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void TestReturnStructFromArg(ref IntPtr pStruct);

        // 封送结构体中的字符指针字段(string)
        [DllImport("NativeLib.dll",  CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void GetEmployeeInfo(ref MsEmployee employee);

        // 封送结构体中的字符指针字段(IntPtr)
        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void GetEmployeeInfo(ref MsEmployee_IntPtrString employee);

        // 封送结构体中的字符数组字段
        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void GetEmployeeInfo2(ref MsEmployee2 employee);

        // 封送结构体中的布尔字段

        // 非托管端
        //  typedef struct _MSEMPLOEE_EX
        //  {
        //      UINT        employessID;
        //      wchar_t*    displayName;
        //      char*       alias;
        //      bool        isInRedmond;        // 1字节
        //      short       employedYear;
        //      BOOL        idFrame;            // 4字节
        //  } MSEMPLOYEE_EX, *PMSEMPLOYEE_EX;

        // 托管端
        //  [StructLayout(LoyoutKing.Sequential)]
        //  private struct MsEmployeeEx
        //  {
        //      public uint EmployeeID;
        //      [MarshalAs(UnmanagedType.LPWStr)]
        //      public string DisplayName;
        //      [MarshalAs(UnmanagedType.LPStr)]
        //      public string Alias;
        //      [MarshalAs(UnmanagedType.T1)]
        //      public bool IsInRedmond;
        //      public short EMployedYear;
        //      [MarshalAs(UnmanagedType.Bool)]
        //      public bool IsFrame;
        //  }

        // 封送结构体字段指向结构体的指针
        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void TestStructInStructByRef(ref Person person);

        // 封送结构体字段为结构体变量
        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void TestStructInStructByVal(ref Person2 person);

        // 将托管类/结构体封送给非托管结构体的区别
        // 在.NET平台下类与结构体的主要区别是：
        //  - 类是引用类型，结构体是值类型
        //  - 类字段的默认访问属性为【private】，结构体的默认访问属性为【public】
        
        // 除以下的区别外，封送托管类与托管结构体到非托管结构体大体相同。
        // | UnmanagedStruct    | ManagedStruct         | ManagedClass         |
        // | ------------------ | --------------------- | -------------------- |
        // | UnmanagedStruct    | ManagedStruct         |                      |
        // | UnmanagedStruct *  | ref/out ManagedStruct | ManagedClass         |
        // | UnmanagedStruct ** |                       | ref/out ManagedClass |

        private static void Main()
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送作为返回值的字符串
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                ManagedSimpleStruct simpleStruct = new ManagedSimpleStruct
                {
                    intValue = 10,
                    shortValue = 20,
                    floatValue = 3.5f,
                    doubleValue = 6.8f
                };
                TestStructArgumentByVal(simpleStruct);
                Console.WriteLine("\n结构体新数据：int = {0}, short = {1}, float = {2:f6}, double = {3:f6}",
                                simpleStruct.intValue,
                                simpleStruct.shortValue,
                                simpleStruct.floatValue,
                                simpleStruct.doubleValue);
                Console.WriteLine("================================================");
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 结构体作为输入参数(址传递)
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                Console.WriteLine("托管代码定义的结构体在非托管代码中的大小为: {0}字节",
                Marshal.SizeOf(typeof(ManagedSimpleStruct)));
                var argStruct = new ManagedSimpleStruct
                {
                    intValue = 1,
                    shortValue = 2,
                    floatValue = 3.0f,
                    doubleValue = 4.5f
                };

                TestStructArgumentByRef(ref argStruct);

                Console.WriteLine("\n结构体新数据: int = {0}, short = {1}, float = {2:f6}, double = {3:f6}",
                    argStruct.intValue,
                    argStruct.shortValue,
                    argStruct.floatValue,
                    argStruct.doubleValue);
                Console.WriteLine("================================================");
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送从函数内部返回结构体(返回值)
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                {
                    // 内存由CoTaskMemAlloc分配
                    var pStruct = TestReturnStruct();
                    var retStruct = (ManagedSimpleStruct)Marshal.PtrToStructure(pStruct, typeof(ManagedSimpleStruct));

                    Marshal.FreeCoTaskMem(pStruct);
                    Console.WriteLine("\n返回的结构体数据：int = {0}, short = {1}, float = {2:f6}, double = {3:f6}",
                                    retStruct.intValue,
                                    retStruct.shortValue,
                                    retStruct.floatValue,
                                    retStruct.doubleValue);
                    Console.WriteLine("==============================================================");
                }

                
                {
                    // 内存由New分配
                    var pStruct = TestReturnNewStruct();
                    var retStruct = (ManagedSimpleStruct)Marshal.PtrToStructure(pStruct, typeof(ManagedSimpleStruct));

                    FreeStruct(pStruct);

                    Console.WriteLine("\n返回的结构体数据：int = {0}, short = {1}, float = {2:f6}, double = {3:f6}",
                                    retStruct.intValue,
                                    retStruct.shortValue,
                                    retStruct.floatValue,
                                    retStruct.doubleValue);
                    Console.WriteLine("================================================");
                }
            }
            
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送从函数内部返回结构体(参数)
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                var ppStruct = IntPtr.Zero;
                TestReturnStructFromArg(ref ppStruct);
                var retStruct = (ManagedSimpleStruct)Marshal.PtrToStructure(ppStruct, typeof(ManagedSimpleStruct));
                Marshal.FreeCoTaskMem(ppStruct);
                Console.WriteLine("\n返回的结构体数据：int = {0}, short = {1}, float = {2:f6}, double = {3:f6}",
                                retStruct.intValue,
                                retStruct.shortValue,
                                retStruct.floatValue,
                                retStruct.doubleValue);
                Console.WriteLine("================================================");
            }
            
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送结构体中的字符指针字段(string)
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                var employee = new MsEmployee { EmployeeID = 10001 };
                GetEmployeeInfo(ref employee);

                Console.WriteLine("\n员工信息:");
                Console.WriteLine("ID: {0}", employee.EmployeeID);
                Console.WriteLine("工龄:{0}", employee.EmployedYear);
                Console.WriteLine("Alias: {0}", employee.Alias);
                Console.WriteLine("姓名: {0}", employee.DisplayName);
                Console.WriteLine("================================================");
            }
            
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送结构体中的字符指针字段(IntPtr)
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                var employee = new MsEmployee_IntPtrString { EmployeeID = 10001 };
                GetEmployeeInfo(ref employee);

                var displayName = Marshal.PtrToStringAnsi(employee.DisplayName);
                var alias = Marshal.PtrToStringAnsi(employee.Alias);

                Marshal.FreeCoTaskMem(employee.DisplayName);
                Marshal.FreeCoTaskMem(employee.Alias);

                Console.WriteLine("\n员工信息:");
                Console.WriteLine("ID: {0}", employee.EmployeeID);
                Console.WriteLine("工龄:{0}", employee.EmployedYear);
                Console.WriteLine("Alias: {0}", alias);
                Console.WriteLine("姓名: {0}", displayName);
                Console.WriteLine("================================================");
            }
            
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送结构体中的字符数组字段
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                var employee = new MsEmployee2 {EmployeeID = 10002};
                GetEmployeeInfo2(ref employee);

                Console.WriteLine("\n员工信息:");
                Console.WriteLine("ID: {0}", employee.EmployeeID);
                Console.WriteLine("工龄:{0}", employee.EmployedYear);
                Console.WriteLine("Alias: {0}", employee.Alias);
                Console.WriteLine("姓名: {0}", employee.DisplayName);
                Console.WriteLine("================================================");
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送结构体字段指向结构体的指针
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                var name = new PersonName {last = "Cui", first = "Xiaoyuan"};
                var person = new Person {age = 27};

                var nameBuffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(name));
                Marshal.StructureToPtr(name, nameBuffer, false);

                person.name = nameBuffer;

                Console.WriteLine("调用前显示姓名为：{0}", name.displayName);
                TestStructInStructByRef(ref person);

                var newValue = (PersonName)Marshal.PtrToStructure(person.name, typeof(PersonName));

                Marshal.DestroyStructure(nameBuffer, typeof(PersonName));

                Console.WriteLine("调用后显示姓名为：{0}", newValue.displayName);
                Console.WriteLine("================================================");
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送结构体字段为结构体变量
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                Console.WriteLine("\n结构体作为值类型成员");
                var person = new Person2
                {
                    name =
                    {
                        last = "Huang", 
                        first = "Jizhou", 
                        displayName = string.Empty
                    },
                    age = 26
                };

                TestStructInStructByVal(ref person);
                Console.WriteLine("================================================");
            }

            Console.WriteLine("\r\n按任意键退出...");
            Console.Read();
        }
    }
}