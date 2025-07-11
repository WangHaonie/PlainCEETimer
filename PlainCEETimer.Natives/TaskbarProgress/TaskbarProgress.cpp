#include "pch.h"
#include "TaskbarProgress.h"

/*

实现任务栏图标上的进度条 参考：

任务栏扩展 - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/shell/taskbar-extensions#progress-bars

ITaskbarList3(shobjidl_core.h) - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nn-shobjidl_core-itaskbarlist3

*/

static ITaskbarList3* pTaskbarList = nullptr;
static HWND handle = nullptr;
static int _enable = 0;

void InitializeTaskbarList(HWND hWnd, BOOL enable)
{
    if (enable)
    {
        if (hWnd && SUCCEEDED(CoCreateInstance(CLSID_TaskbarList, nullptr, CLSCTX_ALL, IID_ITaskbarList3, (void**)&pTaskbarList)))
        {
            handle = hWnd;
            pTaskbarList->HrInit();
            _enable = enable;
        }
    }
}

void SetTaskbarProgressState(TBPFLAG tbpFlags)
{
    if (_enable)
    {
        pTaskbarList->SetProgressState(handle, tbpFlags);
    }
}

void SetTaskbarProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal)
{
    if (_enable)
    {
        pTaskbarList->SetProgressValue(handle, ullCompleted, ullTotal);
    }
}

void ReleaseTaskbarList()
{
    if (_enable && pTaskbarList)
    {
        pTaskbarList->Release();
        CoUninitialize();
        pTaskbarList = nullptr;
        handle = nullptr;
    }
}
