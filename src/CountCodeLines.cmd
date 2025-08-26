@echo off
cd /d %~dp0
cloc . --include-lang=C#,C++ --by-file
pause >nul