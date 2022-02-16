using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PlatformInvoke
{
    internal class Program
    {
        /*
         * 进行平台调用【必须】在托管代码中对非托管代码进行重新声明，才能在托管代码中使用【非托管DLL导出的函数】，而且在托管代码中
         * 为非托管代码编写托管定义时，还必须为其加上【DllImportAttribute】这个必须特性。
         * 
         * - dllName
         *      默认构造函数：public DllImportAttribute (string dllName);
         *      参数【dllName】指明需要导入函数的DLL的名称。该名称可以是绝对路径，也可以是相对路径。
         *      如果指定为相对路径，CLR搜索路径依次如下：
         *          1. 当前目录
         *          2. Windows系统目录
         *          3. PATH环境变量所列出的所有目录中进行搜索
         *          4. 都未找到抛出 DllNotFoundException
         *
         * - CallingConvention
         *      由于非托管DLL在导出非托管函数时可以采用一些不同的调用约定，这样就可能会出现使用【默认值】无法正确调用非托管函数的
         *      情况。因此，要在托管代码中成功地调用非托管函数，就必须根据导出非托管函数时所采用的调用约定来确定该字段的值。
         *      - Cdecl
         *          调用方(即进行平台调用的托管代码)负责清理堆栈。使得调用方能够调用具有【可变参数的函数】。
         *      - StdCall
         *          被调用方(即进行平台调用时的非托管函数)负责清理堆栈。
         *      - Winapi
         *          此成员实际上不是调用约定，而是使用默认平台调用约定。
         *      - ThisCall
         *          此调用约定主要用于`调用C++类中的方法`，而再调用C/C++导出函数(即普通(flat)函数)时一般不使用此之。第一个参数是
         *          this指针(指向C++对象的指针)。它存储在寄存器ECX中。其他参数被推送到堆栈上。
         *      - FastCall
         *          虽然CallingConvention枚举中包含此值，但是.NET却不支持此值，也不支持调用约定
         *
         * - CharSet
         *      .NET采用 Unicode 编码，而 C/C++ 之类的非托管代码却【没有对字符串编码进行强制规定】。因此在非托管代码中处理字符串
         *      时，用户既可以采用 ANSI 编码，也可以采用 Unicode 编码。正是由于这二者之间在字符串编码的处理上存在差异，才导致在对
         *      非托管函数进行平台调用时，必须指定与非托管函数相一致的编码方式。
         *
         *      CharSet字段的主要作用有两个：
         *          - 一是控制【字符串的封送处理方式】。将【System.String】封送为【ANSI】还是【UNICODE】字符串。
         *          - 二是确定平台调用【在非托管DLL中查找函数名的方式】。
         *
         *      在对Win32 API函数进行平台调用时，如果为【CharSet】指定了【CharSet.Unicode】，那么【CLR】就会自动调用【Unicode】
         *      版本的函数。如果指定【CharSet】字段为【CharSet.ANSI】，那么【CLR】就会调用【ANSI】版本函数。
         *      - CharSet.Ansi（默认值）
         *          平台调用将字符串从托管格式封送为ANSI格式
         *
         *          当【ExactSpelling】字段为【true】时，平台调用将【只搜索指定的名称】。例如，如果指定【MessageBox】，则平台调
         *          用将搜索【MessageBox】，如果找不到完全相同的名称则搜索失败。
         *
         *          当【ExactSpeclling】字段为【false】时，平台调用将先搜索【MessageBox】，如果未找到，则搜索【MessageBoxA】
         *      - CharSet.Unicode
         *          平台调用将字符串从托管格式封送为Unicode格式
         *
         *          当【ExactSpelling】字段为【true】时，平台调用将【只搜索指定的名称】。例如，如果指定【MessageBox】，则平台调
         *          用将搜索【MessageBox】，如果找不到完全相同的名称则搜索失败。
         *
         *          当【ExactSpeclling】字段为【false】时，平台调用将先搜索【MessageBoxW】，如果未找到，则搜索【MessageBox】
         *      - CharSet.Auto
         *          平台调用在运行时根据目标平台在【ANSI】和【UNICODE】格式之间进行选择
         *
         * - EntryPoint
         *      指定要调用的【DLL入口点】，实际上是要调用函数的地址。如果【没有指定该值】，意味着托管代码中的【非托管函数定义的函
         *      数名必须与非托管函数保持一致】，否则 CLR 就会抛出【EntryPointNotFoundException】异常。
         *      EntryPoint字段的一个重要作用是【指定要调用的非托管函数的精确名称】。由于C++允许函数重载，因此可能出现多个具有相同
         *      函数名的函数。
         *
         *
         * - ExactSpeclling
         *      控制是否应修改入口点以对应字符集。对于不同的编程语言，默认值将有所不同。
         *
         * - PreserveSig
         *      控制托管方法签名是否应转换成返回【HRESULT】且返回值有一个附加[out,retval]参数的非托管签名。
         *      默认值位true，即不应转换签名
         *
         * - SetLastError
         *      使调用方能够使用【Marshal.GetLastWin32Error】API函数确定在执行该方法时是否出错。
         *      在【Visual Basic】中，默认为【true】
         *      在【C#】和【C++】中，默认值为【false】
         *
         * - ThrowOnUmmappableChar
         *      如果将【Unicode】字符转换成【ANSI】字符时，无对应的字符，是否抛出异常。
         */
        // 指定为ANSI编码，将调用【ANSI】版本的非托管函数
        [DllImport("NativeLib.dll" /*, CharSet = CharSet.Ansi*/)]
        private static extern void ReverseAnsiString(string rawString, StringBuilder reversedString);

        // 指定为UNICODE编码，将调用【UNICODE】版本的非托管函数
        [DllImport("NativeLib.dll", CharSet = CharSet.Unicode)]
        private static extern void ReverseUnicodeString(string rawString, StringBuilder reversedString);

        // 非托管函数的托管定义中的函数名可以与非托管中的函数名不同，但必须使用EntryPoint字段指定实际调用非托管函数
        [DllImport("NativeLib.dll", EntryPoint = "PrintMsgByFlag")]
        private static extern void PrintID(ref int id, int flag);

        [DllImport("NativeLib.dll", EntryPoint = "PrintMsgByFlag")]
        private static extern void PrintName(string name, int flag);

        private static void Main()
        {
            // CharSet
            const string rawString = "Bill Gates";
            var reversedString = new StringBuilder(rawString.Length);

            ReverseAnsiString(rawString, reversedString);
            Console.WriteLine("Using ANSI, raw string: {0}, reversed string: {1}", rawString, reversedString);

            ReverseUnicodeString(rawString, reversedString);
            Console.WriteLine("Using Unicode, raw string: {0}, reversed string: {1}", rawString, reversedString);

            // EntryPoint
            var id = 100;
            const string name = "Bill Gates";

            PrintID(ref id, 1);
            PrintName(name, 2);

            Console.WriteLine("\r\n按任意键退出...");
            Console.Read();
        }
    }
}