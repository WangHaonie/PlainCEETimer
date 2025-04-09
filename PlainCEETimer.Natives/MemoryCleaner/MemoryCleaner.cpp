#include "pch.h"
#include "MemoryCleaner.h"

/*

通过清空工作集来实现减少内存占用参考:

.NET EXE memory footprint - Stack Overflow
https://stackoverflow.com/a/223300/21094697

*/

static HANDLE hwProc;

void CleanMemory(int threshold)
{
    PROCESS_MEMORY_COUNTERS_EX pmc = {};

    if (!hwProc)
    {
        hwProc = GetCurrentProcess();
    }

    if (GetProcessMemoryInfo(hwProc, (PROCESS_MEMORY_COUNTERS*)&pmc, sizeof(pmc)))
    {
        if (pmc.PrivateUsage > threshold)
        {
            EmptyWorkingSet(hwProc);
        }
    }
}
