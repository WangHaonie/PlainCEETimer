#include "pch.h"
#include "CommonDialogs.h"

BOOL RunColorDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, COLORREF* lpColor, COLORREF* lpCustomColors)
{
	CHOOSECOLOR cc = {};
	cc.lStructSize = sizeof(cc);
	cc.hwndOwner = hWndOwner;
	cc.hInstance = reinterpret_cast<HWND>(GetModuleHandleW(LIBRARYNAME));
	cc.rgbResult = *lpColor;
	cc.lpCustColors = lpCustomColors;
	cc.Flags = CC_RGBINIT | CC_ANYCOLOR | CC_FULLOPEN | CC_ENABLEHOOK | CC_ENABLETEMPLATE;
	cc.lpfnHook = lpfnHookProc;
	cc.lpTemplateName = MAKEINTRESOURCE(IDD_CHOOSECOLOR);

	BOOL result = ChooseColor(&cc);

	if (result)
	{
		lpColor = &cc.rgbResult;
		lpCustomColors = cc.lpCustColors;
	}

	return result;
}

cexport(BOOL) RunFontDialog(HWND hWndOwner, LPFRHOOKPROC lpfnHookProc, LPLOGFONT lpLogFont, int nSizeMin, int nSizeMax)
{
	CHOOSEFONT cf = {};
	cf.lStructSize = sizeof(cf);
	cf.hwndOwner = hWndOwner;
	cf.lpLogFont = lpLogFont;
	cf.Flags = CF_NOVERTFONTS | CF_TTONLY | CF_FORCEFONTEXIST | CF_LIMITSIZE | CF_SCRIPTSONLY | CF_INITTOLOGFONTSTRUCT | CF_ENABLEHOOK | CF_ENABLETEMPLATE;
	cf.lpfnHook = lpfnHookProc;
	cf.lpTemplateName = MAKEINTRESOURCE(IDD_CHOOSEFONT);
	cf.hInstance = GetModuleHandleW(LIBRARYNAME);
	cf.nSizeMin = nSizeMin;
	cf.nSizeMax = nSizeMax;

	BOOL result = ChooseFont(&cf);

	if (result)
	{
		lpLogFont = cf.lpLogFont;
	}

	return result;
}
