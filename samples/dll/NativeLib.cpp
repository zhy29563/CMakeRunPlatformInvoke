#include <NativeLib.h>


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

void ShowNativeStructSize(size_t size)
{
    // 支持中文字符显示
    _wsetlocale(LC_ALL, L"chs");

    wprintf(L"The size of unmanaged struct is (%d) bytes。\n", size);
}

void __cdecl TestStructArgumentByVal(SIMPLESTRUCT simpleStruct)
{
    ShowNativeStructSize(sizeof(SIMPLESTRUCT));
    // 打印结构体数据
    wprintf(L"\nRaw data of struct: int = %d, short = %d, float = %f, double = %f\n",
            simpleStruct.intValue,
            simpleStruct.shortValue,
            simpleStruct.floatValue,
            simpleStruct.doubleValue);

    // 非托管函数中对结构体的字段进行改变，可以在托管方查看输入结构体是否改变
    simpleStruct.intValue += 10;
}

void TestStructArgumentByRef(PSIMPLESTRUCT pStruct)
{
    ShowNativeStructSize(sizeof(SIMPLESTRUCT));

    if (nullptr != pStruct)
    {
        // 打印初始数据
        wprintf(L"\nRaw data of struct: int = %d, short = %d, float = %f, double = %f\n",
                pStruct->intValue,
                pStruct->shortValue,
                pStruct->floatValue,
                pStruct->doubleValue);

        // 修改数据
        pStruct->intValue++;
        pStruct->shortValue++;
        pStruct->floatValue += 1;
        pStruct->doubleValue += 1;
    }
}

PSIMPLESTRUCT TestReturnStruct(void)
{
    // 使用CoTaskMemAlloc分配内存
    auto *const pStruct = static_cast<PSIMPLESTRUCT>(CoTaskMemAlloc(sizeof(SIMPLESTRUCT)));

    pStruct->intValue = 5;
    pStruct->shortValue = 4;
    pStruct->floatValue = 3.0;
    pStruct->doubleValue = 2.1;

    return pStruct;
}

PSIMPLESTRUCT TestReturnNewStruct(void)
{
    // 使用new分配内存
    auto *const pStruct = new SIMPLESTRUCT();

    pStruct->intValue = 5;
    pStruct->shortValue = 4;
    pStruct->floatValue = 3.0;
    pStruct->doubleValue = 2.1;

    return pStruct;
}

void FreeStruct(PSIMPLESTRUCT pStruct)
{
    if (nullptr != pStruct)
    {
        delete pStruct;
        pStruct = nullptr;
    }
}

void TestReturnStructFromArg(PSIMPLESTRUCT *ppStruct)
{
    if (nullptr != ppStruct)
    {
        *ppStruct = static_cast<PSIMPLESTRUCT>(CoTaskMemAlloc(sizeof(SIMPLESTRUCT)));

        (*ppStruct)->intValue = 5;
        (*ppStruct)->shortValue = 4;
        (*ppStruct)->floatValue = 3.0;
        (*ppStruct)->doubleValue = 2.1;
    }
}

void GetEmployeeInfo(const PMSEMPLOYEE pEmployee)
{
    if (nullptr != pEmployee)
    {
        pEmployee->employedYear = 2;
        pEmployee->alias = static_cast<char *>(CoTaskMemAlloc(255));
        pEmployee->displayName = static_cast<char *>(CoTaskMemAlloc(255));

        strcpy_s(pEmployee->alias, 255, "xcui");
        strcpy_s(pEmployee->displayName, 255, "Xiaoyuan Cui");
    }
}

void GetEmployeeInfo2(PMSEMPLOYEE2 pEmployee)
{
    if (nullptr != pEmployee)
    {
        pEmployee->employedYear = 2;
        strcpy_s(pEmployee->alias, 255, "jizhou");
        strcpy_s(pEmployee->displayName, 255, "Jizhou Huang");
    }
}

void TestStructInStructByRef(PPERSON pPerson)
{
    const auto firstLen = strlen(pPerson->pName->first);
    const auto lastLen = strlen(pPerson->pName->last);

    auto* const temp = static_cast<char*>(CoTaskMemAlloc(sizeof(char) * (firstLen + lastLen + 2)));
	sprintf_s(temp, firstLen + lastLen + 2, "%s %s", pPerson->pName->last, pPerson->pName->first);

	CoTaskMemFree(pPerson->pName->displayName);
	pPerson->pName->displayName = temp;
}

void TestStructInStructByVal(PPERSON2 pPerson)
{
	// 支持中文字符显示
	setlocale(LC_ALL, "chs");
	printf("姓 = %s\n名 = %s\n年龄 = %i\n\n",pPerson->name.last, pPerson->name.first, pPerson->age);
}

