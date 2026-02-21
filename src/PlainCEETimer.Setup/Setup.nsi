!define SETUP_FILENAME_NO_V "0.0.0"

RequestExecutionLevel user
ManifestDPIAware true
SetFont "Segoe UI" 9

!define INSTALLERMUTEXNAME "90d56f33-3a68-4d48-8890-f06b488f4c04-9505e4f0e0cabb73"
 
!ifndef NSIS_PTR_SIZE & SYSTYPE_PTR
!define SYSTYPE_PTR i
!else
!define /ifndef SYSTYPE_PTR p
!endif

!macro ActivateOtherInstance
StrCpy $3 ""
loop:
	FindWindow $3 "#32770" "" "" $3
	StrCmp 0 $3 windownotfound
	StrLen $0 "$(^UninstallCaption)"
	IntOp $0 $0 + 1
	System::Call 'USER32::GetWindowText(${SYSTYPE_PTR}r3, t.r0, ir0)'
	StrCmp $0 "$(^UninstallCaption)" windowfound ""
	StrLen $0 "$(^SetupCaption)"
	IntOp $0 $0 + 1
	System::Call 'USER32::GetWindowText(${SYSTYPE_PTR}r3, t.r0, ir0)'
	StrCmp $0 "$(^SetupCaption)" windowfound loop
windowfound:
	SendMessage $3 0x112 0xF120 0 /TIMEOUT=2000
	System::Call "USER32::SetForegroundWindow(${SYSTYPE_PTR}r3)"
windownotfound:
!macroend
 
!macro SingleInstanceMutex
!ifndef INSTALLERMUTEXNAME
!error "Must define INSTALLERMUTEXNAME"
!endif
System::Call 'KERNEL32::CreateMutex(${SYSTYPE_PTR}0, i1, t"${INSTALLERMUTEXNAME}")?e'
Pop $0
IntCmpU $0 183 "" launch launch
	!insertmacro ActivateOtherInstance
	Abort
launch:
!macroend

!include "MUI2.nsh"
!include "FileFunc.nsh"
!include "WinMessages.nsh"
!define PRODUCT_NAME "高考倒计时"
!define PRODUCT_VERSION "${SETUP_FILENAME_NO_V}"
!define PRODUCT_TITLE "${PRODUCT_NAME} by ${PRODUCT_PUBLISHER}"
!define PRODUCT_PUBLISHER "WangHaonie"
!define PRODUCT_WEB_SITE "https://github.com/WangHaonie/PlainCEETimer"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\CEETimerCSharpWinForms"
!define PRODUCT_UNINST_ROOT_KEY "HKCU"
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\classic-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\classic-uninstall.ico"
!define MUI_LICENSEPAGE_CHECKBOX

SetCompressor lzma

!define MUI_PAGE_CUSTOMFUNCTION_PRE "AutoSkip"
!insertmacro MUI_PAGE_DIRECTORY
!define MUI_PAGE_CUSTOMFUNCTION_PRE "AutoSkip"
!insertmacro MUI_PAGE_LICENSE ".\LicenseLink"
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "SimpChinese"

Var /GLOBAL IsSkip

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "PlainCEETimer_${SETUP_FILENAME_NO_V}_x64_Setup.exe"
InstallDir "$PROFILE\AppData\Local\CEETimerCSharpWinForms"
InstallDirRegKey HKCU "${PRODUCT_UNINST_KEY}" "UninstallString"
ShowInstDetails show
ShowUnInstDetails show
BrandingText "Copyright (C) 2023-2025 WangHaonie"

