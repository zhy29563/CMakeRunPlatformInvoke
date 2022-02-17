using System;
using System.Runtime.InteropServices;

namespace PlatformInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    class ManagedClassBlittable
    {
        private int _intValue;
        private short _shortValue;
        private float _floatValue;
        private double _doubleValue;

        #region Properties
        public int IntValue
        {
            get { return _intValue; }
            set { _intValue = value; }
        }

        public short ShortValue
        {
            get { return _shortValue; }
            set { _shortValue = value; }
        }

        public float FloatValue
        {
            get { return _floatValue; }
            set { _floatValue = value; }
        }

        public double DoubleValue
        {
            get { return _doubleValue; }
            set { _doubleValue = value; }
        }
        #endregion
    }

    internal class Program
    {
        // 具体而言，CLR在进行数据封送时，有两种选择：要么锁定(pin)数据，要么复制(copy)数据。
        // 在默认情况下，CLR会在封送过程中对数据进行负责。例如，如果托管代码要将一个字符串(Unicode)作为
        // ANSI格式的字符传递给非托管代码，那么CLR就会先对该字符串进行复制，然后将复制的字符串转化为ANSI
        // 字符串，最后将字符串的内存地址传递给非托管代码。由于复制的字符串的操作可能会比较费事，因此数据
        // 封送过程中的复制操作也就成为影响平台调用性能的因素之一。

        // 利益方面，CLR也可以通过直接将托管对象锁定在堆内存上，以阻止托管对象在函数调用声明周期内被移动
        // 或回收。一旦托管对象被锁定，就可以直接将直线给托管对象的指针传递给非托管代码了，从而避免了复制
        // 操作，达到优化封送过程的目的。

        // 只有同时满足了以下4个条件，托管对象才能被锁定
        //  1. 必须时托管代码调用本机代码的情形
        //  2. 托管数据类型必须是可直接复制到本机结构中的(Blittable)数据类型，或者在满足某些条件下转变成
        //     Blittable类型的数据类型
        //  3. 传递的不是引用(out/ref)参数
        //  4. 被调用代码和调用代码必须处于同一个线程上下文或者线程单元中

        // Blittable数据类型，对于托管代码和非托管而言，其内存中的表示形式是完全相同的。因此，对于数据封
        // 送而言，Blittable数据类型是一种非常高效的数据类型。因为对于这种数据类型进行封送时，可以不经过
        // 任何类型转换，更不用经过封送拆收器的任何特俗处理，就能在托管代码和非托管代码之间进行传递。

        // Blittable数据类型
        //  System.Byte
        //  System.SByte
        //  System.Int16
        //  System.UInt16
        //  System.Int32
        //  System.UInt32
        //  System.Int64
        //  System.UInt64
        //  System.Single
        //  System.Double
        //  System.IntPtr
        //  System.UIntPtr

        //  需要特别说明的时IntPtr和UIntPtr数据类型，这两种数据类型时平台相关的类型。也就是说，在32为平
        //  台上，他们表示32为整型；而在64位平台上，它们则表示64位整型。因此如果需要处理与平台有关的数据
        //  类型，他们就成为了最佳选择。因此，IntPtr和UIntPtr也可以用来存放指针或句柄，这在封送非托管函
        //  数返回的指针和句柄时特别有用。

        // 除上面列出的Blittable数据类型外，还有一些数据类型也属于Blittable类型，比如数据元素时Blittable
        // 的一位数据，还有只包含Blittable类型的格式化值类型（如成员数据类型全部都是Blittable类型且作为格
        // 式化值类型封送的类）

        // 除此之外，一些非直接复制到本机结构中的数据类型(non-blittable)在某些特定的条件下也能变成blittable，
        // 比如char类型。在默认情况下，char类型是non-blittable类型。这是由于char类型既可以映射成Unicode字符
        // 也可以映射成ANSI字符。但是，由于char类型在CLR中始终是Unicode字符，因此只要在便赐额非托管函数定义
        // 时，将CharSet显示地指定位 CharSet.Unicode，或者单独位char类型加上MarshalAs属性，比如[MarshalAs(UnmanagedType.LPWSTR)],
        // char类型就变成blittable类型了。
        // 如果【引用类型】的【所有字段】都是【Blittable】类型，那么在封送参数时，
        // 即使不指定【Out】特性，也能得到非托管代码更新后的结果。


        [DllImport("NativeLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void TestStructArgumentByRef(ManagedClassBlittable argClass);

        private static void Main()
        {
            ManagedClassBlittable blittableObject = new ManagedClassBlittable();
            blittableObject.IntValue = 1;
            blittableObject.ShortValue = 2;
            blittableObject.FloatValue = 3;
            blittableObject.DoubleValue = 4.5;

            TestStructArgumentByRef(blittableObject);

            Console.WriteLine("\n结构体新数据：int = {0}, short = {1}, float = {2:f6}, double = {3:f6}",
                              blittableObject.IntValue, 
                              blittableObject.ShortValue, 
                              blittableObject.FloatValue, 
                              blittableObject.DoubleValue);
                              
            Console.WriteLine("\r\n按任意键退出...");
            Console.Read();
        }
    }
}