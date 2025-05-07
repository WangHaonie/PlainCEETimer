#include "pch.h"
#include "ThemeManager.h"

/*

窗体标题栏深色样式 参考：

c# - WinForms Dark title bar on Windows 10 - Stack Overflow
https://stackoverflow.com/a/62811758

*/

void FlushWindow(HWND hWnd, int type)
{
	int enabled = 1;
	DwmSetWindowAttribute(hWnd, type == 0 ? 19 : 20, &enabled, sizeof(enabled));
}

/*

SetPreferredAppMode 参考：

win32-darkmode/win32-darkmode/DarkMode.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/DarkMode.h

*/

using fnSetPreferredAppMode = int(WINAPI*)(int preferredAppMode);
fnSetPreferredAppMode SetPreferredAppMode = nullptr;

void FlushApp(int preferredAppMode)
{
	if (!SetPreferredAppMode)
	{
		HMODULE hUxtheme = LoadLibraryExW(L"uxtheme.dll", nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32);

		if (hUxtheme)
		{
			auto addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(135));

			if (addr)
			{
				SetPreferredAppMode = reinterpret_cast<fnSetPreferredAppMode>(addr);
			}
		}
	}

	if (SetPreferredAppMode)
	{
		SetPreferredAppMode(preferredAppMode);
	}
}

static LPCWSTR GetPszSubAppName(int id)
{
	switch (id)
	{
		case 0:
			return L"DarkMode_Explorer";
		case 1:
			return L"DarkMode_CFD";
		case 2:
			return L"DarkMode_ItemsView";
		default:
			return L"Explorer";
	}
}

void SetTheme(HWND hWnd, int type)
{
	SetWindowTheme(hWnd, GetPszSubAppName(type), nullptr);
}

