#include "pch.h"
#include "TaskScheduler.h"

static ITaskService* pService = nullptr;
static ITaskFolder* pFolder = nullptr;
static IRegisteredTask* pReg = nullptr;
static BOOL initialized = FALSE;

void InitializeTaskScheduler()
{
    if (!initialized &&
        SUCCEEDED(CoInitializeEx(nullptr, COINIT_MULTITHREADED)) &&
        SUCCEEDED(CoCreateInstance(CLSID_TaskScheduler, nullptr, CLSCTX_INPROC_SERVER, IID_ITaskService, (void**)&pService)))
    {
        pService->Connect(_variant_t(), _variant_t(), _variant_t(), _variant_t());
        pService->GetFolder(_bstr_t(L"\\"), &pFolder);
        initialized = TRUE;
    }
}

void ImportTaskFromXml(LPCWSTR taskName, LPCWSTR xml)
{
    if (initialized)
    {
        pFolder->RegisterTask(_bstr_t(taskName), _bstr_t(xml), TASK_CREATE_OR_UPDATE, _variant_t(), _variant_t(), TASK_LOGON_INTERACTIVE_TOKEN, _variant_t(L""), &pReg);
    }
}

void ExportTaskAsXml(LPCWSTR taskName, BSTR* pBstrXml)
{
    if (initialized && pBstrXml && SUCCEEDED(pFolder->GetTask(_bstr_t(taskName), &pReg)))
    {
        pReg->get_Xml(pBstrXml);
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
        if (pReg) pReg->Release();
        if (pFolder) pFolder->Release();
        if (pService) pService->Release();
        CoUninitialize();
        pService = nullptr;
        pFolder = nullptr;
        pReg = nullptr;
        initialized = FALSE;
    }
}
