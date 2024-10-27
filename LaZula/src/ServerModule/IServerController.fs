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


[<Interface>]
type IServerController = 
    abstract member AppendConnection : IClientController -> unit
    abstract member StopClient : ContactData -> unit
    abstract member Logger: LoggerModule.ILoggingTool

