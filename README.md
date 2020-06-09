# Engine

This is a basic game engine created for personal practice and use; it uses the [entity-component-system (ECS) pattern](http://t-machine.org/index.php/2007/11/11/entity-systems-are-the-future-of-mmog-development-part-2/) wherever possible.

<!-- TOC depthfrom:2 -->

- [Building](#building)
    - [Prerequisites](#prerequisites)
        - [Requirements](#requirements)
        - [Recommendations](#recommendations)
    - [Instructions](#instructions)
- [Code Conventions](#code-conventions)
- [Cross-Platform Compatibility](#cross-platform-compatibility)
- [Support](#support)
- [Roadmap](#roadmap)
- [License](#license)

<!-- /TOC -->

## Building

### Prerequisites

#### Requirements

- .NET Core 3.1
- Visual Studio 2019
- Windows

#### Recommendations

- Windows 10
- A decent graphics card & processor

### Instructions

1. First, clone the repository recursively with `git clone --recursive https://github.com/xezno/engine`
2. Next, run `git submodule update` to ensure that all submodules are up to date.
3. Once complete, open Source/Ulaid.sln and build the solution. This will fetch / build any dependencies automatically, and then build the engine and example game.

## Code Conventions

Throughout this project, the standard [C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) should be observed and used within any pull requests / any other direct contributions.

The following project-specific naming conventions are used:

- Classes ending in `Entity` derive from the `Entity<T>` base class, and - as the name suggests - are much similar to the entities found within a typical ECS project.
- Classes ending in `Component` derive from the `Component<T>` base class, and are much similar to a component found within a typical ECS project.
- Classes ending in `Manager` derive from the `Manager<T>` base class; unlike their name, they are similar to a system found within a typical ECS project - this name was chosen to prevent any confusion with the `System` namespace within .NET.

## Cross-Platform Compatibility

Cross-platform compatibility is currently untested, and Windows 10 is currently the only supported platform; therefore, your mileage may vary when attempting to run the project on a non-Windows operating system. In the somewhat distant future, however, this should change.

## Support

Since Engine is a personal project that I am developing solely within my free time, I cannot guarantee or provide any maintenance or support right now. In the future, this might change - it all depends on the direction the project takes.

## Roadmap

As previously mentioned, this engine is a free-time project and I have no step-by-step roadmap. That being said, there are a few large goals that I want to achieve within this project:

- Physically based rendering for all objects within a scene
- Decent lighting
- Some form of multiplayer with a text-based chat system

## License

Engine is licensed under the MPL-2.0 license; a copy of this license is available at [Docs/LICENSE.md](https://github.com/xezno/engine/blob/master/Docs/LICENSE.md).
