# MultiTarget

`<MultiTarget>` is a custom property that indicates which target a component is designed to be built for / run on.

The supplied targets are used to create project references, generate solution files, enable/disable TargetFrameworks, and build nuget packages.

## Basic usage

Create a `MultiTarget.props` file in the root of your source project to change the platform targets for your component. This will be picked up automatically by your sample project, unless it has a `MultiTarget.props` of its own defined.

By default, all available targets are enabled:
```xml
<Project>
    <PropertyGroup>
        <MultiTarget>uwp;wasdk;wpf;wasm;linuxgtk;macos;ios;android;</MultiTarget>
    </PropertyGroup>
</Project>
```

For example, to only target UWP, WASM and Android:

```xml
<Project>
    <PropertyGroup>
        <MultiTarget>uwp;wasm;android</MultiTarget>
    </PropertyGroup>
</Project>
```


## ProjectReference Generation

The script `GenerateAllProjectReferences.ps1` will scan for toolkit components and generate `.props` files for each.

## NuGet Packages

The `<MultiTarget>` property is used to define the `TargetFrameworks` supported by that project. Projects packed into a NuGet packages will reflect this.
