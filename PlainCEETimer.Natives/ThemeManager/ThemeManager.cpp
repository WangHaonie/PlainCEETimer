#include "pch.h"
#include "ThemeManager.h"

/*

�����������ɫ��ʽ �ο���

c# - WinForms Dark title bar on Windows 10 - Stack Overflow
https://stackoverflow.com/a/62811758

*/
void FlushDarkWindow(HWND hWnd)
{
	int enabled = 1;
	DwmSetWindowAttribute(hWnd, 20, &enabled, sizeof(enabled));
}

