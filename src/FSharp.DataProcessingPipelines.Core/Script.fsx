// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Messages.fs"
#load "Pipes.fs"
#load "Filters.fs"
#load "Runners.fs"

open System
open FSharp.DataProcessingPipelines.Core
open FSharp.DataProcessingPipelines.Core.Messages
open FSharp.DataProcessingPipelines.Core.Pipes
open FSharp.DataProcessingPipelines.Core.Filters
open FSharp.DataProcessingPipelines.Core.Runners

let testMessage = EventInformationMessage("This is a new test Message!", "This is the context of the new Test Message!")
let mutable messages = (testMessage::[])
messages <- (testMessage::messages)

// OutputPipe type example
type ServiceAOutputPipe () = 
    inherit IOutputPipe<EventInformationMessage> ()  
    override this.Push m = 
        messages <- (List.append messages [m])
        ()

// InputPIpe type example
type ServiceBInputPipe () = 
    inherit IInputPipe<EventInformationMessage> () 
    override this.SetHandler a = ()
    override this.Pull () = 
        match messages with 
        | [] -> new EventInformationMessage("", "")
        | h::t ->
            messages <- t
            h

let outputPipe = new ServiceAOutputPipe()
let inputPipe = new ServiceBInputPipe()

type ServiceAFilter (pipe:ServiceAOutputPipe) =
    inherit DataSource<EventInformationMessage>(pipe)
    override this.Execute () = 
        try
            try
                let msg = EventInformationMessage("This is a new test Message!", "This is the context of the new Test Message!")
                this.OutputPipe.Push msg
            finally
                // Dispose if needed
                ()
        with
            | ex -> 
                // log exception
                ()

type ServiceBFilter (pipe:ServiceBInputPipe) =
    inherit DataSink<EventInformationMessage>(pipe)
    override this.Execute () = 
        let msg = (this.InputPipe.Pull ())
        printfn "%A" (msg)

let myRunnerA = BaseRunner (ServiceAFilter (outputPipe))
let myRunnerB = BaseRunner (ServiceBFilter (inputPipe))

printfn "%d" (messages.Length)

myRunnerA.Start ()
myRunnerB.Start ()
