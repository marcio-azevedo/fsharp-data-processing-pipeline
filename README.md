# Build easily data processing pipelines in F# #
Provides an extensible solution for creating Data Processing Pipelines in F#.

----------

This solution offers the basic infrastructure to build data processing pipelines in F#.
The implementation is based on the [Pipes and Filters Pattern](https://msdn.microsoft.com/en-us/library/dn568100.aspx) and follows some of the Robert Martin's [Clean Architecture](https://blog.8thlight.com/uncle-bob/2012/08/13/the-clean-architecture.html) principles for clean architectures.

The goal is to provide the basic Infrastructure as well as some External Interfaces implementations to allow an easy and fast setup as well as focus on the Entities that represents the Business Rules that you're implementing.

The solution structure is based on the [prototypical .NET solution (file system layout and tooling), recommended by the F# Foundation.](https://github.com/fsprojects/ProjectScaffold/).


