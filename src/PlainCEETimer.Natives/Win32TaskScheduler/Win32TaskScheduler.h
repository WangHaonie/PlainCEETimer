#pragma once

#include <Windows.h>
#include <comdef.h>
#include <taskschd.h>

cexport(void) InitializeTaskScheduler();
cexport(void) TaskSchedulerImportTaskFromXml(LPCWSTR path, LPCWSTR xmlText, TASK_LOGON_TYPE logonType);
cexport(void) TaskSchedulerExportTaskAsXml(LPCWSTR path, LPBSTR pXml);
cexport(void) TaskSchedulerEnableTask(LPCWSTR path);
cexport(void) TaskSchedulerDeleteTask(LPCWSTR path);
cexport(void) ReleaseTaskScheduler();