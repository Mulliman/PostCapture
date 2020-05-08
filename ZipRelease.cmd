echo "Have you run a release build on PostCapture.CaptureOne?"

pause

cd ".\PostCapture.CaptureOne\bin\Release"

rmdir /Q /S Standalone

powershell Compress-Archive ./* "..\..\..\docs\assets\releases\PostCapture.CaptureOne.zip" -force

mkdir Standalone
xcopy ".\Processes" ".\Standalone\Processes" /e /i /h /Y
copy ".\PostCapture.Process.exe" ".\Standalone\PostCapture.Process.exe"
copy ".\PostCapture.Studio.exe" ".\Standalone\PostCapture.Studio.exe"

cd Standalone
powershell Compress-Archive ./* "..\..\..\..\docs\assets\releases\PostCapture.Standalone.zip" -force
cd..

cd "..\..\..\docs\assets\releases"

call del PostCapture.CaptureOne.coplugin
call RENAME PostCapture.CaptureOne.zip PostCapture.CaptureOne.coplugin

pause