namespace FSharp.DataProcessingPipelines.Core

open System
open FSharp.DataProcessingPipelines.Core.Messages
open FSharp.DataProcessingPipelines.Core.Pipes
open FSharp.DataProcessingPipelines.Core.Filters

module Runners = 

    type BaseRunner (filter:IFilter) = 

        member this.Filter = filter

        member this.Start () = 
            // Add some special rule to delay start, or just start the Filter.
            (this.Filter.Execute ())

//        member this.Configuration = new Configuration()
//        member this.Logger = new LoggerConfiguration()
//                              .WriteTo.ColoredConsole()
//                              .WriteTo.RollingFile(@"C:\Log-{Date}.txt")
//                              .CreateLogger();

