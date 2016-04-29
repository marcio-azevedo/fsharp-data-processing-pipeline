namespace FSharp.DataProcessingPipelines.Core

open System
open FSharp.DataProcessingPipelines.Core.Pipes
open FSharp.DataProcessingPipelines.Core.Messages

module Filters = 

    type IFilter () = 

        /// Data transformation executed by the Filter
        abstract member Execute: unit -> unit
        default this.Execute () = ()

    /// Filter that executes the data transformation
    [<AbstractClass>]
    type Filter<'T, 'V>(input:IInputPipe<'T>, output:IOutputPipe<'V>) =

        /// Executes the data transformation.
        inherit IFilter ()

        /// Input Pipe
        member this.InputPipe = input

        /// Output Pipe
        member this.OutputPipe = output

    /// Data Source that triggers the pipeline
    [<AbstractClass>]
    type DataSource<'V>(output:IOutputPipe<'V>) =

        /// Executes the data transformation.
        inherit IFilter ()

        /// Output Pipe
        member this.OutputPipe = output

    /// Data Sink that ends the pipeline data transformation
    [<AbstractClass>]
    type DataSink<'T>(input:IInputPipe<'T>) =

        /// Executes the data transformation.
        inherit IFilter ()

        /// Output Pipe
        member this.InputPipe = input

//    match (filter:IFilter<'T, 'V>) = 
//        | Filter :> Filter<'T,'V> ->
//        | DataSource :> DataSource<'V> -> 
//        | DataSink :> DataSink<'T> - >
//        | _ -> 
