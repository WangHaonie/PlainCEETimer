#include "pch.h"
#include "RoundCorner.h"

/*

WinForms 无边框窗体自动圆角 参考 :

Rounded Corners in C# windows forms - Stack Overflow
https://stackoverflow.com/a/18822204/21094697

Apply rounded corners in desktop apps - Windows apps | Microsoft Learn
https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/ui/apply-rounded-corners#example-3---rounding-an-apps-main-window-in-c

*/

void SetRoundCorner(HWND hWnd, int width, int height, int radius)
{
    SetWindowRgn(hWnd, CreateRoundRectRgn(0, 0, width, height, radius, radius), TRUE);
}

void SetRoundCornerEx(HWND hWnd, BOOL isSmall)
{
    DWM_WINDOW_CORNER_PREFERENCE type = isSmall ? DWMWCP_ROUNDSMALL : DWMWCP_ROUND;
    DwmSetWindowAttribute(hWnd, DWMWA_WINDOW_CORNER_PREFERENCE, &type, sizeof(type));
}