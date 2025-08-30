#include "pch.h"
#include "ShellLink.h"
#include <ShObjIdl.h>
#include <propkey.h>
#include <string>

using namespace std;

static IShellLink* psh = nullptr;
static IPersistFile* ppf = nullptr;
static IPropertyStore* pps = nullptr;
static BOOL initialized = FALSE;

static HRESULT GetPropertyStore(REFPROPERTYKEY key, wstring& value)
{
    PROPVARIANT pv;
    PropVariantInit(&pv);

    auto hr = pps->GetValue(key, &pv);

    if (SUCCEEDED(hr) && pv.vt == VT_LPWSTR)
    {
        value = pv.pwszVal;
    }
    else
    {
        value.clear();
    }

    PropVariantClear(&pv);
    return hr;
}

void InitializeShellLink()
{
    if (!initialized &&
        SUCCEEDED(CoCreateInstance(CLSID_ShellLink, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&psh))))
    {
        psh->QueryInterface(IID_PPV_ARGS(&ppf));
        initialized = TRUE;
    }
}

void ShellLinkCreateLnk(LnkInfo shLnkInfo)
{
    if (initialized)
    {
        psh->SetPath(shLnkInfo.target);
        psh->SetArguments(shLnkInfo.args);
        psh->SetWorkingDirectory(shLnkInfo.workingDir);
        psh->SetHotkey(shLnkInfo.hotkey);
        psh->SetShowCmd(shLnkInfo.showCmd);
        psh->SetDescription(shLnkInfo.description);
        psh->SetIconLocation(shLnkInfo.iconPath, shLnkInfo.iconIndex);
        ppf->Save(shLnkInfo.lnkPath, TRUE);
    }
}

void ShellLinkQueryLnk(LnkInfo* lpshLnkInfo)
{
    if (initialized && lpshLnkInfo &&
        SUCCEEDED(ppf->Load(lpshLnkInfo->lnkPath, STGM_READ)))
    {
        SHGetPropertyStoreFromParsingName(lpshLnkInfo->lnkPath, nullptr, GPS_DEFAULT, IID_PPV_ARGS(&pps));

        wstring shlpath;
        GetPropertyStore(PKEY_Link_TargetParsingPath, shlpath);
        lpshLnkInfo->target = _wcsdup(shlpath.c_str());

        wstring shlargs;
        GetPropertyStore(PKEY_Link_Arguments, shlargs);
        lpshLnkInfo->args = _wcsdup(shlargs.c_str());

        WCHAR shlworkdir[MAX_PATH];
        psh->GetWorkingDirectory(shlworkdir, MAX_PATH);
        lpshLnkInfo->workingDir = _wcsdup(shlworkdir);

        WORD shlkeys;
        psh->GetHotkey(&shlkeys);
        lpshLnkInfo->hotkey = shlkeys;

        int shlshowcmd;
        psh->GetShowCmd(&shlshowcmd);
        lpshLnkInfo->showCmd = shlshowcmd;

        wstring shldescr;
        GetPropertyStore(PKEY_Comment, shldescr);
        lpshLnkInfo->description = _wcsdup(shldescr.c_str());

        WCHAR shliconloc[MAX_PATH];
        int index = 0;
        psh->GetIconLocation(shliconloc, MAX_PATH, &index);
        lpshLnkInfo->iconPath = _wcsdup(shliconloc);
        lpshLnkInfo->iconIndex = index;
    }
}

void ReleaseShellLink()
{
    if (pps) pps->Release();
    if (ppf) ppf->Release();
    if (psh) psh->Release();
    pps = nullptr;
    ppf = nullptr;
    psh = nullptr;
    initialized = FALSE;
}