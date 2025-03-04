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

!define PRODUCT_NAME "高考倒计时"
!define SETUP_FILENAME_NO_V "3.0.8"
!define PRODUCT_VERSION "${SETUP_FILENAME_NO_V}"
!define PRODUCT_TITLE "${PRODUCT_NAME} by ${PRODUCT_PUBLISHER}"
!define PRODUCT_PUBLISHER "WangHaonie"
!define PRODUCT_WEB_SITE "https://github.com/WangHaonie/PlainCEETimer"

# 旧版信息
!define OLD_PRODUCT_NAME "CEETimerCSharpWinForms"
!define OLD_PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\CEETimerCSharpWinForms"
!define OLD_INSTALL_DIR "$PROFILE\AppData\Local\CEETimerCSharpWinForms"

# 新版信息
!define NEW_PRODUCT_NAME "PlainCEETimer"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\PlainCEETimer"
!define PRODUCT_UNINST_ROOT_KEY "HKCU"

!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\nsis3-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\nsis3-uninstall.ico"
!define MUI_LICENSEPAGE_CHECKBOX

SetCompressor lzma

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "..\LICENSE"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "SimpChinese"

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "PlainCEETimer_${SETUP_FILENAME_NO_V}_x64_Setup.exe"
InstallDir "$PROFILE\AppData\Local\PlainCEETimer"
InstallDirRegKey HKCU "${PRODUCT_UNINST_KEY}" "UninstallString"

ShowInstDetails show
ShowUnInstDetails show
BrandingText "Copyright (C) 2023-2024 WangHaonie"

Section -PRE
  # 检测并卸载旧版
  ReadRegStr $0 HKCU "${OLD_PRODUCT_UNINST_KEY}" "UninstallString"
  StrCmp $0 "" no_old_version
  MessageBox MB_OK|MB_ICONINFORMATION "检测到旧版本（CEETimerCSharpWinForms），即将卸载旧版本..."
  nsExec::Exec '"taskkill" /F /IM "${OLD_PRODUCT_NAME}.exe"'
  ExecWait '$0'  ; 执行旧版卸载程序
  Sleep 1000

  # 手动清理旧版残留文件
  DeleteRegKey HKCU "${OLD_PRODUCT_UNINST_KEY}"
  RMDir /r "${OLD_INSTALL_DIR}"

no_old_version:
SectionEnd

Section -POST
  SetOverwrite on
  SetOutPath "$INSTDIR"
  nsExec::Exec '"taskkill" /F /IM "${NEW_PRODUCT_NAME}.exe"'
  
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "${PRODUCT_NAME}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\${NEW_PRODUCT_NAME}.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  
  WriteIniStr "$INSTDIR\GitHub.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  File "..\PlainCEETimer\bin\x64\Release\Newtonsoft.Json.dll"
  File "..\PlainCEETimer\bin\x64\Release\PlainCEETimer.exe"

  CreateDirectory "$SMPROGRAMS\高考倒计时"
  CreateShortCut "$SMPROGRAMS\高考倒计时\高考倒计时.lnk" "$INSTDIR\PlainCEETimer.exe"
  CreateShortCut "$DESKTOP\高考倒计时.lnk" "$INSTDIR\PlainCEETimer.exe"
  CreateShortCut "$SMPROGRAMS\高考倒计时\GitHub.lnk" "$INSTDIR\GitHub.url"
  CreateShortCut "$SMPROGRAMS\高考倒计时\卸载 高考倒计时.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section Uninstall
  nsExec::Exec '"taskkill" /F /IM "PlainCEETimer.exe"'
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegValue ${PRODUCT_UNINST_ROOT_KEY} "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" "${NEW_PRODUCT_NAME}"
  
  Delete "$INSTDIR\GitHub.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\PlainCEETimer.exe"
  Delete "$SMPROGRAMS\高考倒计时\卸载 高考倒计时.lnk"
  Delete "$SMPROGRAMS\高考倒计时\GitHub.lnk"
  Delete "$DESKTOP\高考倒计时.lnk"
  Delete "$SMPROGRAMS\高考倒计时\高考倒计时.lnk"
  RMDir "$SMPROGRAMS\高考倒计时"
  RMDir "$INSTDIR"
  SetAutoClose true
SectionEnd

Function .onInit
  !insertmacro SingleInstanceMutex
FunctionEnd

Function .onInstSuccess
  Exec "$INSTDIR\CEETimerCSharpWinForms.exe"
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