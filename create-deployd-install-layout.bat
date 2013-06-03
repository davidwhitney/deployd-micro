C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /m:8 /p:Configuration=Release deployd-micro.sln
mkdir clientbin
xcopy deployd\bin\release\*.* clientbin /Y
xcopy deployd.watchman\bin\release\*.* clientbin /Y

mkdir serverbin
xcopy deployd-package\bin\release\*.* serverbin /Y
