namespace FSharp.DataProcessingPipelines.Core

open System

module Pipes = 

    [<AbstractClass>]
    type IInputPipe<'T> () = 

        abstract member Subscribe: ('T -> unit) -> unit

    [<AbstractClass>]
    type IOutputPipe<'T> () = 

        abstract member Publish: 'T -> unit
