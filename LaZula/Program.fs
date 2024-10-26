
open LaZula
open LaZula.ServerModule
open LaZula.SystemController
open System.Threading.Tasks
open System

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let serverTaskAction = new TaskActions(
        fun () -> 
            let server = new CommandableSocketServer(8080)
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
                    let arg = cmds[..headerSet];
                    match arg with
                    | "exit" -> serverTaskAction.token.Cancel()
                    | _ -> 
                    begin
                        if(arg.Length < 2) then
                            printfn "Invalid Command"
                            inputLoop()
                        try
                            let arg2 = cmds[headerSet..];
                            if(arg = "all") then 
                                printfn "send to all : %s" arg2
                            else
                                let id = UInt64.Parse(arg)
                                let msg = arg2
                                printfn "[ %d ] send to : %s" id msg
                        with :? FormatException as e -> printfn "Invalid Command error"
                    end
            end
            inputLoop()
        inputLoop()
    ))
    serverTaskAction.task.Wait()
    0 // return an integer exit code