Section -POST
  SetOverwrite on
  SetOutPath "$INSTDIR"
  System::Call 'USER32::SetWindowPos(i $HWNDPARENT, i -1, i 0, i 0, i 0, i 0, i 0x0001|0x0002)'
  nsExec::ExecToLog '"taskkill" /F /IM "CEETimerCSharpWinForms.exe"'
  nsExec::ExecToLog '"taskkill" /F /IM "PlainCEETimer.exe"'
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "Software\Microsoft\Windows\CurrentVersion\Uninstall\CEETimerCSharpWinForms"
  Delete "$INSTDIR\GitHub.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\PlainCEETimer.exe"
  Delete "$INSTDIR\PlainCEETimer.com"
  Delete "$INSTDIR\UnhandledException.txt"
  Delete "$INSTDIR\CEETimerCSharpWinForms.exe"
  Delete "$INSTDIR\CEETimerCSharpWinForms.cfg"
  Delete "$INSTDIR\CEETimerCSharpWinForms.config"
  Delete "$INSTDIR\PlainCEETimer.exe.config"
  Delete "$INSTDIR\PlainCEETimer.Natives.dll"
  Delete "$SMPROGRAMS\高考倒计时\卸载 高考倒计时.lnk"
  Delete "$SMPROGRAMS\高考倒计时\GitHub.lnk"
  Delete "$SMPROGRAMS\高考倒计时\高考倒计时.lnk"
  Delete "$SMPROGRAMS\高考倒计时 by WangHaonie\卸载 高考倒计时.lnk"
  Delete "$SMPROGRAMS\高考倒计时 by WangHaonie\GitHub.lnk"
  Delete "$SMPROGRAMS\高考倒计时 by WangHaonie\高考倒计时.lnk"
  Delete "$DESKTOP\高考倒计时.lnk"
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "${PRODUCT_TITLE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\PlainCEETimer.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  WriteIniStr "$INSTDIR\GitHub.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  File "..\.output\PlainCEETimer.exe"
  File "..\.output\PlainCEETimer.com"
  File "..\.output\Newtonsoft.Json.dll"
  File "..\.output\PlainCEETimer.Natives.dll"
  CreateDirectory "$SMPROGRAMS\高考倒计时 by WangHaonie"
  CreateShortCut "$SMPROGRAMS\高考倒计时 by WangHaonie\高考倒计时.lnk" "$INSTDIR\PlainCEETimer.exe"
  CreateShortCut "$DESKTOP\高考倒计时.lnk" "$INSTDIR\PlainCEETimer.exe"
  CreateShortCut "$SMPROGRAMS\高考倒计时 by WangHaonie\GitHub.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\高考倒计时 by WangHaonie\卸载 高考倒计时.lnk" "$INSTDIR\uninst.exe"
  DetailPrint "正在优化程序集以提高运行速度，请稍候..."
  MessageBox MB_OK|MB_ICONINFORMATION "即将开始优化程序集，请按提示进行。$\n$\n>>现在点击 确定 继续"
  ExecWait '"$INSTDIR\PlainCEETimer.exe" /op /auto'
  SetAutoClose true
SectionEnd

Section Uninstall
  nsExec::Exec '"taskkill" /F /IM "CEETimerCSharpWinForms.exe"'
  nsExec::Exec '"taskkill" /F /IM "PlainCEETimer.exe"'
  ExecWait '"$INSTDIR\PlainCEETimer.exe" /uninst'
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "Software\Microsoft\Windows\CurrentVersion\Uninstall\CEETimerCSharpWinForms"
  Delete "$INSTDIR\GitHub.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\PlainCEETimer.exe"
  Delete "$INSTDIR\PlainCEETimer.config"
  Delete "$INSTDIR\CEETimerCSharpWinForms.exe"
  Delete "$INSTDIR\CEETimerCSharpWinForms.cfg"
  Delete "$INSTDIR\CEETimerCSharpWinForms.config"
  Delete "$INSTDIR\PlainCEETimer.Natives.dll"
  Delete "$SMPROGRAMS\高考倒计时\卸载 高考倒计时.lnk"
  Delete "$SMPROGRAMS\高考倒计时\GitHub.lnk"
  Delete "$SMPROGRAMS\高考倒计时\高考倒计时.lnk"
  Delete "$SMPROGRAMS\高考倒计时 by WangHaonie\卸载 高考倒计时.lnk"
  Delete "$SMPROGRAMS\高考倒计时 by WangHaonie\GitHub.lnk"
  Delete "$SMPROGRAMS\高考倒计时 by WangHaonie\高考倒计时.lnk"
  Delete "$DESKTOP\高考倒计时.lnk"
  RMDir "$SMPROGRAMS\高考倒计时"
  RMDir "$INSTDIR"
  SetAutoClose true
SectionEnd

Function AutoSkip
  StrCmp $IsSkip "1" +2
    Abort
FunctionEnd

Function .onInit
  !insertmacro SingleInstanceMutex
  StrCpy $IsSkip "1"
  ${GetParameters} $R0
  ClearErrors
  ${GetOptions} '$R0' "/Skip" $R1
  IfErrors +2
    StrCpy $IsSkip "0"
FunctionEnd

Function .onInstSuccess
  Exec "$INSTDIR\PlainCEETimer.exe"
FunctionEnd
 
Function un.onInit
  !insertmacro SingleInstanceMutex
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "确认卸载 ${PRODUCT_TITLE}？" IDYES +2
  Abort
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "${PRODUCT_TITLE} 已成功从您的计算机中移除。感谢您的使用！"
FunctionEnd