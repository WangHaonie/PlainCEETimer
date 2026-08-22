#include "pch.h"
#include "Theme.h"
#include "utils.h"
#include "Win32/IATHook.h"
#include <Uxtheme.h>
#include <vsstyle.h>

/*

Win32 深色模式 API 相关 参考：

win32-darkmode/win32-darkmode/DarkMode.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/DarkMode.h

*/

using fnSetPreferredAppMode = int (WINAPI*)(int preferredAppMode);
using fnOpenNcThemeData = decltype(&OpenThemeData);
using fnFlushMenuThemes = void (WINAPI*)();
DeclDelegateType(OpenThemeDataForDpi);
DeclDelegateType(GetSysColor);
DeclDelegateType(GetSysColorBrush);
using fnSetWindowCompositionAttribute = BOOL (WINAPI*)(HWND hWnd, LPWINDOWCOMPOSITIONATTRIBUTEDATA pwcad);
DeclDelegateType(DrawThemeBackground);
using fnGetThemeClass = HRESULT (WINAPI*)(HTHEME hTheme, LPWSTR lpBuffer, int cchBuffer);

static COLORREF g_crFore = 0;
static COLORREF g_crBack = 0;
static BOOL g_fUseDark = FALSE;

DeclDelegateField(SetPreferredAppMode);
DeclDelegateField(OpenNcThemeData);
DeclDelegateField(OpenThemeDataForDpi);
DeclDelegateField(FlushMenuThemes);
DeclDelegateField(GetSysColor);
DeclDelegateField(GetSysColorBrush);
DeclDelegateField(SetWindowCompositionAttribute);
DeclDelegateField(DrawThemeBackground);
DeclDelegateField(GetThemeClass);

DeclIatData(OpenNcThemeData, Comctl);
DeclIatData(OpenThemeDataForDpi, Comctl);
DeclIatData(GetSysColor, Comctl);
DeclIatData(GetSysColorBrush, Comdlg);
DeclIatData(DrawThemeBackground, Comctl);

/*

将非 Explorer 主题的 ScrollBar 应用深色主题 参考：

win32-darkmode/win32-darkmode/DarkMode.h at cc26549b65b25d6f3168a80238792545bd401271 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/cc26549b65b25d6f3168a80238792545bd401271/win32-darkmode/DarkMode.h#L152


非常感谢 ysc3839 的耐心协助：

【C# WinForms】IATHook causes System.AccessViolationException · Issue #32 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/issues/32

*/

static HTHEME WINAPI OpenNcThemeData_(HWND hWnd, LPCWSTR pszClassList)
{
    if (g_fUseDark && WString_Equals(pszClassList, WC_SCROLLBAR, true))
    {
        hWnd = nullptr;
        pszClassList = L"DarkMode_Explorer::ScrollBar";
    }

    return g_OpenNcThemeData(hWnd, pszClassList);
};

static HRESULT WINAPI GetThemeClass_(HTHEME hTheme, LPWSTR lpBuffer, int cchBuffer)
{
    if (!g_GetThemeClass)
    {
        HMODULE hmod = GetModuleHandleA("uxtheme.dll");

        if (hmod)
        {
            FARPROC addr = GetProcAddress(hmod, MAKEINTRESOURCEA(74));

            if (addr)
            {
                g_GetThemeClass = CastToP(fnGetThemeClass, addr);
            }
        }
    }

    if (g_GetThemeClass)
    {
        return g_GetThemeClass(hTheme, lpBuffer, cchBuffer);
    }
}

static void HandleListViewCheckBoxes(HWND& hWnd, LPCWSTR& pszClassList)
{
    if (WString_Equals(pszClassList, WC_BUTTON, true) && !hWnd)
    {
        pszClassList = L"DarkMode_Explorer::Button";
    }
}

static bool HandleProgressBackground(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPCRECT pRect)
{
    if (pRect)
    {
        RECT rc = *pRect;
        
        if (RECT_IS_ZEROCX(rc))
        {
            return false;
        }
    }

    thread_local WCHAR buffer[VSCLASSNAME_BUFFER];
    thread_local HTHEME last = nullptr;

    auto _DrawBackground = [&](COLORREF cr) -> bool
    {
        HBRUSH hbrBack = CreateSolidBrush(cr);
        FillRect(hdc, pRect, hbrBack);
        DeleteObject(hbrBack);
        SetDCBrushColor(hdc, cr);
        FrameRect(hdc, pRect, CastToP(HBRUSH, GetStockObject(DC_BRUSH)));
        return true;
    };

    if (last != hTheme)
    {
        GetThemeClass_(hTheme, buffer, VSCLASSNAME_BUFFER);
        last = hTheme;
    }

    if (WString_Equals(buffer, VSCLASS_PROGRESS, true))
    {
        switch (iPartId)
        {
            case PP_TRANSPARENTBAR:
            case PP_TRANSPARENTBARVERT:
                return _DrawBackground(RGB(19, 19, 19));

            case PP_FILL:
            case PP_FILLVERT:
            {
                switch (iStateId)
                {
                    CASE(PBFS_NORMAL, _DrawBackground(RGB(108, 203, 95)));
                    CASE(PBFS_ERROR, _DrawBackground(RGB(255, 153, 164)));
                    CASE(PBFS_PAUSED, _DrawBackground(RGB(252, 225, 0)));
                    CASE(PBFS_PARTIAL, _DrawBackground(RGB(0, 120, 212)));
                }
            }
        }
    }

    return false;
}

