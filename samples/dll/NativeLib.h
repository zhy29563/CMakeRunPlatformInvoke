#pragma once
#include <cstdio>
#include <cstring>
#include <cstdlib>
#include <combaseapi.h>
#include <corecrt_wstdio.h>
#include <combaseapi.h>
#include <cwchar>
#include <new>
#include <clocale>
#include <iostream>

typedef struct _SIMPLESTRUCT
{
    int intValue;
    short shortValue;
    float floatValue;
    double doubleValue;
} SIMPLESTRUCT, *PSIMPLESTRUCT;

typedef struct _MSEMPLOYEE
{
    int employeeID;
    short employedYear;
    char *displayName;
    char *alias;
} MSEMPLOYEE, *PMSEMPLOYEE;

typedef struct _MSEMPLOYEE2
{
    unsigned int employeeID;
    short employedYear;
    char displayName[255];
    char alias[255];
} MSEMPLOYEE2, *PMSEMPLOYEE2;

typedef struct _PERSONNAME
{
    char *first;
    char *last;
    char *displayName;
} PERSONNAME, *PPERSONNAME;

typedef struct _PERSON
{
    PPERSONNAME pName;
    int age;
} PERSON, *PPERSON;

typedef struct _PERSON2
{
    PERSONNAME name;
    int age;
} PERSON2, *PPERSON2;

