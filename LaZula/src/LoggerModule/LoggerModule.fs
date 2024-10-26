module LoggerModule
open System

[<Interface>]
type ILoggingTool = 
    abstract member Log : string -> unit
    abstract member Log : string * string -> unit
    abstract member Log : byte[] -> unit
    abstract member Log : byte[] * string -> unit
    abstract member Log : uint64 * byte[] -> unit
    abstract member Log : uint64 * string -> unit
    abstract member MessageLog : uint64 * string * string -> unit
    abstract member AcceptLog : uint64 * string -> unit
    abstract member DisconnectLog : uint64 * string -> unit
    abstract member ErrorLog : Exception -> unit
    abstract member LogEtc : Printf.TextWriterFormat<'T> -> 'T

[<Class>]
type ConsoleLogger() = 
    let actor = printfn 
    interface ILoggingTool with
        member this.Log (msg:string) = 
            actor "%s" msg
        member this.Log (msg:string, msg2:string) = 
            actor "%s %s" msg msg2
        member this.Log (msg:byte[]) = 
            actor "%A" msg
        member this.Log (msg:byte[], msg2:string) = 
            actor "%A %s" msg msg2
        member this.Log (id:uint64, msg:byte[]) = 
            actor "[%d] %A" id msg
        member this.LogEtc (a:Printf.TextWriterFormat<'T>) = actor a;
        member this.Log (id:uint64, msg:string) = 
            actor "[%d] %s" id msg
        member this.MessageLog (id:uint64, msg:string, msg2:string) = 
            actor "[%d] %s recevied : %s" id msg msg2
        member this.AcceptLog (id:uint64, ip:string) = 
            actor "[%d] ACCEPT CLIENT  FROM: %s" id ip
        member this.DisconnectLog (id:uint64 , ip:string) = 
            actor "[%d] : %s  DISCONNECT CLIENT" id ip
        member this.ErrorLog (ex:Exception) =
            actor "ERROR: %s" ex.Message

