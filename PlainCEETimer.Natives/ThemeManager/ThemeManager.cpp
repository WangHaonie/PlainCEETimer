#include "pch.h"
#include "ThemeManager.h"

/*

�����������ɫ��ʽ �ο���

c# - WinForms Dark title bar on Windows 10 - Stack Overflow
https://stackoverflow.com/a/62811758

*/
void FlushDarkWindow(HWND hWnd, int type)
{
	int enabled = 1;
	DwmSetWindowAttribute(hWnd, type == 0 ? 19 : 20, &enabled, sizeof(enabled));
}