extern "C"
{
    __declspec(dllexport) void __stdcall PrintMsg(const char *msg);

    __declspec(dllexport) void __stdcall ReverseAnsiString(char *rawString, char *reversedString);

    __declspec(dllexport) void __stdcall ReverseUnicodeString(wchar_t *rawString, wchar_t *reversedString);

    // 该函数的第一个参数是一个void*指针，可以输入任意指针类型，通过第二个参数来判断具体输入的类型
    __declspec(dllexport) void __stdcall PrintMsgByFlag(void *msgData, int flag);

    __declspec(dllexport) wchar_t *__stdcall GetStringMalloc();
    __declspec(dllexport) void __stdcall FreeMallocMemory(void *pBuffer);

    __declspec(dllexport) wchar_t *__stdcall GetStringNew();
    __declspec(dllexport) void __stdcall FreeNewMemory(void *pBuffer);

    __declspec(dllexport) wchar_t *__stdcall GetStringCoTaskMemAlloc();
    __declspec(dllexport) void __stdcall FreeCoTaskMemAllocMemory(void *pBuffer);

    __declspec(dllexport) int __stdcall Multiply(const int factorA, const int factorB);

    __declspec(dllexport) void __stdcall TestStringArgumentsFixLength(const wchar_t *inString,
                                                                      wchar_t *outString,
                                                                      int bufferSize);
    __declspec(dllexport) void __stdcall TestStringMarshalArguments(const char *inAnsiString,
                                                                    const wchar_t *inUnicodeString,
                                                                    wchar_t *outUnicodeString,
                                                                    int outBufferSize);

    __declspec(dllexport) void __stdcall TestStringArgumentOut(const int id, wchar_t **ppString);
    __declspec(dllexport) wchar_t *__stdcall TestStringAsResult(int id);

    __declspec(dllexport) void __stdcall TestStructArgumentByVal(SIMPLESTRUCT simpleStruct);
    __declspec(dllexport) void __stdcall TestStructArgumentByRef(PSIMPLESTRUCT pStruct);
    __declspec(dllexport) PSIMPLESTRUCT __stdcall TestReturnStruct(void);
    __declspec(dllexport) PSIMPLESTRUCT __stdcall TestReturnNewStruct(void);
    __declspec(dllexport) void __stdcall FreeStruct(PSIMPLESTRUCT pStruct);
    __declspec(dllexport) void __stdcall TestReturnStructFromArg(PSIMPLESTRUCT *ppStruct);
    __declspec(dllexport) void __stdcall GetEmployeeInfo(const PMSEMPLOYEE pEmployee);
    __declspec(dllexport) void __stdcall GetEmployeeInfo2(PMSEMPLOYEE2 pEmployee);
    __declspec(dllexport) void __stdcall TestStructInStructByRef(PPERSON pPerson);
    __declspec(dllexport) void __stdcall TestStructInStructByVal(PPERSON2 pPerson);
    __declspec(dllexport) void __stdcall TestStructArgumentByRef(PSIMPLESTRUCT pStruct);
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 托管结构体与拖过类作为封送参数的区别(TestManagedStructAndClass，简称 TMSC)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
typedef struct _TestManagedStructAndClass
{
    int id;
    char *name;
} TestManagedStructAndClass, *PTestManagedStructAndClass, **PPTestManagedStructAndClass;
// 非托管函数的参数为结构体，在非托管函数的托管声明中仅能使用托管结构体作为参数
extern "C" inline __declspec(dllexport) void __stdcall TMSC_ParameterIsStruct(TestManagedStructAndClass s)
{
    s.name = static_cast<char *>(CoTaskMemAlloc(255));
    strcpy_s(s.name, 255, "zhang san");

    std::cout << "=========================================" << std::endl;
    std::cout << "unmanaged, the id is " << s.id << std::endl;
    std::cout << "unmanaged, the name is " << s.name << std::endl;
}
// 非托管函数的参数为结构体指针，在非托管函数的托管声明中既可以使用托管结构体作为参数，也可以使用托管类作为参数
extern "C" inline __declspec(dllexport) void __stdcall TMSC_ParameterIsStructPointer(PTestManagedStructAndClass ps)
{
    ps->name = static_cast<char *>(CoTaskMemAlloc(255));
    strcpy_s(ps->name, 255, "li si");
    std::cout << "=========================================" << std::endl;
    std::cout << "unmanaged, the id is " << ps->id << std::endl;
    std::cout << "unmanaged, the name is " << ps->name << std::endl;
}

// 非托管函数的参数为二级结构体指针，在非托管函数的托管声明中仅可以使用托管类作为参数
extern "C" inline __declspec(dllexport) void __stdcall TMSC_ParameterIsStructPointerPointer(PPTestManagedStructAndClass pps)
{
    *pps = static_cast<PTestManagedStructAndClass>(CoTaskMemAlloc(sizeof(TestManagedStructAndClass)));
    PTestManagedStructAndClass ps = *pps;
    ps->id = 88888;
    ps->name = static_cast<char *>(CoTaskMemAlloc(255));
    strcpy_s(ps->name, 255, "wang wu");

    std::cout << "=========================================" << std::endl;
    std::cout << "unmanaged, the id is " << ps->id << std::endl;
    std::cout << "unmanaged, the name is " << ps->name << std::endl;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 方向属性
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
typedef struct _TestDirection
{
    int id;
    char *name;
} TestDirection, *PTestDirection, **PPTestDirection;

// 非托管函数的参数是对象而非指针或引用
extern "C" inline __declspec(dllexport) void __stdcall Direction_ParameterIsStruct(TestDirection d)
{
    d.name = static_cast<char *>(CoTaskMemAlloc(255));
    strcpy_s(d.name, 255, "zhang san");

    std::cout << "=========================================" << std::endl;
    std::cout << "unmanaged, the id is " << d.id << std::endl;
    std::cout << "unmanaged, the name is " << d.name << std::endl;
}

extern "C" inline __declspec(dllexport) void __stdcall Direction_ParameterIsPointer(PTestDirection pd)
{
    pd->name = static_cast<char *>(CoTaskMemAlloc(255));
    strcpy_s(pd->name, 255, "li si");
    std::cout << "=========================================" << std::endl;
    std::cout << "unmanaged, the id is " << pd->id << std::endl;
    std::cout << "unmanaged, the name is " << pd->name << std::endl;
}

extern "C" inline __declspec(dllexport) void __stdcall Direction_ParameterIsPointerPointer(PPTestDirection pps)
{
    *pps = static_cast<PTestDirection>(CoTaskMemAlloc(sizeof(TestDirection)));
    PTestDirection ps = *pps;
    ps->id = 88888;
    ps->name = static_cast<char *>(CoTaskMemAlloc(255));
    strcpy_s(ps->name, 255, "wang wu");
    std::cout << "=========================================" << std::endl;
    std::cout << "unmanaged, the id is " << ps->id << std::endl;
    std::cout << "unmanaged, the name is " << ps->name << std::endl;
}

