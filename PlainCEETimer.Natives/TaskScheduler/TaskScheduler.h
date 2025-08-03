#pragma once

#include <Windows.h>
#include <taskschd.h>
#include <comdef.h>

cexport(void) InitializeTaskScheduler();
cexport(void) TSchImportTaskFromXmlString(LPCWSTR taskName, LPCWSTR strXml);
cexport(void) TSchExportTaskAsXmlString(LPCWSTR taskName, BSTR* pbstrXml);
cexport(void) TSchEnableTask(LPCWSTR taskName);
cexport(void) TSchDeleteTask(LPCWSTR taskName);
cexport(void) ReleaseTaskScheduler();