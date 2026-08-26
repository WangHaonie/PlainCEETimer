#include "pch.h"
#include "Theme.h"
#include "Utils.h"
#include "Win32/IATHook.h"
#include <Uxtheme.h>
#include <vsstyle.h>
#include <vssym32.h>
#include <detours.h>

/*

Win32 深色模式 API 相关 参考：

win32-darkmode/win32-darkmode/DarkMode.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/DarkMode.h

*/

using fnSetPreferredAppMode = PreferredAppMode (WINAPI*)(PreferredAppMode preferredAppMode);
using fnOpenNcThemeData = decltype(&OpenThemeData);
using fnFlushMenuThemes = void (WINAPI*)();
DeclDelegateType(OpenThemeDataForDpi);
DeclDelegateType(GetSysColor);
DeclDelegateType(GetSysColorBrush);
using fnSetWindowCompositionAttribute = BOOL (WINAPI*)(HWND hwnd, const WINDOWCOMPOSITIONATTRIBDATA* pwcad);
DeclDelegateType(DrawThemeBackground);
using fnGetThemeClass = HRESULT (WINAPI*)(HTHEME hTheme, LPWSTR lpBuffer, int cchBuffer);
DeclDelegateType(DrawThemeText);

static int g_iHookThemeBackgroundRef = 0;

static WCHAR themeClassCache[VSCLASSNAME_BUFFER];
static HTHEME lastOpenedTheme = nullptr;

DeclDelegateField(SetPreferredAppMode);
DeclDelegateField(OpenNcThemeData);
DeclDelegateField(OpenThemeDataForDpi);
DeclDelegateField(FlushMenuThemes);
DeclDelegateField(GetSysColor);
DeclDelegateField(GetSysColorBrush);
DeclDelegateField(SetWindowCompositionAttribute);
DeclDelegateField(DrawThemeBackground);
DeclDelegateField(GetThemeClass);
DeclDelegateField(DrawThemeText);

DeclIatData(OpenNcThemeData, Comctl);
DeclIatData(OpenThemeDataForDpi, Comctl);
DeclIatData(GetSysColor, Comctl);
DeclIatData(GetSysColorBrush, Comdlg);
DeclIatData(GetSysColor, Comdlg);
DeclIatData(DrawThemeBackground, Comctl);
DeclIatData(DrawThemeText, Comctl);

static int WINAPI SetPreferredAppMode(PreferredAppMode preferredAppMode)
{
    if (INITFUNC(g_SetPreferredAppMode, UXTHEME_DLL, ORD2STR(135)))
    {
        return g_SetPreferredAppMode(preferredAppMode);
    }

    return 0;
}

static void WINAPI FlushMenuThemes()
{
    if (INITFUNC(g_FlushMenuThemes, UXTHEME_DLL, ORD2STR(136)))
    {
        return g_FlushMenuThemes();
    }
}

static BOOL WINAPI SetWindowCompositionAttribute(HWND hwnd, const WINDOWCOMPOSITIONATTRIBDATA* pwcad)
{
    if (INITFUNC(g_SetWindowCompositionAttribute, USER32_DLL, nameof(SetWindowCompositionAttribute)))
    {
        return g_SetWindowCompositionAttribute(hwnd, pwcad);
    }

    return FALSE;
}

static HRESULT WINAPI GetThemeClass(HTHEME hTheme, LPWSTR lpBuffer, int cchBuffer)
{
    if (INITFUNC(g_GetThemeClass, UXTHEME_DLL, ORD2STR(74)))
    {
        return g_GetThemeClass(hTheme, lpBuffer, cchBuffer);
    }

    return HRESULT_FROM_WIN32(ERROR_PROC_NOT_FOUND);
}

static bool CacheThemeClass(HTHEME hTheme)
{
    if (lastOpenedTheme != hTheme && SUCCEEDED(GetThemeClass(hTheme, themeClassCache, VSCLASSNAME_BUFFER)))
        lastOpenedTheme = hTheme;

    if (!lastOpenedTheme) return false;
    return true;
}

