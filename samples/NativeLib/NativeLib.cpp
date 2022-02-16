#include <NativeLib.h>
#include <cstring>

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