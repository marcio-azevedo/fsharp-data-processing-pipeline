// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r @"../../packages/WindowsAzure.ServiceBus.3.1.7/lib/net45-full/Microsoft.ServiceBus.dll"
#load "AzureServiceBus.fs"
//#r @"../../packages/Microsoft.WindowsAzure.ConfigurationManager.3.2.1/lib/net40/Microsoft.WindowsAzure.Configuration.dll"
//#r @"../../packages/Newtonsoft.Json.8.0.3/lib/net45/Newtonsoft.Json.dll"
//#r @"../../packages/FSharp.CloudAgent.0.3/lib/net40/FSharp.CloudAgent.dll"

open System
open System.Runtime.Serialization
open Microsoft.ServiceBus
open Microsoft.ServiceBus.Messaging
open FSharp.DataProcessingPipelines.Infrastructure.AzureServiceBus

[<Literal>]
let ServiceBusConnectionString = "host=localhost" //TODO: set an existing Azure host!
//
//// A DTO
//type SampleMessage = { Id : string; Name : string }
//
//let myQueue = AzureServiceBus.getQueue (ServiceBusConnectionString, "TestQueue")
//
//let currentClient = AzureServiceBus.getServiceBusclient (ServiceBusConnectionString, "TestQueue")
//
//let onMessageAction (message:BrokeredMessage) = 
//    printfn "%s" (message.SessionId)
//    ()
//
//// Configure the callback options.
//let options = new OnMessageOptions(AutoComplete = false, AutoRenewTimeout = (TimeSpan.FromMinutes(1.0)))
//
//// Callback to handle received messages.
//currentClient.OnMessage((fun (message) -> onMessageAction message), options)
//
//let testMessage = new BrokeredMessage()
//currentClient.Send(testMessage)
//
//// https://azure.microsoft.com/en-us/documentation/articles/service-bus-queues-topics-subscriptions/

