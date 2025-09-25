#pragma once

#include <Windows.h>
#include <comdef.h>

cexport(void) InitializeTaskScheduler();
cexport(void) TaskSchedulerImportTaskFromXml(LPCWSTR taskName, LPCWSTR strXml);
cexport(void) TaskSchedulerExportTaskAsXml(LPCWSTR taskName, LPBSTR pbstrXml);
cexport(void) TaskSchedulerEnableTask(LPCWSTR taskName);
cexport(void) TaskSchedulerDeleteTask(LPCWSTR taskName);
cexport(void) ReleaseTaskScheduler();