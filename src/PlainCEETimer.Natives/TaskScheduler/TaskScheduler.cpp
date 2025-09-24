#include "pch.h"
#include "TaskScheduler.h"
#include <taskschd.h>

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

void TaskSchedulerImportTaskFromXml(LPCWSTR taskName, LPCWSTR strXml)
{
    if (Initialized && ResetRegisteredTask())
    {
        ptf->RegisterTask(_bstr_t(taskName), _bstr_t(strXml), TASK_CREATE_OR_UPDATE, _variant_t(), _variant_t(), TASK_LOGON_INTERACTIVE_TOKEN, _variant_t(L""), &prt);
    }
}

void TaskSchedulerExportTaskAsXml(LPCWSTR taskName, BSTR* pbstrXml)
{
    if (Initialized && ResetRegisteredTask() && SUCCEEDED(ptf->GetTask(_bstr_t(taskName), &prt)))
    {
        prt->get_Xml(pbstrXml);
    }
}

void TaskSchedulerEnableTask(LPCWSTR taskName)
{
    if (Initialized && ResetRegisteredTask() && SUCCEEDED(ptf->GetTask(_bstr_t(taskName), &prt)))
    {
        VARIANT_BOOL enabled;
        prt->get_Enabled(&enabled);
        
        if (enabled == VARIANT_FALSE)
        {
            prt->put_Enabled(VARIANT_TRUE);
        }
    }
}

void TaskSchedulerDeleteTask(LPCWSTR taskName)
{
    if (Initialized)
    {
        ptf->DeleteTask(_bstr_t(taskName), 0);
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
