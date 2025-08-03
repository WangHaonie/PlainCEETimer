#include "pch.h"
#include "ShellLink.h"
#include <propkey.h>
#include <string>

using namespace std;

static IShellLink* pShellLink = nullptr;
static IPersistFile* pPersistFile = nullptr;
static IPropertyStore* pPropertyStore = nullptr;
static BOOL initialized = FALSE;

static HRESULT GetPropertyStore(REFPROPERTYKEY key, wstring& out)
{
    PROPVARIANT pv;
    PropVariantInit(&pv);

    auto hr = pPropertyStore->GetValue(key, &pv);

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
        SUCCEEDED(CoCreateInstance(CLSID_ShellLink, nullptr, CLSCTX_INPROC_SERVER, IID_IShellLinkW, (LPVOID*)&pShellLink)))
    {
        pShellLink->QueryInterface(IID_IPersistFile, (LPVOID*)&pPersistFile);
        initialized = TRUE;
    }
}

void ShellLinkCreateLnk(SHLNKINFO shLnkInfo)
{
    if (initialized)
    {
        pShellLink->SetPath(shLnkInfo.pszFile);
        pShellLink->SetArguments(shLnkInfo.pszArgs);
        pShellLink->SetWorkingDirectory(shLnkInfo.pszWorkDir);
        pShellLink->SetHotkey(shLnkInfo.wHotkey);
        pShellLink->SetShowCmd(shLnkInfo.iShowCmd);
        pShellLink->SetDescription(shLnkInfo.pszDescr);
        pShellLink->SetIconLocation(shLnkInfo.pszIconPath, shLnkInfo.iIcon);
        pPersistFile->Save(shLnkInfo.pszLnkPath, TRUE);
    }
}

void ShellLinkExportLnk(LPSHLNKINFO lpshLnkInfo)
{
    if (initialized &&
        SUCCEEDED(pPersistFile->Load(lpshLnkInfo->pszLnkPath, STGM_READ)))
    {
        SHGetPropertyStoreFromParsingName(lpshLnkInfo->pszLnkPath, nullptr, GPS_DEFAULT, IID_IPropertyStore, (LPVOID*)&pPropertyStore);

        wstring shlpath;
        GetPropertyStore(PKEY_Link_TargetParsingPath, shlpath);
        lpshLnkInfo->pszFile = _wcsdup(shlpath.c_str());

        wstring shlargs;
        GetPropertyStore(PKEY_Link_Arguments, shlargs);
        lpshLnkInfo->pszArgs = _wcsdup(shlargs.c_str());

        WCHAR shlworkdir[MAX_PATH];
        pShellLink->GetWorkingDirectory(shlworkdir, MAX_PATH);
        lpshLnkInfo->pszWorkDir = _wcsdup(shlworkdir);

        WORD shlkeys;
        pShellLink->GetHotkey(&shlkeys);
        lpshLnkInfo->wHotkey = shlkeys;

        int shlshowcmd;
        pShellLink->GetShowCmd(&shlshowcmd);
        lpshLnkInfo->iShowCmd = shlshowcmd;

        wstring shldescr;
        GetPropertyStore(PKEY_Comment, shldescr);
        lpshLnkInfo->pszDescr = _wcsdup(shldescr.c_str());

        WCHAR shliconloc[MAX_PATH];
        int index = 0;
        pShellLink->GetIconLocation(shliconloc, MAX_PATH, &index);
        lpshLnkInfo->pszIconPath = _wcsdup(shliconloc);
        lpshLnkInfo->iIcon = index;
    }
}

void ReleaseShellLink()
{
    if (pPropertyStore) pPropertyStore->Release();
    if (pPersistFile) pPersistFile->Release();
    if (pShellLink) pShellLink->Release();
    pPropertyStore = nullptr;
    pPersistFile = nullptr;
    pShellLink = nullptr;
    initialized = FALSE;
}