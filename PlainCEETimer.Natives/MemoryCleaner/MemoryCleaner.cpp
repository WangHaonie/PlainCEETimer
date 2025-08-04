#include "pch.h"
#include "MemoryCleaner.h"
#include <psapi.h>

/*

通过清空工作集来实现减少内存占用参考:

.NET EXE memory footprint - Stack Overflow
https://stackoverflow.com/a/223300/21094697

*/

void ClearProcessMemory()
{
    EmptyWorkingSet(GetCurrentProcess());
}

SIZE_T GetProcessMemoryEx()
{
    PROCESS_MEMORY_COUNTERS_EX2 pmc2 = {};

    if (GetProcessMemoryInfo(GetCurrentProcess(), (PPROCESS_MEMORY_COUNTERS)&pmc2, sizeof(pmc2)))
    {
        return pmc2.PrivateWorkingSetSize;
    }

    return 0;
}
