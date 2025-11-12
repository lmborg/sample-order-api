Param(
  [string]$RootDir = "OrderApi"
)

$ErrorActionPreference = "Stop"

Write-Host ">> Creating solution folder: $RootDir"
New-Item -ItemType Directory -Force -Path $RootDir | Out-Null
Set-Location $RootDir

Write-Host ">> Creating solution and projects"
dotnet new sln -n OrderApi

dotnet new web -n Api
dotnet new classlib -n Application
dotnet new classlib -n Domain
dotnet new classlib -n Infrastructure

dotnet new xunit -n Application.Tests
dotnet new xunit -n Api.IntegrationTests

Write-Host ">> Arranging folders (src/ and tests/)"
New-Item -ItemType Directory -Force -Path src, tests | Out-Null
Move-Item -Force Api src\Api
Move-Item -Force Application src\Application
Move-Item -Force Domain src\Domain
Move-Item -Force Infrastructure src\Infrastructure
Move-Item -Force Application.Tests tests\Application.Tests
Move-Item -Force Api.IntegrationTests tests\Api.IntegrationTests

Write-Host ">> Adding projects to solution"
dotnet sln OrderApi.sln add src\Api\Api.csproj
dotnet sln OrderApi.sln add src\Application\Application.csproj
dotnet sln OrderApi.sln add src\Domain\Domain.csproj
dotnet sln OrderApi.sln add src\Infrastructure\Infrastructure.csproj
dotnet sln OrderApi.sln add tests\Application.Tests\Application.Tests.csproj
dotnet sln OrderApi.sln add tests\Api.IntegrationTests\Api.IntegrationTests.csproj

Write-Host ">> Adding project references"
dotnet add src\Api\Api.csproj reference src\Application\Application.csproj
dotnet add src\Api\Api.csproj reference src\Infrastructure\Infrastructure.csproj
dotnet add src\Application\Application.csproj reference src\Domain\Domain.csproj
dotnet add src\Infrastructure\Infrastructure.csproj reference src\Domain\Domain.csproj
dotnet add src\Infrastructure\Infrastructure.csproj reference src\Application\Application.csproj
dotnet add tests\Application.Tests\Application.Tests.csproj reference src\Application\Application.csproj
dotnet add tests\Api.IntegrationTests\Api.IntegrationTests.csproj reference src\Api\Api.csproj
dotnet add tests\Api.IntegrationTests\Api.IntegrationTests.csproj reference src\Infrastructure\Infrastructure.csproj

Write-Host ">> Installing NuGet packages"
# Api
dotnet add src\Api\Api.csproj package Microsoft.AspNetCore.OpenApi --version 8.0.8
dotnet add src\Api\Api.csproj package Swashbuckle.AspNetCore --version 6.6.2
dotnet add src\Api\Api.csproj package FluentValidation.AspNetCore --version 11.3.0

# Application
dotnet add src\Application\Application.csproj package FluentValidation --version 11.9.0

# Infrastructure
dotnet add src\Infrastructure\Infrastructure.csproj package Microsoft.EntityFrameworkCore --version 8.0.8
dotnet add src\Infrastructure\Infrastructure.csproj package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.8
dotnet add src\Infrastructure\Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.8

# Integration Tests
dotnet add tests\Api.IntegrationTests\Api.IntegrationTests.csproj package Microsoft.AspNetCore.Mvc.Testing --version 8.0.8
dotnet add tests\Api.IntegrationTests\Api.IntegrationTests.csproj package Microsoft.Data.Sqlite --version 8.0.8
dotnet add tests\Api.IntegrationTests\Api.IntegrationTests.csproj package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.8
dotnet add tests\Api.IntegrationTests\Api.IntegrationTests.csproj package FluentAssertions --version 6.12.0

# Unit Tests
dotnet add tests\Application.Tests\Application.Tests.csproj package FluentAssertions --version 6.12.0

Write-Host ">> Restore, build, and test"
dotnet restore OrderApi.sln
dotnet build OrderApi.sln -c Debug --no-restore
dotnet test OrderApi.sln --no-build

@"
----------------------------------------------------------
DONE.

Next steps:
1) Author code files (DbContext, entities, Program.cs endpoints, handlers).
2) Create EF migrations once your model is defined:

   cd src\Api
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate --project ..\Infrastructure --startup-project .
   dotnet ef database update --project ..\Infrastructure --startup-project .
   cd ..\..

3) Run the API:
   dotnet run --project src\Api\Api.csproj
   $env:ASPNETCORE_ENVIRONMENT="Demo"; dotnet run --project src\Api\Api.csproj  # demo seed profile (if implemented)
----------------------------------------------------------
"@ | Write-Host
