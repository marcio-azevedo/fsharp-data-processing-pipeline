namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSharp.DataProcessingPipelines.RabbitMQ")>]
[<assembly: AssemblyProductAttribute("FSharp.DataProcessingPipelines")>]
[<assembly: AssemblyDescriptionAttribute("Provides an extensible solution for creating Data Processing Pipelines in F#.")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
    let [<Literal>] InformationalVersion = "1.0"
