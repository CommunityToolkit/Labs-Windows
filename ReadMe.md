
# 🧪 Community Toolkit Labs for Windows 🧪

Welcome to the home of Toolkit Labs experiments for Windows (built on top of WinUI 2, WinUI 3, and [Uno Platform](https://platform.uno)). Find out more about Toolkit Labs in our [Wiki here](https://aka.ms/toolkit/wiki/labs). It includes more about our motivations for having this space as well as how to setup the NuGet feed required to easily use experiments found in this repo.

This is the starting place for all new features to make it into the [Windows Community Toolkit](https://aka.ms/wct). It is a useful prototyping space as well as a space to work collaboratively on polishing a feature. This allows a final PR into the main Toolkit repo to go as smoothly as possible once a feature is ready to go.

## Sample App

You can build the main Sample App solution to see all the experiments currently available in this repository. If there's a specific experiment you're interested in, you can navigate to its directory and open up it's individual solution to see samples specific to that feature. You can also read the next section to find out how to grab a pre-built NuGet package for each feature available here.

## Using an Experiment

If you skipped the [Wiki page on Toolkit Labs](https://aka.ms/toolkit/wiki/labs), then you can go to the [wiki page on Preview Packages](https://aka.ms/toolkit/wiki/previewpackages) and find out how to add our NuGet Feed to Visual Studio:

```
https://pkgs.dev.azure.com/dotnet/CommunityToolkit/_packaging/CommunityToolkit-Labs/nuget/v3/index.json
```

If you find an experiment useful, please up-vote 👍 its corresponding issue. Each experiment has an issue assigned to it with the `experiment` label for tracking. Please file any feedback or issues about that experiment on that singular issue. For any other questions or concerns, please open a Discussion.

## Adding a new Experiment

To start a new experiment, open up a new Discussion to propose your idea with the community. Be sure to follow the template and highlight reasons why and how your idea can aid other developers.

Once there is traction an issue will be created to track your experiment and its progress.

TODO: Add info about copying template.

Then open a PR to start your experiment, not everything needs to be done in your initial PR. The Labs space is a great place to work on something over time, get feedback from the community, and collaborate with others. However, your initial PR should compile and have enough content for folks to understand how to leverage your component.

## Modifying an Experiment

First fork the repo and create a new branch specific to your modification.

To work on an experiment you can navigate to it's subdirectory and open its own solution file. This will let you work on the feature, samples, docs, or unit tests for that specific component only in isolation.

Then submit a PR back to the repo with your modifications. Whoever owns the experiment can then work with you to integrate your changes. A maintainer will merge a PR once sign-off from the experiment owner is received.

## When is an Experiment done?

Not all experiments are successful, and that's ok! That's why we experiment! 👨‍🔬🔬👩‍🔬

If there is enough interest in an experiment, it can be time to move it into the main Windows Community Toolkit repo. These experiments should have all the components required implemented like a sample, documentation, and unit tests.

Open up an issue on the main Toolkit repo using the `Toolkit Labs Transfer` Issue Template. (TODO: Link) Use that issue to discuss where in the Toolkit the new component should be placed and what release it should be shipped in. An initial review pass of the code will happen as well. Once the transfer issue is approved, open up a PR to copy over the experiment to its new home.
