call build.bat
if %errorlevel% neq 0 exit /b %errorlevel%

.nuget\nuget pack "deployd\deployd.csproj" -Properties Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%

.nuget\nuget pack "deployd-package\deployd-package.csproj" -Properties Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%