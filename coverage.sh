dotnet restore

cd tools/code-coverage

# Instrument assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --workdir ../../ --assemblies test/**/bin/**/*.dll --sources src/**/*.cs

# Reset hits count in case minicover was run for this project
dotnet minicover reset

cd ../../

for project in test/**/*.csproj; do dotnet test $project; done

cd tools/code-coverage

dotnet restore --force --no-cache

# Uninstrument assemblies, it's important if you're going to publish or deploy build outputs
dotnet minicover uninstrument --workdir ../../

# Create html reports inside folder coverage-html
dotnet minicover htmlreport --workdir ../../ --threshold 90

# Print console report
# This command returns failure if the coverage is lower than the threshold
dotnet minicover report --workdir ../../ --threshold 90

cd ../..