/*

将非 Explorer 主题的 ScrollBar 应用深色主题 参考：

win32-darkmode/win32-darkmode/DarkMode.h at cc26549b65b25d6f3168a80238792545bd401271 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/cc26549b65b25d6f3168a80238792545bd401271/win32-darkmode/DarkMode.h#L152


非常感谢 ysc3839 的耐心协助：

【C# WinForms】IATHook causes System.AccessViolationException · Issue #32 · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/issues/32

*/

static void HandleScrollBarElements(HWND& hWnd, LPCWSTR& pszClassList)
{
    if (WString_Equals(pszClassList, WC_SCROLLBAR, true))
    {
        hWnd = nullptr;
        pszClassList = L"DarkMode_Explorer::ScrollBar";
    }
}

static void HandleListViewCheckBoxes(HWND& hWnd, LPCWSTR& pszClassList)
{
    if (WString_Equals(pszClassList, WC_BUTTON, true) && !hWnd)
    {
        pszClassList = L"DarkMode_Explorer::Button";
    }
}

static bool PnCommonPaint(HDC hdc, LPRECT lpRect, COLORREF crBack, COLORREF crBorder, bool bBorder)
{
    HBRUSH hbrBack = CreateSolidBrush(crBack);
    FillRect(hdc, lpRect, hbrBack);
    DeleteObject(hbrBack);
    SetDCBrushColor(hdc, bBorder ? crBorder : crBack);
    FrameRect(hdc, lpRect, CastToP(HBRUSH, GetStockObject(DC_BRUSH)));
    return true;
}

static bool PnDrawMcArrow(HDC hdc, LPRECT lpRect, bool bLeft, COLORREF crFill)
{
    if (!lpRect) return false;

    LONG cx = lpRect->right - lpRect->left;
    LONG cy = lpRect->bottom - lpRect->top;

    LONG maxcx = cx - (cx / 4) * 2;
    LONG maxcy = cy - (cy / 4) * 2;

    if (maxcx <= 0 || maxcy <= 0) return false;

    LONG w = maxcx;
    LONG h = w * 2;

    if (h > maxcy)
    {
        h = maxcy;
        w = h / 2;
    }

    if (w < 1) w = 1;
    if (h < 2) h = 2;

    LONG l = lpRect->left + (cx - w) / 2;
    LONG t = lpRect->top + (cy - h) / 2;
    LONG r = l + w;
    LONG b = t + h;
    LONG y = t + h / 2;

    HBRUSH hbr = CreateSolidBrush(crFill);
    HBRUSH hbrOld = CastToP(HBRUSH, SelectObject(hdc, hbr));
    HPEN hpnOld = CastToP(HPEN, SelectObject(hdc, GetStockObject(NULL_PEN)));

    POINT pts[3];

    if (bLeft)
    {
        pts[0] = { l, y };
        pts[1] = { r, t };
        pts[2] = { r, b };
    }
    else
    {
        pts[0] = { r, y };
        pts[1] = { l, t };
        pts[2] = { l, b };
    }

    Polygon(hdc, pts, 3);
    SelectObject(hdc, hbrOld);
    SelectObject(hdc, hpnOld);
    DeleteObject(hbr);
    return true;
}

/*

Progress 控件深色主题 灵感来自：

systeminformer/SystemInformer/delayhook.c at master · winsiderss/systeminformer
https://github.com/winsiderss/systeminformer/blob/103cc43d77a6cd388d04c03371d019866d0521d6/SystemInformer/delayhook.c#L1335-L1345

*/

static bool HandleProgressBackground(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPRECT pRect)
{
    if (pRect)
    {
        RECT rc = *pRect;
        if (RECT_IS_ZEROCX(rc)) return false;
    }
    
    switch (iPartId)
    {
        case PP_TRANSPARENTBAR:
        case PP_TRANSPARENTBARVERT:
            return PnCommonPaint(hdc, pRect, DCOLOR_PROGRESS_BACK, COLOR_EMPTY, false);

        case PP_FILL:
        case PP_FILLVERT:
        {
            switch (iStateId)
            {
                CASE(PBFS_NORMAL, PnCommonPaint(hdc, pRect, DCOLOR_PROGRESS_BACK_NORMAL, COLOR_EMPTY, false));
                CASE(PBFS_ERROR, PnCommonPaint(hdc, pRect, DCOLOR_PROGRESS_BACK_ERROR, COLOR_EMPTY, false));
                CASE(PBFS_PAUSED, PnCommonPaint(hdc, pRect, DCOLOR_PROGRESS_BACK_PAUSED, COLOR_EMPTY, false));
                CASE(PBFS_PARTIAL, PnCommonPaint(hdc, pRect, DCOLOR_PROGRESS_BACK_PARTIAL, COLOR_EMPTY, false));
            }
        }
    }

    return false;
}

