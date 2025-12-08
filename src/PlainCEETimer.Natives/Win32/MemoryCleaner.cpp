#include "pch.h"
#include "MemoryCleaner.h"
#include <psapi.h>
#include <Windows.h>

/*

通过清空工作集来实现减少内存占用参考:

.NET EXE memory footprint - Stack Overflow
https://stackoverflow.com/a/223300/21094697

*/

void ClearProcessWS()
{
    EmptyWorkingSet(GetCurrentProcess());
}

SIZE_T GetProcessPrivateWS()
{
    /*
    
    使用新版 API 的 PMC_EX2 结构获取进程 Private WS 参考：

    PROCESS_MEMORY_COUNTERS_EX2 - Win32 apps | Microsoft Learn
    https://learn.microsoft.com/en-us/windows/win32/api/psapi/ns-psapi-process_memory_counters_ex2

    */

    PROCESS_MEMORY_COUNTERS_EX2 pmc2 = {};

    if (GetProcessMemoryInfo(GetCurrentProcess(), (PPROCESS_MEMORY_COUNTERS)&pmc2, sizeof(pmc2)))
    {
        return pmc2.PrivateWorkingSetSize;
    }

    return 0;
}
