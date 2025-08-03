#include "pch.h"
#include "ShellLink.h"
#include <propkey.h>
#include <string>

using namespace std;

static IShellLink* psh = nullptr;
static IPersistFile* ppf = nullptr;
static IPropertyStore* pps = nullptr;
static BOOL initialized = FALSE;

static HRESULT GetPropertyStore(REFPROPERTYKEY key, wstring& out)
{
    PROPVARIANT pv;
    PropVariantInit(&pv);

    auto hr = pps->GetValue(key, &pv);

    if (SUCCEEDED(hr) && pv.vt == VT_LPWSTR)
    {
        out = pv.pwszVal;
    }
    else
    {
        out.clear();
    }

    PropVariantClear(&pv);
    return hr;
}

void InitializeShellLink()
{
    if (!initialized &&
        SUCCEEDED(CoCreateInstance(CLSID_ShellLink, nullptr, CLSCTX_INPROC_SERVER, IID_IShellLinkW, (LPVOID*)&psh)))
    {
        psh->QueryInterface(IID_IPersistFile, (LPVOID*)&ppf);
        initialized = TRUE;
    }
}

void ShLkCreateLnk(SHLNKINFO shLnkInfo)
{
    if (initialized)
    {
        psh->SetPath(shLnkInfo.pszFile);
        psh->SetArguments(shLnkInfo.pszArgs);
        psh->SetWorkingDirectory(shLnkInfo.pszWorkDir);
        psh->SetHotkey(shLnkInfo.wHotkey);
        psh->SetShowCmd(shLnkInfo.iShowCmd);
        psh->SetDescription(shLnkInfo.pszDescr);
        psh->SetIconLocation(shLnkInfo.pszIconPath, shLnkInfo.iIcon);
        ppf->Save(shLnkInfo.pszLnkPath, TRUE);
    }
}

void ShLkQueryLnk(LPSHLNKINFO lpshLnkInfo)
{
    if (initialized &&
        SUCCEEDED(ppf->Load(lpshLnkInfo->pszLnkPath, STGM_READ)))
    {
        SHGetPropertyStoreFromParsingName(lpshLnkInfo->pszLnkPath, nullptr, GPS_DEFAULT, IID_IPropertyStore, (LPVOID*)&pps);

        wstring shlpath;
        GetPropertyStore(PKEY_Link_TargetParsingPath, shlpath);
        lpshLnkInfo->pszFile = _wcsdup(shlpath.c_str());

        wstring shlargs;
        GetPropertyStore(PKEY_Link_Arguments, shlargs);
        lpshLnkInfo->pszArgs = _wcsdup(shlargs.c_str());

        WCHAR shlworkdir[MAX_PATH];
        psh->GetWorkingDirectory(shlworkdir, MAX_PATH);
        lpshLnkInfo->pszWorkDir = _wcsdup(shlworkdir);

        WORD shlkeys;
        psh->GetHotkey(&shlkeys);
        lpshLnkInfo->wHotkey = shlkeys;

        int shlshowcmd;
        psh->GetShowCmd(&shlshowcmd);
        lpshLnkInfo->iShowCmd = shlshowcmd;

        wstring shldescr;
        GetPropertyStore(PKEY_Comment, shldescr);
        lpshLnkInfo->pszDescr = _wcsdup(shldescr.c_str());

        WCHAR shliconloc[MAX_PATH];
        int index = 0;
        psh->GetIconLocation(shliconloc, MAX_PATH, &index);
        lpshLnkInfo->pszIconPath = _wcsdup(shliconloc);
        lpshLnkInfo->iIcon = index;
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