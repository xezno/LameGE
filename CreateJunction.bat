@echo off

rem This is done because it's a lot more useful to have a junction to a separate, 
rem permanent content folder in the repo's root directory than to keep copying 
rem the folder every time the project is built, and it's arguably better 
rem maintenance-wise than hard-coding in a relative path to the content folder 
rem itself. 

rem The game folder
SET gameProject=ExampleGame

rem The build architecture
SET arch=x64

rem The solution configuration
SET config=Debug

rem The target .NET version
SET netVersion=net5.0-windows7.0

rem Create a junction from @/Content to

echo Creating link...
mklink /j .\Source\%gameProject%\bin\%arch%\%config%\%netVersion%\Content .\Content

echo Done.