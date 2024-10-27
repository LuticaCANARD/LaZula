namespace LaZula.ServerModule
open System
open System.Net
open System.Net.Sockets
open System.IO
open System.Threading.Tasks


[<Class>]
type CommandableSocketServer(port:int) =
    let mutable maxClientIndex : uint64 = 0UL; 
    let server:TcpListener = new TcpListener(IPAddress.Any, port)
    let port:int = port
    let mutable conntctedClients : Map<uint64,IClientController> = Map[]
    member public this.GetClientLastId = maxClientIndex
    interface IServerController with
        member this.AppendConnection(client) = 
            conntctedClients <- conntctedClients.Add(maxClientIndex,client)
            (this :> IServerController).Logger.LogEtc "[%d] Applied ... %s" maxClientIndex (client.clientData.IpAddress)
            maxClientIndex <- maxClientIndex + 1UL
        member this.StopClient(client:ContactData) = 
            conntctedClients.Remove(client.id) |> ignore
        member this.Logger = new LoggerModule.ConsoleLogger()

    member inline this.Log = 
        (this:>IServerController).Logger.LogEtc
    member public this.GetClientById(id:uint64) = conntctedClients.TryFind(id)
    member this.StartServer () = 
        if port < 0 || port > 65535 then
            failwith "Invalid port"
        if server.Server.IsBound = true then
            failwith "Server is already running"
        server.Start()
        this.Log "Starting server on port %d" port
        
        let rec serverLoop () = 
            try
                let client = server.AcceptTcpClient()
                (this:>IServerController).Logger.AcceptLog(maxClientIndex,(client.Client.RemoteEndPoint.ToString())) |> ignore
                let clientUnit = new SocketClientManagementUnit(client,this,maxClientIndex)
                clientUnit.Start() |> ignore
                serverLoop()
            with
            | :? SocketException as e -> // 소켓 에러인 경우
                (this:>IServerController).Logger.LogEtc "Socket exception %s" e.Message; 
            | ex ->  // 그 외의 에러인 경우, 재시도.
                (this:>IServerController).Logger.LogEtc "Error: %s" ex.Message
                serverLoop()
        serverLoop()
