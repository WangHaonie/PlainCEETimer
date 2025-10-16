#pragma once

#include <Windows.h>
#include <comdef.h>
#include <ShObjIdl.h>
#include <taskschd.h>

// ITaskbarList3

cexport(void) InitializeTaskbarList();
cexport(void) TaskbarListSetProgressState(HWND hWnd, TBPFLAG tbpFlags);
cexport(void) TaskbarListSetProgressValue(HWND hWnd, ULONGLONG ullCompleted, ULONGLONG ullTotal);
cexport(void) ReleaseTaskbarList();

// ITaskService, ITaskFolder, IRegisteredTask

cexport(void) InitializeTaskScheduler();
cexport(void) TaskSchedulerImportTaskFromXml(LPCWSTR path, LPCWSTR xmlText, TASK_LOGON_TYPE logonType);
cexport(void) TaskSchedulerExportTaskAsXml(LPCWSTR path, LPBSTR pXml);
cexport(void) TaskSchedulerEnableTask(LPCWSTR path);
cexport(void) TaskSchedulerDeleteTask(LPCWSTR path);
cexport(void) ReleaseTaskScheduler();

// IShellLink

typedef struct tagLNKFILEINFO
{
    LPCWSTR lnkPath;
    LPWSTR pszTarget;
    LPWSTR pszArgs;
    LPWSTR pszWorkingDir;
    WORD wHotkey;
    INT iShowCmd;
    LPWSTR pszDescription;
    LPWSTR pszIconPath;
    INT iIcon;
} LNKFILEINFO, * LPLNKFILEINFO;

cexport(void) InitializeShellLink();
cexport(void) ShellLinkCreateLnk(LNKFILEINFO lnkFileInfo);
cexport(void) ShellLinkQueryLnk(LPLNKFILEINFO lpLnkFileInfo);
cexport(void) ReleaseShellLink();