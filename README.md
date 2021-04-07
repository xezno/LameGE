<p align="center">
    <h1 align="center">
        LameGE
    </h1>
    <p align="center">
        A basic game engine.
        <br>
        <a href="https://github.com/xezno/Engine/issues">Issues</a> |
        <a href="https://github.com/xezno/Engine/pulls">Pull Requests</a>
    </p>
    <p align="center">
        <img src="https://img.shields.io/github/repo-size/xezno/Engine?style=flat-square" alt="Repo Size">
        <img src="https://img.shields.io/github/contributors/xezno/Engine?style=flat-square" alt="Contributors">
        <img src="https://img.shields.io/github/stars/xezno/Engine?style=flat-square" alt="Stars"> 
        <img src="https://img.shields.io/github/forks/xezno/Engine?style=flat-square" alt="Forks">
        <img src="https://img.shields.io/badge/license-MIT-green?style=flat-square" alt="License">
        <img src="https://img.shields.io/github/issues/xezno/Engine?style=flat-square" alt="Issues">
    </p>
</p>

## Building

### Prerequisites

#### Requirements

- .NET 5.0
- Visual Studio 2019
- Windows
- CMake (for libstbi)

#### Recommendations

- Windows 10
- A decent graphics card & processor

### Instructions

#### Building with the example project
1. First, clone the repository recursively with `git clone --recursive https://github.com/xezno/engine`
2. Next, run `git submodule update` to ensure that all submodules are up to date.
3. Once done, create a junction to `(root dir)/Content` in `(root dir)/Source/ExampleGame/bin/x64/Debug/net5.0-windows7.0`. You will need to provide some of your own content, as some copyrighted material has been omitted from this repository (for legal reasons).
4. Now, compile stbi; this can be done by executing the following commands in a terminal window:
 - `cd (root-dir)/Other/stbi-sharp/`
 - `mkdir build`
 - `cd build`
 - `cmake ../libstbi`
 - You will then need to build the project located at `(root-dir)/Other/stbi-sharp/build/INSTALL.vcxproj`.
5. Once complete, open `Source/Engine.sln` and build the solution, using `ExampleGame` as the startup project. This will fetch / build any NuGet dependencies automatically, and then build the engine and example game.

## Contributing

Contributions to this project are greatly appreciated; please follow these steps in order to submit your contribution to the project:

1. Fork the project
2. Create a branch under the name `YourName/FeatureName`
3. Once you've made all the changes you need to make, go ahead and submit a Pull Request.

## Code Conventions

Throughout this project, the standard [C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) should be observed and used within any pull requests / any other direct contributions.

The following project-specific naming conventions are used:

- Classes ending in `Entity` derive from the `Entity<T>` base class, and - as the name suggests - are much similar to the entities found within a typical ECS project.
- Classes ending in `Component` derive from the `Component<T>` base class, and are much similar to a component found within a typical ECS project.
- Classes ending in `Manager` derive from the `Manager<T>` base class; unlike their name, they are similar to a system found within a typical ECS project - this name was chosen to prevent any confusion with the `System` namespace within .NET.

## Support

Feel free to [open an issue](https://github.com/xezno/Engine/issues/new) if you encounter any bugs or problems, have any feature requests, or have any questions.

## License

This project is licensed under the MPL-2.0 license; a copy of this license is available at [Docs/LICENSE.md](https://github.com/xezno/Engine/blob/main/Docs/LICENSE.md).

## Acknowledgements
* [Badges](https://shields.io)