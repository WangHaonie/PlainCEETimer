#include "pch.h"
#include "ShellLink.h"
#include "Utils.h"
#include <ShObjIdl_core.h>
#include <Windows.h>

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

void ShellLinkCreateLnk(LPLNKFILEINFO lpLnkFileInfo)
{
    if (init)
    {
        psh->SetPath(lpLnkFileInfo->pszTarget);
        psh->SetArguments(lpLnkFileInfo->pszArgs);
        psh->SetWorkingDirectory(lpLnkFileInfo->pszWorkingDir);
        psh->SetHotkey(lpLnkFileInfo->wHotkey);
        psh->SetShowCmd(lpLnkFileInfo->iShowCmd);
        psh->SetDescription(lpLnkFileInfo->pszDescription);
        psh->SetIconLocation(lpLnkFileInfo->pszIconPath, lpLnkFileInfo->iIcon);
        ppf->Save(lpLnkFileInfo->lnkPath, FALSE);
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