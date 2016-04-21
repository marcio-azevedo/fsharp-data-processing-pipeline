namespace FSharp.DataProcessingPipelines.Core

open System

module Pipes = 

    [<AbstractClass>]
    type IInputPipe<'T> () = 

        abstract member SetHandler: Action -> unit

        abstract member Pull: unit -> 'T

    [<AbstractClass>]
    type IOutputPipe<'T> () = 

        abstract member Push: 'T -> unit
