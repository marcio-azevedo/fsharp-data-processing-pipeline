// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r @"../../packages/RabbitMQ.Client/lib/net45/RabbitMQ.Client.dll"
#r @"../../packages/EasyNetQ/lib/net45/EasyNetQ.dll"
#r @"bin/Debug/FSharp.DataProcessingPipelines.Core.dll"
#load "RabbitMQModule.fs"

//namespace Cfn.HOL.ServiceBusArchitecture.Pipeline.Messages

open System
open System.Globalization
open System.Threading
open RabbitMQ.Client
open EasyNetQ
open FSharp.DataProcessingPipelines.Core
open FSharp.DataProcessingPipelines.Core.Messages
open FSharp.DataProcessingPipelines.Core.Pipes
open FSharp.DataProcessingPipelines.Core.Filters
open FSharp.DataProcessingPipelines.Core.Runners
open FSharp.DataProcessingPipelines.RabbitMQ.Infrastructure

type BaseMessage (id:int, events:(DateTime * String) list) = 
    member this.Id = id
    member this.Events = events

type ServiceBInputPipe
    (serviceBus:IBus, subscriberId:String, topic:String, locale:String) = 
    inherit RabbitMQInputPipe<BaseMessage> (serviceBus, subscriberId, topic, locale)
    let mutable msg = new BaseMessage(0, [])
    override this.Message
        with get() = msg
        and set(value) = msg <- value

type ServiceAOutputPipe
    (serviceBus:IBus, topic:String) = 
    inherit RabbitMQOutputPipe<BaseMessage> (serviceBus, topic)

type ServiceAFilter (pipe:ServiceAOutputPipe) =
    inherit DataSource<BaseMessage>(pipe)
    override this.Execute () = 
        try
            try
                let msg = BaseMessage(1, [(DateTime.Now, "Test Message created in F# by the ServiceAFilter!")])
                this.OutputPipe.Push msg
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
        let msg = (this.InputPipe.Pull ())
        this.PrintsMessage msg

    member this.PrintsMessage (baseMsg:BaseMessage) = 
        printfn "--- Printing output in ServiceBFilter ---"
        match baseMsg.Events with
        | [] -> printfn "---"
        | l -> 
            for msg in baseMsg.Events do
                let (d,s) = msg
                printfn "%d %A %s" (baseMsg.Id) d s

// https://api.cloudamqp.com/sso/4e7ff84d-254c-4006-be58-19d039505b04/details
// https://www.rabbitmq.com/tutorials/amqp-concepts.html
// https://github.com/EasyNetQ/EasyNetQ/wiki/Connecting-to-RabbitMQ

let ServiceBusHost = "host=chicken.rmq.cloudamqp.com;virtualHost=dwtdlpzl;username=dwtdlpzl;password=x2x5-wlGHC2XfhzDFCgoHWbIlPdeAEK_;timeout=0"
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
            raise ex

for i in 1..100 do 
    let message = new BaseMessage(i,[(DateTime.Now, "Exercise5 Test Started!")])
    serviceBus.Publish<BaseMessage>(message, ServiceAInputPipeTopic)

let outputPipe = new ServiceAOutputPipe(serviceBus, ServiceAInputPipeTopic)
let inputPipe = new ServiceBInputPipe(serviceBus, ServiceASubscriberId, ServiceBInputPipeTopic, Culture)

let myRunnerA = BaseRunner (ServiceAFilter (outputPipe))
let myRunnerB = BaseRunner (ServiceBFilter (inputPipe))

myRunnerA.Start ()
myRunnerB.Start ()
