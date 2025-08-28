#include "pch.h"
#include "CommonDialogs.h"

cexport(BOOL) RunFontDialog(HWND hWndOwner, LPLOGFONT lpLogFont, LPFRHOOKPROC lpfnHookProc, int nSizeMin, int nSizeMax)
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
