# MultiTarget

`<MultiTarget>` is a custom property that indicates which target a project is designed to be built for / run on.

The supplied targets are used to create project references, generate solution files, enable/disable TargetFrameworks, and build nuget packages.

## Basic usage

Create a `MultiTarget.props` file in the same folder as your `.csproj` to change its MultiTarget.

By default, all available targets are enabled:
```xml
<Project>
    <PropertyGroup>
        <MultiTarget>uwp;wasdk;wpf;wasm;linuxgtk;macos;ios;android;</MultiTarget>
    </PropertyGroup>
</Project>
```

A project with this `MultiTarget.props` would only target UWP, WASM and Android:

```xml
<Project>
    <PropertyGroup>
        <MultiTarget>uwp;wasm;android</MultiTarget>
    </PropertyGroup>
</Project>
```

## ProjectReference Generation

The script `GenerateAllProjectReferences.ps1` will recursively scan the provided folders for `.csproj` files and generate a `.props` file with a MultiTarget-aware `<ProjectReference>` for each one. 

## NuGet Packages

The `<MultiTarget>` property is used to define the `TargetFrameworks` supported by that project. Projects packed into a NuGet packages will reflect this.
