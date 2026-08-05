#include "pch.h"
#include "TaskScheduler.h"
#include "Utils.h"
#include <comdef.h>
#include <taskschd.h>
#include <Windows.h>

static ITaskService* pts = nullptr;
static ITaskFolder* ptf = nullptr;
static IRegisteredTask* prt = nullptr;
static bool init = false;

void NATIVESAPI InitializeTaskScheduler()
{
    if (!init &&
        SUCCEEDED(CoCreateInstance(CLSID_TaskScheduler, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&pts))))
    {
        pts->Connect(_variant_t(), _variant_t(), _variant_t(), _variant_t());
        pts->GetFolder(_bstr_t(L"\\"), &ptf);
        init = true;
    }
}

void NATIVESAPI TaskSchedulerImportTaskFromXml(LPCWSTR path, LPCWSTR xmlText, TASK_LOGON_TYPE logonType)
{
    if (init)
    {
        ptf->RegisterTask(_bstr_t(path), _bstr_t(xmlText), TASK_CREATE_OR_UPDATE, _variant_t(), _variant_t(), logonType, _variant_t(), &prt);
    }

    ReleasePPI(&prt);
}

BOOL NATIVESAPI TaskSchedulerExportTaskAsXml(LPCWSTR path, LPBSTR pXml)
{
    if (init
        && SUCCEEDED(ptf->GetTask(_bstr_t(path), &prt))
        && SUCCEEDED(prt->get_Xml(pXml)))
    {
        return TRUE;
    }

    ReleasePPI(&prt);
    return FALSE;
}

BOOL NATIVESAPI TaskSchedulerExistsTask(LPCWSTR path)
{
    if (init && SUCCEEDED(ptf->GetTask(_bstr_t(path), &prt)))
    {
        return TRUE;
    }

    ReleasePPI(&prt);
    return FALSE;
}

void NATIVESAPI TaskSchedulerEnableTask(LPCWSTR path)
{
    if (init && SUCCEEDED(ptf->GetTask(_bstr_t(path), &prt)))
    {
        VARIANT_BOOL enabled;
        prt->get_Enabled(&enabled);
        
        if (enabled == VARIANT_FALSE)
        {
            prt->put_Enabled(VARIANT_TRUE);
        }
    }

    ReleasePPI(&prt);
}

void NATIVESAPI TaskSchedulerDeleteTask(LPCWSTR path)
{
    if (init)
    {
        ptf->DeleteTask(_bstr_t(path), 0);
    }
}

void NATIVESAPI ReleaseTaskScheduler()
{
    ReleasePPI(&ptf);
    ReleasePPI(&pts);
    init = false;
}
