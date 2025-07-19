#include "pch.h"
#include "TaskScheduler.h"

static ITaskService* pService = nullptr;
static ITaskFolder* pFolder = nullptr;
static IRegisteredTask* pReg = nullptr;
static BOOL initialized = FALSE;

static BOOL ResetPtrRegisteredTask()
{
    if (pReg)
    {
        pReg->Release();
        pReg = nullptr;
    }

    return TRUE;
}

void InitializeTaskScheduler()
{
    if (!initialized &&
        SUCCEEDED(CoInitializeEx(nullptr, COINIT_MULTITHREADED)) &&
        SUCCEEDED(CoCreateInstance(CLSID_TaskScheduler, nullptr, CLSCTX_INPROC_SERVER, IID_ITaskService, (LPVOID*)&pService)))
    {
        pService->Connect(_variant_t(), _variant_t(), _variant_t(), _variant_t());
        pService->GetFolder(_bstr_t(L"\\"), &pFolder);
        initialized = TRUE;
    }
}

void ImportTaskFromXml(LPCWSTR taskName, BSTR bstrXml)
{
    if (initialized && ResetPtrRegisteredTask())
    {
        pFolder->RegisterTask(_bstr_t(taskName), bstrXml, TASK_CREATE_OR_UPDATE, _variant_t(), _variant_t(), TASK_LOGON_INTERACTIVE_TOKEN, _variant_t(L""), &pReg);
    }
}

void ExportTaskAsXml(LPCWSTR taskName, BSTR* pbstrXml)
{
    if (initialized && pbstrXml && ResetPtrRegisteredTask() && SUCCEEDED(pFolder->GetTask(_bstr_t(taskName), &pReg)))
    {
        pReg->get_Xml(pbstrXml);
    }
}

void EnableScheduleTask(LPCWSTR taskName)
{
    if (initialized && ResetPtrRegisteredTask() && SUCCEEDED(pFolder->GetTask(_bstr_t(taskName), &pReg)))
    {
        VARIANT_BOOL enabled;
        pReg->get_Enabled(&enabled);
        
        if (enabled == VARIANT_FALSE)
        {
            pReg->put_Enabled(VARIANT_TRUE);
        }
    }
}

void DeleteScheduleTask(LPCWSTR taskName)
{
    if (initialized)
    {
        pFolder->DeleteTask(_bstr_t(taskName), 0);
    }
}

void ReleaseTaskScheduler()
{
    if (initialized)
    {
        ResetPtrRegisteredTask();
        if (pFolder) pFolder->Release();
        if (pService) pService->Release();
        pService = nullptr;
        pFolder = nullptr;
        CoUninitialize();
        initialized = FALSE;
    }
}
