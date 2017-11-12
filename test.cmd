@echo off

for /d %%a in (".\test\*") do (
    dotnet test %%a/%%~nxa.csproj
)
