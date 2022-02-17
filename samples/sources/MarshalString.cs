using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PlatformInvoke
{
    internal class Program
    {
        /*
         * 字符串的封送处理
         *
         * C风格字符串及字符串重定义对照说明表
         *  类型        对何种类型的重定义(typedef)       说明
         *  WCHAR       whar_t                          Unicode
         *  TCHAR       与平台相关，可以是char/wchar_t    ANSI、MBCS、Unicode
         *  LPSTR       char*                           以null终止的ANSI或MBCS字符串
         *  LPCSTR      const char*                     以null终止的ANSI或MBCS字符串常量
         *  LPWSTR      WCHAR*                          以null终止的UNICODE字符串
         *  LPCWSTR     const WCHAR*                    以null终止的UNICODE字符串常量
         *  LPTSYR      TCHAR*                          以null终止的TCHAR字符串
         *  LPCTSYR     const TCHAR*                    以null终止的TCHAR字符串常量
         *  OLECHAR     wchar_t                         UNICODE字符
         *  LPOLESTR    OLECHAR*                        OLECHAR字符串
         *  LPCOLESTR   const OLECHAR*                  OLECHAR字符串常量
         *
         * C++字符串类及其类型对照表
         *
         *  字符串类     字符串类型                        可以转化为合中字符串
         *  _bstr_t     BSTR                            const wchar_t*
         *  _variant_t  BSTR                            ANSI、MBCS或UNICODE字符
         *  string      MBCS                            const char*(调用c_str()方法)
         *  wstring     Unicode                         const wchar_t*(调用c_str()方法)
         *  CComBSTR    BSTR                            const wchar_t*或BSTR
         *  CComVariant BSTR                            const wchar_t*或BSTR（使用ChangeType()，然后获取VAEIANT的成员bstrVal）
         *  CString     TCHAR                           在ANSI或MBCS下，可以转化为const char*；在Unicode下，可以转化为const wchar_t*
         *  COLeVarint  BSTR                            const wchar_t*或BSTR（使用ChangeType()，然后获取VAEIANT的成员bstrVal）
         */

        // 字符串作为参数（使用CharSet控制封送行为）
        //   1. 由于非托管函数的参数为【wchar_t】，表示Unicode字符串，因此需要显示地将CharSet设置为【CharSet.Unicode】
        //   2. 第一个参数是不可变的，使用string类型与之匹配。
        //   3. 第二个参数需要被更改，因此使用StringBuilder。
        [DllImport("NativeLib.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TestStringArgumentsFixLength(string inString, StringBuilder outString, int bufferSize);

        // 方向属性
        //   当应用于方法参数和返回值时，【In】和【Out】属性能够控制封送处理的方向，因此它们又称为方向属性。
        //   【In】特性告诉CLR在调用开始时将数据从调用方封送到被调用方
        //   【Out】特许不过则告知在返回时将数据送被调用方封送回调用方

        // 字符串作为参数（使用MarshalAs控制字符串的封送行为）
        //  用于字符串封送处理的UnmanagedType枚举值表
        //  枚举类型           非托管格式说明
        //  UnmanagedType.AnsiBStr  具有预设长度并包含ANSI字符的COM样式的BSTR
        //  UnmanagedType.BStr      具有预设长度并包含Unicode字符的COM样式的BSTR
        //  UnmanagedType.LPStr     指向以null终止的ANSI字符数组的指针
        //  UnmanagedType.LPTStr    指向以null终止、平台相关的字符数组的指针（默认值）
        //  UnmanagedType.LPWStr    指向以null终止的Unicode字符数组的指针
        //  UnmanagedType.TBStr     具有预设长度并包含平台相关字符的COM样式的BSTR
        //  VBByRefStr              该值使用Visual Basic.NET能够更改非托管代码中的字符串，并使修改结果能在托管代码中反映出来。
        //                          该值仅支持平台调用的情况
        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void TestStringMarshalArguments([MarshalAs(UnmanagedType.LPStr)] string inAnsiString,
                                                              [MarshalAs(UnmanagedType.LPWStr)] string inUnicodeString,
                                                              [MarshalAs(UnmanagedType.LPWStr)] StringBuilder outStringBuffer,
                                                              int outBufferSize);

        // 封送字符串作为参数(非托管代码分配内存)
        // 
        // 以上样例有一个共同之处，那就是在对非托管函数进行平台调用时，【必须预先在托管代码中分配好定长的字符串缓冲区】，
        // 并传递给非托管函数。以下例子说明如何在非托管代码中分配内存。
        [DllImport("NativeLib.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TestStringArgumentOut")]
        static extern void TestStringArgumentOutIntPtr(int id, ref IntPtr outString);

        [DllImport("NativeLib.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TestStringArgumentOut(int id, ref string outString);

        // 封送作为返回值的字符串
        [DllImport("NativeLib.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl,  EntryPoint = "TestStringAsResult")]
        private static extern IntPtr TestStringAsResultIntPtr(int id);

        [DllImport("NativeLib.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern string TestStringAsResult(int id);

        private static void Main()
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 字符串作为参数（使用CharSet控制封送行为）
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            const string inString = "This is a input string.";
            var bufferSize = inString.Length;
            var sb = new StringBuilder(bufferSize);

            TestStringArgumentsFixLength(inString, sb, bufferSize + 1);

            Console.WriteLine("Original: {0}", inString);
            Console.WriteLine("Copied: {0}", sb);
            Console.WriteLine("================================================");
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 字符串作为参数（使用MarshalAs控制字符串的封送行为）
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            const string string1 = "Hello";
            const string string2 = "世界!";

            var outBufferSize = string1.Length + string2.Length + 2;
            var outBuffer = new StringBuilder(outBufferSize);
            TestStringMarshalArguments(string1, string2, outBuffer, outBufferSize);

            Console.WriteLine("非托管函数返回的字符串为: {0}", outBuffer.ToString());
            Console.WriteLine("================================================");
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送字符串作为参数(非托管代码分配内存)
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 手动释放内存
            var strIntPtr = IntPtr.Zero;
            TestStringArgumentOutIntPtr(1, ref strIntPtr);

            var strResult = Marshal.PtrToStringUni(strIntPtr);
            Marshal.FreeCoTaskMem(strIntPtr);
            Console.WriteLine("Return string IntPtr: {0}", strResult);

            // 自动释放内存
            TestStringArgumentOut(2, ref strResult);
            Console.WriteLine("Return string value: {0}", strResult);
            Console.WriteLine("================================================");
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 封送作为返回值的字符串
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // 手动释放内存
            var strPtr = TestStringAsResultIntPtr(1);
            var result = Marshal.PtrToStringUni(strPtr);

            Marshal.FreeCoTaskMem(strPtr);
            Console.WriteLine("Return string IntPtr: {0}", result);

            // 自动释放内存
            result = TestStringAsResult(2);
            Console.WriteLine("Return string value: {0}", result);

            Console.WriteLine("\r\n按任意键退出...");
            Console.Read();
        }
    }
}