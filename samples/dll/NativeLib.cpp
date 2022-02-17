#include <NativeLib.h>
#include <cstring>
#include <cstdlib>
#include <combaseapi.h>
#include <corecrt_wstdio.h>
#include <combaseapi.h>
#include <cwchar>
#include <new>

void PrintMsg(const char *msg)
{
    printf("%s\n", msg);
}

void ReverseAnsiString(char *rawString, char *reversedString)
{
    const auto strLength = static_cast<int>(strlen(rawString));

    strcpy(reversedString, rawString);

    for (auto i = 0; i < strLength / 2; i++)
    {
        const auto tempChar = reversedString[i];
        reversedString[i] = reversedString[strLength - 1 - i];
        reversedString[strLength - 1 - i] = tempChar;
    }
}

void ReverseUnicodeString(wchar_t *rawString, wchar_t *reversedString)
{
    const auto strLength = static_cast<int>(wcslen(rawString));

    wcscpy(reversedString, rawString);

    for (int i = 0; i < strLength / 2; i++)
    {
        const auto tempChar = reversedString[i];
        reversedString[i] = reversedString[strLength - 1 - i];
        reversedString[strLength - 1 - i] = tempChar;
    }
}

void __stdcall PrintMsgByFlag(void *msgData, int flag)
{
    if (flag == 1) // integer
    {
        auto *const pID = static_cast<int *>(msgData);
        printf("Your ID is: %i\n", *pID);
    }
    else if (flag == 2) // char
    {
        auto *const pName = static_cast<char *>(msgData);
        printf("Your name is: %s\n", pName);
    }
}

wchar_t *GetStringMalloc()
{
    const auto iBufferSize = 128;

    // malloc 分配，需使用 free进行释放
    auto *const pBuffer = static_cast<wchar_t *>(malloc(iBufferSize));
    if (nullptr != pBuffer)
    {
        wcscpy_s(pBuffer, iBufferSize / sizeof(wchar_t), L"String from MALLOC");
    }
    return pBuffer;
}

void FreeMallocMemory(void *pBuffer)
{
    if (nullptr != pBuffer)
    {
        free(pBuffer);
        pBuffer = nullptr;

        printf("Release memory is sucessful.\r\n");
    }
}

wchar_t *GetStringNew()
{
    const auto iBufferSize = 128;
    auto *const pBuffer = new wchar_t[iBufferSize];
    if (nullptr != pBuffer)
    {
        wcscpy_s(pBuffer, iBufferSize / sizeof(wchar_t), L"String from NEW");
    }
    return pBuffer;
}

void FreeNewMemory(void *pBuffer)
{
    if (nullptr != pBuffer)
    {
        delete pBuffer;
        pBuffer = nullptr;
        printf("Release memory is sucessful.\r\n");
    }
}

wchar_t *GetStringCoTaskMemAlloc()
{
    const int iBufferSize = 128;
    auto *const pBuffer = static_cast<wchar_t *>(CoTaskMemAlloc(iBufferSize));
    if (nullptr != pBuffer)
    {
        wcscpy_s(pBuffer, iBufferSize / sizeof(wchar_t), L"String from CoTaskMemAlloc");
    }
    return pBuffer;
}

void FreeCoTaskMemAllocMemory(void *pBuffer)
{
    if (nullptr != pBuffer)
    {
        CoTaskMemFree(pBuffer);
        pBuffer = nullptr;
    }
}

int Multiply(const int factorA, const int factorB)
{
    return factorA * factorB;
}

void TestStringArgumentsFixLength(const wchar_t *inString, wchar_t *outString, int bufferSize)
{
    if (nullptr != inString)
    {
        wcscpy_s(outString, bufferSize, inString);
    }
}

void TestStringMarshalArguments(const char *inAnsiString,
                                const wchar_t *inUnicodeString,
                                wchar_t *outUnicodeString, int outBufferSize)
{
    const auto ansiStrLength = strlen(inAnsiString);
    const auto uniStrLength = wcslen(inUnicodeString);

    const auto totalSize = ansiStrLength + uniStrLength + 2;
    auto *const tempBuffer = new (std::nothrow) wchar_t[totalSize];
    if (nullptr == tempBuffer)
    {
        return;
    }

    wmemset(tempBuffer, 0, totalSize);
    size_t num;
    mbstowcs_s(&num, tempBuffer, totalSize, inAnsiString, totalSize);
    wcscat_s(tempBuffer, totalSize, L" ");
    wcscat_s(tempBuffer, totalSize, inUnicodeString);
    wcscpy_s(outUnicodeString, outBufferSize, tempBuffer);

    delete[] tempBuffer;
}

void TestStringArgumentOut(const int id, wchar_t **ppString)
{
    if (nullptr != ppString)
    {
        const auto bufferSize = 128;
        *ppString = static_cast<wchar_t *>(CoTaskMemAlloc(bufferSize));
        swprintf_s(*ppString, bufferSize / sizeof(wchar_t), L"Out string of ID: %d", id);
    }
}

wchar_t *TestStringAsResult(int id)
{
    const auto bufferSize = 64;
    auto *result = static_cast<wchar_t *>(CoTaskMemAlloc(bufferSize));
    swprintf_s(result, bufferSize / sizeof(wchar_t), L"Result of ID: %d", id);
    return result;
}