// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open EasyNetQ
open FSharp.DataProcessingPipelines.Core
open FSharp.DataProcessingPipelines.Core.Messages
open FSharp.DataProcessingPipelines.Core.Pipes
open FSharp.DataProcessingPipelines.Core.Filters
open FSharp.DataProcessingPipelines.Core.Runners
open FSharp.DataProcessingPipelines.Infrastructure.RabbitMQ

type BaseMessage (id:int, events:(DateTime * String) list) = 
    member this.Id = id
    member this.Events = events

type ServiceBInputPipe
    (serviceBus:IBus, subscriberId:String, topic:String, locale:String) = 
    inherit RabbitMQInputPipe<BaseMessage> (serviceBus, subscriberId, topic, locale)

type ServiceAOutputPipe
    (serviceBus:IBus, topic:String) = 
    inherit RabbitMQOutputPipe<BaseMessage> (serviceBus, topic)

type ServiceAFilter (pipe:ServiceAOutputPipe) =
    inherit DataSource<BaseMessage>(pipe)
    override this.Execute () = 
        try
            try
                let msg = BaseMessage(1, [(DateTime.Now, "Test Message created in F# by the ServiceAFilter!")])
                this.OutputPipe.Publish msg
            finally
                // Dispose if needed
                ()
        with
            | ex -> 
                // log exception
                ()

type ServiceBFilter (pipe:ServiceBInputPipe) =
    inherit DataSink<BaseMessage>(pipe)
    override this.Execute () = 
        let handler (msg:BaseMessage) = 
            printfn "--- Service B Execute -> %d: " (msg.Id)
            for i in msg.Events do
                printfn "(%A, %s)" (fst i) (snd i)
            printfn "-----------------------------------------"
        this.InputPipe.Subscribe (handler)

let ServiceBusHost = "host=localhost" //TODO: set an existing RabbitMQ host!
let Culture = "en-US"

let ServiceASubscriberId = "ServiceASubscriberId"
let ServiceAInputPipeTopic = "ServiceAInputPipeTopic"
let ServiceAOutputPipeTopic = "ServiceAOutputPipeTopic"

let ServiceBSubscriberId = "ServiceBSubscriberId";
let ServiceBInputPipeTopic = "ServiceAOutputPipeTopic"
let ServiceBOutputPipeTopic = "ServiceDInputPipeTopic"

let serviceBus = 
    try
        RabbitHutch.CreateBus(ServiceBusHost)
    with
        | ex -> 
            let innerException = ex.InnerException
            printfn "%A %A" (ex.Message) (innerException)
            reraise()

//for i in 1..10 do 
//    let message = new BaseMessage(i,[(DateTime.Now, "Exercise Test Started!")])
//    serviceBus.Publish<BaseMessage>(message, ServiceAInputPipeTopic)

let outputPipe = new ServiceAOutputPipe(serviceBus, ServiceAInputPipeTopic)
let inputPipe = new ServiceBInputPipe(serviceBus, ServiceBSubscriberId, ServiceAInputPipeTopic, Culture)

let myRunnerA = BaseRunner (ServiceAFilter (outputPipe))
let myRunnerB = BaseRunner (ServiceBFilter (inputPipe))


[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    try
        myRunnerB.Start ()
        myRunnerA.Start ()
        printfn "myRunnerA.Start () -> Message sent to Queue!" 
    with
        | ex -> 
            let innerException = ex.InnerException
            printfn "%A %A" (ex.Message) (innerException)
            reraise()
    0 // return an integer exit code