/*

DateTimePicker 控件深色主题 灵感来自：

systeminformer/SystemInformer/delayhook.c at master · winsiderss/systeminformer
https://github.com/winsiderss/systeminformer/blob/103cc43d77a6cd388d04c03371d019866d0521d6/SystemInformer/delayhook.c#L1596-L1620

*/

static bool HandleDtpBackground(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPRECT pRect)
{
    if (iPartId == DP_DATEBORDER)
    {
        COLORREF crBorder = COLOR_EMPTY;

        switch (iStateId)
        {
            CASE_AB(DPDB_NORMAL, crBorder, DCOLOR_DATEPICKER_BORDER);
            CASE_AB(DPDB_HOT, crBorder, DCOLOR_DATEPICKER_BORDER_HOT);
            CASE_AB(DPDB_FOCUSED, crBorder, DCOLOR_DATEPICKER_BORDER_FOCUSED);
            CASE_AB(DPDB_DISABLED, crBorder, DCOLOR_DATEPICKER_BORDER_DISABLED);
        }

        return PnCommonPaint(hdc, pRect, DCOLOR_DATEPICKER_BACK, crBorder, true);
    }

    return false;
}

static bool HandleDtpText(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPCWSTR pszText, int cchText, DWORD dwTextFlags, LPRECT pRect)
{
    if (iPartId == DP_DATETEXT)
    {
        static DTTOPTS options = { sizeof(DTTOPTS), DTT_TEXTCOLOR };

        switch (iStateId)
        {
            case DPDT_NORMAL:
            case DPDT_SELECTED:
                options.crText = DCOLOR_DATEPICKER_FORE;
                break;
            case DPDT_DISABLED:
                options.crText = DCOLOR_DATEPICKER_FORE_DISABLED;
                break;
        }

        return SUCCEEDED(DrawThemeTextEx(hTheme, hdc, iPartId, iStateId, pszText, cchText, dwTextFlags, pRect, &options));
    }

    return false;
}

static bool HandleMcArrows(HDC hdc, LPRECT pRect, bool bLeft, int iStateId)
{
    switch (iStateId)
    {
        CASE(MCNP_NORMAL, PnDrawMcArrow(hdc, pRect, bLeft, DCOLOR_MONTHCAL_ARROW));
        CASE(MCNP_HOT, PnDrawMcArrow(hdc, pRect, bLeft, DCOLOR_MONTHCAL_ARROW_HOT));
        CASE(MCNP_PRESSED, PnDrawMcArrow(hdc, pRect, bLeft, DCOLOR_MONTHCAL_ARROW_PRESSED));
        default: return false;
    }

    return true;
}

static bool HandleMcBackground(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPRECT pRect)
{
    switch (iPartId)
    {
        case MC_BACKGROUND:
        case MC_GRIDBACKGROUND:
        case MC_GRIDCELLBACKGROUND:
        case MC_COLHEADERSPLITTER:
            return PnCommonPaint(hdc, pRect, DCOLOR_MONTHCAL_BACK, DCOLOR_MONTHCAL_BORDER, true);
        case MC_NAVNEXT:
        case MC_NAVPREV:
            return HandleMcArrows(hdc, pRect, iPartId == MC_NAVPREV, iStateId);
    }

    return false;
}

