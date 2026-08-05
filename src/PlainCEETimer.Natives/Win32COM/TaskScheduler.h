#pragma once

#include <taskschd.h>
#include <Windows.h>

NATIVES_EXPORT void NATIVESAPI InitializeTaskScheduler();
NATIVES_EXPORT void NATIVESAPI TaskSchedulerImportTaskFromXml(LPCWSTR path, LPCWSTR xmlText, TASK_LOGON_TYPE logonType);
NATIVES_EXPORT BOOL NATIVESAPI TaskSchedulerExportTaskAsXml(LPCWSTR path, LPBSTR pXml);
NATIVES_EXPORT BOOL NATIVESAPI TaskSchedulerExistsTask(LPCWSTR path);
NATIVES_EXPORT void NATIVESAPI TaskSchedulerEnableTask(LPCWSTR path);
NATIVES_EXPORT void NATIVESAPI TaskSchedulerDeleteTask(LPCWSTR path);
NATIVES_EXPORT void NATIVESAPI ReleaseTaskScheduler();