#pragma once

#include <Windows.h>
#include <taskschd.h>
#include <comdef.h>

cexport(void) InitializeTaskScheduler();
cexport(void) ImportTaskFromXmlString(LPCWSTR taskName, LPCWSTR strXml);
cexport(void) ExportTaskAsXmlString(LPCWSTR taskName, BSTR* pbstrXml);
cexport(void) EnableScheduleTask(LPCWSTR taskName);
cexport(void) DeleteScheduleTask(LPCWSTR taskName);
cexport(void) ReleaseTaskScheduler();