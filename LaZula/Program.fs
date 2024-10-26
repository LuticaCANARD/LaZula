
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
                let nams = Console.ReadLine()
                printfn "get Command! :  %s" nams
            end
            inputLoop()
        inputLoop()
    ))
    serverTaskAction.task.Wait()
    0 // return an integer exit code