static void HandleColorDlgLumArrow(int& nIndex)
{
    if (nIndex == COLOR_BTNTEXT)
    {
        nIndex = COLOR_WINDOW;
    }
}

static HTHEME WINAPI OpenThemeDataForDpi_(HWND hWnd, LPCWSTR pszClassList, UINT dpi)
{
    HandleListViewCheckBoxes(hWnd, pszClassList);
    return g_OpenThemeDataForDpi(hWnd, pszClassList, dpi);
};

static HRESULT WINAPI DrawThemeBackground_(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPCRECT pRect, LPCRECT pClipRect)
{
    if (HandleProgressBackground(hTheme, hdc, iPartId, iStateId, pRect))
    {
        return S_OK;
    }

    return g_DrawThemeBackground(hTheme, hdc, iPartId, iStateId, pRect, pClipRect);
}

static DWORD WINAPI GetSysColor_(int nIndex)
{
    if (g_fUseDark)
    {
        switch (nIndex)
        {
            CASE(COLOR_WINDOW, g_crBack);
            CASE(COLOR_WINDOWTEXT, g_crFore);
        }
    }

    return g_GetSysColor(nIndex);
}

static HBRUSH WINAPI GetSysColorBrush_(int nIndex)
{
    HandleColorDlgLumArrow(nIndex);
    return g_GetSysColorBrush(nIndex);
}

static BOOL EnableBlurBehind(HWND hWnd, BOOL bAcrylic, DWORD abgrGradient, bool bEnabled)
{
    if (!bAcrylic)
    {
        return FALSE; // to do
    }

    if (!g_SetWindowCompositionAttribute)
    {
        HMODULE hUser32 = GetModuleHandleA("user32.dll");

        if (hUser32)
        {
            FARPROC addr = GetProcAddress(hUser32, "SetWindowCompositionAttribute");

            if (addr)
            {
                g_SetWindowCompositionAttribute = CastToP(fnSetWindowCompositionAttribute, addr);
            }
        }
    }

    if (g_SetWindowCompositionAttribute)
    {
        ACCENT_POLICY ap =
        {
            ACCENT_ENABLE_ACRYLICBLURBEHIND,
            ACCENT_WINDOWS11_LUMINOSITY | ACCENT_BORDER_ALL,
            abgrGradient
        };

        WINDOWCOMPOSITIONATTRIBUTEDATA wcad =
        {
            WCA_ACCENT_POLICY,
            &ap,
            sizeof(ACCENT_POLICY)
        };

        return g_SetWindowCompositionAttribute(hWnd, &wcad);
    }

    return FALSE;
}

static BOOL ApplySystemBackdropCore(HWND hWnd, DWORD dwFlags, PVOID pvData)
{
    DWORD dwType = ASB_GET_BACKDROP(dwFlags);
    BOOL bEnabled = ASB_GET_STATUS(dwFlags);

    switch (dwType)
    {
        case ASBT_MICA:
        case ASBT_MICAALT:
        dwmapi:
        {
            DWM_SYSTEMBACKDROP_TYPE value = ASB_GET_STATUS(dwFlags) ? CastToS(DWM_SYSTEMBACKDROP_TYPE, dwType) : DWMSBT_NONE;

            if (FAILED(DwmSetWindowAttribute(hWnd, DWMWA_SYSTEMBACKDROP_TYPE, &value, sizeof(value))))
            {
                return SUCCEEDED(DwmSetWindowAttribute(hWnd, DWMWA_MICA_EFFECT, &bEnabled, sizeof(bEnabled)));
            }

            return TRUE;
        }

        case ASBT_ACRYLIC:
        case ASBT_AERO:
        {
            if (dwType == ASBT_ACRYLIC && ASB_DEFINED_FOPTIONS(dwFlags, ASBF_USE_DWMAPI))
            {
                goto dwmapi;
            }

            if (pvData)
            {
                DWORD abgrGradient = *CastToP(LPDWORD, pvData);
                return EnableBlurBehind(hWnd, dwType == ASBT_ACRYLIC, abgrGradient, bEnabled);
            }
        }
    }

    return FALSE;
}

