; BasicBrowser Installer NSIS Script
; get NSIS at http://tenet.dl.sourceforge.net/project/nsis/NSIS%202/2.46/nsis-2.46-setup.exe
; As a program that all Power PC users should have, Notepad ++ is recommended to edit this file

;AddBrandingImage top 20
;Icon youtube_withLink.ico
Caption "BasicBrowser(Hybrid) Installer"
Name "BasicBrowser(Hybrid)"
AutoCloseWindow true
ShowInstDetails show

LicenseBkColor /windows
LicenseData "LICENSE.md"
LicenseForceSelection checkbox "I have read and understand this notice"
LicenseText "Please read the notice below before installing BasicBrowser. If you understand the notice, click the checkbox below and click Next."

InstallDir $PROGRAMFILES\DeavmiOSS\BasicBrowser(Hybrid)

OutFile "bin\Release\BasicBrowser-Installer.exe"

; Pages

Page license
Page components
Page directory
Page instfiles
UninstPage uninstConfirm
UninstPage instfiles

; Sections

Section "Executable & Uninstaller"
  SectionIn RO
  SetOutPath $INSTDIR
  File "bin\Release\BasicBrowser.exe"
  File "bin\Release\Skybound.Gecko.dll"
  WriteUninstaller "BasicBrowser(Hybrid)-Uninst.exe"
SectionEnd

Section "Gecko DLLs (If you want to open a GeckoFX tab)"
  ; DLLs
  File "bin\Release\*.dll"
  ; Sub-Folders
  File /r "bin\Release\chrome"
  File /r "bin\Release\components"
  File /r "bin\Release\defaults"
  File /r "bin\Release\dictionaries"
  File /r "bin\Release\greprefs"
  File /r "bin\Release\modules"
  File /r "bin\Release\plugins"
  File /r "bin\Release\res"
SectionEnd

Section "Start Menu Shortcuts"
  CreateDirectory "$SMPROGRAMS\DeavmiOSS"
  CreateShortCut "$SMPROGRAMS\DeavmiOSS\BasicBrowser(Hybrid).lnk" "$INSTDIR\BasicBrowser.exe" "" "$INSTDIR\BasicBrowser.exe" "" "" "" "BasicBrowser(Hybrid)"
  CreateShortCut "$SMPROGRAMS\DeavmiOSS\Uninstall BasicBrowser(Hybrid).lnk" "$INSTDIR\BasicBrowser(Hybrid)-Uninst.exe" "" "" "" "" "" "Uninstall BasicBrowser(Hybrid)"
  ;Syntax for CreateShortCut: link.lnk target.file [parameters [icon.file [icon_index_number [start_options [keyboard_shortcut [description]]]]]]
SectionEnd

Section "Desktop Shortcut"
  CreateShortCut "$DESKTOP\BasicBrowser(Hybrid).lnk" "$INSTDIR\BasicBrowser.exe" "" "$INSTDIR\BasicBrowser.exe" "" "" "" "BasicBrowser(Hybrid)"
SectionEnd

Section "Quick Launch Shortcut"
  CreateShortCut "$QUICKLAUNCH\BasicBrowser(Hybrid).lnk" "$INSTDIR\BasicBrowser.exe" "" "$INSTDIR\BasicBrowser.exe" "" "" "" "BasicBrowser(Hybrid)"
SectionEnd

SubSection "Open in BasicBrowser(Hybrid)"
  Section "Add to Open With menu"
    WriteRegStr HKCR "Applications\BasicBrowser(Hybrid).exe\shell\open\command" "" "$\"$INSTDIR\BasicBrowser.exe$\" $\"%1$\""
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.html\OpenWithList" "l" "BasicBrowser(Hybrid).exe"
  SectionEnd
  
  Section "Set as default program"
    WriteRegStr HKCR "Applications\BasicBrowser(Hybrid).exe\shell\open\command" "" "$\"$INSTDIR\BasicBrowser.exe$\" $\"%1$\""
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.html\UserChoice" "Progid" "Applications\BasicBrowser(Hybrid).exe"
  SectionEnd
  
SubSectionEnd

;Section "More apps from DeavmiOSS"
; this should have sub options for available apps, that are downloaded
;SectionEnd

; Uninstaller

Section "Uninstall"
  Delete $INSTDIR\BasicBrowser(Hybrid)-Uninst.exe   ; Remove Application Files
  Delete $INSTDIR\BasicBrowser.exe
  RMDir $INSTDIR
  
  Delete $SMPROGRAMS\DeavmiOSS\BasicBrowser(Hybrid).lnk   ; Remove Start Menu Shortcuts & Folder
  Delete "$SMPROGRAMS\DeavmiOSS\Uninstall BasicBrowser(Hybrid).lnk"
  RMDir $SMPROGRAMS\DeavmiOSS
  
  Delete $DESKTOP\BasicBrowser(Hybrid).lnk   ; Remove Desktop Shortcut
  Delete $QUICKLAUNCH\BasicBrowser(Hybrid).lnk   ; Remove Quick Launch Shortcut
  
  DeleteRegKey HKCR Applications\BasicBrowser(Hybrid).exe ; Remove open with association
  DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.html\OpenWithList" "k"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.html\UserChoice" "Progid" "Applications\chrome.exe"
SectionEnd

; Functions

Function .onInit
  MessageBox MB_YESNO "This will install BasicBrowser(Hybrid). Do you wish to continue?" IDYES gogogo
    Abort
  gogogo:
  ;SetBrandingImage "[/RESIZETOFIT] youtube_withLink.ico"
  SetShellVarContext all
  SetAutoClose true
FunctionEnd

Function .onInstSuccess
    MessageBox MB_YESNO "Install Succeeded! Open ReadMe?" IDNO NoReadme
      ExecShell "open" "https://github.com/Walkman100/BasicBrowser/blob/hybrid/README.md#basicbrowser-"
    NoReadme:
FunctionEnd

; Uninstaller

Function un.onInit
    MessageBox MB_YESNO "This will uninstall BasicBrowser(Hybrid). Continue?" IDYES NoAbort
      Abort ; causes uninstaller to quit.
    NoAbort:
    SetShellVarContext all
    SetAutoClose true
FunctionEnd

Function un.onUninstFailed
    MessageBox MB_OK "Uninstall Cancelled"
FunctionEnd

Function un.onUninstSuccess
    MessageBox MB_OK "Uninstall Completed"
FunctionEnd