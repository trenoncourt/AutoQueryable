@echo off

dotnet restore

dotnet build

dotnet pack ".\src\AutoQueryable" -c Release -o ".\bin\NuGetPackages"

pause