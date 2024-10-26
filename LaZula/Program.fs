
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
                printfn "get Command! :  %s" cmds
                let arg = cmds.Split(' ')
                match arg.[0] with
                | "exit" -> serverTaskAction.token.Cancel()
                | _ -> begin
                    let id = UInt64.Parse(arg.[0])
                    let msg = arg.[1]
                    printfn "[ %d ] send to : %s" id msg
                end
            end
            inputLoop()
        inputLoop()
    ))
    serverTaskAction.task.Wait()
    0 // return an integer exit code
