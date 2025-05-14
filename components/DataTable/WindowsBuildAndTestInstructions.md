# Windows Build and Test Instructions for DataTable Component

## Overview
This document provides instructions to build and test the DataTable component on a Windows environment. The project targets .NET 5.0 with Windows SDK dependencies, which require building on Windows with the appropriate SDKs installed.

## Prerequisites
- Windows 10 or later
- Visual Studio 2019 or later with:
  - .NET 5.0 SDK
  - Universal Windows Platform development workload
  - Windows 10 SDK (version 19041 or later)
- NuGet package manager

## Building the Project
1. Open the solution or project in Visual Studio.
2. Restore NuGet packages.
3. Build the solution targeting `net5.0-windows10.0.19041.0`.

Alternatively, use the command line:
```bash
dotnet restore components/DataTable/tests/DataTable.Tests.csproj
dotnet build components/DataTable/tests/DataTable.Tests.csproj
```

## Running Tests
Run tests using Visual Studio Test Explorer or via command line:
```bash
dotnet test components/DataTable/tests/DataTable.Tests.csproj --logger "console;verbosity=detailed"
```

## Testing Plan
Perform thorough testing covering the following areas:

- Core DataTable functionalities:
  - Sorting
  - Filtering
  - Pagination
- UI rendering and interaction:
  - Column resizing
  - Selection and multi-selection
- Performance:
  - Virtualization
  - Caching mechanisms
- Edge cases and error handling:
  - Invalid inputs
  - Boundary conditions

## Notes
- Building and testing on non-Windows platforms is not supported due to Windows SDK dependencies.
- Ensure all Windows SDK components are installed to avoid build errors related to Windows Metadata components.

For any issues or further assistance, please reach out to the development team.
