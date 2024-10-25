
open LaZula
open LaZula.ServerModule
open System.Threading.Tasks

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let serverTask = Task.Factory.StartNew(fun () -> 
        let server = new CommandableSocketServer(8080)
        server.StartServer()
    )
    serverTask.Wait()
    0 // return an integer exit code
