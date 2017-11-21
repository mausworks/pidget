@echo off

:parse
if "%~1"=="" goto endparse
if "%~1"=="--no-report" set noreport=1
if "%~1"=="--no-test" set notest=1
shift
goto parse
:endparse

set project=Pidget.AspNet

set packages="..\..\tools\code-coverage\.packages\"
set dotnet="C:\Program Files\dotnet\dotnet.exe"
set opencover="%packages%\OpenCover\4.6.519\tools\OpenCover.Console.exe"
set reportgenerator="%packages%\ReportGenerator\3.0.2\tools\ReportGenerator.exe"
set coveragefile=.cov\coverage.xml
set coveragedir=.cov\

if not exist %packages% (
  dotnet restore ..\..\tools\code-coverage\code-coverage.csproj --packages %packages%
)

if "%notest%"=="" %opencover%^
  -target:%dotnet%^
  -targetargs:"test /p:Coverage=true"^
  -filter:"+[%project%*]* -[*.Test]* -[Xunit.*]* -[Moq.*]*"^
  -output:%coveragefile%^
  -register:user^
  -oldStyle^
  -skipautoprops^
  -hideskipped:All

if "%noreport%"=="" %reportgenerator%^
  -targetdir:%coveragedir%^
  -reporttypes:Html;Badges^
  -reports:%coveragefile%^
  -verbosity:Error & start "Report" "%coveragedir%index.htm"