void NATIVESAPI EnableDarkModeForApp(BOOL enabled)
{
    g_fUseDark = enabled;

    if (!g_SetPreferredAppMode)
    {
        HMODULE hUxtheme = LoadLibraryEx(L"uxtheme.dll", nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32);

        if (hUxtheme)
        {
            auto addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(135));

            if (addr)
            {
                g_SetPreferredAppMode = CastToP(fnSetPreferredAppMode, addr);
            }

            if (addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(49)))
            {
                g_OpenNcThemeData = CastToP(fnOpenNcThemeData, addr);
                
                if (InitializeIatHook(HOOK_COMCTL32_OPENNCTHEMEDATA_ARGS, IatHookComctlOpenNcThemeData))
                {
                    ReplaceFunction(IatHookComctlOpenNcThemeData, OpenNcThemeData_);
                    IatHookComctlOpenNcThemeData.OldFunc = g_OpenNcThemeData;
                }
            }

            if (addr = GetProcAddress(hUxtheme, MAKEINTRESOURCEA(136)))
            {
                g_FlushMenuThemes = CastToP(fnFlushMenuThemes, addr);
            }
        }
    }

    if (g_SetPreferredAppMode) g_SetPreferredAppMode(enabled ? 2 : 0);
    if (g_FlushMenuThemes) g_FlushMenuThemes();
}

void NATIVESAPI ComctlHookSysColor(COLORREF crFore, COLORREF crBack)
{
    if (!InitializeIatHook(HOOK_COMCTL32_GETSYSCOLOR_ARGS, IatHookComctlGetSysColor))
    {
        return;
    }

    if (!g_GetSysColor)
    {
        g_GetSysColor = IatHookComctlGetSysColor.OldFunc;
    }

    if (ReplaceFunction(IatHookComctlGetSysColor, GetSysColor_))
    {
        g_crFore = crFore;
        g_crBack = crBack;
    }
}

void NATIVESAPI ComctlUnhookSysColor()
{
    UnhookIat(IatHookComctlGetSysColor);
    g_crFore = 0;
    g_crBack = 0;
}

void NATIVESAPI ComctlHookOpenTheme()
{
    HookIat(HOOK_COMCTL32_OPENTHEMEDATAFORDPI_ARGS,
        IatHookComctlOpenThemeDataForDpi,
        g_OpenThemeDataForDpi,
        OpenThemeDataForDpi_
    );
}

void NATIVESAPI ComctlUnhookOpenTheme()
{
    UnhookIat(IatHookComctlOpenThemeDataForDpi);
}

void NATIVESAPI ComctlHookThemeBackground()
{
    HookIat(HOOK_COMCTL32_DRAWTHEMEBACKGROUND_ARGS,
        IatHookComctlDrawThemeBackground,
        g_DrawThemeBackground,
        DrawThemeBackground_
    );
}

void NATIVESAPI ComctlUnhookThemeBackground()
{
    UnhookIat(IatHookComctlDrawThemeBackground);
}

void NATIVESAPI ComdlgHookGetSysColorBrush()
{
    HookIat(HOOK_COMDLG32_GETSYSCOLORBRUSH_ARGS,
        IatHookComdlgGetSysColorBrush,
        g_GetSysColorBrush,
        GetSysColorBrush_
    );
}

void NATIVESAPI ComdlgUnhookGetSysColorBrush()
{
    UnhookIat(IatHookComdlgGetSysColorBrush);
}

/*

窗体标题栏深色样式 参考：

c# - WinForms Dark title bar on Windows 10 - Stack Overflow
https://stackoverflow.com/a/62811758

*/

void NATIVESAPI EnableDarkModeForWindowFrame(HWND hWnd, BOOL after20h1, BOOL enabled)
{
    if (hWnd)
    {
        DwmSetWindowAttribute(hWnd, after20h1
            ? DWMWA_USE_IMMERSIVE_DARK_MODE
            : DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1,
            &enabled, sizeof(enabled));
    }
}

void NATIVESAPI SetWindowBorderColor(HWND hWnd, COLORREF color, BOOL enabled)
{
    if (hWnd)
    {
        COLORREF c = enabled ? color : DWMWA_COLOR_DEFAULT;
        DwmSetWindowAttribute(hWnd, DWMWA_BORDER_COLOR, &c, sizeof(c));
    }
}

DWORD NATIVESAPI GetSystemAccentColor()
{
    DWORD result = 0;
    BOOL flag = FALSE;
    DwmGetColorizationColor(&result, &flag);
    return result;
}

BOOL NATIVESAPI ApplySystemBackdrop(HWND hWnd, DWORD dwFlags, PVOID pvData)
{
    if (hWnd && ApplySystemBackdropCore(hWnd, dwFlags, pvData))
    {
        MARGINS margins;
        int* s = CastToP(int*, &margins);
        int* e = s + 4;
        int value = ASB_GET_STATUS(dwFlags) ? -1 : 0;
        while (s < e) *s++ = value;
        DwmExtendFrameIntoClientArea(hWnd, &margins);
        return TRUE;
    }

    return FALSE;
}
