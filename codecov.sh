dotnet build

cd tools/code-coverage

dotnet restore

# Instrument assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --workdir ../../ --assemblies test/**/bin/**/*.dll --sources src/**/*.cs

# Reset hits count in case minicover was run for this project
dotnet minicover reset

cd ../../

for project in test/**/*.csproj; do dotnet test $project --no-build; done

cd tools/code-coverage

dotnet minicover uninstrument --workdir ../../

dotnet minicover opencoverreport --workdir ../../ --output coverage.xml

cd ../..

curl -s https://codecov.io/bash | bash -s - -t ac12fe3c-28ae-4b9c-b65c-4d3879e7e942 -f "!*.csproj*"
