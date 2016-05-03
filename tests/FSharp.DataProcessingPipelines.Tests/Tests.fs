module FSharp.DataProcessingPipelines.Tests

open System
open NUnit.Framework
open FSharp.DataProcessingPipelines.Core
open FSharp.DataProcessingPipelines.Core.Messages
open FSharp.DataProcessingPipelines.Core.Pipes
open FSharp.DataProcessingPipelines.Core.Filters
open FSharp.DataProcessingPipelines.Core.Runners

// Types and vals to supoprt unit testing

let mutable messages = ([]:EventInformationMessage list)

// OutputPipe type example
type ServiceAOutputPipe () = 
    inherit IOutputPipe<EventInformationMessage> ()  
    override this.Publish (m) = 
        messages <- (List.append messages [m])

// InputPIpe type example
type ServiceBInputPipe () = 
    inherit IInputPipe<EventInformationMessage> () 
    override this.Subscribe (handler:(EventInformationMessage -> unit)) = 
        match messages with 
        | [] -> printfn "Messages is empty"
        | h::t ->
            messages <- t
            handler h

type ServiceAFilter (pipe:ServiceAOutputPipe) =
    inherit DataSource<EventInformationMessage>(pipe)
    override this.Execute () = 
        try
            try
                let msg = EventInformationMessage("This is a new test Message!", "This is the context of the new Test Message!")
                printfn "Publish msg to output pipe at Service A: %A" (msg.ToString())
                this.OutputPipe.Publish msg
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
        let handler (msg) = printfn "Service B Execute -> %A" (msg)
        this.InputPipe.Subscribe (handler)

/// Tests

let outputPipe = new ServiceAOutputPipe()
let inputPipe = new ServiceBInputPipe()

let myRunnerA = BaseRunner (ServiceAFilter (outputPipe))
let myRunnerB = BaseRunner (ServiceBFilter (inputPipe))

let testMessage = EventInformationMessage("This is a new test Message!", "This is the context of the new Test Message!")

[<Test>]
let ``Service B consumes 2 messages`` () =
    messages <- (testMessage::[])
    messages <- (testMessage::messages)
    myRunnerB.Start ()
    myRunnerB.Start ()
    let result = messages.Length
    printfn "%i" (result)
    Assert.AreEqual(0,result)
    
[<Test>]
let ``Service A publishes 2 messages`` () =
    myRunnerA.Start ()
    myRunnerA.Start ()
    let result = messages.Length
    printfn "%i" (result)
    Assert.AreEqual(2,result)


