#include "pch.h"
#include "ShellLink.h"
#include <ShObjIdl.h>
#include <propkey.h>
#include <string>

using namespace std;

static IShellLink* psh = nullptr;
static IPersistFile* ppf = nullptr;
static BOOL initialized = FALSE;

void InitializeShellLink()
{
    if (!initialized &&
        SUCCEEDED(CoCreateInstance(CLSID_ShellLink, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&psh))))
    {
        psh->QueryInterface(IID_PPV_ARGS(&ppf));
        initialized = TRUE;
    }
}

void ShellLinkCreateLnk(LNKFILEINFO lnkFileInfo)
{
    if (initialized)
    {
        psh->SetPath(lnkFileInfo.pszTarget);
        psh->SetArguments(lnkFileInfo.pszArgs);
        psh->SetWorkingDirectory(lnkFileInfo.pszWorkingDir);
        psh->SetHotkey(lnkFileInfo.wHotkey);
        psh->SetShowCmd(lnkFileInfo.iShowCmd);
        psh->SetDescription(lnkFileInfo.pszDescription);
        psh->SetIconLocation(lnkFileInfo.pszIconPath, lnkFileInfo.iIcon);
        ppf->Save(lnkFileInfo.lnkPath, TRUE);
    }
}

void ShellLinkQueryLnk(LPLNKFILEINFO lpLnkFileInfo)
{
    if (initialized && lpLnkFileInfo &&
        SUCCEEDED(ppf->Load(lpLnkFileInfo->lnkPath, STGM_READ)))
    {
        WCHAR t[MAX_PATH];
        WCHAR a[INFOTIPSIZE];
        WCHAR wd[MAX_PATH];
        WCHAR d[MAX_PATH];
        WCHAR ip[MAX_PATH];

        psh->GetPath(t, MAX_PATH, nullptr, 0);
        psh->GetArguments(a, INFOTIPSIZE);
        psh->GetWorkingDirectory(wd, MAX_PATH);
        psh->GetHotkey(&lpLnkFileInfo->wHotkey);
        psh->GetShowCmd(&lpLnkFileInfo->iShowCmd);
        psh->GetDescription(d, MAX_PATH);
        psh->GetIconLocation(ip, MAX_PATH, &lpLnkFileInfo->iIcon);

        lpLnkFileInfo->pszTarget = _wcsdup(t);
        lpLnkFileInfo->pszArgs = _wcsdup(a);
        lpLnkFileInfo->pszWorkingDir = _wcsdup(wd);
        lpLnkFileInfo->pszDescription = _wcsdup(d);
        lpLnkFileInfo->pszIconPath = _wcsdup(ip);
    }
}

void ReleaseShellLink()
{
    if (ppf) ppf->Release();
    if (psh) psh->Release();
    ppf = nullptr;
    psh = nullptr;
    initialized = FALSE;
}