namespace LaZula.ServerModule
open System
open System.Threading
open System.Net.Sockets
open System.IO
open System.Threading.Tasks
open System.Text



[<Class>]
type SocketClientManagementUnit(client:TcpClient,parent:IServerController,clientId:uint64) =
    let client = client
    let stream = client.GetStream()
    let reader = new StreamReader(stream)
    let writer = new StreamWriter(stream)
    let clientId = clientId
    let mutable isRunning = false
    let mutable loopTask:Task = null
    let clientData = {IpAddress = client.Client.RemoteEndPoint.ToString();client=client;id=clientId}
    let mutable cancelTokenController = new CancellationTokenSource()

    do
        begin
            writer.AutoFlush <- true
        end
    interface IDisposable with
        member this.Dispose() = this.Clean()
    interface IClientController with
        member this.clientData = clientData
        member this.Send (msg:string) = writer.WriteLine(msg)
        member this.Send (msg:byte[]) = stream.Write(msg, 0, msg.Length)
        member this.Receive () = 
            let msg = reader.ReadLine()
            if msg = null then
                null
            else
                msg.ToCharArray() |> Array.map (fun x -> byte x)

    member this.Send (msg:string) = 
        (this :> IClientController).Send(msg)
    member this.Send (msg:byte[]) = 
        (this :> IClientController).Send(msg)

    member this.Clean () = 
        isRunning <- false
        client.Close()
        stream.Close()
        reader.Close()
        writer.Close()
        parent.Logger.DisconnectLog(clientData.id,clientData.IpAddress)
        if loopTask <> null then
            cancelTokenController.Cancel()
            parent.StopClient(clientData)
            loopTask.Wait()

    member this.ServerLoop() = 
        let rec HandleClient() = 
            let line = (this :> IClientController).Receive()
            if line = null then
                this.Clean()
            else
                parent.Logger.MessageLog(clientData.id,clientData.IpAddress,(Encoding.UTF8.GetString(line)))
                HandleClient()
        loopTask <- (Task.Factory.StartNew(HandleClient,cancelTokenController.Token)) 

    member this.Start() =
        isRunning <- true
        this.ServerLoop()
        parent.AppendConnection(this :> IClientController)
        this

