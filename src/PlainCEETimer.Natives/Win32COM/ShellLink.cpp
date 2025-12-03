#include "pch.h"
#include "ShellLink.h"
#include "Utils.h"
#include <Windows.h>
#include <ShObjIdl_core.h>

static IShellLink* psh = nullptr;
static IPersistFile* ppf = nullptr;
static bool init = false;

void InitializeShellLink()
{
    if (!init &&
        SUCCEEDED(CoCreateInstance(CLSID_ShellLink, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&psh))))
    {
        psh->QueryInterface(IID_PPV_ARGS(&ppf));
        init = true;
    }
}

void ShellLinkCreateLnk(LNKFILEINFO lnkFileInfo)
{
    if (init)
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
    if (init && lpLnkFileInfo &&
        SUCCEEDED(ppf->Load(lpLnkFileInfo->lnkPath, STGM_READ)))
    {
        LPWSTR t = CoTaskStrAllocW(MAX_PATH, nullptr);
        LPWSTR a = CoTaskStrAllocW(INFOTIPSIZE, nullptr);
        LPWSTR wd = CoTaskStrAllocW(MAX_PATH, nullptr);
        LPWSTR d = CoTaskStrAllocW(MAX_PATH, nullptr);
        LPWSTR ip = CoTaskStrAllocW(MAX_PATH, nullptr);

        psh->GetPath(t, MAX_PATH, nullptr, 0);
        psh->GetArguments(a, INFOTIPSIZE);
        psh->GetWorkingDirectory(wd, MAX_PATH);
        psh->GetHotkey(&lpLnkFileInfo->wHotkey);
        psh->GetShowCmd(&lpLnkFileInfo->iShowCmd);
        psh->GetDescription(d, MAX_PATH);
        psh->GetIconLocation(ip, MAX_PATH, &lpLnkFileInfo->iIcon);

        lpLnkFileInfo->pszTarget = t;
        lpLnkFileInfo->pszArgs = a;
        lpLnkFileInfo->pszWorkingDir = wd;
        lpLnkFileInfo->pszDescription = d;
        lpLnkFileInfo->pszIconPath = ip;
    }
}

void ReleaseShellLink()
{
    ReleasePPI(&ppf);
    ReleasePPI(&psh);
    init = false;
}