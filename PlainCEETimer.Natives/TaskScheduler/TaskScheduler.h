#pragma once

#include <Windows.h>
#include <comdef.h>

cexport(void) InitializeTaskScheduler();
cexport(void) TaskSchdImportTaskFromXml(LPCWSTR taskName, LPCWSTR strXml);
cexport(void) TaskSchdExportTaskAsXml(LPCWSTR taskName, BSTR* pbstrXml);
cexport(void) TaskSchdEnableTask(LPCWSTR taskName);
cexport(void) TaskSchdDeleteTask(LPCWSTR taskName);
cexport(void) ReleaseTaskScheduler();