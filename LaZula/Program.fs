﻿
open LaZula
open LaZula.ServerModule
open LaZula.SystemController
open System.Threading.Tasks
open System

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let server = new CommandableSocketServer(8080)
    let serverTaskAction = new TaskActions(
        fun () -> 
            server.StartServer()
    )
    let uiTaskAction = new TaskActions(fun () -> (
        let rec inputLoop () = 
            begin
                let cmds = Console.ReadLine()
                printfn "get Command! : %s" cmds
                let headerSet = cmds.IndexOf(' ')
                if headerSet = -1 then
                     match cmds with
                     | "exit" -> begin
                            printfn "exit"
                            serverTaskAction.token.Cancel()
                            exit(0)
                        end 
                     | _ -> begin
                            printfn "Invalid Command";
                        end
                else
                    let _arg = cmds[..headerSet];
                    match _arg with
                    | "exit" -> serverTaskAction.token.Cancel()
                    | _ -> 
                    begin
                        let arg = _arg.Split(' ')[0]
                        if(_arg.Length < 2) then
                            printfn "Invalid Command"
                            inputLoop()
                        try
                            let arg2 = cmds[headerSet+1..];
                            if(arg = "all") then 
                                printfn "send to all : %s" arg2
                            elif (arg = "connect") then 
                                begin
                                    try
                                        let argss = arg2.Split(' ')
                                        let ipWithPort = argss.[0]
                                        let ip = Net.IPAddress.Parse(ipWithPort.Split(':')[0])
                                        let port = Int32.Parse(ipWithPort.Split(':')[1])
                                        printfn "connect to : %s" (id.ToString())
                                        let client = new System.Net.Sockets.TcpClient()
                                        try 
                                            client.Client.Connect(ip,port) |> ignore
                                            (server:>IServerController).AppendConnection(new SocketClientManagementUnit(client,server,server.GetClientLastId))
                                        with
                                        | :? System.Net.Sockets.SocketException as e -> printfn "Socket Error"
                                        | :? System.ArgumentNullException as e -> printfn "Invalid Command error"
                                    with :? FormatException as e -> printfn "Invalid Command error"
                                end
                            else
                                try 
                                    let id:uint64 = UInt64.Parse(arg)
                                    let client = server.GetClientById(id)
                                    if client.IsNone then
                                        printfn "There is no Client %d" id
                                    else
                                        let encoder = System.Text.Encoding.UTF8
                                        let msg =  encoder.GetBytes("t" + arg2)
                                        client.Value.clientData.client.Client.Send(msg) |> ignore
                                        printfn "[ %d ] send to : %s" id arg2
                                        
                                with 
                                | :? System.FormatException as e -> printfn "Invalid Command error1"
                                | :? System.ArgumentNullException as e -> printfn "Invalid Command error2"
                                | :? System.ArgumentOutOfRangeException as e -> printfn "Invalid Command error3"
                                | :? System.OverflowException as e -> printfn "Invalid Command error4"
                                | :? System.Net.Sockets.SocketException as e -> printfn "Invalid Command error5"
                                | :? System.ObjectDisposedException as e -> printfn "Invalid Command error6"
                                | :? System.InvalidOperationException as e -> printfn "Invalid Command error7"
                                | :? System.IO.IOException as e -> printfn "Invalid Command error8"
                                | :? System.ArgumentException as e -> printfn "Invalid Command error9"

                        with :? FormatException as e -> printfn "Invalid Command error"
                    end
            end
            inputLoop()
        inputLoop()
    ))
    serverTaskAction.task.Wait()
    0 // return an integer exit code
