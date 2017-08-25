@echo off
rem ------------------------------------------------------------------------------
rem 
rem  (C) Brüel & Kjær Sound And Vibration A/S
rem 
rem  Description  : Publishes 
rem  
rem ------------------------------------------------------------------------------

echo Press a key to publish the SystemInfo component, or Ctrl-C to cancel.
pause > NUL:

rem Set default directory to location of this batch file
cd %~d0%~p0

set DirFile="..\Publish"

if not exist %DirFile% mkdir %DirFile%

call :xcopym /y /r /f LeastMeanSquare\bin\Release\LeastMeanSquare.dll %DirFile%
call :xcopym /y /r /f LeastMeanSquare\bin\Release\LeastMeanSquare.pdb %DirFile%
call :xcopym /y /r /f LeastMeanSquare-TestIir\bin\Release\LeastMeanSquare-TestIir.exe %DirFile%
call :xcopym /y /r /f LeastMeanSquare-TestIir\bin\Release\LeastMeanSquare-TestIir.pdb %DirFile%

goto :eof

:xcopym 
  xcopy %1 %2 %3 %4 %5
  IF ErrorLevel 1 GOTO :Err
  goto :eof   
        
:Err     
   pause

LeastMeanSquare-TestIir