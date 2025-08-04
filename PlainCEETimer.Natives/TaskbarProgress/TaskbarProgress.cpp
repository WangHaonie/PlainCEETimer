#include "pch.h"
#include "TaskbarProgress.h"

/*

实现任务栏图标上的进度条 参考：

任务栏扩展 - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/shell/taskbar-extensions#progress-bars

ITaskbarList3(shobjidl_core.h) - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nn-shobjidl_core-itaskbarlist3

*/

static ITaskbarList3* ptl = nullptr;
static HWND targetHwnd = nullptr;
static BOOL initialized = FALSE;

void InitializeTaskbarList(HWND hWnd)
{
    if (!initialized && hWnd &&
        SUCCEEDED(CoCreateInstance(CLSID_TaskbarList, nullptr, CLSCTX_INPROC_SERVER, IID_ITaskbarList3, (LPVOID*)&ptl)))
    {
        targetHwnd = hWnd;
        ptl->HrInit();
        initialized = TRUE;
    }
}

void TaskListSetProgressState(TBPFLAG tbpFlags)
{
    if (initialized)
    {
        ptl->SetProgressState(targetHwnd, tbpFlags);
    }
}

void TaskListSetProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal)
{
    if (initialized)
    {
        ptl->SetProgressValue(targetHwnd, ullCompleted, ullTotal);
    }
}

void ReleaseTaskbarList()
{
    if (ptl) ptl->Release();
    ptl = nullptr;
    targetHwnd = nullptr;
    initialized = FALSE;
}
