#include "pch.h"
#include "DpiAwareness.h"

//
// 通过代码设置 DPI 感知 参考：
//
// 为进程设置默认 DPI 感知 (Windows) - Win32 apps | Microsoft Learn
// https://learn.microsoft.com/zh-cn/windows/win32/hidpi/setting-the-default-dpi-awareness-for-a-process
//

void SetProcessDpiAwarenessEx(int windowsid)
{
	switch (windowsid)
	{
		case 1:
			SetProcessDpiAwareness(PROCESS_PER_MONITOR_DPI_AWARE);
			break;
		case 2:
			SetProcessDpiAwarenessContext(((DPI_AWARENESS_CONTEXT)-4));
			break;
		case 0:
		default:
			SetProcessDPIAware();
			break;
	}
}