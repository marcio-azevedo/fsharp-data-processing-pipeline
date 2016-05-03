// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r @"../../packages/RabbitMQ.Client/lib/net45/RabbitMQ.Client.dll"
#r @"../../packages/EasyNetQ/lib/net45/EasyNetQ.dll"
#r @"bin/Debug/FSharp.DataProcessingPipelines.Core.dll"
#load "RabbitMQModule.fs"

//@"C:\cfn\Celfinet.Spikes\Hands-on-Lab\3-ServiceBusArchitecture\src\Cfn.HOL.ServiceBusArchitecture.Exercise4.Final\FSharp.Test\"
//#r @"bin/Debug/Cfn.Engine.Pipeline.Extensibility.dll"
//#r @"bin/Debug/Cfn.Engine.ServiceBus.RabbitMQ.dll"
//#r @"C:/cfn/Celfinet.Spikes/Hands-on-Lab/3-ServiceBusArchitecture/src/Cfn.HOL.ServiceBusArchitecture.Exercise4.Final/Cfn.HOL.ServiceBusArchitecture.Pipeline/bin/Debug/Cfn.HOL.ServiceBusArchitecture.Pipeline.dll"
//#r @"C:/cfn/Celfinet.Spikes/Hands-on-Lab/3-ServiceBusArchitecture/src/Cfn.HOL.ServiceBusArchitecture.Exercise4.Final/Cfn.HOL.ServiceBusArchitecture.Filters/bin/Debug/Cfn.HOL.ServiceBusArchitecture.Filters.dll"
//#r @"C:\cfn\Celfinet.Spikes\Hands-on-Lab\3-ServiceBusArchitecture\src\Cfn.HOL.ServiceBusArchitecture.Exercise4.Final\Cfn.HOL.Configuration\bin\Debug\Cfn.HOL.ConfigurationGateway.dll"

//namespace Cfn.HOL.ServiceBusArchitecture.Pipeline.Messages

open System
open System.Collections.Generic
open System.Globalization
open System.Threading
open RabbitMQ.Client
open EasyNetQ
open FSharp.DataProcessingPipelines.Core
open FSharp.DataProcessingPipelines.Core.Messages
open FSharp.DataProcessingPipelines.Core.Pipes
open FSharp.DataProcessingPipelines.Core.Filters
open FSharp.DataProcessingPipelines.Core.Runners
open FSharp.DataProcessingPipelines.Infrastructure.RabbitMQ

// See ObservableCollection -> http://www.fssnip.net/dv
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
                printfn "--- Service A Publishes Msg ---"
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
            raise ex

let outputPipe = new ServiceAOutputPipe(serviceBus, ServiceAInputPipeTopic)
let inputPipe = new ServiceBInputPipe(serviceBus, ServiceBSubscriberId, ServiceAInputPipeTopic, Culture)

let myRunnerA = BaseRunner (ServiceAFilter (outputPipe))
let myRunnerB = BaseRunner (ServiceBFilter (inputPipe))

myRunnerA.Start ()
myRunnerB.Start ()
