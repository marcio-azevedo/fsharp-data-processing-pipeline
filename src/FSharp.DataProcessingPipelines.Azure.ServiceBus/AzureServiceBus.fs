namespace FSharp.DataProcessingPipelines.Azure.ServiceBus

// RootManageSharedAccessKey = 
// Endpoint=sb://cfn-pipeline.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ddygS+p2VDMlFPsrGuYOp20dOSiPknLZVgwkI5eLmoY=
open System
open System.Runtime.Serialization
open Microsoft.ServiceBus
open Microsoft.ServiceBus.Messaging

module AzureServiceBus = 

    let namespaceManager (connectionString:string) =
        try
            Some (NamespaceManager.CreateFromConnectionString(connectionString))
        with
        | ex -> 
            printfn "%s" (ex.InnerException.Message)
            None

    let getQueue (connectionString:string, queueName:string) = 
        match (namespaceManager connectionString) with 
        | None -> None
        | Some nm ->
            try
                match (nm.QueueExists(queueName)) with
                | false -> Some (nm.CreateQueue(queueName))
                | true -> Some (nm.GetQueue queueName)
            with
            | ex -> 
                (printfn "%s" ex.InnerException.Message)
                None

    let getServiceBusclient (connectionString:string, queueName:string) = 
        QueueClient.CreateFromConnectionString(connectionString, queueName)

