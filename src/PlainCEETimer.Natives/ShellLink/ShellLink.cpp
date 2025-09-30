#include "pch.h"
#include "ShellLink.h"
#include <ShObjIdl.h>

static IShellLink* psh = nullptr;
static IPersistFile* ppf = nullptr;
static bool Initialized = false;

void InitializeShellLink()
{
    if (!Initialized &&
        SUCCEEDED(CoCreateInstance(CLSID_ShellLink, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&psh))))
    {
        psh->QueryInterface(IID_PPV_ARGS(&ppf));
        Initialized = true;
    }
}

void ShellLinkCreateLnk(LNKFILEINFO lnkFileInfo)
{
    if (Initialized)
    {
        psh->SetPath(lnkFileInfo.pszTarget);
        psh->SetArguments(lnkFileInfo.pszArgs);
        psh->SetWorkingDirectory(lnkFileInfo.pszWorkingDir);
        psh->SetHotkey(lnkFileInfo.wHotkey);
        psh->SetShowCmd(lnkFileInfo.iShowCmd);
        psh->SetDescription(lnkFileInfo.pszDescription);
        psh->SetIconLocation(lnkFileInfo.pszIconPath, lnkFileInfo.iIcon);
        ppf->Save(lnkFileInfo.lnkPath, FALSE);
    }
}

void ShellLinkQueryLnk(LPLNKFILEINFO lpLnkFileInfo)
{
    if (Initialized && lpLnkFileInfo &&
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

        lpLnkFileInfo->pszTarget = CoTaskStrAllocW(t);
        lpLnkFileInfo->pszArgs = CoTaskStrAllocW(a);
        lpLnkFileInfo->pszWorkingDir = CoTaskStrAllocW(wd);
        lpLnkFileInfo->pszDescription = CoTaskStrAllocW(d);
        lpLnkFileInfo->pszIconPath = CoTaskStrAllocW(ip);
    }
}

void ReleaseShellLink()
{
    if (ppf) ppf->Release();
    if (psh) psh->Release();
    ppf = nullptr;
    psh = nullptr;
    Initialized = false;
}