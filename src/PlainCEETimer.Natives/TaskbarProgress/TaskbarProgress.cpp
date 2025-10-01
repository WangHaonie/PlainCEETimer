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
static bool Initialized = false;

void InitializeTaskbarList()
{
    if (!Initialized &&
        SUCCEEDED(CoCreateInstance(CLSID_TaskbarList, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&ptl))))
    {
        ptl->HrInit();
        Initialized = true;
    }
}

void TaskbarListSetProgressState(HWND hWnd, TBPFLAG tbpFlags)
{
    if (Initialized && hWnd)
    {
        ptl->SetProgressState(hWnd, tbpFlags);
    }
}

void TaskbarListSetProgressValue(HWND hWnd, ULONGLONG ullCompleted, ULONGLONG ullTotal)
{
    if (Initialized && hWnd)
    {
        ptl->SetProgressValue(hWnd, ullCompleted, ullTotal);
    }
}

void ReleaseTaskbarList()
{
    if (ptl) ptl->Release();
    ptl = nullptr;
    Initialized = false;
}
