# ECSEngine

This is a basic engine created for personal practice and use; it uses the [entity-component-system (ECS) pattern](http://t-machine.org/index.php/2007/11/11/entity-systems-are-the-future-of-mmog-development-part-2/) wherever possible.

## Code Conventions

Throughout this project, the standard [C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) are used and should be used in any pull requests or other direct contributions.

The following project-specific naming conventions are used:

- Classes ending in `Entity` derive from the `Entity<T>` base class, and - as the name suggests - are much similar to the entities found within a typical ECS project.

- Classes ending in `Component` derive from the `Component<T>` base class, and are much similar to a component found within a typical ECS project.

- Classes ending in `Manager` derive from the `Manager<T>` base class; unlike their name, they are similar to a system found within a typical ECS project - this name was chosen to prevent any confusion with the `System` namespace within the .NET Framework.


