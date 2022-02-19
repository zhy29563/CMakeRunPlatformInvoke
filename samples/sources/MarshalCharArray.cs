using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    internal class Program
    {
        [DllImport("NativeLib.dll", EntryPoint = "TestArrayOfString", CallingConvention = CallingConvention.StdCall)]
        private static extern void TestStringArrayAsInputParameter([In, Out] string[] charArray, int arraySize);


        [DllImport("NativeLib.dll", EntryPoint = "TestRefArrayOfString", CallingConvention = CallingConvention.StdCall)]
        private static extern int TestStringArrayAsOutputParameter(out IntPtr charArray, out int arraySize);

        private static void Main()
        {
            // 字符数组作为参数输入
            {
                string[] strings =
                {
                    "This is the first string.",
                    "Those are brown horse.",
                    "The quick brown fox jumps over a lazy dog."
                };

                Console.WriteLine("\nBefore modified:");
                foreach (string originalString in strings)
                {
                    Console.WriteLine(originalString);
                }

                TestStringArrayAsInputParameter(strings, strings.Length);

                Console.WriteLine("\nAfter modified:");
                foreach (string reversedString in strings)
                {
                    Console.WriteLine(reversedString);
                }
                Console.WriteLine("================================================");
            }

            // 字符数组作为参数输出
            {
                // 因为数组是在非托管代码内分配的，所以需要通过返回值或参数给出
                // 在这里arraySize和returnCount的返回值应该是一样的
                int returnCount = TestStringArrayAsOutputParameter(out var arrayPtr, out var arraySize);
                // 根据返回值确定字符串数量，在托管代码中声明相对应的指针数组
                IntPtr[] arrayPtrs = new IntPtr[returnCount];
                // 将非托管数组中的内容拷贝到托管代码中
                Marshal.Copy(arrayPtr, arrayPtrs, 0, returnCount);

                Console.WriteLine("\n The size of array: {0}", returnCount);
                Console.WriteLine("The element of array: ");
                // 声明字符串数组，用于存放最终的结果
                string[] strings = new string[returnCount];
                for (int i = 0; i < returnCount; i++)
                {
                    strings[i] = Marshal.PtrToStringUni(arrayPtrs[i]);
                    // 释放非托管字符串内存
                    Marshal.FreeCoTaskMem(arrayPtrs[i]);
                    Console.WriteLine("#{0}: {1}", i, strings[i]);
                }

                // 释放非托管字符串数组内存
                Marshal.FreeCoTaskMem(arrayPtr);
            }
        }
    }
}