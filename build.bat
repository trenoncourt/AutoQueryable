@echo off

dotnet restore

dotnet build

dotnet pack ".\src\AutoQueryable" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.AspNet" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.AspNet.Filter" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.AspNetCore.Filter" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.AspNetCore.Swagger" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.Extensions.Autofac" -c Release -o ".\bin\NuGetPackages"
dotnet pack ".\src\AutoQueryable.Extensions.DependencyInjection" -c Release -o ".\bin\NuGetPackages"

pause