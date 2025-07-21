#include "pch.h"
#include "TaskbarProgress.h"

/*

实现任务栏图标上的进度条 参考：

任务栏扩展 - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/shell/taskbar-extensions#progress-bars

ITaskbarList3(shobjidl_core.h) - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nn-shobjidl_core-itaskbarlist3

*/

static ITaskbarList3* pList = nullptr;
static HWND targetHwnd = nullptr;
static BOOL initialized = FALSE;

void InitializeTaskbarList(HWND hWnd, BOOL enable)
{
    if (enable && !initialized && hWnd &&
        SUCCEEDED(CoCreateInstance(CLSID_TaskbarList, nullptr, CLSCTX_ALL, IID_ITaskbarList3, (LPVOID*)&pList)))
    {
        targetHwnd = hWnd;
        pList->HrInit();
        initialized = TRUE;
    }
}

void SetTaskbarProgressState(TBPFLAG tbpFlags)
{
    if (initialized)
    {
        pList->SetProgressState(targetHwnd, tbpFlags);
    }
}

void SetTaskbarProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal)
{
    if (initialized)
    {
        pList->SetProgressValue(targetHwnd, ullCompleted, ullTotal);
    }
}

void ReleaseTaskbarList()
{
    if (initialized)
    {
        if (pList) pList->Release();
        pList = nullptr;
        targetHwnd = nullptr;
        initialized = FALSE;
    }
}
