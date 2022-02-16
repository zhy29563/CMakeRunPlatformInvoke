#pragma once
#include <cstdio>

extern "C"
{
    __declspec(dllexport) void __stdcall PrintMsg(const char *msg);

    __declspec(dllexport) void __stdcall ReverseAnsiString(char *rawString, char *reversedString);

    __declspec(dllexport) void __stdcall ReverseUnicodeString(wchar_t *rawString, wchar_t *reversedString);

    // 该函数的第一个参数是一个void*指针，可以输入任意指针类型，通过第二个参数来判断具体输入的类型
    __declspec(dllexport) void __stdcall PrintMsgByFlag(void *msgData, int flag);

    __declspec(dllexport) wchar_t* __stdcall GetStringMalloc();
    __declspec(dllexport) void     __stdcall FreeMallocMemory(void* pBuffer);

    __declspec(dllexport) wchar_t* __stdcall GetStringNew();
    __declspec(dllexport) void     __stdcall FreeNewMemory(void* pBuffer);

    __declspec(dllexport) wchar_t* __stdcall GetStringCoTaskMemAlloc();
    __declspec(dllexport) void     __stdcall FreeCoTaskMemAllocMemory(void* pBuffer);
}