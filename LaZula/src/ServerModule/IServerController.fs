namespace LaZula.ServerModule

open System.Net
open System.Net.Sockets



[<Struct>]
type ContactData = {
    IpAddress:string
    client:TcpClient
    id:uint64
}

[<Interface>]
type IClientController =
    abstract member clientData : ContactData
    abstract member Send : string -> unit
    abstract member Send : byte[] -> unit
    abstract member Receive : unit -> byte[]
    abstract member MakeRelay : uint64 -> unit


[<Interface>]
type IServerController = 
    abstract member AppendConnection : IClientController -> unit
    abstract member StopClient : ContactData -> unit
    abstract member Logger: LoggerModule.ILoggingTool
    abstract member MakeRelayConnection : uint64 * uint64 -> unit
    abstract member SendRelay : uint64 * byte[] * uint64 -> bool