static bool HandleMcText(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPCWSTR pszText, int cchText, DWORD dwTextFlags, LPRECT pRect)
{
    static DTTOPTS options = { sizeof(DTTOPTS), DTT_TEXTCOLOR };

    switch (iPartId)
    {
        case MC_GRIDCELL:
        {
            switch (iStateId)
            {
                CASE_AB(0, options.crText, DCOLOR_MONTHCAL_FORE);
                CASE_AB(MCGC_HOT, options.crText, DCOLOR_MONTHCAL_FORE_HOT);
                case MCGC_SELECTED:
                case MCGC_SELECTEDHOT:
                    options.crText = DCOLOR_MONTHCAL_FORE_SELECTED;
                    break;
                default:
                    return false;
            }

            goto paint;
        }

        case MC_GRIDCELLUPPER:
        {
            switch (iStateId)
            {
                CASE_AB(0, options.crText, DCOLOR_MONTHCAL_FORE);
                CASE_AB(MCGCU_HOT, options.crText, DCOLOR_MONTHCAL_FORE_HOT);
                case MCGCU_SELECTED:
                case MCGCU_SELECTEDHOT:
                    options.crText = DCOLOR_MONTHCAL_FORE_SELECTED;
                    break;
                default:
                    return false;
            }

            goto paint;
        }

        case MC_TRAILINGGRIDCELL:
        {
            switch (iStateId)
            {
                CASE_AB(MCTGC_HOT, options.crText, DCOLOR_MONTHCAL_FORE_HOT);
                CASE_AB(MCTGC_SELECTED, options.crText, DCOLOR_MONTHCAL_FORE_SELECTED);
                default: return false;
            }

            goto paint;
        }

        case MC_TRAILINGGRIDCELLUPPER:
        {
            switch (iStateId)
            {
                CASE_AB(MCTGCU_HOT, options.crText, DCOLOR_MONTHCAL_FORE_HOT);
                CASE_AB(MCTGCU_SELECTED, options.crText, DCOLOR_MONTHCAL_FORE_SELECTED);
                default: return false;
            }

            goto paint;
        }
    }

    return false;
paint:
    return SUCCEEDED(DrawThemeTextEx(hTheme, hdc, iPartId, iStateId, pszText, cchText, dwTextFlags, pRect, &options));
}

static bool HandleControlVsBg(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPRECT pRect)
{
    if (CacheThemeClass(hTheme))
    {
        if (WString_Equals(themeClassCache, VSCLASS_PROGRESS, true))
        {
            return HandleProgressBackground(hTheme, hdc, iPartId, iStateId, pRect);
        }

        if (WString_Equals(themeClassCache, VSCLASS_DATEPICKER, true))
        {
            return HandleDtpBackground(hTheme, hdc, iPartId, iStateId, pRect);
        }

        if (WString_Equals(themeClassCache, VSCLASS_MONTHCAL, true))
        {
            return HandleMcBackground(hTheme, hdc, iPartId, iStateId, pRect);
        }
    }

    return false;
}

static bool HandleControlVsText(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPCWSTR pszText, int cchText, DWORD dwTextFlags, LPRECT pRect)
{
    if (CacheThemeClass(hTheme))
    {
        if (WString_Equals(themeClassCache, VSCLASS_DATEPICKER, true))
        {
            return HandleDtpText(hTheme, hdc, iPartId, iStateId, pszText, cchText, dwTextFlags, pRect);
        }

        if (WString_Equals(themeClassCache, VSCLASS_MONTHCAL, true))
        {
            return HandleMcText(hTheme, hdc, iPartId, iStateId, pszText, cchText, dwTextFlags, pRect);
        }
    }

    return false;
}

static void HandleColorDlgLumArrow(int& nIndex)
{
    if (nIndex == COLOR_BTNTEXT) nIndex = COLOR_WINDOW;
}

static HTHEME WINAPI OpenNcThemeData_(HWND hWnd, LPCWSTR pszClassList)
{
    HandleScrollBarElements(hWnd, pszClassList);
    return g_OpenNcThemeData(hWnd, pszClassList);
};

static HTHEME WINAPI OpenThemeDataForDpi_(HWND hWnd, LPCWSTR pszClassList, UINT dpi)
{
    HandleListViewCheckBoxes(hWnd, pszClassList);
    return g_OpenThemeDataForDpi(hWnd, pszClassList, dpi);
};

static HRESULT WINAPI DrawThemeBackground_(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPCRECT pRect, LPCRECT pClipRect)
{
    if (HandleControlVsBg(hTheme, hdc, iPartId, iStateId, (LPRECT)pRect))
    {
        return S_OK;
    }

    return g_DrawThemeBackground(hTheme, hdc, iPartId, iStateId, pRect, pClipRect);
}

