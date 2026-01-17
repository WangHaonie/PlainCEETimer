#pragma once

#include <taskschd.h>
#include <Windows.h>

cexport(void) InitializeTaskScheduler();
cexport(void) TaskSchedulerImportTaskFromXml(LPCWSTR path, LPCWSTR xmlText, TASK_LOGON_TYPE logonType);
cexport(BOOL) TaskSchedulerExportTaskAsXml(LPCWSTR path, LPBSTR pXml);
cexport(BOOL) TaskSchedulerExistsTask(LPCWSTR path);
cexport(void) TaskSchedulerEnableTask(LPCWSTR path);
cexport(void) TaskSchedulerDeleteTask(LPCWSTR path);
cexport(void) ReleaseTaskScheduler();