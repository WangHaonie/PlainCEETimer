#include "pch.h"
#include "Win32UI/Win32UI.h"

BOOL RunColorDialog(HWND hWndOwner, LPCCHOOKPROC lpfnHookProc, LPCOLORREF lpColor, LPCOLORREF lpCustomColors)
{
    CHOOSECOLOR cc = { sizeof(cc) };
    DWORD flags = CC_ANYCOLOR | CC_FULLOPEN | CC_ENABLETEMPLATE;

    if (lpfnHookProc)
    {
        flags |= CC_ENABLEHOOK;
        cc.lpfnHook = lpfnHookProc;
    }

    if (lpColor)
    {
        flags |= CC_RGBINIT;
        cc.rgbResult = *lpColor;
    }

    if (lpCustomColors)
    {
        cc.lpCustColors = lpCustomColors;
    }

    cc.Flags = flags;
    cc.hwndOwner = hWndOwner;
    cc.hInstance = reinterpret_cast<HWND>(GetModuleHandleW(LIBRARYNAME));
    cc.lpTemplateName = MAKEINTRESOURCE(IDD_CHOOSECOLOR);

    if (ChooseColor(&cc))
    {
        if (lpColor) *lpColor = cc.rgbResult;
        if (lpCustomColors) lpCustomColors = cc.lpCustColors;
        return TRUE;
    }

    return FALSE;
}

BOOL RunFontDialog(HWND hWndOwner, LPCFHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, INT nSizeLimit)
{
    CHOOSEFONT cf = { sizeof(cf) };
    DWORD flags = CF_NOVERTFONTS | CF_TTONLY | CF_FORCEFONTEXIST | CF_SCRIPTSONLY | CF_ENABLETEMPLATE;

    if (lpfnHookProc)
    {
        flags |= CF_ENABLEHOOK;
        cf.lpfnHook = lpfnHookProc;
    }

    if (lpLogFont)
    {
        flags |= CF_INITTOLOGFONTSTRUCT;
        cf.lpLogFont = lpLogFont;
    }

    if (nSizeLimit > 0)
    {
        flags |= CF_LIMITSIZE;
        cf.nSizeMin = HIWORD(nSizeLimit);
        cf.nSizeMax = LOWORD(nSizeLimit);
    }

    cf.Flags = flags;
    cf.hwndOwner = hWndOwner;
    cf.hInstance = GetModuleHandleW(LIBRARYNAME);
    cf.lpTemplateName = MAKEINTRESOURCE(IDD_CHOOSEFONT);

    if (ChooseFont(&cf))
    {
        if (lpLogFont) lpLogFont = cf.lpLogFont;
        return TRUE;
    }

    return FALSE;
}