static HRESULT WINAPI DrawThemeText_(HTHEME hTheme, HDC hdc, int iPartId, int iStateId, LPCWSTR pszText, int cchText, DWORD dwTextFlags, DWORD dwTextFlags2, LPCRECT pRect)
{
    if (HandleControlVsText(hTheme, hdc, iPartId, iStateId, pszText, cchText, dwTextFlags, (LPRECT)pRect))
    {
        return S_OK;
    }

    return g_DrawThemeText(hTheme, hdc, iPartId, iStateId, pszText, cchText, dwTextFlags, dwTextFlags2, pRect);
}

static DWORD WINAPI GetSysColor_(int nIndex)
{
    switch (nIndex)
    {
        CASE(COLOR_WINDOW, DCOLOR_TEXT_BACK);
        CASE(COLOR_BTNFACE, DCOLOR_TEXT_BACK);
        CASE(COLOR_WINDOWTEXT, DCOLOR_TEXT_FORE);
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

    ACCENT_POLICY ap =
    {
        ACCENT_ENABLE_ACRYLICBLURBEHIND,
        ACCENT_WINDOWS11_LUMINOSITY | ACCENT_BORDER_ALL,
        abgrGradient
    };

    WINDOWCOMPOSITIONATTRIBDATA wcad =
    {
        WCA_ACCENT_POLICY, &ap, sizeof(ACCENT_POLICY)
    };

    return SetWindowCompositionAttribute(hWnd, &wcad);
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
    if (enabled)
    {
        if (INITFUNC(g_OpenNcThemeData, UXTHEME_DLL, ORD2STR(49))
            && InitializeIatHook(HOOK_COMCTL32_OPENNCTHEMEDATA_ARGS, IatHookComctlOpenNcThemeData)
            && ReplaceFunction(IatHookComctlOpenNcThemeData, OpenNcThemeData_))
        {
            IatHookComctlOpenNcThemeData.OldFunc = g_OpenNcThemeData;
        }
    }
    else
    {
        UnhookIat(IatHookComctlOpenNcThemeData);
    }
    
    SetPreferredAppMode(enabled ? ForceDark : Default);
    FlushMenuThemes();
}

void NATIVESAPI PnHookSysColor()
{
    HookIat(HOOK_COMCTL32_GETSYSCOLOR_ARGS,
        IatHookComctlGetSysColor,
        g_GetSysColor,
        GetSysColor_
    );

    HookIat(HOOK_COMDLG32_GETSYSCOLOR_ARGS,
        IatHookComdlgGetSysColor,
        g_GetSysColor,
        GetSysColor_
    );
}

void NATIVESAPI PnUnhookSysColor()
{
    UnhookIat(IatHookComctlGetSysColor);
    UnhookIat(IatHookComdlgGetSysColor);
}

void NATIVESAPI PnHookOpenTheme()
{
    HookIat(HOOK_COMCTL32_OPENTHEMEDATAFORDPI_ARGS,
        IatHookComctlOpenThemeDataForDpi,
        g_OpenThemeDataForDpi,
        OpenThemeDataForDpi_
    );
}

void NATIVESAPI PnUnhookOpenTheme()
{
    UnhookIat(IatHookComctlOpenThemeDataForDpi);
}

void NATIVESAPI PnHookThemeBackground()
{
    HookIat(HOOK_COMCTL32_DRAWTHEMEBACKGROUND_ARGS,
        IatHookComctlDrawThemeBackground,
        g_DrawThemeBackground,
        DrawThemeBackground_
    );

    HookIat(HOOK_COMCTL32_DRAWTHEMETEXT_ARGS,
        IatHookComctlDrawThemeText,
        g_DrawThemeText,
        DrawThemeText_
    );

    ++g_iHookThemeBackgroundRef;
}

void NATIVESAPI PnUnhookThemeBackground()
{
    if (--g_iHookThemeBackgroundRef == 0)
    {
        UnhookIat(IatHookComctlDrawThemeBackground);
        UnhookIat(IatHookComctlDrawThemeText);
    }
}

void NATIVESAPI PnHookSysColorBrush()
{
    HookIat(HOOK_COMDLG32_GETSYSCOLORBRUSH_ARGS,
        IatHookComdlgGetSysColorBrush,
        g_GetSysColorBrush,
        GetSysColorBrush_
    );
}

void NATIVESAPI PnUnhookSysColorBrush()
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
