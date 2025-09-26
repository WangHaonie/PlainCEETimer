#include "pch.h"
#include "Win32TaskScheduler.h"

static ITaskService* pts = nullptr;
static ITaskFolder* ptf = nullptr;
static IRegisteredTask* prt = nullptr;
static bool Initialized = false;

static bool ResetRegisteredTask()
{
    if (prt)
    {
        prt->Release();
        prt = nullptr;
    }

    return true;
}

void InitializeTaskScheduler()
{
    if (!Initialized &&
        SUCCEEDED(CoCreateInstance(CLSID_TaskScheduler, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&pts))))
    {
        pts->Connect(_variant_t(), _variant_t(), _variant_t(), _variant_t());
        pts->GetFolder(_bstr_t(L"\\"), &ptf);
        Initialized = true;
    }
}

void TaskSchedulerImportTaskFromXml(LPCWSTR path, LPCWSTR xmlText, TASK_LOGON_TYPE logonType)
{
    if (Initialized && ResetRegisteredTask())
    {
        ptf->RegisterTask(_bstr_t(path), _bstr_t(xmlText), TASK_CREATE_OR_UPDATE, _variant_t(), _variant_t(), logonType, _variant_t(L""), &prt);
    }
}

void TaskSchedulerExportTaskAsXml(LPCWSTR path, LPBSTR pXml)
{
    if (Initialized && ResetRegisteredTask() && SUCCEEDED(ptf->GetTask(_bstr_t(path), &prt)))
    {
        prt->get_Xml(pXml);
    }
}

void TaskSchedulerEnableTask(LPCWSTR path)
{
    if (Initialized && ResetRegisteredTask() && SUCCEEDED(ptf->GetTask(_bstr_t(path), &prt)))
    {
        VARIANT_BOOL enabled;
        prt->get_Enabled(&enabled);
        
        if (enabled == VARIANT_FALSE)
        {
            prt->put_Enabled(VARIANT_TRUE);
        }
    }
}

void TaskSchedulerDeleteTask(LPCWSTR path)
{
    if (Initialized)
    {
        ptf->DeleteTask(_bstr_t(path), 0);
    }
}

void ReleaseTaskScheduler()
{
    ResetRegisteredTask();
    if (ptf) ptf->Release();
    if (pts) pts->Release();
    pts = nullptr;
    ptf = nullptr;
    Initialized = false;
}
