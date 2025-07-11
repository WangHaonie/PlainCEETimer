#pragma once

#include <Windows.h>
#include <taskschd.h>
#include <comdef.h>

cexport(void) InitializeTaskScheduler();
cexport(void) ImportTaskFromXml(LPCWSTR taskName, BSTR bstrXml);
cexport(void) ExportTaskAsXml(LPCWSTR taskName, BSTR* pBstrXml);
cexport(void) DeleteScheduleTask(LPCWSTR taskName);
cexport(void) ReleaseTaskScheduler();