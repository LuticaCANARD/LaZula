namespace LaZula.ServerModule.CommandableSocketServer
open System
open System.Net
open System.Net.Sockets
open System.IO
open System.Threading.Tasks

type CommandableSocketServer(port:int) =
    
    let server:TcpListener = new TcpListener(IPAddress.Any, port)
    let port:int = port
    let conntctedClients : list<Task> = [];

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
                let stream = client.GetStream()
                let reader = new StreamReader(stream)
                let writer = new StreamWriter(stream)
                writer.AutoFlush <- true

                let rec HandleClient() = 
                    let line = reader.ReadLine()
                    printfn "aa... %s" line
                    if line = null then
                        client.Close()
                        server.Stop()
                    else
                        printfn "Received: %s" line
                        HandleClient()
                List.append conntctedClients [Task.Factory.StartNew(fun () -> HandleClient())] |> ignore
                serverLoop()
            with 
            | :? SocketException as e -> // 소켓 에러인 경우
                printfn "Socket exception %s" e.Message; 
            | ex ->  // 그 외의 에러인 경우, 재시도.
                printfn "Error: %s" ex.Message
                serverLoop()

        serverLoop()


