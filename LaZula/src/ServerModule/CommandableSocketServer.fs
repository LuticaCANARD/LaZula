﻿namespace LaZula.ServerModule
open System
open System.Net
open System.Net.Sockets
open System.IO
open System.Threading.Tasks


[<Class>]
type CommandableSocketServer(port:int) =

    let server:TcpListener = new TcpListener(IPAddress.Any, port)
    let port:int = port
    let conntctedClients : Map<String,IClientController> = Map[]
    interface IServerController with
        member this.StartConnection(client) = 
            conntctedClients.Add(client.clientData.IpAddress,client) |> ignore
        member this.StopClient(client:ContactData) = 
            conntctedClients.Remove(client.IpAddress) |> ignore
    member this.StartServer () = 
        if port < 0 || port > 65535 then
            failwith "Invalid port"
        if server.Server.IsBound = true then
            failwith "Server is already running"
        server.Start()
        printfn "Starting server on port %d" port
        
        let rec serverLoop () = 
            try
                let client = server.AcceptTcpClient()
                printfn "Client connected From... %s" (client.Client.RemoteEndPoint.ToString()) |> ignore
                let clientUnit = new SocketClientManagementUnit(client,this)
                conntctedClients.Add(client.Client.RemoteEndPoint.ToString(),clientUnit.Start()) |> ignore
                serverLoop()
            with 
            | :? SocketException as e -> // 소켓 에러인 경우
                printfn "Socket exception %s" e.Message; 
            | ex ->  // 그 외의 에러인 경우, 재시도.
                printfn "Error: %s" ex.Message

                serverLoop()

        serverLoop()