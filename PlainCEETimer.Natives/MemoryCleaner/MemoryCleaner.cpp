#include "pch.h"
#include "MemoryCleaner.h"

/*

通过清空工作集来实现减少内存占用参考:

.NET EXE memory footprint - Stack Overflow
https://stackoverflow.com/a/223300/21094697

*/

static HANDLE hProc = nullptr;

void CleanMemory(SIZE_T threshold)
{
    PROCESS_MEMORY_COUNTERS_EX pmc = {};

    if (!hProc)
    {
        hProc = GetCurrentProcess();
    }

    if (GetProcessMemoryInfo(hProc, (PPROCESS_MEMORY_COUNTERS)&pmc, sizeof(pmc)))
    {
        if (pmc.PrivateUsage > threshold)
        {
            EmptyWorkingSet(hProc);
        }
    }
}
