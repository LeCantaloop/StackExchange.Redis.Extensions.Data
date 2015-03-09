"%~dp0.nuget\NuGet.exe" restore
%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /p:Configuration=Debug "%~dp0StackExchange.Redis.Extensions.Data.sln"
%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /p:Configuration=Release "%~dp0StackExchange.Redis.Extensions.Data.sln"