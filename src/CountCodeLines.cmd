@echo off
cd /d %~dp0
cloc . --vcs=git --include-lang="C/C++ Header,C#,C++,XAML" --by-file
pause >nul