#include "pch.h"
#include "CommonDialogs.h"

BOOL RunColorDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, COLORREF* lpColor, COLORREF* lpCustomColors)
{
    if (hWndOwner && lpfnHookProc && lpColor && lpCustomColors)
    {
        CHOOSECOLOR cc = { sizeof(cc) };
        cc.hwndOwner = hWndOwner;
        cc.hInstance = reinterpret_cast<HWND>(GetModuleHandleW(LIBRARYNAME));
        cc.rgbResult = *lpColor;
        cc.lpCustColors = lpCustomColors;
        cc.Flags = CC_RGBINIT | CC_ANYCOLOR | CC_FULLOPEN | CC_ENABLEHOOK | CC_ENABLETEMPLATE;
        cc.lpfnHook = lpfnHookProc;
        cc.lpTemplateName = MAKEINTRESOURCE(IDD_CHOOSECOLOR);

        if (ChooseColor(&cc))
        {
            *lpColor = cc.rgbResult;
            lpCustomColors = cc.lpCustColors;
            return TRUE;
        }
    }

    return FALSE;
}

BOOL RunFontDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, int nSizeLimit)
{
    if (hWndOwner && lpfnHookProc && lpLogFont && nSizeLimit > 0)
    {
        CHOOSEFONT cf = { sizeof(cf) };
        cf.hwndOwner = hWndOwner;
        cf.lpLogFont = lpLogFont;
        cf.Flags = CF_NOVERTFONTS | CF_TTONLY | CF_FORCEFONTEXIST | CF_LIMITSIZE | CF_SCRIPTSONLY | CF_INITTOLOGFONTSTRUCT | CF_ENABLEHOOK | CF_ENABLETEMPLATE;
        cf.lpfnHook = lpfnHookProc;
        cf.lpTemplateName = MAKEINTRESOURCE(IDD_CHOOSEFONT);
        cf.hInstance = GetModuleHandleW(LIBRARYNAME);
        cf.nSizeMin = LOWORD(nSizeLimit);
        cf.nSizeMax = HIWORD(nSizeLimit);

        if (ChooseFont(&cf))
        {
            lpLogFont = cf.lpLogFont;
            return TRUE;
        }
    }

    return FALSE;
}
