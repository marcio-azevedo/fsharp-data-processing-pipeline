namespace FSharp.DataProcessingPipelines.Infrastructure


open System
open System.Globalization
open System.Threading
open System.Runtime.Serialization
open FSharp.DataProcessingPipelines.Core
open FSharp.DataProcessingPipelines.Core.Pipes
open Microsoft.ServiceBus
open Microsoft.ServiceBus.Messaging

/// Documentation for AzureServiceBus Pipes
///
/// ## Example
///
///     let h = Library.hello 1
///     printfn "%d" h
///
//module AzureServiceBus = 

//http://fsprojects.github.io/FSharp.CloudAgent/tutorial.html
//
//    /// OutputPipe type implementation for Azure Service Bus
//    type AzureServiceBusOutputPipe<'T when 'T : not struct> (serviceBus:TopicClient, topic:String) = 
//
//        inherit IOutputPipe<'T> ()
//
//        let Bus = serviceBus
//        let Topic = topic
//
//        override this.Publish m = 
//            try
//                let brokeredMessage = new BrokeredMessage()
//                Bus.Send(brokeredMessage)
//            with
//                | ex -> 
//                    // log exception
//                    reraise()
//
//    /// InputPipe type implementation for Azure Service Bus
//    type AzureServiceBusInputPipe<'T when 'T : not struct> 
//        (serviceBus:TopicClient, subscriberId:String, topic:String, locale:String) = 
//
//        inherit IInputPipe<'T> ()
//
//        let Bus = serviceBus
//        let SubscriberId = subscriberId
//        let Topic = topic
//        let Locale = locale
//
//        override this.Subscribe (handler:('T -> unit)) = 
//            let InternalHandler (message:'T) = 
//                Thread.CurrentThread.CurrentCulture <- new CultureInfo(Locale)
//                handler message
//            try
////                Bus.Subscribe<'T>(SubscriberId, (fun m -> InternalHandler m), (fun x -> x.WithTopic(Topic) |> ignore)) |> ignore
//                ()
//            with
//            | ex -> 
//                // log exception
//                reraise()

//    let namespaceManager (connectionString:string) =
//        try
//            Some (NamespaceManager.CreateFromConnectionString(connectionString))
//        with
//        | ex -> 
//            printfn "%s" (ex.InnerException.Message)
//            None
//
//    let getQueue (connectionString:string, queueName:string) = 
//        match (namespaceManager connectionString) with 
//        | None -> None
//        | Some nm ->
//            try
//                match (nm.QueueExists(queueName)) with
//                | false -> Some (nm.CreateQueue(queueName))
//                | true -> Some (nm.GetQueue queueName)
//            with
//            | ex -> 
//                (printfn "%s" ex.InnerException.Message)
//                None
//
//    let getServiceBusclient (connectionString:string, queueName:string) = 
//        QueueClient.CreateFromConnectionString(connectionString, queueName)
//
