@echo off

dotnet restore

dotnet build

dotnet pack ".\src\AutoQueryable" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.Core" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.Providers.OData" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.AspNet.Filter" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.AspNetCore.Filter" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.Nancy.Filter" -c Release -o ".\bin\NuGetPackages"

pause