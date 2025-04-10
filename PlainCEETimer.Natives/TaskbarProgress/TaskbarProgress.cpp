#include "pch.h"
#include "TaskbarProgress.h"

/*

实现任务栏图标上的进度条 参考：

任务栏扩展 - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/shell/taskbar-extensions#progress-bars

ITaskbarList3(shobjidl_core.h) - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nn-shobjidl_core-itaskbarlist3

ITaskbarList3：：SetProgressState(shobjidl_core.h) - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nf-shobjidl_core-itaskbarlist3-setprogressstate

ITaskbarList3：：SetProgressValue(shobjidl_core.h) - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nf-shobjidl_core-itaskbarlist3-setprogressvalue

ITaskbarList：：HrInit(shobjidl_core.h) - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nf-shobjidl_core-itaskbarlist-hrinit

IUnknown：：Release - Win32 apps | Microsoft Learn
https://learn.microsoft.com/zh-cn/windows/win32/api/unknwn/nf-unknwn-iunknown-release

*/
static ITaskbarList3* _TaskbarList;
static HWND WindowHandle;

void InitilizeTaskbarList(HWND hWnd)
{
	WindowHandle = hWnd;

	if (SUCCEEDED(CoCreateInstance(CLSID_TaskbarList, NULL, CLSCTX_ALL, IID_ITaskbarList3, (void**)&_TaskbarList)))
	{
		_TaskbarList->HrInit();
	}
}

void SetTaskbarProgressState(int tbpFlags)
{
	if (_TaskbarList && WindowHandle)
	{
		_TaskbarList->SetProgressState(WindowHandle, (TBPFLAG)tbpFlags);
	}
}

void SetTaskbarProgressValue(ULONGLONG ullCompleted, ULONGLONG ullTotal)
{
	if (_TaskbarList && WindowHandle)
	{
		_TaskbarList->SetProgressValue(WindowHandle, ullCompleted, ullTotal);
	}
}

void ReleaseTaskbarList()
{
	if (_TaskbarList)
	{
		_TaskbarList->Release();
		_TaskbarList = nullptr;
	}

	WindowHandle = nullptr;
}
