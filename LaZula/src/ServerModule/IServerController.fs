namespace LaZula.ServerModule

open System.Net
open System.Net.Sockets



[<Struct>]
type ContactData = {
    IpAddress:string
    client:TcpClient
}

[<Interface>]
type IClientController =
    abstract member clientData : ContactData
    abstract member Send : string -> unit
    abstract member Send : byte[] -> unit
    abstract member Receive : unit -> byte[]


[<Interface>]
type IServerController = 
    abstract member StartConnection : IClientController -> unit
    abstract member StopClient : ContactData -> unit

