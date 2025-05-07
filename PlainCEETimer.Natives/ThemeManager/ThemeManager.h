#pragma once

#include <dwmapi.h>

cexport void stdcall FlushWindow(HWND hWnd, int type);
cexport void stdcall FlushApp(int preferredAppMode);
cexport void stdcall SetTheme(HWND hWnd, int type);
