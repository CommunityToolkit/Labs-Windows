# Windows Community Toolkit Labs - Experiment project template

This directory includes the template for creating new labs experiments.

## dotnet new

To use the template to create a new experiment, open a command prompt in the root directory and run the following commands:

```ascii
dotnet new --install .\template\lab\

cd labs

dotnet new labexp -n MyExperimentNameHere
```

This creates a new experiment called "MyExperimentNameHere".
You can now open `./labs/MyExperimentNameHere/MyExperimentNameHere.sln` and start your experiment.

### Inside the generated solution

The solution includes many projects but is not as complicated as you might first think.

#### Things you can ignore

The `Labs Dependencies` folder can be ignored. The projects it contains are referenced in other projects and you should not change anything here.

The `Platforms` folder contains projects that host your sample(s) on different platforms. Run any of these projects to see your sample running inside a UWP, WASM, or WinAppSdk/WinUI3 app. Again, you shouldn't modify anything in these projects.

The `Tests` folder contains the tests. The shared project `ProjectTemplate.Tests` is where you will write tests. You can use Test Explorer to run all your tests for both UWP and WinAppSdk. Otherwise, you can ignore the projects that end in `.UnitTests.Uwp` and `.UnitTests.WinAppSdk`. More on that below.

#### Where to add your code

The main code of your experiment will go in the project `CommunityToolkit.Labs.WinUI.MyExperimentNameHere`. When an experiment is merged into Labs, this code will be bundled automatically in a NuGet package and pushed to the Labs DevOps feed. This will let others try out your experiment and provide feedback to further your experiment.
You will find an empty class in `MyExperimentNameHere.cs` that you can use as your starting point.

The project `MyExperimentNameHere.Sample`is where you can put code that will allow you to demonstrate and exercise the experiment. In this project you'll find a sample page that includes an example of how to use settings and properties that can be controlled within the sample app. This folder also contains a markdown file that contains the documentation for the experiment and how to use it.

Tests for the code in the experiment go in the `Tests/MyExperimentNameHere.Tests` project. This is a shared project that is referenced by the other test projects. This makes it easy to check that the experiment's code works in more than one place. There's an example test inside the `ExampleMyExperimentNameHereTestClass.cs` file

### If things go wrong

Hopefully, you'll have no problems creating your experiment. However, here are details of how to address some of the things that might go wrong.

#### Missing components or workloads

Visual Studio will prompt if any required components or workloads are missing. Make sure all these are installed before continuing.

#### Creating an experiment in the wrong place

The generated solution and some of the projects it contains rely on relative paths that assume the experiment is created in the `labs` directory. If the experiment is created elsewhere, the error message "One or more projects in the solution were not loaded correctly." will be shown when opening the solution. Deleting the incorrect solution and recreating in the correct location is the most reliable way to address this.
