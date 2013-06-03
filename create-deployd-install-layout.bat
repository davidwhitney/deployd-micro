C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /m:8 /p:Configuration=Release deployd-micro.sln
mkdir installed-layout
xcopy deployd\bin\release\*.* installed-layout /Y
xcopy deployd.watchman\bin\release\*.* installed-layout /Y
xcopy deployd-package\bin\release\*.* installed-layout /Y