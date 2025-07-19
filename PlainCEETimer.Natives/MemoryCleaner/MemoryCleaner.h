#pragma once

#include <windows.h>
#include <psapi.h>

cexport(void) ClearProcessMemory();
cexport(SIZE_T) GetProcessMemoryEx();