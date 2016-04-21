namespace FSharp.DataProcessingPipelines.RabbitMQ

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

module Infrastructure =

    /// OutputPipe type implementation for RabbitMQ
    [<AbstractClass>]
    type RabbitMQOutputPipe<'T when 'T : not struct> (serviceBus:IBus, topic:String) = 

        inherit IOutputPipe<'T> ()

        let Bus = serviceBus
        let Topic = topic

        override this.Push m = 
            Bus.Publish<'T>(m, Topic)

    /// InputPipe type implementation for RabbitMQ
    [<AbstractClass>]
    type RabbitMQInputPipe<'T when 'T : not struct> 
        (serviceBus:IBus, subscriberId:String, topic:String, locale:String) = 

        inherit IInputPipe<'T> ()

        let IsDisposed = false
        let mutable Handler = new Action((fun () -> ()))
        let Bus = serviceBus
        let SubscriberId = subscriberId
        let Topic = topic
        let Locale = locale

        abstract member Message : ('T) with get, set

        member this.HandleMessage (nextMessage:'T) =
            Thread.CurrentThread.CurrentCulture <- new CultureInfo(locale)
            this.Message <- nextMessage
            Handler

        override this.SetHandler (messageHandler) = 
            Handler <- messageHandler
            Bus.Subscribe<'T>(
                SubscriberId, 
                (fun m -> 
                    (this.HandleMessage m)
                    |> ignore), 
                (fun x -> 
                    x.WithTopic(topic)
                    |> ignore))
            |> ignore

        override this.Pull () = this.Message
