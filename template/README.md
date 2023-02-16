# Windows Community Toolkit Labs - Experiment project template

This directory includes the template for creating new labs experiments.

## dotnet new

To use the template to create a new experiment, open a command prompt in the **root directory** and run the following commands:

```ascii
dotnet new --install .\template\ProjectTemplate\

cd components

dotnet new labexp -n MyExperimentNameHere
```

This creates a new experiment called "MyExperimentNameHere".
You can now open `./components/MyExperimentNameHere/MyExperimentNameHere.sln` and start your experiment.

### Inside the generated solution

The solution includes many projects but is not as complicated as you might first think.

#### Things you can ignore

The `Labs Dependencies` folder can be ignored. The projects it contains are referenced in other projects and you should not change anything here.

The `Platforms` folder contains projects that host your sample(s) on different platforms. Run any of these projects to see your sample running inside a UWP, WASM, or WinAppSdk/WinUI3 app. Again, you shouldn't modify anything in these projects.

The `Tests` folder contains projects used to run the tests on different platforms UWP or WinAppSDK. Again, you shouldn't modify anything in these projects. Details of where to create tests for the code in the experiment are below.

#### Where to add your code

The main code of your experiment will go in the project `CommunityToolkit.Labs.WinUI.MyExperimentNameHere`. When an experiment is merged into Labs, this code will be bundled automatically in a NuGet package and pushed to the Labs DevOps feed. This will let others try out your experiment and provide feedback to further your experiment.
You will find an empty class in `MyExperimentNameHere.cs` that you can use as your starting point or one of the templated variants. You can find more info in the `MyExperimentNameHere.md` file in the sample project.

The project `MyExperimentNameHere.Sample`is where you can put code that will allow you to demonstrate and exercise the experiment. In this project you'll find a sample page that includes an example of how to use settings and properties that can be controlled within the sample app. This folder also contains a **markdown** file that contains the documentation for the experiment and how to use it.

You can add additional markdown files as desired, each one will create a new page in the aggregated sample app. Generally you'll want to have one page per component your experiment is creating. A page can have multiple samples embedded in it to showcase different scenarios. Try to keep examples light-weight and showcase singular elements of a component in different ways. If you have a complex end-to-end example, consider giving it its own page to break-down how the example works and showcase the singular complex sample.

Tests for the code in the experiment go in the `MyExperimentNameHere.Tests` project. This is a shared project that is referenced by the other test projects. This makes it easy to check that the experiment's code works in more than one place. There's an example test inside the `ExampleMyExperimentNameHereTestClass.cs` file

### If things go wrong

Hopefully, you'll have no problems creating your experiment. However, here are details of how to address some of the things that might go wrong.

#### Missing components or workloads

Visual Studio will prompt if any required components or workloads are missing. Make sure all these are installed before continuing.

#### Creating an experiment in the wrong place

The generated solution and some of the projects it contains rely on relative paths that assume the experiment is created in the `components` directory. If the experiment is created elsewhere, the error message "One or more projects in the solution were not loaded correctly." will be shown when opening the solution. Deleting the incorrect solution and recreating in the correct location is the most reliable way to address this.

#### Long Paths

Labs requires long paths to be enabled. You'll want to modify the `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem\LongPathsEnabled` registry key to be a `1` and well as run the following command from an _elevated_ command prompt:

```dos
> git config --system core.longpaths true
```

This may require a reboot.

#### Updating NuGet Package

Packages are manually versioned right now, [see this issue here](https://github.com/CommunityToolkit/Labs-Windows/issues/133). In order to update your NuGet package, update the `<Version>` tag in your library's `CommunityToolkit.Labs.WinUI.MyExperimentNameHere.csproj` file.

#### WebAssembly Sample Project

Sometimes it can be tricky to run the WASM sample project head under Visual Studio with the default IIS run configuration. Use the drop-down within the run button itself to instead select the `ExperimentName.Wasm` configuration over `IIS Express`.

#### Uno Templates

It can be helpful to install the [Uno Platform Templates](https://marketplace.visualstudio.com/items?itemName=unoplatform.uno-platform-addin-2022) in order to use their templates when creating new Pages in Sample and Test projects.

For instance, you never want to add a 'Content Page' or use the other Xamarin based templates, use UWP, WinUI 3/WinAppSDK, or (ideally) the Uno based item templates when adding new XAML pages/dictionaries to projects.

#### Sample Page

If when adding a new page to the sample project you run into errors, try resetting the csproj file, it's setup to automatically include all XAML files.

#### Windows.UI.Xaml.Controls (WUXC) vs. Microsoft.UI.Xaml.Controls (MUXC) w/ WinUI 2 vs. WinUI 3

If you are referring to a control from the system like `TextBlock`, the build system will automatically pick the system one on UWP or the WinUI 3 one in the Windows App SDK.

However, if you need to refer to a component that was part of the WinUI 2 library like `NavigationView` or `ItemsRepeater`, then preface your C# code type with `MUXC.` to clarify you are referring to the WinUI 2 or WinUI 3 versions of the components. In XAML this is done automatically as the namespace is the same (and it is effectively ignored in the WinUI 3 case).

#### DEP0600 error (WinAppSDK)

Your experiment name is probably too long (max 32 characters), use a shorter more concise name when generating your template. See [this issue here](https://github.com/microsoft/microsoft-ui-xaml/issues/7059).
