namespace FSharp.DataProcessingPipelines.Core

open System

/// Documentation for Messages
///
/// ## Example
///
///     let h = Library.hello 1
///     printfn "%d" h
///
module Messages = 

    type MessageType =
        | Debug
        | Information
        | Warning
        | Error

    type ICommandMessage = 

        /// Action
        abstract member Action : String with get

        /// Type
        abstract member Type : MessageType with get

    type IEventMessage = 

        /// Action
        abstract member Timestamp : DateTime with get

        /// Context
        abstract member Context : String with get

        /// Message
        abstract member Message : String with get

        /// Type
        abstract member Type : MessageType with get

    type EventErrorMessage (e:Exception) = 

        member this.Exception = e

        interface IEventMessage with 
            member this.Context = 
                e.StackTrace
            
            member this.Message = 
                match e.InnerException with
                | null -> (e.Message)
                | _ -> (e.Message + " - " + e.InnerException.Message)
            
            member this.Type = MessageType.Error

            member this.Timestamp = DateTime.Now

    type EventWarningMessage (m:String, c:String) = 

        interface IEventMessage with 
            member this.Context = c
            
            member this.Message = m
            
            member this.Type = MessageType.Warning

            member this.Timestamp = DateTime.Now

    type EventInformationMessage (m:String, c:String) = 

        interface IEventMessage with 
            member this.Context = c
            
            member this.Message = m
            
            member this.Type = MessageType.Information

            member this.Timestamp = DateTime.Now
            

    type EventDebugMessage (m:String, c:String) = 

        interface IEventMessage with 
            member this.Context = c
            
            member this.Message = m
            
            member this.Type = MessageType.Debug

            member this.Timestamp = DateTime.Now
