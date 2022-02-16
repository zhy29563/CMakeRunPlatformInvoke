#include <NativeLib.h>
#include <cstring>
#include <cstdlib>
#include <combaseapi.h>

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

wchar_t* GetStringMalloc()
{
    const auto iBufferSize = 128;
    
    // malloc 分配，需使用 free进行释放
    auto* const pBuffer = static_cast<wchar_t*>(malloc(iBufferSize));
	if (nullptr != pBuffer)
	{
		wcscpy_s(pBuffer, iBufferSize / sizeof(wchar_t), L"String from MALLOC");
	}
	return pBuffer;
}

void FreeMallocMemory(void* pBuffer)
{
	if (nullptr != pBuffer)
	{
		free(pBuffer);
		pBuffer = nullptr;

        printf("Release memory is sucessful.\r\n");
	}
}

wchar_t* GetStringNew()
{
    const auto iBufferSize = 128;
    auto* const pBuffer = new wchar_t[iBufferSize];
	if (nullptr != pBuffer)
	{
		wcscpy_s(pBuffer, iBufferSize / sizeof(wchar_t), L"String from NEW");
	}
	return pBuffer;
}

void FreeNewMemory(void* pBuffer)
{
	if (nullptr != pBuffer)
	{
		delete pBuffer;
		pBuffer = nullptr;
        printf("Release memory is sucessful.\r\n");
	}
}

wchar_t* GetStringCoTaskMemAlloc()
{
    const int iBufferSize = 128;
    auto* const pBuffer = static_cast<wchar_t*>(CoTaskMemAlloc(iBufferSize));
	if (nullptr != pBuffer)
	{
		wcscpy_s(pBuffer, iBufferSize / sizeof(wchar_t), L"String from CoTaskMemAlloc");
	}
	return pBuffer;
}

void FreeCoTaskMemAllocMemory(void* pBuffer)
{
	if (nullptr != pBuffer)
	{
		CoTaskMemFree(pBuffer);
		pBuffer = nullptr;
	}
}