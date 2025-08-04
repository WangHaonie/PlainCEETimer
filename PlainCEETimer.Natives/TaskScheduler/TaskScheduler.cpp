#include "pch.h"
#include "TaskScheduler.h"
#include <taskschd.h>

static ITaskService* pts = nullptr;
static ITaskFolder* ptf = nullptr;
static IRegisteredTask* prt = nullptr;
static BOOL initialized = FALSE;

static BOOL ResetPtrRegisteredTask()
{
    if (prt)
    {
        prt->Release();
        prt = nullptr;
    }

    return TRUE;
}

void InitializeTaskScheduler()
{
    if (!initialized &&
        SUCCEEDED(CoCreateInstance(CLSID_TaskScheduler, nullptr, CLSCTX_INPROC_SERVER, IID_ITaskService, (LPVOID*)&pts)))
    {
        pts->Connect(_variant_t(), _variant_t(), _variant_t(), _variant_t());
        pts->GetFolder(_bstr_t(L"\\"), &ptf);
        initialized = TRUE;
    }
}

void TaskSchdImportTaskFromXml(LPCWSTR taskName, LPCWSTR strXml)
{
    if (initialized && ResetPtrRegisteredTask())
    {
        ptf->RegisterTask(_bstr_t(taskName), _bstr_t(strXml), TASK_CREATE_OR_UPDATE, _variant_t(), _variant_t(), TASK_LOGON_INTERACTIVE_TOKEN, _variant_t(L""), &prt);
    }
}

void TaskSchdExportTaskAsXml(LPCWSTR taskName, BSTR* pbstrXml)
{
    if (initialized && ResetPtrRegisteredTask() && SUCCEEDED(ptf->GetTask(_bstr_t(taskName), &prt)))
    {
        prt->get_Xml(pbstrXml);
    }
}

void TaskSchdEnableTask(LPCWSTR taskName)
{
    if (initialized && ResetPtrRegisteredTask() && SUCCEEDED(ptf->GetTask(_bstr_t(taskName), &prt)))
    {
        VARIANT_BOOL enabled;
        prt->get_Enabled(&enabled);
        
        if (enabled == VARIANT_FALSE)
        {
            prt->put_Enabled(VARIANT_TRUE);
        }
    }
}

void TaskSchdDeleteTask(LPCWSTR taskName)
{
    if (initialized)
    {
        ptf->DeleteTask(_bstr_t(taskName), 0);
    }
}

void ReleaseTaskScheduler()
{
    ResetPtrRegisteredTask();
    if (ptf) ptf->Release();
    if (pts) pts->Release();
    pts = nullptr;
    ptf = nullptr;
    initialized = FALSE;
}
